using System.Text.RegularExpressions;

namespace platform_demo.Utility
{   
    public static partial class ConfigurationUtility
    {
        /// <summary>
        /// Masks the password in a connection string for safe logging.
        /// </summary>
        public static string MaskConnectionString(string? connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return "[NOT CONFIGURED]";
            }

            // Mask the password field specifically
            var masked = Regex.Replace(connectionString, @"(Password\s*=\s*)([^;]+)", "$1***", RegexOptions.IgnoreCase);

            // Also mask User Id if present (sometimes contains sensitive info)
            masked = Regex.Replace(masked, @"(User Id\s*=\s*)([^;]+)", "$1***", RegexOptions.IgnoreCase);

            return masked;
        }

        public static string GetEnvironmentVariableOrThrow(string variableName, string defaultValue = "")
        {
            var value = Environment.GetEnvironmentVariable(variableName);
            return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
        }
    }
}
