using BCrypt.Net;

namespace TTTM.Core.Security;

/// <summary>
/// Provides salted BCrypt password hashing and verification with cost factor 11 by default.
/// </summary>
public sealed class PasswordHasher : IPasswordHasher
{
    /// <summary>
    /// Default BCrypt cost factor required by project security standards.
    /// </summary>
    public const int DefaultWorkFactor = 11;

    /// <summary>
    /// Minimum allowed BCrypt cost factor.
    /// </summary>
    public const int MinWorkFactor = 4;

    /// <summary>
    /// Maximum allowed BCrypt cost factor.
    /// </summary>
    public const int MaxWorkFactor = 31;

    private readonly int _workFactor;

    /// <summary>
    /// Initializes a new instance of <see cref="PasswordHasher"/>.
    /// </summary>
    /// <param name="workFactor">BCrypt work factor (4 to 31). Defaults to <see cref="DefaultWorkFactor"/> (11).</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="workFactor"/> is outside [4, 31].</exception>
    public PasswordHasher(int workFactor = DefaultWorkFactor)
    {
        if (workFactor is < MinWorkFactor or > MaxWorkFactor)
        {
            throw new ArgumentOutOfRangeException(
                nameof(workFactor),
                workFactor,
                $"BCrypt work factor phải nằm trong khoảng [{MinWorkFactor}, {MaxWorkFactor}].");
        }

        _workFactor = workFactor;
    }

    /// <inheritdoc />
    public int WorkFactor => _workFactor;

    /// <inheritdoc />
    /// <exception cref="ArgumentException">Thrown when <paramref name="plainPassword"/> is null, empty, or whitespace.</exception>
    public string HashPassword(string plainPassword)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(plainPassword);
        return BCrypt.Net.BCrypt.HashPassword(plainPassword, _workFactor);
    }

    /// <inheritdoc />
    public bool VerifyPassword(string plainPassword, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(plainPassword) || string.IsNullOrWhiteSpace(passwordHash))
        {
            return false;
        }

        try
        {
            return BCrypt.Net.BCrypt.Verify(plainPassword, passwordHash);
        }
        catch (SaltParseException)
        {
            return false;
        }
    }

    /// <inheritdoc />
    public bool NeedsRehash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            return true;
        }

        try
        {
            return BCrypt.Net.BCrypt.PasswordNeedsRehash(passwordHash, _workFactor);
        }
        catch (SaltParseException)
        {
            return true;
        }
    }
}
