using NSE.Security.Identity.Jwt.Model;

namespace NSE.Security.Identity.Interfaces;

public interface IJwtBuilder
{
    IJwtBuilder WithEmail(string email);
    IJwtBuilder WithUsername(string username);
    IJwtBuilder WithUserId(string id);
    IJwtBuilder WithJwtClaims();
    IJwtBuilder WithUserClaims();
    IJwtBuilder WithUserRoles();
    IJwtBuilder WithRefreshToken();
    Task<string> BuildToken();
    Task<UserResponse> BuildUserResponse();
    Task<RefreshTokenValidation> ValidateRefreshToken(string refreshToken);
}