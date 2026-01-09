using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace NSE.Security.Identity.Jwt;

internal static class Extensions
{
    public static void RemoveRefreshToken(this ICollection<Claim> claims)
    {
        var refreshToken = claims.FirstOrDefault(f => f.Type == "LastRefreshToken");
        if (refreshToken is not null)
            claims.Remove(refreshToken);
    }

    public static string GetJwtId(this ClaimsIdentity principal)
    {
        return principal.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
    }
}