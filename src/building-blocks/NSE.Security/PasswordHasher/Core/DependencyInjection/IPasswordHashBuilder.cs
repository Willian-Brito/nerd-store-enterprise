
using NSE.Security.PasswordHasher.Core;

namespace Microsoft.Extensions.DependencyInjection;

public interface IPasswordHashBuilder
{
    /// <summary>Gets the services.</summary>
    /// <value>The services.</value>
    IServiceCollection Services { get; }
    ImprovedPasswordHasherOptions Options { get; }
    IPasswordHashBuilder WithMemLimit(int memLimit);

    IPasswordHashBuilder WithOpsLimit(long opsLimit);

    IPasswordHashBuilder WithStrengthen(PasswordHasherStrength strength);
    IPasswordHashBuilder ChangeWorkFactor(int workFactor);
    IPasswordHashBuilder ChangeSaltRevision(BcryptSaltRevision saltRevision);
}