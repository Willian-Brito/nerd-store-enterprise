using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace NSE.Security.Identity.User;

public static class ClaimsPrincipalExtensions
{
    public static string GetUserId(this ClaimsPrincipal principal)
    {
        ArgumentNullException.ThrowIfNull(principal);

        var claim = principal.FindFirst(JwtRegisteredClaimNames.Sub);
        if (claim is null)
            claim = principal.FindFirst(ClaimTypes.NameIdentifier);

        return claim?.Value;
    }

    public static string GetUserEmail(this ClaimsPrincipal principal)
    {
        ArgumentNullException.ThrowIfNull(principal);
        
        var claim = principal.FindFirst(JwtRegisteredClaimNames.Sub);
        if (claim is null)
            claim = principal.FindFirst(ClaimTypes.Email);

        return claim?.Value;
    }
    public static string GetUserId(this ClaimsIdentity principal)
    {
        ArgumentNullException.ThrowIfNull(principal);

        var claim = principal.FindFirst(JwtRegisteredClaimNames.Sub);
        if (claim is null)
            claim = principal.FindFirst(ClaimTypes.NameIdentifier);

        return claim?.Value;
    }

    public static string GetUserEmail(this ClaimsIdentity principal)
    {
        ArgumentNullException.ThrowIfNull(principal);
        
        var claim = principal.FindFirst(JwtRegisteredClaimNames.Sub);
        if (claim is null)
            claim = principal.FindFirst(ClaimTypes.Email);

        return claim?.Value;
    }
    
    public static string GetUserToken(this ClaimsPrincipal principal)
    {
        ArgumentNullException.ThrowIfNull(principal);

        var claim = principal.FindFirst("JWT");
        return claim?.Value;
    }

    public static string GetUserRefreshToken(this ClaimsPrincipal principal)
    {
        ArgumentNullException.ThrowIfNull(principal);

        var claim = principal.FindFirst("RefreshToken");
        return claim?.Value;
    }
}