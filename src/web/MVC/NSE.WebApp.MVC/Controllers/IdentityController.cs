using Microsoft.AspNetCore.Mvc;
using NSE.WebApp.MVC.Models;
using NSE.WebApp.MVC.Services.Auth;

namespace NSE.WebApp.MVC.Controllers;

[Route("users")]
public class IdentityController : MainController
{
    private readonly IAuthService _authService;
    
    public IdentityController(IAuthService authService)
    {
        _authService = authService;
    }
    
    [HttpGet]
    [Route("")]
    public IActionResult Register()
    {
        return View();
    }
    
    [HttpPost]
    [Route("")]
    public async Task<IActionResult> Register(UserRegister userRegister)
    {
        if (!ModelState.IsValid) return View(userRegister);

        var response = await _authService.Register(userRegister);

        if (ResponseHasErrors(response.ResponseResult)) return View(userRegister);

        await _authService.DoLogin(response);
        return RedirectToAction("Index", "Catalog");
    }
    
    [HttpGet]
    [Route("login")]
    public IActionResult Login(string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login(UserLogin userLogin, string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (!ModelState.IsValid) return View(userLogin);

        var loginResponse = await _authService.Login(userLogin);

        if (ResponseHasErrors(loginResponse.ResponseResult)) return View(userLogin);

        await _authService.DoLogin(loginResponse);

        if (string.IsNullOrEmpty(returnUrl)) return RedirectToAction("Index", "Catalog");

        return LocalRedirect(returnUrl);
    }
    
    [HttpGet]
    [Route("logout")]
    public async Task<IActionResult> Logout()
    {
        await _authService.Logout();
        return RedirectToAction("Index", "Catalog");
    }
}