using Diagrammatic2.Components.Shared.Requests;
using Diagrammatic2.Components.Shared.Responses;


//this method refresh the user's token on the background so that the user doesn't log out by the app
namespace Diagrammatic_test.Services
{ 
    public class RefreshService : IHostedService, IDisposable
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private Timer _timer;
        private string _currentToken;
        private string _refreshToken;

        public RefreshService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(15)); // Adjust the interval as needed
            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("DiagrammaticClient");
                var request = new RefreshTokenRequest
                {
                    Token = _currentToken,
                    RefreshToken = _refreshToken
                };

                var response = await httpClient.PostAsJsonAsync("identity/refresh", request);

                if (response.IsSuccessStatusCode)
                {
                    var authSuccessResponse = await response.Content.ReadFromJsonAsync<AuthSuccessResponse>();
                    _currentToken = authSuccessResponse.Token; 
                    _refreshToken = authSuccessResponse.RefreshToken;
                }
                else
                {
                    var errorResponse = await response.Content.ReadFromJsonAsync<AuthFailedResponse>();
                    // Handle error response (e.g., log error)
                }
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log error)
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public void SetTokens(string currentToken, string refreshToken)
        {
            _currentToken = currentToken;
            _refreshToken = refreshToken;
        }
        
        //Clears tokens after Successfull Logout
        public void ClearTokens()
        {
            _currentToken = null;
            _refreshToken = null;
        }
    }
}
