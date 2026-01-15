using NSE.WebApp.MVC.Models;

namespace NSE.WebApp.MVC.Services.Auth;

public interface IAuthService
{
    Task<UserLoginResponse> Login(UserLogin userLogin);

    Task<UserLoginResponse> Register(UserRegister userRegister);

    Task DoLogin(UserLoginResponse response);
    Task Logout();

    bool ExpiredToken();

    Task<bool> ValidRefreshToken();
}