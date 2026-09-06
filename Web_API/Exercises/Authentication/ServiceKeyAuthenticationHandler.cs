using System;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Exercises.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Exercises.Authentication
{
    public class ServiceKeyAuthenticationOptions : AuthenticationSchemeOptions
    {
        public const string SchemeName = "ServiceKey";

        /// <summary>
        /// The expected key. When left empty the scheme authenticates nobody, so a
        /// deployment that forgets to configure one fails closed.
        /// </summary>
        public string ApiKey { get; set; }
    }

    /// <summary>
    /// Authenticates the background timer worker, which has no user account but still
    /// has to drive round progression for every game. It presents a shared key and gets
    /// an identity in <see cref="Constants.ROLE_SERVICE"/>.
    /// </summary>
    public class ServiceKeyAuthenticationHandler : AuthenticationHandler<ServiceKeyAuthenticationOptions>
    {
        public const string HeaderName = "X-Service-Key";

        // WebSocket upgrades cannot carry custom headers from the browser/Node SignalR
        // clients, so the key is also accepted from the query string, the same way
        // SignalR itself passes bearer tokens.
        public const string QueryName = "serviceKey";

        public ServiceKeyAuthenticationHandler(
            IOptionsMonitor<ServiceKeyAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var expected = Options.ApiKey;
            if (string.IsNullOrEmpty(expected))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            string presented = null;
            if (Request.Headers.TryGetValue(HeaderName, out var header))
            {
                presented = header.ToString();
            }
            else if (Request.Query.TryGetValue(QueryName, out var query))
            {
                presented = query.ToString();
            }

            // No key offered at all is not a failure: the request may still authenticate
            // with a user's bearer token under the other scheme.
            if (string.IsNullOrEmpty(presented))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            if (!FixedTimeEquals(presented, expected))
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid service key."));
            }

            var claims = new[]
            {
                // Guid.Empty keeps GetUserId() total for service calls; nothing is ever
                // attributed to the worker as an owner.
                new Claim("id", Guid.Empty.ToString()),
                new Claim(ClaimTypes.Name, "timer-worker"),
                new Claim(ClaimTypes.Role, Constants.ROLE_SERVICE)
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        private static bool FixedTimeEquals(string a, string b)
        {
            var left = Encoding.UTF8.GetBytes(a);
            var right = Encoding.UTF8.GetBytes(b);
            return CryptographicOperations.FixedTimeEquals(left, right);
        }
    }
}
