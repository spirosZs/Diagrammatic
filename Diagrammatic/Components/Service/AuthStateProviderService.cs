using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

public class AuthStateProviderService : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private readonly ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());
    private ClaimsPrincipal _currentUser;

    public AuthStateProviderService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
        _currentUser = _anonymous;
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        // ✅ Always return the current in-memory user (starts as anonymous)
        return Task.FromResult(new AuthenticationState(_currentUser));
    }

    // Called when user logs in
    public async Task MarkUserAsAuthenticated(string token)
    {
        await _localStorage.SetItemAsync("authToken", token);

        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, "User"), // TODO: parse JWT for real claims
        }, "apiauth_type");

        _currentUser = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
    }

    // Called when user logs out
    public async Task MarkUserAsLoggedOut()
    {
        await _localStorage.RemoveItemAsync("authToken");
        _currentUser = _anonymous;

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
    }
}
