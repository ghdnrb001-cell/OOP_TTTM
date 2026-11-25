namespace TTTM.Core.Config;

/// <summary>
/// Provides access to externalized database connection strings and system configuration settings.
/// </summary>
public interface IDatabaseConfig
{
    /// <summary>
    /// Gets the primary database connection string (<c>DefaultConnection</c>).
    /// </summary>
    string ConnectionString { get; }

    /// <summary>
    /// Gets the full path to the loaded external configuration file.
    /// </summary>
    string ConfigFilePath { get; }

    /// <summary>
    /// Gets the configured SQL command timeout in seconds.
    /// </summary>
    int CommandTimeoutSeconds { get; }

    /// <summary>
    /// Gets the configured BCrypt work factor.
    /// </summary>
    int BCryptWorkFactor { get; }

    /// <summary>
    /// Gets the maximum failed login attempts permitted before lockout.
    /// </summary>
    int MaxFailedLoginAttempts { get; }

    /// <summary>
    /// Gets the account lockout duration in minutes.
    /// </summary>
    int LockoutMinutes { get; }

    /// <summary>
    /// Gets the display name of the system.
    /// </summary>
    string SystemName { get; }

    /// <summary>
    /// Gets the standard currency display format string.
    /// </summary>
    string CurrencyFormat { get; }

    /// <summary>
    /// Gets the standard date display format string.
    /// </summary>
    string DateFormat { get; }

    /// <summary>
    /// Retrieves a named connection string from <c>ConnectionStrings</c>.
    /// </summary>
    string GetConnectionString(string name = DatabaseConfig.DefaultConnectionName);

    /// <summary>
    /// Returns a display-safe connection string with password fields masked.
    /// </summary>
    string GetMaskedConnectionString(string name = DatabaseConfig.DefaultConnectionName);
}
