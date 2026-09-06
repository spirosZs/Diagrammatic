using System.Text;
using System.Threading.Tasks;
using Exercises.Authentication;
using Exercises.Data;
using Exercises.Data.DbContext;
using Exercises.Options;
using Exercises.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Exercises.Helpers
{
    public static class Auth
    {
        /// <summary>Authorization policy guarding the game hub.</summary>
        public const string GameHubPolicy = Constants.POLICY_GAME_HUB;

        public const string GameHubPath = "/gameHub";

        public static void AddAuthSettings(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = new JwtSettings();
            configuration.Bind(nameof(jwtSettings), jwtSettings);
            services.AddSingleton(jwtSettings);

            services.AddScoped<IIdentityService, IdentityService>();

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Secret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = false,
                ValidateLifetime = true
            };
            
            services.AddSingleton(tokenValidationParameters);
            
            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.SaveToken = true;
                    x.TokenValidationParameters = tokenValidationParameters;
                    x.Events = new JwtBearerEvents
                    {
                        // A browser cannot set an Authorization header on a WebSocket
                        // upgrade, so SignalR clients pass the token in the query string.
                        // Without this the hub's [Authorize] would reject every real user
                        // the moment the connection upgrades from negotiate.
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            if (!string.IsNullOrEmpty(accessToken) &&
                                context.HttpContext.Request.Path.StartsWithSegments(GameHubPath))
                            {
                                context.Token = accessToken;
                            }

                            return Task.CompletedTask;
                        }
                    };
                })
                .AddScheme<ServiceKeyAuthenticationOptions, ServiceKeyAuthenticationHandler>(
                    ServiceKeyAuthenticationOptions.SchemeName,
                    options => options.ApiKey = configuration["ServiceApiKey"]);

            services.AddAuthorization(options =>
            {
                // The hub carries both real users (teachers driving their own game) and
                // the keyed timer worker, so its policy has to accept either identity.
                options.AddPolicy(GameHubPolicy, policy =>
                {
                    policy.AddAuthenticationSchemes(
                        JwtBearerDefaults.AuthenticationScheme,
                        ServiceKeyAuthenticationOptions.SchemeName);
                    policy.RequireAuthenticatedUser();
                });
            });
        }
    }
}