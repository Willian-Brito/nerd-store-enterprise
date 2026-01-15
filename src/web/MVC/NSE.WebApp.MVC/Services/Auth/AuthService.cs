using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using NSE.Core.Communication;
using NSE.Core.Utilities;
using NSE.Security.Identity.User;
using NSE.WebApp.MVC.Extensions;
using NSE.WebApp.MVC.Models;

namespace NSE.WebApp.MVC.Services.Auth;

public class AuthService : Service, IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAspNetUser _user;

    public AuthService(
        HttpClient httpClient,
        IOptions<AppSettings> settings,
        IAspNetUser user,
        IHttpContextAccessor httpContextAccessor
    )
    {
        httpClient.BaseAddress = new Uri(settings.Value.AuthUrl);

        _httpClient = httpClient;
        _user = user;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<UserLoginResponse> Login(UserLogin userLogin)
    {
        var loginContent = GetContent(userLogin);
        var response = await _httpClient.PostAsync("/api/identity/auth", loginContent);

        if (!ManageResponseErrors(response))
            return new UserLoginResponse
            {
                ResponseResult = await DeserializeResponse<ResponseResult>(response)
            };

        return await DeserializeResponse<UserLoginResponse>(response);
    }

    public async Task<UserLoginResponse> Register(UserRegister userRegister)
    {
        var userRegistrationContent = GetContent(userRegister);
        var response = await _httpClient.PostAsync("/api/identity/new-account", userRegistrationContent);

        if (!ManageResponseErrors(response))
            return new UserLoginResponse
            {
                ResponseResult = await DeserializeResponse<ResponseResult>(response)
            };

        return await DeserializeResponse<UserLoginResponse>(response);
    }

    public async Task DoLogin(UserLoginResponse response)
    {
        var token = FormatToken(response.AccessToken);

        var claims = new List<Claim>
        {
            new("JWT", response.AccessToken),
            new ("RefreshToken", response.RefreshToken)
        };
        claims.AddRange(token.Claims);

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        
        var authProperties = new AuthenticationProperties
        {
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8),
            IsPersistent = true
        };

        await _httpContextAccessor.HttpContext!.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            claimsPrincipal,
            authProperties
        );
    }

    public async Task Logout()
    {
        await _httpContextAccessor.HttpContext!.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            null
        );
    }

    public bool ExpiredToken()
    {
        var jwt = _user.GetUserToken();
        if (jwt.IsMissing()) return false;

        var token = FormatToken(jwt);
        return token.ValidTo.ToLocalTime() < DateTime.Now;
    }

    public async Task<bool> ValidRefreshToken()
    {
        var refreshToken = await UseRefreshToken(_user.GetUserRefreshToken());

        if (refreshToken.AccessToken == null || refreshToken.ResponseResult != null) return false;
        
        await DoLogin(refreshToken);
        
        return true;
    }
    
    private async Task<UserLoginResponse> UseRefreshToken(string refreshToken)
    {
        var refreshTokenContent = GetContent(refreshToken);

        var response = await _httpClient.PostAsync("/api/identity/refresh-token", refreshTokenContent);

        if (!ManageResponseErrors(response))
            return new UserLoginResponse
            {
                ResponseResult = await DeserializeResponse<ResponseResult>(response)
            };

        return await DeserializeResponse<UserLoginResponse>(response);
    }
    
    private static JwtSecurityToken FormatToken(string jwtToken)
    {
        return new JwtSecurityTokenHandler().ReadToken(jwtToken) as JwtSecurityToken;
    }
}