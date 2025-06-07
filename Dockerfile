FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ["src/HealthTracker.Api/HealthTracker.Api.csproj", "src/HealthTracker.Api/"]
COPY ["src/HealthTracker.Shared/HealthTracker.Shared.csproj", "src/HealthTracker.Shared/"]
RUN dotnet restore "src/HealthTracker.Api/HealthTracker.Api.csproj"

# Copy everything else and build
COPY ["src/HealthTracker.Api/", "src/HealthTracker.Api/"]
COPY ["src/HealthTracker.Shared/", "src/HealthTracker.Shared/"]
RUN dotnet publish "src/HealthTracker.Api/HealthTracker.Api.csproj" -c Release -o /app/publish --no-restore

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Create a directory for the database
RUN mkdir -p /data
VOLUME /data

# Set environment variable for database path
ENV HEALTH_TRACKER_DB_PATH="/data/healthtracker.db"

COPY --from=build /app/publish .
EXPOSE 8080

# Set the entry point
ENTRYPOINT ["dotnet", "HealthTracker.Api.dll"]
