using Microsoft.Extensions.DependencyInjection;

namespace NSE.Security.Jwt.Core.Interfaces;

public interface IJwksBuilder
{
    IServiceCollection Services { get; }
}