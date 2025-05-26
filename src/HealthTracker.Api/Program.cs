using HealthTracker.Api.Data;
using HealthTracker.Core.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();

// Register database services
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=health_tracker.db";
builder.Services.AddSingleton<IDbConnectionFactory>(new SqliteConnectionFactory(connectionString));
builder.Services.AddSingleton<DbInitializer>();

var app = builder.Build();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var dbInitializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
    await dbInitializer.InitializeAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Weight endpoints
app.MapPost("/weight", (WeighIn weighIn) =>
{
    return Results.Created("/weight/1", weighIn);
})
.WithName("CreateWeighIn");

app.MapGet("/weight/last/{count:int}", (int count) =>
{
    return Results.Ok(new List<WeighIn>());
})
.WithName("GetLastWeighIns");

// Run endpoints
app.MapPost("/run", (Run run) =>
{
    return Results.Created("/run/1", run);
})
.WithName("CreateRun");

app.MapGet("/run/last/{count:int}", (int count) =>
{
    return Results.Ok(new List<Run>());
})
.WithName("GetLastRuns");

app.Run();

// Make Program class accessible for testing
public partial class Program { }
