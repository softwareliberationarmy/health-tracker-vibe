using Dapper;
using Microsoft.Data.Sqlite;

namespace HealthTracker.Api;

public class DbInitializer
{
    private readonly string _connectionString;

    public DbInitializer(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task InitializeAsync()
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        // Create weighins table
        await connection.ExecuteAsync("""
            CREATE TABLE IF NOT EXISTS weighins (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                date TEXT NOT NULL,
                weight REAL NOT NULL CHECK(weight BETWEEN 100 AND 300),
                bmi REAL NOT NULL,
                fat REAL CHECK(fat BETWEEN 0 AND 100),
                muscle REAL CHECK(muscle BETWEEN 0 AND 100),
                restingMetab INTEGER CHECK(restingMetab > 1000),
                visceralFat INTEGER CHECK(visceralFat BETWEEN 10 AND 30)
            )
            """);

        // Create runs table
        await connection.ExecuteAsync("""
            CREATE TABLE IF NOT EXISTS runs (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                date TEXT NOT NULL,
                distance REAL CHECK(distance > 0),
                distanceUnit TEXT NOT NULL,
                time INTEGER NOT NULL
            )
            """);
    }
}
