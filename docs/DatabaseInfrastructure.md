# Database Infrastructure Documentation

## Overview

The Health Tracker application uses SQLite as its database engine. The database is created and initialized automatically when the API starts up. This document explains the database design, configuration, and usage.

## Database Schema

### Tables

#### `weighins` Table

| Column       | Type    | Constraints                             |
| ------------ | ------- | --------------------------------------- |
| id           | INTEGER | Primary Key, Auto Increment             |
| date         | TEXT    | ISO 8601 format, Required               |
| weight       | REAL    | Required, Between 100 and 300          |
| bmi          | REAL    | Required                                |
| fat          | REAL    | Between 0 and 100                       |
| muscle       | REAL    | Between 0 and 100                       |
| restingMetab | INTEGER | Greater than 1000                       |
| visceralFat  | INTEGER | Between 10 and 30                       |

#### `runs` Table

| Column       | Type    | Constraints                             |
| ------------ | ------- | --------------------------------------- |
| id           | INTEGER | Primary Key, Auto Increment             |
| date         | TEXT    | ISO 8601 format, Required               |
| distance     | REAL    | Greater than 0, Required                |
| distanceUnit | TEXT    | Required (e.g., "mi", "km")             |
| time         | INTEGER | Required (stored as seconds)            |

## Configuration

The database connection string can be configured in several ways (in order of precedence):

1. **Environment Variable**: `HEALTH_TRACKER_DB_PATH` 
   - Example: `HEALTH_TRACKER_DB_PATH=/data/healthtracker.db`

2. **appsettings.json**: 
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Data Source=healthtracker.db"
     }
   }
   ```

3. **Default**: If neither of the above is specified, the default is `Data Source=healthtracker.db`

## API Database Services

### `IDatabaseService` Interface

The `IDatabaseService` interface provides methods for database operations:

- `InitializeDatabaseAsync()` - Creates tables if they don't exist
- `GetConnectionAsync()` - Returns an open database connection
- `IsHealthy()` - Checks if the database is available
- `GetWeighInsCountAsync()` - Returns the count of weigh-in records
- `GetRunsCountAsync()` - Returns the count of run records
- `GetLastWeighInDateAsync()` - Returns the date of the last weigh-in
- `GetLastRunDateAsync()` - Returns the date of the last run

### Health Endpoint

The API provides a `/health` endpoint that checks database connectivity:

- `GET /health` - Returns 200 OK if database is healthy, 503 Service Unavailable otherwise

## Docker Considerations

When running in Docker:

1. Set the `HEALTH_TRACKER_DB_PATH` environment variable to a path in a mounted volume
2. Ensure the container has write permissions to the database file location
3. Use a data volume to persist the database file across container restarts

### Docker Setup

The application includes a Dockerfile and docker-compose.yml that configures the API with a persistent SQLite database:

```bash
# Build and start the Docker container
./build.ps1 -Target RunApi

# Access the API at http://localhost:50000

# Check the health status
curl http://localhost:50000/health

# Stop the Docker container
./build.ps1 -Target StopApi
```

The Docker container is configured with:
- Port mapping: 50000:8080
- A named volume for database persistence: health-tracker-data
- Environment variable for database path: HEALTH_TRACKER_DB_PATH=/data/healthtracker.db

## Tests

The database infrastructure includes:

- **Unit Tests**: Testing database service logic in isolation
- **Integration Tests**: Testing database initialization and connectivity with in-memory SQLite
- **Health Endpoint Tests**: Testing the `/health` endpoint returns correct status codes
