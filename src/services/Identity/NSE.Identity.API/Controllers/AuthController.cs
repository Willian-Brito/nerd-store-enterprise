using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using NSE.Core.Messages.Base;
using NSE.Core.Messages.Integration;
using NSE.Identity.API.Models;
using NSE.Queue.Abstractions;
using NSE.Security.Identity.Interfaces;
using NSE.Security.Jwt.Core.Interfaces;
using NSE.WebAPI.Core.Controllers;

namespace NSE.Identity.API.Controllers;

[Route("api/identity")]
public class AuthController : MainController
{
    private readonly IQueue _queue;
    private readonly IJwtBuilder _jwtBuilder;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    
    public AuthController(
        IJwtBuilder jwtBuilder,
        SignInManager<IdentityUser> signInManager,
        UserManager<IdentityUser> userManager,
        IQueue queue
    )
    {
        _jwtBuilder = jwtBuilder;
        _signInManager = signInManager;
        _userManager = userManager;
        _queue = queue;
    }
    
    [HttpPost("new-account")]
    public async Task<ActionResult> Register(NewUser newUser)
    {
        if (!ModelState.IsValid) return CustomResponse(ModelState);
    
        var user = new IdentityUser
        {
            UserName = newUser.Email,
            Email = newUser.Email,
            EmailConfirmed = true
        };
    
        var result = await _userManager.CreateAsync(user, newUser.Password);
    
        if (result.Succeeded)
        {
            var customerResult = await RegisterUser(newUser);
    
            if (!customerResult.ValidationResult.IsValid)
            {
                await _userManager.DeleteAsync(user);
                return CustomResponse(customerResult.ValidationResult);
            }
    
            var jwt = await _jwtBuilder
                .WithEmail(newUser.Email)
                .WithJwtClaims()
                .WithUserClaims()
                .WithUserRoles()
                .WithRefreshToken()
                .BuildUserResponse();
    
            return CustomResponse(jwt);
        }
    
        foreach (var error in result.Errors) AddErrorToStack(error.Description);
    
        return CustomResponse();
    }
    
    [HttpPost("auth")]
    public async Task<ActionResult> Login(UserLogin userLogin)
    {
        if (!ModelState.IsValid) return CustomResponse(ModelState);
    
        var result = await _signInManager.PasswordSignInAsync(
            userLogin.Email, 
            userLogin.Password,
            false, 
            true
        );
    
        if (result.Succeeded)
        {
            var jwt = await _jwtBuilder
                .WithEmail(userLogin.Email)
                .WithJwtClaims()
                .WithUserClaims()
                .WithUserRoles()
                .WithRefreshToken()
                .BuildUserResponse();
            return CustomResponse(jwt);
        }
    
        if (result.IsLockedOut)
        {
            AddErrorToStack("User temporary blocked. Too many tries.");
            return CustomResponse();
        }
    
        AddErrorToStack("User or Password incorrect");
        return CustomResponse();
    }
    
    private async Task<ResponseMessage> RegisterUser(NewUser newUser)
    {
        var user = await _userManager.FindByEmailAsync(newUser.Email);
        ArgumentNullException.ThrowIfNull(user);

        var userRegistered =  new UserRegisteredIntegrationEvent(
            Guid.Parse(user.Id), 
            newUser.Name, 
            newUser.Email, 
            newUser.SocialNumber
        );

        try
        {
            var response = await _queue.RequestAsync<UserRegisteredIntegrationEvent, ResponseMessage>(userRegistered);
            return response;
        }
        catch (Exception)
        {
            await _userManager.DeleteAsync(user);
            throw;
        }
    }
    
    [HttpPost("refresh-token")]
    public async Task<ActionResult> RefreshToken([FromBody] string refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
        {
            AddErrorToStack("Invalid Refresh Token");
            return CustomResponse();
        }

        var token = await _jwtBuilder.ValidateRefreshToken(refreshToken);

        if (!token.IsValid)
        {
            AddErrorToStack("Expired Refresh Token");
            return CustomResponse();
        }

        var jwt = await _jwtBuilder
            .WithUserId(token.UserId)
            .WithJwtClaims()
            .WithUserClaims()
            .WithUserRoles()
            .WithRefreshToken()
            .BuildUserResponse();

        return CustomResponse(jwt);
    }
    
    [HttpPost("validate-jwt")]
    public async Task<ActionResult> ValidateJwt([FromServices] IJwtService jwtService, [FromForm] string jwt)
    {
        var handler = new JsonWebTokenHandler();

        var result = await handler.ValidateTokenAsync(jwt, new TokenValidationParameters
        {
            ValidIssuer = "https://nerdstore.academy",
            ValidAudience = "NerdStore",
            ValidateAudience = true,
            ValidateIssuer = true,
            RequireSignedTokens = false,
            IssuerSigningKey = await jwtService.GetCurrentSecurityKey()
        });

        if (!result.IsValid)
            return BadRequest();

        return Ok(result.Claims.Select(s => new { s.Key, s.Value }));
    }
}