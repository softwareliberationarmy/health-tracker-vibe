using System;
using System.Data;
using System.Threading.Tasks;

namespace HealthTracker.Api.Services
{
    /// <summary>
    /// Interface for database operations
    /// </summary>
    public interface IDatabaseService
    {
        /// <summary>
        /// Initializes the database and creates tables if they don't exist
        /// </summary>
        Task InitializeDatabaseAsync();

        /// <summary>
        /// Gets a connection to the database
        /// </summary>
        /// <returns>An open database connection</returns>
        Task<IDbConnection> GetConnectionAsync();

        /// <summary>
        /// Checks if the database is available and healthy
        /// </summary>
        /// <returns>True if the database is healthy, false otherwise</returns>
        Task<bool> IsHealthy();

        /// <summary>
        /// Gets the count of weigh-ins in the database
        /// </summary>
        Task<int> GetWeighInsCountAsync();

        /// <summary>
        /// Gets the count of runs in the database
        /// </summary>
        Task<int> GetRunsCountAsync();

        /// <summary>
        /// Gets the date of the last weigh-in
        /// </summary>
        Task<DateTime?> GetLastWeighInDateAsync();

        /// <summary>
        /// Gets the date of the last run
        /// </summary>
        Task<DateTime?> GetLastRunDateAsync();
    }
}
