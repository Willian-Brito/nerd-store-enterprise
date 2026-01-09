using Microsoft.IdentityModel.Tokens;

namespace NSE.Security.Jwt.Extensions;

public sealed class JwkList
{
    public DateTime When { get; set; }
    public JsonWebKeySet Jwks { get; set; }
    
    public JwkList(JsonWebKeySet jwkTaskResult)
    {
        Jwks = jwkTaskResult;
        When = DateTime.Now;
    }
}