using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AuthTest
{
    public static class ClaimUtils
    {
        public static string GetUserId(IEnumerable<Claim> claims)
        {
            var claimTypes = new[]
            {
            JwtRegisteredClaimNames.Sub,
            ClaimTypes.NameIdentifier,
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
            "user_id",
            "userId",
            "id"
        };

            foreach (var claimType in claimTypes)
            {
                var value = claims.FirstOrDefault(c => c.Type == claimType)?.Value;
                if (!string.IsNullOrEmpty(value))
                    return value;
            }

            return null;
        }

        public static string GetUsername(IEnumerable<Claim> claims)
        {
            var claimTypes = new[]
            {
            JwtRegisteredClaimNames.UniqueName,
            JwtRegisteredClaimNames.Name,
            ClaimTypes.Name,
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name",
            "username",
            "user",
            "preferred_username"
        };

            foreach (var claimType in claimTypes)
            {
                var value = claims.FirstOrDefault(c => c.Type == claimType)?.Value;
                if (!string.IsNullOrEmpty(value))
                    return value;
            }

            return null;
        }

        public static string GetEmail(IEnumerable<Claim> claims)
        {
            var claimTypes = new[]
            {
            JwtRegisteredClaimNames.Email,
            ClaimTypes.Email,
            "email"
        };

            foreach (var claimType in claimTypes)
            {
                var value = claims.FirstOrDefault(c => c.Type == claimType)?.Value;
                if (!string.IsNullOrEmpty(value))
                    return value;
            }

            return null;
        }
    }
}