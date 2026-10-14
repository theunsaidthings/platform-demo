using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using platform_demo.Utility;
using System.Text.RegularExpressions;

namespace platform_demo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<HealthController> _logger;

        public HealthController(IConfiguration configuration, ILogger<HealthController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            // Read connection string from environment variable ONLY
            var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

            // Alternative: You could also use _configuration.GetConnectionString("DefaultConnection")
            // but that would read from appsettings.json, which we're avoiding for secrets

            var maskedConnectionString = ConfigurationUtility.MaskConnectionString(connectionString);

            var healthStatus = new
            {
                status = "Healthy and Hot Reloading!!!",
                timestamp = DateTime.UtcNow,
                database = new
                {
                    configured = !string.IsNullOrWhiteSpace(connectionString),
                    connectionString = maskedConnectionString, // Safe to log
                    host = ExtractHost(connectionString),
                    database_name = ExtractDatabaseName(connectionString)
                },
                environment = new
                {
                    environment_name = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development",
                    port = Environment.GetEnvironmentVariable("ASPNETCORE_URLS")?.Split(';').FirstOrDefault() ?? "5000"
                }
            };

            var message = $"Health check performed. DB Configured: IsConfigured: {!string.IsNullOrWhiteSpace(connectionString)}";

            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                _logger.LogInformation(message);
            }
            else
            {
                _logger.LogError(message);
            }
            

            return Ok(healthStatus);
        }

        [HttpGet("liveness")]
        public IActionResult CheckLiveness()
        {
            return Ok(new { status = "Alive" });
        }
        
        [HttpGet("readiness")]
        public IActionResult CheckReadiness()
        {
            var dbConn = _configuration["DB_CONNECTION_STRING"];
            if (string.IsNullOrEmpty(dbConn))
            {
                _logger.LogWarning("Readiness probe failed: Database connection string is missing.");
                return StatusCode(503); // 503 Service Unavailable
            }
            return Ok(new { status = "Ready", databaseConfigured = true });
        }

        private static string? ExtractHost(string? connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                return null;

            var hostMatch = Regex.Match(connectionString, @"(Server|Host|Data Source)\s*=\s*([^;]+)", RegexOptions.IgnoreCase);

            return hostMatch.Success ? hostMatch.Groups[2].Value.Trim() : null;
        }

        private static string? ExtractDatabaseName(string? connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                return null;

            var dbMatch = Regex.Match(connectionString, @"(Database|Initial Catalog)\s*=\s*([^;]+)", RegexOptions.IgnoreCase);

            return dbMatch.Success ? dbMatch.Groups[2].Value.Trim() : null;
        }
    }
}
