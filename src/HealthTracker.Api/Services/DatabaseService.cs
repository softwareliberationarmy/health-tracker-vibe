using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HealthTracker.Api.Services
{
    /// <summary>
    /// Service for SQLite database operations
    /// </summary>
    public class DatabaseService : IDatabaseService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DatabaseService> _logger;
        private readonly string _connectionString;

        public DatabaseService(IConfiguration configuration, ILogger<DatabaseService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            // Get connection string from configuration, or use environment variable if set
            var dbPath = Environment.GetEnvironmentVariable("HEALTH_TRACKER_DB_PATH");
            if (string.IsNullOrEmpty(dbPath))
            {
                _connectionString = _configuration.GetSection("ConnectionStrings:DefaultConnection").Value
                                   ?? "Data Source=healthtracker.db";
            }
            else
            {
                _connectionString = $"Data Source={dbPath}";
            }

            _logger.LogInformation("Using database connection: {ConnectionString}", _connectionString);
        }

        /// <inheritdoc/>
        public async Task InitializeDatabaseAsync()
        {
            _logger.LogInformation("Initializing database...");

            try
            {
                using var connection = await GetConnectionAsync();

                // Create weighins table if it doesn't exist
                await connection.ExecuteAsync(@"
                    CREATE TABLE IF NOT EXISTS weighins (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        date TEXT NOT NULL,
                        weight REAL NOT NULL CHECK(weight BETWEEN 100 AND 300),
                        bmi REAL NOT NULL,
                        fat REAL CHECK(fat BETWEEN 0 AND 100),
                        muscle REAL CHECK(muscle BETWEEN 0 AND 100),
                        restingMetab INTEGER CHECK(restingMetab > 1000),
                        visceralFat INTEGER CHECK(visceralFat BETWEEN 10 AND 30)
                    )");

                // Create runs table if it doesn't exist
                await connection.ExecuteAsync(@"
                    CREATE TABLE IF NOT EXISTS runs (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        date TEXT NOT NULL,
                        distance REAL NOT NULL CHECK(distance > 0),
                        distanceUnit TEXT NOT NULL,
                        time INTEGER NOT NULL
                    )");

                _logger.LogInformation("Database initialization completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing database");
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<IDbConnection> GetConnectionAsync()
        {
            var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }

        /// <inheritdoc/>
        public async Task<bool> IsHealthy()
        {
            try
            {
                using var connection = await GetConnectionAsync();
                await connection.ExecuteScalarAsync<int>("SELECT 1");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database health check failed");
                return false;
            }
        }

        /// <inheritdoc/>
        public async Task<int> GetWeighInsCountAsync()
        {
            try
            {
                using var connection = await GetConnectionAsync();
                return await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM weighins");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting weigh-ins count");
                return 0;
            }
        }

        /// <inheritdoc/>
        public async Task<int> GetRunsCountAsync()
        {
            try
            {
                using var connection = await GetConnectionAsync();
                return await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM runs");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting runs count");
                return 0;
            }
        }

        /// <inheritdoc/>
        public async Task<DateTime?> GetLastWeighInDateAsync()
        {
            try
            {
                using var connection = await GetConnectionAsync();
                var dateString = await connection.ExecuteScalarAsync<string>(
                    "SELECT date FROM weighins ORDER BY date DESC LIMIT 1");

                if (string.IsNullOrEmpty(dateString))
                    return null;

                return DateTime.Parse(dateString);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting last weigh-in date");
                return null;
            }
        }

        /// <inheritdoc/>
        public async Task<DateTime?> GetLastRunDateAsync()
        {
            try
            {
                using var connection = await GetConnectionAsync();
                var dateString = await connection.ExecuteScalarAsync<string>(
                    "SELECT date FROM runs ORDER BY date DESC LIMIT 1");

                if (string.IsNullOrEmpty(dateString))
                    return null;

                return DateTime.Parse(dateString);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting last run date");
                return null;
            }
        }
    }
}
