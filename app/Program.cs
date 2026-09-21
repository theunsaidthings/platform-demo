
using platform_demo.Services;
using platform_demo.Utility;
using PlatformDemo.Services;

namespace platform_demo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Add at the very beginning of Program.cs
            DotNetEnv.Env.Load();

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddSingleton<IItemStore, ItemStore>();
            builder.Services.AddSingleton<DatabaseConfigService>();
            
            builder.Logging.ClearProviders();
            builder.Logging.AddJsonConsole(options =>
            {
                options.IncludeScopes = true;
                options.TimestampFormat = "yyyy-MM-ddTHH:mm:ssZ ";
                options.JsonWriterOptions = new System.Text.Json.JsonWriterOptions
                {
                    Indented = true // Production standard: one JSON object per line
                };
            });
            
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Read configuration from environment variables
            var dbConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
            if (string.IsNullOrEmpty(dbConnectionString))
            {
                throw new InvalidOperationException("CRITICAL: Failed to establish handshake with primary database replica.");
            }
            var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            // Log startup configuration (masked)
            var maskedConnectionString = ConfigurationUtility.MaskConnectionString(dbConnectionString);
            Console.WriteLine($"=== Application Starting ===");
            Console.WriteLine($"Environment: {environmentName}");
            Console.WriteLine($"Port: {port}");
            Console.WriteLine($"Database Connection: {maskedConnectionString}");
            Console.WriteLine($"==========================");

            // Optional: Fail fast if critical config is missing
            if (string.IsNullOrWhiteSpace(dbConnectionString))
            {
                Console.WriteLine("WARNING: DB_CONNECTION_STRING environment variable is not set!");
                Console.WriteLine("The application will start but database operations will fail.");
            }

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            // Try to use the PORT environment variable
            var urls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS") ?? $"http://localhost:{port}";
            app.Urls.Add(urls);

            Console.WriteLine($"Application started on: {urls}");

            app.Run();
        }
    }
}
