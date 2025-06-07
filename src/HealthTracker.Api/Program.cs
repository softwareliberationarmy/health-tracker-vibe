using HealthTracker.Api.Services;
using HealthTracker.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Register application services
builder.Services.AddSingleton<IDatabaseService, DatabaseService>();
builder.Services.AddSingleton<IHealthService, HealthService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();

// Initialize the database
try
{
    using (var scope = app.Services.CreateScope())
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Initializing database on startup");

        var databaseService = scope.ServiceProvider.GetRequiredService<IDatabaseService>();
        await databaseService.InitializeDatabaseAsync();
        // Verify database is healthy after initialization
        bool isHealthy = await databaseService.IsHealthy();
        if (!isHealthy)
        {
            logger.LogError("Database initialization completed but health check failed");
            throw new InvalidOperationException("Database initialization failed health check");
        }

        logger.LogInformation("Database initialization completed successfully");
    }
}
catch (Exception ex)
{
    // Log error but allow application to continue (it will fail health checks)
    using (var scope = app.Services.CreateScope())
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error during database initialization");
    }
}

// Add the /health endpoint to check database status
app.MapGet("/health", async (IDatabaseService databaseService) =>
{
    var isHealthy = await databaseService.IsHealthy();
    if (isHealthy)
    {
        return Results.Ok(new { Status = "Healthy", Message = "Database is operational" });
    }
    return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
})
.WithName("HealthCheck")
.WithDescription("Checks database health")
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status503ServiceUnavailable);

// Add the /about endpoint
app.MapGet("/about", async (IHealthService healthService) =>
{
    var aboutInfo = await healthService.GetAboutInfoAsync();
    return Results.Ok(aboutInfo);
})
.WithName("GetAboutInfo")
.WithDescription("Returns version and health statistics information")
.Produces<AboutResponse>(StatusCodes.Status200OK);

app.Run();

// Make the Program class accessible for integration testing
public partial class Program
{
    protected Program() { } // Protected constructor to fix compiler warning
}
