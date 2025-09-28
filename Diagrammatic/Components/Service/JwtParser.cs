namespace Diagrammatic_test.Components.Service
{
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;

    public static class JwtParser
    {
        public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            if (string.IsNullOrWhiteSpace(jwt))
                return Array.Empty<Claim>();

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt); // Decodes without validating
            return token.Claims;
        }
    }

}
