
using NSE.Security.PasswordHasher.Core;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Helper for DI configuration
/// </summary>
public class PasswordHasherBuilder : IPasswordHashBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PasswordHasherBuilder"/> class.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <exception cref="System.ArgumentNullException">services</exception>
    public PasswordHasherBuilder(IServiceCollection services)
    {
        Services = services ?? throw new ArgumentNullException(nameof(services));
        Options = new ImprovedPasswordHasherOptions();
    }

    public ImprovedPasswordHasherOptions Options { get; }

    public IServiceCollection Services { get; }

    /// <summary>
    /// memlimit is the maximum amount of RAM that the function will use, in bytes.
    /// </summary>
    public IPasswordHashBuilder WithMemLimit(int memLimit)
    {
        Options.MemLimit = memLimit;
        return this;
    }

    /// <summary>
    /// opslimit represents a maximum amount of computations to perform.
    /// Raising this number will make the function require more CPU cycles to compute a key.
    /// This number must be between
    /// </summary>
    public IPasswordHashBuilder WithOpsLimit(long opsLimit)
    {
        Options.OpsLimit = opsLimit;
        return this;
    }

    /// <summary>
    /// Password Strengten. If set will change values from OpsLimit and MemLimit
    /// </summary>
    public IPasswordHashBuilder WithStrengthen(PasswordHasherStrength strength)
    {
        Options.Strength = strength;
        return this;
    }

    /// <summary>
    /// The log2 of the number of rounds of hashing to apply.
    /// The work factor therefore increases as 2^workFactor
    /// </summary>
    public IPasswordHashBuilder ChangeWorkFactor(int workFactor)
    {
        Options.WorkFactor = workFactor;
        return this;
    }

    /// <summary>
    /// Allows you to override the salt revision used in the output
    /// </summary>
    public IPasswordHashBuilder ChangeSaltRevision(BcryptSaltRevision saltRevision)
    {
        Options.SaltRevision = saltRevision;
        return this;
    }
}