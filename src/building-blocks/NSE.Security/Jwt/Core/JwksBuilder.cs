using Microsoft.Extensions.DependencyInjection;
using NSE.Security.Jwt.Core.Interfaces;

namespace NSE.Security.Jwt.Core;

public class JwksBuilder : IJwksBuilder
{
    public JwksBuilder(IServiceCollection services)
    {
        Services = services ?? throw new ArgumentNullException(nameof(services));
    }

    public IServiceCollection Services { get; }
}