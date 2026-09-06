using Blazored.SessionStorage;
using Diagrammatic2.Components.Shared.Requests;
using Diagrammatic2.Components.Shared.Responses;


namespace Diagrammatic_test.Services
{
    /// <summary>
    /// Keeps one signed-in user's token fresh in the background so their session does not
    /// expire while they are working.
    /// </summary>
    /// <remarks>
    /// This is registered per circuit, i.e. one instance per signed-in user. It used to be
    /// a singleton hosted service holding a single _currentToken/_refreshToken pair for the
    /// whole server: every login overwrote the previous user's tokens, so with a class of
    /// students only the most recent login was ever refreshed, and any one student logging
    /// out called ClearTokens() and stopped refresh for everyone still working.
    ///
    /// Refreshed tokens are also written back to session storage, which is where the rest
    /// of the app reads "authToken" from. Without that write-back the refresh only ever
    /// updated private fields and the user's session still went stale.
    /// </remarks>
    public class RefreshService : IAsyncDisposable
    {
        private const string AuthTokenKey = "authToken";
        private const string RefreshTokenKey = "refreshToken";

        private static readonly TimeSpan RefreshInterval = TimeSpan.FromMinutes(15);

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ISessionStorageService _sessionStorage;
        private readonly ILogger<RefreshService> _logger;

        // Guards the token fields and timer against the periodic callback racing a
        // login/logout on the circuit.
        private readonly SemaphoreSlim _gate = new(1, 1);

        private Timer? _timer;
        private string? _currentToken;
        private string? _refreshToken;

        public RefreshService(
            IHttpClientFactory httpClientFactory,
            ISessionStorageService sessionStorage,
            ILogger<RefreshService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _sessionStorage = sessionStorage;
            _logger = logger;
        }

        /// <summary>Called after a successful login to start refreshing this user's token.</summary>
        public void SetTokens(string currentToken, string refreshToken)
        {
            _currentToken = currentToken;
            _refreshToken = refreshToken;

            // Only start ticking once there is something to refresh. The old service armed
            // its timer at startup with null tokens and posted a null refresh request.
            _timer ??= new Timer(async _ => await RefreshAsync(), null, RefreshInterval, RefreshInterval);
        }

        /// <summary>Called on logout. Only ever affects the user who logged out.</summary>
        public void ClearTokens()
        {
            _currentToken = null;
            _refreshToken = null;
            _timer?.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private async Task RefreshAsync()
        {
            await _gate.WaitAsync();
            try
            {
                if (string.IsNullOrEmpty(_currentToken) || string.IsNullOrEmpty(_refreshToken))
                {
                    return;
                }

                var httpClient = _httpClientFactory.CreateClient("DiagrammaticClient");
                var response = await httpClient.PostAsJsonAsync("identity/refresh", new RefreshTokenRequest
                {
                    Token = _currentToken,
                    RefreshToken = _refreshToken
                });

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadFromJsonAsync<AuthFailedResponse>();
                    _logger.LogWarning("Token refresh failed: {Errors}",
                        error?.Errors is null ? response.StatusCode.ToString() : string.Join("; ", error.Errors));
                    return;
                }

                var authSuccessResponse = await response.Content.ReadFromJsonAsync<AuthSuccessResponse>();
                if (authSuccessResponse is null)
                {
                    return;
                }

                _currentToken = authSuccessResponse.Token;
                _refreshToken = authSuccessResponse.RefreshToken;

                // Session storage is JS interop, so it only works while this circuit is
                // connected. A disconnected circuit is not an error worth surfacing: the
                // refreshed token simply stays in memory until the user comes back.
                try
                {
                    await _sessionStorage.SetItemAsync(AuthTokenKey, authSuccessResponse.Token);
                    await _sessionStorage.SetItemAsync(RefreshTokenKey, authSuccessResponse.RefreshToken);
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(ex, "Could not write the refreshed token to session storage.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Token refresh threw.");
            }
            finally
            {
                _gate.Release();
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_timer is not null)
            {
                await _timer.DisposeAsync();
                _timer = null;
            }

            _gate.Dispose();
        }
    }
}
