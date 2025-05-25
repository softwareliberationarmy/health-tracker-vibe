## Chunk B: Database Schema + Minimal API Endpoints

### Prompt B.1: Define Data Models in Core Project

```text
Context:
- We need to define the data structures for `WeighIn` and `Run` records.
- These models will be shared between the API and potentially the CLI later.
- They should reside in the `HealthTracker.Core` project.
- Refer to `spec.md` for column names, types, and constraints.

Task:
- In `HealthTracker.Core`, create a `Models` folder.
- Inside `Models`, create `WeighIn.cs` with properties:
  - `Id` (long)
  - `Date` (DateTime)
  - `Weight` (double) - lbs
  - `Bmi` (double)
  - `Fat` (double) - percentage
  - `Muscle` (double) - percentage
  - `RestingMetab` (int) - kcal
  - `VisceralFat` (int)
- Inside `Models`, create `Run.cs` with properties:
  - `Id` (long)
  - `Date` (DateTime)
  - `Distance` (double)
  - `DistanceUnit` (string) - e.g., "mi", "km"
  - `TimeInSeconds` (int)
- Ensure properties have appropriate public getters and setters.
```

### Prompt B.2: Write Failing Test for Database Initialization

```text
Context:
- The API will use SQLite and Dapper.
- We need a mechanism to create the database schema if it doesn't exist.
- This will be handled by a `DbInitializer` class in the `HealthTracker.Api` project.
- Tests are in `HealthTracker.Tests`.

Task:
- In `HealthTracker.Tests`, create a new test class (e.g., `DbInitializerTests`).
- Add NuGet packages `Microsoft.Data.Sqlite` and `Dapper` to `HealthTracker.Api` and `HealthTracker.Tests`.
- Write a failing xUnit test method that:
  - Creates an in-memory SQLite connection.
  - Instantiates `DbInitializer` (to be created next) with this connection string.
  - Calls a method like `InitializeAsync()` on the `DbInitializer`.
  - Uses Dapper to query the `sqlite_master` table to verify that `weighins` and `runs` tables have been created.
  - Asserts that both tables exist.
- This test will fail because `DbInitializer` and its method don't exist yet.
```

### Prompt B.3: Implement DbInitializer and Migration Script

```text
Context:
- A failing test expects `DbInitializer.InitializeAsync()` to create `weighins` and `runs` tables.
- The schema details are in `spec.md` (column names, types, constraints).

Task:
- In `HealthTracker.Api`, create a `Data` folder.
- Inside `Data`, create `DbInitializer.cs`.
- Implement the `DbInitializer` class with a constructor that takes an `IDbConnectionFactory` (to be created) and an `InitializeAsync()` method.
- The `InitializeAsync()` method should:
  - Open a connection using the factory.
  - Execute SQL `CREATE TABLE IF NOT EXISTS` statements for `weighins` and `runs` tables, matching the schema in `spec.md` (including primary keys, types, and CHECK constraints).
  - Use Dapper's `ExecuteAsync` for these commands.
- Create an `IDbConnectionFactory` interface in `HealthTracker.Api.Data` and a concrete `SqliteConnectionFactory` that takes a connection string and provides `IDbConnection` instances.
- Update the test to use the factory.
- Run the test from Prompt B.2 to confirm it now passes.
```

### Prompt B.4: Write Failing Integration Tests for API Endpoints

```text
Context:
- Database initialization is working.
- We need to scaffold API endpoints: `POST /weight`, `GET /weight/last/{n}`, `POST /run`, `GET /run/last/{n}`.
- We will use `Microsoft.AspNetCore.Mvc.Testing` for in-memory API tests.

Task:
- Add `Microsoft.AspNetCore.Mvc.Testing` to `HealthTracker.Tests`.
- In `HealthTracker.Tests`, create a new test class (e.g., `ApiEndpointsTests`).
- Write failing xUnit integration tests for each of the four endpoints:
  - `POST /weight`: Send a valid JSON payload (use a sample `WeighIn` object). Expect HTTP 201 Created (or 200 OK for now if simpler, to be refined).
  - `GET /weight/last/5`: Expect HTTP 200 OK and a JSON array (empty for now).
  - `POST /run`: Send a valid JSON payload (sample `Run` object). Expect HTTP 201 Created (or 200 OK).
  - `GET /run/last/5`: Expect HTTP 200 OK and a JSON array (empty for now).
- These tests will fail as the endpoints are not yet defined in `HealthTracker.Api`.
```

### Prompt B.5: Scaffold Minimal API Endpoints with Stub Handlers

```text
Context:
- Failing integration tests exist for four API endpoints.
- The `HealthTracker.Api` project needs these endpoints implemented in `Program.cs` (Minimal APIs).

Task:
- In `HealthTracker.Api`'s `Program.cs`:
  - Register the `SqliteConnectionFactory` and `DbInitializer` for dependency injection.
  - Call `DbInitializer.InitializeAsync()` during application startup.
  - Map the following POST and GET endpoints:
    - `POST /weight`: Takes a `WeighIn` object from the request body. For now, return `Results.Ok("WeighIn received")` or `Results.Created("/weight/1", weighIn)`.
    - `GET /weight/last/{count:int}`: Takes an integer `count`. For now, return `Results.Ok(new List<WeighIn>())`.
    - `POST /run`: Takes a `Run` object. For now, return `Results.Ok("Run received")` or `Results.Created("/run/1", run)`.
    - `GET /run/last/{count:int}`: Takes an integer `count`. For now, return `Results.Ok(new List<Run>())`.
  - Ensure the `WeighIn` and `Run` models from `HealthTracker.Core` are used.
- Run the integration tests from Prompt B.4 to confirm they now pass (or return the stubbed 200/201 responses).
```

### Prompt B.6: Refine API Startup and Test Dependencies

```text
Context:
- API endpoints are scaffolded and basic integration tests pass.
- `DbInitializer` should run once at startup.
- Tests might need a way to ensure a clean database for each run or use a test-specific DB.

Task:
- In `HealthTracker.Api`'s `Program.cs`, ensure `DbInitializer.InitializeAsync()` is called appropriately (e.g., get the service and run it).
- In `ApiEndpointsTests`, configure the `WebApplicationFactory` to use an in-memory SQLite database for tests (e.g., `DataSource=:memory:` connection string) or ensure the test DB is cleaned/recreated for each test run if using a file-based test DB.
- Refactor any setup code in tests for clarity.
- Re-run all tests to ensure they pass.
- Commit changes: "feat: Implement DB initialization and scaffold API endpoints".
```
