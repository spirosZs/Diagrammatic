using Diagrammatic2.Components.Shared.Requests;
using Diagrammatic2.Components.Shared.Responses;
using System.Threading.Tasks;

namespace Diagrammatic1.Components.Authentication.Service
{
    public interface IAuthService
    {
        Task<AuthSuccessResponse> Register(UserRegistrationRequest request);
        Task<AuthSuccessResponse> Login(UserLoginRequest request);
        Task<AuthSuccessResponse> RefreshToken(RefreshTokenRequest request);
    }
}
