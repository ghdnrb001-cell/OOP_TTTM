namespace TTTM.Core.Security;

/// <summary>
/// Defines password hashing and verification operations using salted BCrypt.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Gets the configured BCrypt work factor (cost factor).
    /// </summary>
    int WorkFactor { get; }

    /// <summary>
    /// Hashes a plain-text password using a unique per-password random salt.
    /// </summary>
    /// <param name="plainPassword">The non-empty plain-text password to hash.</param>
    /// <returns>A BCrypt hash string containing algorithm version, cost factor, salt, and digest.</returns>
    string HashPassword(string plainPassword);

    /// <summary>
    /// Verifies a plain-text password against a stored BCrypt hash.
    /// </summary>
    /// <param name="plainPassword">The candidate plain-text password.</param>
    /// <param name="passwordHash">The stored BCrypt hash.</param>
    /// <returns><see langword="true"/> if the password matches the hash; otherwise, <see langword="false"/>.</returns>
    bool VerifyPassword(string plainPassword, string passwordHash);

    /// <summary>
    /// Determines whether an existing BCrypt hash should be re-hashed with the current work factor.
    /// </summary>
    /// <param name="passwordHash">The stored BCrypt hash.</param>
    /// <returns><see langword="true"/> if the hash uses a lower cost factor or is invalid; otherwise, <see langword="false"/>.</returns>
    bool NeedsRehash(string passwordHash);
}
