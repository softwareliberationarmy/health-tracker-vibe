# Vertical Slice 1: Infrastructure & `--about` Command Prompts

This document contains the step-by-step prompts for implementing Vertical Slice 1, which establishes the end-to-end infrastructure and implements the `health --about` command.

## Step 1: Create Solution Structure

**Given** I need a health tracker application with CLI and API components  
**When** I create the solution structure  
**Then** I should have organized projects for CLI, API, shared models, and tests

**Prompt:**
Create a .NET solution structure for the health tracker project. Set up the following projects:

- `HealthTracker.Cli` - Console application for the CLI tool
- `HealthTracker.Api` - Web API project using minimal APIs
- `HealthTracker.Shared` - Class library for shared models and contracts
- `HealthTracker.Tests.Unit` - xUnit test project for unit tests
- `HealthTracker.Tests.Integration` - xUnit test project for integration tests
- `HealthTracker.Tests.E2E` - xUnit test project for end-to-end tests

Include appropriate NuGet package references:

- CLI: `System.CommandLine`, `Spectre.Console`
- API: `Dapper`, `Microsoft.Data.Sqlite`
- Tests: `xUnit`, `FluentAssertions`, `Microsoft.AspNetCore.Mvc.Testing`

Create a simple .gitignore file appropriate for .NET projects.

---

## Step 2: Define Shared Models

**Given** I need to share data contracts between CLI and API  
**When** I define the shared models  
**Then** I should have models for API responses and requests

**Prompt:**
Create shared model classes in `HealthTracker.Shared` for the API contracts:

1. `AboutResponse` - represents the response from `/about` endpoint with properties:

   - `ApiVersion` (string)
   - `WeighInsCount` (int)
   - `RunsCount` (int)
   - `LastWeighInDate` (DateTime?)
   - `LastRunDate` (DateTime?)

2. `WeighInRequest` - represents a weight logging request
3. `RunRequest` - represents a run logging request

Add appropriate data annotations for validation. Write unit tests for model validation using the Given/When/Then format to verify required fields and constraints.

---

## Step 3: Implement Basic CLI Command Parsing

**Given** I need a CLI that can parse the `--about` command  
**When** I implement command line parsing  
**Then** The CLI should recognize and route the `--about` command

**Prompt:**
Implement basic command parsing in `HealthTracker.Cli` using `System.CommandLine`:

**Scenario:**

- **Given** the CLI application is running
- **When** a user types `health --about`
- **Then** the application should route to an about handler

Create:

1. A `Program.cs` with command line setup
2. An `AboutCommand` class that handles the `--about` option
3. For now, have the about handler return mock data (hardcoded version info)

Write unit tests that verify:

- The command parser correctly identifies the `--about` option
- The about handler is called when `--about` is specified
- Mock the console output to verify the display format

---

## Step 4: Add HTTP Client Infrastructure to CLI

**Given** The CLI needs to communicate with the API  
**When** I add HTTP client capabilities  
**Then** The CLI should be able to make API calls

**Prompt:**
Add HTTP client infrastructure to the CLI:

**Scenario:**

- **Given** the CLI needs to call the API's `/about` endpoint
- **When** the about command executes
- **Then** it should make an HTTP GET request and display the response

Implement:

1. An `IApiClient` interface with a `GetAboutAsync()` method
2. An `ApiClient` class that implements the interface using `HttpClient`
3. Update the `AboutCommand` to use the API client instead of mock data
4. Add configuration for the API base URL (default to `http://localhost:50000`)

Write unit tests that:

- Mock the `IApiClient` to return test data
- Verify the about command calls the API client
- Test error handling when the API is unavailable

---

## Step 5: Implement API Project Structure

**Given** I need a minimal API that serves the `/about` endpoint  
**When** I create the API structure  
**Then** I should have a working web API with dependency injection

**Prompt:**
Create the basic API structure in `HealthTracker.Api`:

**Scenario:**

- **Given** the API application starts
- **When** a GET request is made to `/about`
- **Then** it should return version and statistics information

Implement:

1. `Program.cs` with minimal API setup and dependency injection
2. An `IHealthService` interface with methods for getting statistics
3. A `HealthService` class that implements the interface (return mock data for now)
4. Register the service in DI container
5. Add the `/about` endpoint that returns an `AboutResponse`

Write integration tests using `TestServer`:

- Test that the `/about` endpoint returns 200 OK
- Verify the response structure matches `AboutResponse`
- Test with mock data to ensure the endpoint works end-to-end

---

## Step 6: Add SQLite Database Infrastructure

**Given** The API needs to store and retrieve data from SQLite  
**When** I add database infrastructure  
**Then** The API should connect to SQLite and initialize tables

**Prompt:**
Add SQLite database infrastructure to the API:

**Scenario:**

- **Given** the API starts up
- **When** the database connection is established
- **Then** the required tables should be created if they don't exist

Implement:

1. A `IDatabaseService` interface with methods for initialization and health checks
2. A `DatabaseService` class using Dapper and SQLite
3. SQL scripts to create `weighins` and `runs` tables (as per spec)
4. Database initialization logic that runs on startup
5. Update `HealthService` to use the database for getting actual counts

Configuration:

- Add connection string configuration
- Add database file path configuration (environment variable support)

Write unit tests for:

- Database initialization logic
- SQL generation for table creation

Write integration tests for:

- Database connection and table creation
- Getting counts from empty database returns 0

---

## Step 7: Create Dockerfile and Docker Setup

**Given** The API needs to run in Docker with SQLite  
**When** I create Docker configuration  
**Then** The API should run in a container with persistent data

**Prompt:**
Create Docker configuration for the API:

**Scenario:**

- **Given** Docker is installed
- **When** I build and run the health tracker API container
- **Then** the API should be accessible on port 50000 and persist data

Create:

1. `Dockerfile` for the API project:

   - Use appropriate .NET runtime image
   - Copy API binaries
   - Set environment variables for database path
   - Expose port 8080 (mapped to 50000 externally)
   - Set up volume for SQLite database persistence

2. `.dockerignore` file to optimize build context

3. Test the Docker setup by:
   - Building the image
   - Running a container
   - Verifying the `/about` endpoint responds
   - Checking that data persists across container restarts

---

## Step 8: Add PowerShell Build Script

**Given** I need to automate building, testing, and Docker operations  
**When** I create a PowerShell build script  
**Then** I should be able to run common tasks with simple commands

**Prompt:**
Create a `build.ps1` PowerShell script that supports:

**Scenarios:**

- **Given** I want to build all projects: `.\build.ps1 -Target Build`
- **Given** I want to run all tests: `.\build.ps1 -Target Test`
- **Given** I want to build Docker image: `.\build.ps1 -Target Docker`
- **Given** I want to run the API container: `.\build.ps1 -Target RunApi`

Implement targets for:

1. `Build` - Build all projects in the solution
2. `Test` - Run all unit and integration tests
3. `Clean` - Clean build artifacts
4. `Docker` - Build the Docker image
5. `RunApi` - Start the API container (stop existing if running)
6. `StopApi` - Stop the API container

Include error handling and informative output using `Write-Host` with colors.

---

## Step 9: Implement End-to-End Test

**Given** The entire infrastructure is in place  
**When** I create an end-to-end test  
**Then** I should be able to test the complete flow from CLI to API to database

**Prompt:**
Create an end-to-end test that validates the complete `--about` command flow:

**Scenario:**

- **Given** the API is running in Docker
- **When** I execute `health --about` from the CLI
- **Then** I should see the API version and database statistics

Implement in `HealthTracker.Tests.E2E`:

1. Test setup that starts the Docker container before tests
2. Test cleanup that stops the container after tests
3. A test that:
   - Executes the CLI with `--about` argument
   - Captures the console output
   - Verifies the output contains expected information (API version, counts)
   - Verifies the API was actually called (not just mock data)

Use `Process.Start()` to execute the CLI and capture output.
Include proper async/await patterns and timeout handling.

---

## Step 10: Package CLI as NuGet Tool

**Given** The CLI should be distributed as a global .NET tool  
**When** I configure packaging  
**Then** The CLI should be installable via `dotnet tool install`

**Prompt:**
Configure the CLI project for distribution as a .NET global tool:

**Scenario:**

- **Given** the CLI is built and packaged
- **When** I run `dotnet tool install -g healthcli --add-source ./nupkg`
- **Then** I should be able to run `health --about` from anywhere

Configure:

1. Update `HealthTracker.Cli.csproj`:

   - Add `<PackAsTool>true</PackAsTool>`
   - Add `<ToolCommandName>health</ToolCommandName>`
   - Set appropriate package metadata (version, description, etc.)

2. Update build script with:
   - `Pack` target to create .nupkg file
   - `Install` target to install the tool locally
   - `Uninstall` target to remove the tool

Test the packaging:

- Build and pack the CLI
- Install it as a global tool
- Verify `health --about` works from any directory
- Test with the API running in Docker

---

## Step 11: Add Documentation and Final Integration

**Given** All components are implemented  
**When** I add documentation and verify integration  
**Then** The vertical slice should be complete and documented

**Prompt:**
Create comprehensive documentation and verify the complete integration:

**Documentation to create:**

1. `README.md` with:

   - Project overview
   - Prerequisites (Docker, .NET)
   - Build and setup instructions
   - Usage examples for `--about` command
   - Development workflow

2. Update any inline code documentation

**Final integration verification:**

1. Run the complete workflow:

   - Build solution: `.\build.ps1 -Target Build`
   - Run tests: `.\build.ps1 -Target Test`
   - Build Docker: `.\build.ps1 -Target Docker`
   - Start API: `.\build.ps1 -Target RunApi`
   - Pack CLI: `.\build.ps1 -Target Pack`
   - Install CLI: `.\build.ps1 -Target Install`
   - Test end-to-end: `health --about`

2. Create a final E2E test that verifies the complete user journey

3. Ensure all tests pass and the system works as specified

**Acceptance criteria verification:**

- [ ] All tests (unit, integration, E2E) are passing
- [ ] Code is linted and formatted consistently
- [ ] Docker container runs API successfully
- [ ] CLI installs and works as global tool
- [ ] Documentation is complete and accurate
- [ ] Build script automates all common tasks
