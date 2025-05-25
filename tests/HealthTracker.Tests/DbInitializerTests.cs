using Dapper;
using FluentAssertions;
using HealthTracker.Api.Data;
using Microsoft.Data.Sqlite;

namespace HealthTracker.Tests;

public class DbInitializerTests
{
    [Fact]
    public async Task InitializeAsync_ShouldCreateWeighInsAndRunsTables()
    {
        // Arrange - Use a named in-memory database for sharing between connections
        var connectionString = "Data Source=test_db;Mode=Memory;Cache=Shared";
        using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync();

        var connectionFactory = new SqliteConnectionFactory(connectionString);
        var dbInitializer = new DbInitializer(connectionFactory);

        // Act
        await dbInitializer.InitializeAsync();

        // Assert - Use the same connection string to check for tables
        var tables = await connection.QueryAsync<string>(
            "SELECT name FROM sqlite_master WHERE type='table' AND name IN ('weighins', 'runs')");

        var tableList = tables.ToList();
        tableList.Should().Contain("weighins");
        tableList.Should().Contain("runs");
        tableList.Should().HaveCount(2);
    }
}
