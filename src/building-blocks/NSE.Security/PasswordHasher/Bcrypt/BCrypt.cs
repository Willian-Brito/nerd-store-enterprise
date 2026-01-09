using BCrypt.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using NSE.Security.PasswordHasher.Core;
using NSE.Security.PasswordHasher.Core.Utilities;

namespace NSE.Security.PasswordHasher.Bcrypt;

public class BCrypt<TUser> : IPasswordHasher<TUser> where TUser : class
{
    private readonly PasswordHasher<TUser> _identityHasher;
    private readonly ImprovedPasswordHasherOptions _options;

    /// <summary>
    /// Creates a new instance of <see cref="PasswordHasher{TUser}"/>.
    /// </summary>
    /// <param name="identityHasher">AspNet Identity PasswordHasher</param>
    /// <param name="optionsAccessor">The options for this instance.</param>
    public BCrypt(PasswordHasher<TUser> identityHasher, IOptions<ImprovedPasswordHasherOptions> optionsAccessor = null)
    {
        _identityHasher = identityHasher;
        _options = optionsAccessor?.Value ?? new ImprovedPasswordHasherOptions();
    }

    public string HashPassword(TUser user, string password)
    {
        ArgumentNullException.ThrowIfNull(password);
        ArgumentNullException.ThrowIfNull(user);

        return BCrypt.Net.BCrypt.HashPassword(password, _options.WorkFactor, GetSaltRevision());
    }

    /// <summary>
    /// From BcryptSaltRevision to SaltRevision
    /// </summary>
    /// <returns></returns>
    private SaltRevision GetSaltRevision()
    {
        switch (_options.SaltRevision)
        {
            case BcryptSaltRevision.Revision2:
                return SaltRevision.Revision2;
            case BcryptSaltRevision.Revision2A:
                return SaltRevision.Revision2A;
            case BcryptSaltRevision.Revision2B:
                return SaltRevision.Revision2B;
            case BcryptSaltRevision.Revision2X:
                return SaltRevision.Revision2X;
            case BcryptSaltRevision.Revision2Y:
                return SaltRevision.Revision2Y;
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
        return BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword)
            ? PasswordVerificationResult.Success
            : PasswordVerificationResult.Failed;
    }
}