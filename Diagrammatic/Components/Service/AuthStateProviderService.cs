using Blazored.SessionStorage; // Change from SessionStorage
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

public class AuthStateProviderService : AuthenticationStateProvider
{
    private readonly ISessionStorageService _sessionStorage;
    private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

    public AuthStateProviderService(ISessionStorageService sessionStorage)
    {
        _sessionStorage = sessionStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            // Now fetching from SessionStorage
            var token = await _sessionStorage.GetItemAsync<string>("authToken");

            if (string.IsNullOrWhiteSpace(token) || token == "null")
                return new AuthenticationState(_anonymous);

            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "User") }, "apiauth_type");
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }
        catch
        {
            return new AuthenticationState(_anonymous);
        }
    }

    public async Task MarkUserAsAuthenticated(string token)
    {
        await _sessionStorage.SetItemAsync("authToken", token);
        var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "User") }, "apiauth_type");
        var user = new ClaimsPrincipal(identity);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    public async Task MarkUserAsLoggedOut()
    {
        await _sessionStorage.RemoveItemAsync("authToken");
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
    }
}