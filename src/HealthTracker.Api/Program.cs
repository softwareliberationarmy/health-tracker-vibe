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
public partial class Program { }
