using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;

namespace platform_demo.Utility
{
    public class DataAccessUtility
    {
        private readonly string _connectionString;

        public DataAccessUtility(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        /// <summary>
        /// Reads data from the database. 
        /// Note: Since there is no real DB yet, this returns mock data, 
        /// but the connection lifecycle is fully structured.
        /// </summary>
        public async Task<IEnumerable<dynamic>> ReadSampleDataAsync()
        {
            // In a real scenario, you would use a specific provider like SqlConnection, NpgsqlConnection, etc.
            // For demonstration, we use the abstract DbProviderFactory or a specific one.
            await using var connection = new SqlConnection(_connectionString);

            // We aren't actually opening it since there's no DB, but this shows the pattern.
            // await connection.OpenAsync(); 

            // Mocking the data read since the DB doesn't exist yet
            var mockData = new List<dynamic>
            {
                new { Id = 1, Name = "Sample Record 1", Source = "Mocked" },
                new { Id = 2, Name = "Sample Record 2", Source = "Mocked" }
            };

            return mockData;
        }
    }

}
