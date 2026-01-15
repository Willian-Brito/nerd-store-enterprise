using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace NSE.Security.Identity.User;

public interface IAspNetUser
{
    string Name { get; }
    Guid GetUserId();
    string GetUserEmail();
    string GetUserToken();
    string GetUserRefreshToken();
    bool IsAuthenticated();
    bool IsInRole(string role);
    IEnumerable<Claim> GetUserClaims();
    HttpContext GetHttpContext();
}