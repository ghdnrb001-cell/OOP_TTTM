using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using TTTM.Core.Security;

namespace TTTM.Core.Config;

/// <summary>
/// Loads database connection strings and system parameters from an external <c>appsettings.json</c> file
/// without hard-coded machine paths or embedded credentials.
/// </summary>
public sealed partial class DatabaseConfig : IDatabaseConfig
{
    /// <summary>
    /// Default external JSON configuration file name.
    /// </summary>
    public const string DefaultConfigFileName = "appsettings.json";

    /// <summary>
    /// Default connection string key in the <c>ConnectionStrings</c> section.
    /// </summary>
    public const string DefaultConnectionName = "DefaultConnection";

    public const int DefaultCommandTimeoutSeconds = 30;
    public const int DefaultMaxFailedLoginAttempts = 5;
    public const int DefaultLockoutMinutes = 15;
    public const string DefaultCurrencyFormat = "#,##0 VNĐ";
    public const string DefaultDateFormat = "dd/MM/yyyy";

    private const int MaxAncestorSearchDepth = 6;

    private static readonly string[] PlaceholderMarkers =
    [
        "YOUR_SQL_SERVER_INSTANCE",
        "YOUR_USERNAME",
        "YOUR_STRONG_PASSWORD"
    ];

    private readonly IConfiguration _configuration;

    private DatabaseConfig(IConfiguration configuration, string configFilePath)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        ConfigFilePath = configFilePath;

        ConnectionString = GetConnectionString(DefaultConnectionName);
        CommandTimeoutSeconds = ReadIntInRange("DatabaseSettings:CommandTimeoutSeconds", DefaultCommandTimeoutSeconds, 1, 3600);
        BCryptWorkFactor = ReadIntInRange(
            "SecuritySettings:BCryptWorkFactor",
            PasswordHasher.DefaultWorkFactor,
            PasswordHasher.MinWorkFactor,
            PasswordHasher.MaxWorkFactor);
        MaxFailedLoginAttempts = ReadIntInRange("SecuritySettings:MaxFailedLoginAttempts", DefaultMaxFailedLoginAttempts, 1, 100);
        LockoutMinutes = ReadIntInRange("SecuritySettings:LockoutMinutes", DefaultLockoutMinutes, 1, 1440);

        SystemName = ReadString("ApplicationSettings:SystemName", "Hệ thống Quản lý Trung tâm Thương mại (TTTM)");
        CurrencyFormat = ReadString("ApplicationSettings:CurrencyFormat", DefaultCurrencyFormat);
        DateFormat = ReadString("ApplicationSettings:DateFormat", DefaultDateFormat);
    }

    /// <inheritdoc />
    public string ConnectionString { get; }

    /// <inheritdoc />
    public string ConfigFilePath { get; }

    /// <inheritdoc />
    public int CommandTimeoutSeconds { get; }

    /// <inheritdoc />
    public int BCryptWorkFactor { get; }

    /// <inheritdoc />
    public int MaxFailedLoginAttempts { get; }

    /// <inheritdoc />
    public int LockoutMinutes { get; }

    /// <inheritdoc />
    public string SystemName { get; }

    /// <inheritdoc />
    public string CurrencyFormat { get; }

    /// <inheritdoc />
    public string DateFormat { get; }

    /// <summary>
    /// Loads configuration from the specified file path or locates <c>appsettings.json</c> relative to the runtime base directory.
    /// </summary>
    /// <param name="configFilePath">Optional explicit path to a JSON configuration file.</param>
    /// <returns>A validated <see cref="DatabaseConfig"/> instance.</returns>
    /// <exception cref="FileNotFoundException">Thrown when no configuration file can be found.</exception>
    /// <exception cref="InvalidDataException">Thrown when the configuration file contains invalid JSON.</exception>
    /// <exception cref="InvalidOperationException">Thrown when required connection strings or settings are missing or invalid.</exception>
    public static DatabaseConfig Load(string? configFilePath = null)
    {
        string resolvedPath = ResolveConfigFilePath(configFilePath);
        string? directory = Path.GetDirectoryName(resolvedPath);
        string fileName = Path.GetFileName(resolvedPath);

        if (string.IsNullOrWhiteSpace(directory) || string.IsNullOrWhiteSpace(fileName))
        {
            throw new InvalidOperationException($"Đường dẫn tệp cấu hình không hợp lệ: '{resolvedPath}'.");
        }

        try
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(directory)
                .AddJsonFile(fileName, optional: false, reloadOnChange: false)
                .Build();

            return new DatabaseConfig(configuration, resolvedPath);
        }
        catch (Exception ex) when (ex is FormatException or InvalidDataException)
        {
            throw new InvalidDataException(
                $"Tệp cấu hình '{resolvedPath}' không đúng định dạng JSON hợp lệ: {ex.Message}",
                ex);
        }
    }

    /// <inheritdoc />
    public string GetConnectionString(string name = DefaultConnectionName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        string? connectionString = _configuration.GetConnectionString(name);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"Không tìm thấy chuỗi kết nối '{name}' trong mục 'ConnectionStrings' của tệp '{ConfigFilePath}'.");
        }

        foreach (string marker in PlaceholderMarkers)
        {
            if (connectionString.Contains(marker, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"Chuỗi kết nối '{name}' trong '{ConfigFilePath}' vẫn đang chứa giá trị mẫu '{marker}'. Vui lòng cập nhật thông tin SQL Server thực tế.");
            }
        }

        return connectionString;
    }

    /// <inheritdoc />
    public string GetMaskedConnectionString(string name = DefaultConnectionName)
    {
        string raw = GetConnectionString(name);
        return PasswordMaskRegex().Replace(raw, "$1=***");
    }

    private int ReadIntInRange(string key, int defaultValue, int minValue, int maxValue)
    {
        string? rawValue = _configuration[key];
        if (string.IsNullOrWhiteSpace(rawValue))
        {
            return defaultValue;
        }

        if (!int.TryParse(rawValue, out int parsed) || parsed < minValue || parsed > maxValue)
        {
            throw new InvalidOperationException(
                $"Tham số '{key}' có giá trị '{rawValue}' không hợp lệ (yêu cầu số nguyên trong khoảng [{minValue}, {maxValue}]).");
        }

        return parsed;
    }

    private string ReadString(string key, string defaultValue)
    {
        string? value = _configuration[key];
        return string.IsNullOrWhiteSpace(value) ? defaultValue : value.Trim();
    }

    private static string ResolveConfigFilePath(string? explicitPath)
    {
        if (explicitPath is not null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(explicitPath);
            string fullPath = Path.GetFullPath(explicitPath);
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException(
                    $"Không tìm thấy tệp cấu hình tại '{fullPath}'. Hãy sao chép 'appsettings.example.json' thành 'appsettings.json'.",
                    fullPath);
            }

            return fullPath;
        }

        foreach (string candidate in EnumerateCandidatePaths())
        {
            if (File.Exists(candidate))
            {
                return candidate;
            }
        }

        throw new FileNotFoundException(
            $"Không tìm thấy tệp '{DefaultConfigFileName}' tại thư mục thực thi hoặc thư mục 'src/TTTM.GUI'. " +
            "Hãy sao chép 'appsettings.example.json' thành 'appsettings.json' và cấu hình chuỗi kết nối 'DefaultConnection'.",
            DefaultConfigFileName);
    }

    private static IEnumerable<string> EnumerateCandidatePaths()
    {
        string baseDir = AppDomain.CurrentDomain.BaseDirectory;
        string currentDir = Directory.GetCurrentDirectory();

        yield return Path.Combine(baseDir, DefaultConfigFileName);
        yield return Path.Combine(currentDir, DefaultConfigFileName);

        foreach (string root in new[] { baseDir, currentDir })
        {
            DirectoryInfo? dir = new DirectoryInfo(root);
            for (int depth = 0; depth < MaxAncestorSearchDepth && dir is not null; depth++)
            {
                bool isSolutionRoot =
                    File.Exists(Path.Combine(dir.FullName, "TTTM.sln")) ||
                    File.Exists(Path.Combine(dir.FullName, "appsettings.example.json")) ||
                    Directory.Exists(Path.Combine(dir.FullName, "src", "TTTM.GUI")) ||
                    Directory.Exists(Path.Combine(dir.FullName, ".git"));

                if (isSolutionRoot)
                {
                    yield return Path.Combine(dir.FullName, DefaultConfigFileName);
                    yield return Path.Combine(dir.FullName, "src", "TTTM.GUI", DefaultConfigFileName);
                    yield break;
                }

                dir = dir.Parent;
            }
        }
    }

    [GeneratedRegex(@"(?i)(Password|Pwd)\s*=\s*(""[^""]*""|'[^']*'|[^;]*)", RegexOptions.CultureInvariant)]
    private static partial Regex PasswordMaskRegex();
}
