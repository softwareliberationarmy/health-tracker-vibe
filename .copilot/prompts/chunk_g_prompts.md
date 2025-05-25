## Chunk G: Docker Container and E2E Tests

This chunk focuses on containerizing the API and implementing end-to-end tests.

### Prompt G.1: Write End-to-End Test Specification

```text
Context:
- The API should run in a Docker container with embedded SQLite.
- E2E tests should use a real containerized API, not mocks.
- Tests need to start/stop containers and verify full data flow.

Task:
- In `HealthTracker.Tests`, create `EndToEndTests.cs`.
- **Test Specification (not implementation yet):**
  - Write test method signatures and comments describing what they should do:
    - `Can_LogAndRetrieveWeighIn_ThroughContainerizedApi()`: Start container, POST weight data, GET it back, verify data integrity.
    - `Can_LogAndRetrieveRun_ThroughContainerizedApi()`: Similar for run data.
    - `Can_HandleMultipleWeighIns_AndReturnLastN()`: POST multiple weights, GET last N, verify order and count.
  - Add `[Fact(Skip = "Container not ready")]` attributes to prevent running until implementation is ready.
- This establishes the test structure without implementation details.
```

### Prompt G.2: Create Basic Dockerfile for API

```text
Context:
- `HealthTracker.Api` needs to run in a Docker container.
- Container should include the API and SQLite database.
- Port 8080 should be exposed (mapped to 50000 externally per spec).

Task:
- In `HealthTracker.Api` project root, create `Dockerfile`:
  - Use `mcr.microsoft.com/dotnet/aspnet:8.0` as base image.
  - Use `mcr.microsoft.com/dotnet/sdk:8.0` for build stage.
  - Copy source files and restore dependencies.
  - Build the API project.
  - Set working directory and expose port 8080.
  - Configure SQLite database path via environment variable (e.g., `DB_PATH=/app/data/health.db`).
  - Set entrypoint to run the API.
- Create `.dockerignore` to exclude unnecessary files (bin, obj, .git, etc.).
- This creates a basic containerized API setup.
```

### Prompt G.3: Update API for Docker Configuration

```text
Context:
- The API needs to be configured to run properly in Docker.
- Database path should be configurable via environment variables.
- API should listen on all interfaces (0.0.0.0) when containerized.

Task:
- In `HealthTracker.Api\Program.cs`:
  - Update `WebApplicationBuilder` configuration:
    - Read database path from environment variable `DB_PATH` with fallback to default.
    - Configure Kestrel to listen on `http://0.0.0.0:8080` when in container.
  - Update `SqliteConnectionFactory` to use the configurable database path.
  - Ensure `DbInitializer` creates directory structure if it doesn't exist.
- Test locally that the API can start with environment variable configuration.
- This makes the API container-ready without breaking local development.
```

### Prompt G.4: Create PowerShell Build Script for Docker Operations

```text
Context:
- `build.ps1` should support Docker container operations for testing.
- Need functions to build, start, stop, and clean up containers.

Task:
- Create or update `build.ps1` in solution root:
  - Add function `Build-DockerImage`:
    - Builds Docker image from `HealthTracker.Api\Dockerfile`.
    - Tags image as `health-tracker-api:latest`.
  - Add function `Start-DockerContainer`:
    - Runs container with port mapping `50000:8080`.
    - Sets environment variables for database path.
    - Uses volume for data persistence (optional for tests).
    - Returns container ID for later management.
  - Add function `Stop-DockerContainer`:
    - Stops and removes container by ID or name.
  - Add parameter support: `.\build.ps1 -Target DockerBuild`, `.\build.ps1 -Target DockerStart`, etc.
- Test these functions manually to ensure container lifecycle works.
```

### Prompt G.5: Write Failing E2E Test Infrastructure

```text
Context:
- E2E tests need to manage Docker container lifecycle.
- Tests should use real HTTP calls to containerized API.
- Need test fixtures for container setup/teardown.

Task:
- In `EndToEndTests.cs`:
  - Add NuGet package `Docker.DotNet` to `HealthTracker.Tests` for container management.
  - Create `DockerFixture` class implementing `IAsyncLifetime`:
    - `InitializeAsync()`: Build and start Docker container, wait for API to be ready.
    - `DisposeAsync()`: Stop and remove container.
    - Provide `ApiBaseUrl` property for tests to use.
  - Update test class to use `IClassFixture<DockerFixture>`.
  - Remove `[Skip]` attributes from test methods but leave them empty for now.
- Run tests - they should fail as implementation is missing, but infrastructure should be in place.
```

### Prompt G.6: Implement E2E Test for Weight Data Flow

```text
Context:
- Docker infrastructure is ready.
- First E2E test should verify complete weight data flow.

Task:
- In `EndToEndTests.cs`:
  - Implement `Can_LogAndRetrieveWeighIn_ThroughContainerizedApi()`:
    - Create `HttpClient` pointing to container API.
    - Create sample `WeighIn` object with test data.
    - POST to `/weight` endpoint, assert 201 Created or 200 OK.
    - GET from `/weight/last/1` endpoint.
    - Deserialize response and assert data matches what was POSTed.
    - Verify date conversion (UTC storage, proper retrieval).
- Run this test - it should pass if Docker setup is correct.
- This verifies the API works correctly in containerized environment.
```

### Prompt G.7: Implement E2E Test for Run Data Flow

```text
Context:
- Weight E2E test is working.
- Need similar test for run data.

Task:
- In `EndToEndTests.cs`:
  - Implement `Can_LogAndRetrieveRun_ThroughContainerizedApi()`:
    - Similar to weight test but using `Run` model.
    - POST to `/run` with sample run data (distance, unit, time, date).
    - GET from `/run/last/1`.
    - Assert data integrity including distance, unit, time conversion.
- This verifies run endpoints work correctly in container.
```

### Prompt G.8: Implement E2E Test for Multiple Records and Ordering

```text
Context:
- Basic E2E tests for single records are working.
- Need to test multiple records and proper ordering.

Task:
- In `EndToEndTests.cs`:
  - Implement `Can_HandleMultipleWeighIns_AndReturnLastN()`:
    - POST 7 different `WeighIn` objects with different dates (ensuring chronological order).
    - GET from `/weight/last/5`.
    - Assert response contains exactly 5 records.
    - Assert records are ordered by date descending (most recent first).
    - Verify the correct 5 most recent records are returned.
  - Similar test for runs if desired: `Can_HandleMultipleRuns_AndReturnLastN()`.
- This verifies repository ordering and pagination logic works correctly.
```

### Prompt G.9: Integrate Docker Operations into Build Script

```text
Context:
- Docker operations are working manually.
- Build script should orchestrate full E2E testing workflow.

Task:
- Update `build.ps1`:
  - Add `RunE2ETests` target:
    - Build Docker image.
    - Start container.
    - Wait for API health check or readiness.
    - Run E2E tests (`dotnet test --filter "Category=E2E"`).
    - Stop and clean up container regardless of test results.
  - Add error handling to ensure container cleanup happens even if tests fail.
  - Add `RunAllTests` target that runs both unit and E2E tests.
- Tag E2E tests with `[Trait("Category", "E2E")]` for filtering.
- Test the full workflow: `.\build.ps1 -Target RunE2ETests`.
```

### Prompt G.10: Refine Docker Configuration and Test Stability

```text
Context:
- E2E tests are working but may need stability improvements.
- Docker configuration should be production-ready.

Task:
- **Docker Improvements:**
  - Add health check to Dockerfile (`HEALTHCHECK` instruction).
  - Optimize Docker image size (multi-stage build, minimize layers).
  - Add proper logging configuration for containerized API.
- **Test Stability:**
  - Add retry logic for container startup in `DockerFixture`.
  - Implement proper API readiness checks before running tests.
  - Add test isolation - ensure each test uses fresh database or cleans up data.
- **Documentation:**
  - Update README with Docker usage instructions.
  - Document E2E test execution process.
- Run full test suite multiple times to verify stability.
- Commit changes: "feat: Implement Docker containerization and E2E tests".
```
