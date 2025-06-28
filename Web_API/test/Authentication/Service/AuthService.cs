using Diagrammatic2.Components.Shared.Requests;
using Diagrammatic2.Components.Shared.Responses;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Diagrammatic1.Components.Authentication.Service
{
    public class AuthService : IAuthService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<AuthSuccessResponse> Register(UserRegistrationRequest request)
        {
            var httpClient = _httpClientFactory.CreateClient("DiagrammaticClient");
            var response = await httpClient.PostAsJsonAsync("identity/register", request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<AuthSuccessResponse>();
            }
            else
            {
                var authFailedResponse = await response.Content.ReadFromJsonAsync<AuthFailedResponse>();
                throw new HttpRequestException("Registration failed: " + string.Join(", ", authFailedResponse.Errors));
            }
        }

        public async Task<AuthSuccessResponse> Login(UserLoginRequest request)
        {
            var httpClient = _httpClientFactory.CreateClient("DiagrammaticClient");
            var response = await httpClient.PostAsJsonAsync("identity/login", request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<AuthSuccessResponse>();
            }
            else
            {
                var authFailedResponse = await response.Content.ReadFromJsonAsync<AuthFailedResponse>();
                throw new HttpRequestException("Login failed: " + string.Join(", ", authFailedResponse.Errors));
            }
        }

        public async Task<AuthSuccessResponse> RefreshToken(RefreshTokenRequest request)
        {
            var httpClient = _httpClientFactory.CreateClient("DiagrammaticClient");
            var response = await httpClient.PostAsJsonAsync("identity/refresh", request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<AuthSuccessResponse>();
            }
            else
            {
                var authFailedResponse = await response.Content.ReadFromJsonAsync<AuthFailedResponse>();
                throw new HttpRequestException("Token refresh failed: " + string.Join(", ", authFailedResponse.Errors));
            }
        }
    }
}
