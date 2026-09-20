using platform_demo.Utility;

namespace PlatformDemo.Services;

public class DatabaseConfigService
{
    private readonly string? _connectionString;
    private readonly ILogger<DatabaseConfigService> _logger;

    public DatabaseConfigService(ILogger<DatabaseConfigService> logger)
    {
        _logger = logger;
        _connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
    }

    public bool IsConfigured => !string.IsNullOrWhiteSpace(_connectionString);

    public string GetMaskedConnectionString()
        => ConfigurationUtility.MaskConnectionString(_connectionString);

    public string? GetConnectionString() => _connectionString;

    public void ValidateConfiguration()
    {
        if (string.IsNullOrWhiteSpace(_connectionString))
        {
            _logger.LogError("Database connection string is not configured!");
            throw new InvalidOperationException(
                "DB_CONNECTION_STRING environment variable must be set");
        }

        // Additional validation logic here
        if (!_connectionString.Contains("Server") && !_connectionString.Contains("Host"))
        {
            _logger.LogWarning("Connection string may be malformed: missing Server/Host");
        }
    }
}