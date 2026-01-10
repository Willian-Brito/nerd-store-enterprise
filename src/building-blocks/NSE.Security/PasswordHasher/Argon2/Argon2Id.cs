using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using NSE.Security.PasswordHasher.Core;
using NSE.Security.PasswordHasher.Core.Utilities;
using Sodium;

namespace NSE.Security.PasswordHasher.Argon2;

public class Argon2Id<TUser> : IPasswordHasher<TUser> where TUser : class
{
    private readonly PasswordHasher<TUser> _identityHasher;
    private readonly ImprovedPasswordHasherOptions _options;

    /// <summary>
    /// Creates a new instance of <see cref="PasswordHasher{TUser}"/>.
    /// </summary>
    /// <param name="identityHasher">AspNet Identity PasswordHasher</param>
    /// <param name="optionsAccessor">The options for this instance.</param>
    public Argon2Id(PasswordHasher<TUser> identityHasher, IOptions<ImprovedPasswordHasherOptions> optionsAccessor = null)
    {
        _identityHasher = identityHasher;
        _options = optionsAccessor?.Value ?? new ImprovedPasswordHasherOptions();
    }

    public string HashPassword(TUser user, string password)
    {
        ArgumentNullException.ThrowIfNull(password);
        ArgumentNullException.ThrowIfNull(user);

        if (_options.OpsLimit.HasValue && _options.MemLimit.HasValue)
            return PasswordHash.ArgonHashString(password, _options.OpsLimit.Value, _options.MemLimit.Value);

        return _options.Strength switch
        {
            PasswordHasherStrength.Interactive => PasswordHash.ArgonHashString(password).Replace("\0", string.Empty),
            PasswordHasherStrength.Moderate => PasswordHash.ArgonHashString(password, PasswordHash.StrengthArgon.Moderate).Replace("\0", string.Empty),
            PasswordHasherStrength.Sensitive => PasswordHash.ArgonHashString(password, PasswordHash.StrengthArgon.Sensitive).Replace("\0", string.Empty),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public PasswordVerificationResult VerifyHashedPassword(TUser user, string hashedPassword, string providedPassword)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(hashedPassword);
        ArgumentNullException.ThrowIfNull(providedPassword);

        var info = new HashInfo(hashedPassword);
        if (info.IsAspNetV2 || info.IsAspNetV3)
        {
            var result = _identityHasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
            if (result == PasswordVerificationResult.Success ||
                result == PasswordVerificationResult.SuccessRehashNeeded)
                return PasswordVerificationResult.SuccessRehashNeeded;

            return PasswordVerificationResult.Failed;
        }

        return PasswordHash.ArgonHashStringVerify(hashedPassword, providedPassword)
            ? PasswordVerificationResult.Success
            : PasswordVerificationResult.Failed;
    }
}