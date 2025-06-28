using System.Linq;
using System.Threading.Tasks;
using Exercises.Common;
using Exercises.Common.Authorization.Requests;
using Exercises.Common.Authorization.Responses;
using Exercises.Services;
using Microsoft.AspNetCore.Mvc;

namespace Exercises.Controllers
{
    public class IdentityController : Controller
    {
        private readonly IIdentityService _identityService;
        
        public IdentityController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        // <summary>
        /// Register
        /// </summary>
        /// <param name="request"></param>
        [HttpPost(Router.Register)]
        [Produces("application/json")]
        [ProducesResponseType(200, Type = typeof(AuthSuccessResponse))]
        [ProducesResponseType(400, Type = typeof(AuthFailedResponse))]
        [Consumes("application/json")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
                });
            }

            var authResponse = await _identityService.RegisterAsync(request.Email, request.Password);

            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = authResponse.Errors
                });
            }

            return Ok(new AuthSuccessResponse
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken
            });
        }

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="request"></param>
        [HttpPost(Router.Login)]
        [Produces("application/json")]
        [ProducesResponseType(200, Type = typeof(AuthSuccessResponse))]
        [ProducesResponseType(400, Type = typeof(AuthFailedResponse))]
        [Consumes("application/json")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            var authResponse = await _identityService.LoginAsync(request.Email, request.Password);

            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = authResponse.Errors
                });
            }
            
            return Ok(new AuthSuccessResponse
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken
            });
        }

        /// <summary>
        /// Refresh Token
        /// </summary>
        /// <param name="request"></param>
        [HttpPost(Router.Refresh)]
        [Produces("application/json")]
        [ProducesResponseType(200, Type = typeof(AuthSuccessResponse))]
        [ProducesResponseType(400, Type = typeof(AuthFailedResponse))]
        [Consumes("application/json")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            var authResponse = await _identityService.RefreshTokenAsync(request.Token, request.RefreshToken);

            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = authResponse.Errors
                });
            }
            
            return Ok(new AuthSuccessResponse
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken
            });
        }
    }
}