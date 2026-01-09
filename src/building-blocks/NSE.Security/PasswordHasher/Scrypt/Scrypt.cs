using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using NSE.Security.PasswordHasher.Core;
using NSE.Security.PasswordHasher.Core.Utilities;
using Sodium;

namespace NSE.Security.PasswordHasher.Scrypt;

public class Scrypt<TUser> : IPasswordHasher<TUser> where TUser : class
{
    private readonly PasswordHasher<TUser> _identityHasher;
    private readonly ImprovedPasswordHasherOptions _options;

    /// <summary>
    /// Creates a new instance of <see cref="PasswordHasher{TUser}"/>.
    /// </summary>
    /// <param name="identityHasher">AspNet Identity PasswordHasher</param>
    /// <param name="optionsAccessor">The options for this instance.</param>
    public Scrypt(PasswordHasher<TUser> identityHasher, IOptions<ImprovedPasswordHasherOptions> optionsAccessor = null)
    {
        _identityHasher = identityHasher;
        _options = optionsAccessor?.Value ?? new ImprovedPasswordHasherOptions();
    }

    public string HashPassword(TUser user, string password)
    {
        ArgumentNullException.ThrowIfNull(password);
        ArgumentNullException.ThrowIfNull(user);

        if (_options.OpsLimit.HasValue && _options.MemLimit.HasValue)
            return PasswordHash.ScryptHashString(password, _options.OpsLimit.Value, _options.MemLimit.Value);

        switch (_options.Strength)
        {
            case PasswordHasherStrength.Interactive:
                return PasswordHash.ScryptHashString(password);
            case PasswordHasherStrength.Moderate:
                return PasswordHash.ScryptHashString(password, PasswordHash.Strength.MediumSlow);
            case PasswordHasherStrength.Sensitive:
                return PasswordHash.ScryptHashString(password, PasswordHash.Strength.Sensitive);
            default:
                throw new ArgumentOutOfRangeException();
        }
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

        return PasswordHash.ScryptHashStringVerify(hashedPassword, providedPassword)
            ? PasswordVerificationResult.Success
            : PasswordVerificationResult.Failed;
    }
}