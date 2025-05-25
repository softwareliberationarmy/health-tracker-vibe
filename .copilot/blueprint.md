# Blueprint for Building the Health Tracker CLI & API

## 1. High-Level, Step-by-Step Blueprint

1. **Project Scaffolding & Tooling**  
   • Create a .NET solution with separate projects for CLI, API, core models, and tests.  
   • Configure Spectre.Console, System.CommandLine, Dapper, SQLite, and Docker support.  
   • Set up CI configuration and PowerShell build script.

2. **Database Schema & Minimal API Skeleton**  
   • Define SQLite schema (weighins, runs) and create migration script.  
   • Scaffold a Minimal API with endpoints `POST /weight`, `GET /weight/last/{n}`, `POST /run`, `GET /run/last/{n}`.  
   • Wire Dapper for data access and return sample JSON.

3. **CLI Command Infrastructure**  
   • Configure `health` root command with global options (`--version`, `--verbose`).  
   • Build out `log` and `view` sub-command structure.

4. **Weight Logging & Viewing Features (TDD Cycle)**  
   • Write failing tests for `health log weight` prompts, validation, and API POST.  
   • Implement prompt logic and HTTP client POST to API.  
   • Write tests for `health view weight --last`.  
   • Implement HTTP GET and table formatting via Spectre.Console.

5. **Run Logging & Viewing Features (TDD Cycle)**  
   • Write tests for `health log run` parsing, prompts, and API POST.  
   • Implement distance/time parsing, unit validation, HTTP client calls.  
   • Write tests for `health view run --last`.  
   • Implement GET endpoint consumption and table formatting.

6. **Error Handling & Validation**  
   • Add CLI input validation tests (dates, ranges, formats).  
   • Implement red-text validation messages and `--verbose` stack traces.

7. **Dockerization & End-to-End Tests**  
   • Create Dockerfile to host API and SQLite.  
   • Write E2E tests against a containerized API (using TestServer and real SQLite).  
   • Integrate E2E into the PowerShell build script.

8. **Refinement & Documentation**  
   • Refactor code for maintainability and remove duplication.  
   • Generate README sections and usage examples.  
   • Publish CLI to private NuGet in CI.

---

## 2. First Breakdown into Iterative Chunks

- **Chunk A**: Solution & project scaffolding + Version command
- **Chunk B**: Database schema + Minimal API endpoints
- **Chunk C**: CLI command setup (`log`/`view`) + global options
- **Chunk D**: Weight feature (log + view) in full TDD cycle
- **Chunk E**: Run feature (log + view) in full TDD cycle
- **Chunk F**: Validation & error‐handling enhancements
- **Chunk G**: Docker container and E2E tests
- **Chunk H**: Final refactoring, docs, and CI publishing

---

## 3. Second Breakdown: Detailed Steps per Chunk

**Chunk A**

1. Create `HealthTracker.sln` and four projects: `Cli`, `Api`, `Core`, `Tests`.
2. Add references: `Core` in `Cli` and `Api`; test projects reference each.
3. Write xUnit test for `--version` output.
4. Implement version command in CLI minimally.
5. Run tests and refactor.

**Chunk B**

1. Write unit test defining desired SQLite schema and Dapper mapping.
2. Create migration script and `DbInitializer` class.
3. Write integration test for API endpoint skeleton returning `200 OK`.
4. Scaffold Minimal API endpoints with stub handlers.
5. Run tests and refine.

**Chunk C**

1. Write tests for `health log` and `health view` command routing.
2. Configure `System.CommandLine` subcommands and options.
3. Ensure `help` and invalid command handling.
4. Write tests for global `--verbose` flag behavior in commands.
5. Implement placeholder handlers for `log` and `view` to satisfy tests.
6. Run tests and refactor.

**Chunk D**

1. Write xUnit tests for parsing weight log arguments or prompting flow (fields: weight, bmi, fat, muscle, restingMetab, visceralFat, date).
2. Include tests for validation rules (range checks and date parsing).
3. Implement minimal prompt logic with `Spectre.Console` to satisfy tests.
4. Write mock HTTP client unit tests for POST `/weight` JSON payload structure.
5. Implement HTTP client call in CLI to POST to API endpoint.
6. Run tests and refactor.

**Chunk E**

1. Write tests for parsing run log arguments or interactive prompts (distance with unit, time, date).
2. Include tests for time format parsing (`MM:SS`, `HH:MM:SS`) and distance unit validation.
3. Implement minimal parsing and prompting logic in CLI.
4. Write mock HTTP client tests for POST `/run` payload.
5. Implement HTTP client call in CLI for run logging.
6. Run tests and refactor.

**Chunk F**

1. Write tests for CLI input validation failures (future dates, out-of-range values) displaying errors in red.
2. Write tests verifying `--verbose` flag emits full stack traces on exceptions.
3. Implement validation logic and error formatting in CLI using `Spectre.Console`.
4. Write API unit tests for validation errors returning HTTP 400 with messages.
5. Implement validation in Minimal API endpoints and return appropriate responses.
6. Run tests and refactor.

**Chunk G**

1. Write end-to-end test specification for containerized API (start container, POST/GET flows).
2. Create `Dockerfile` in `Api` project to build and host API with embedded SQLite.
3. Write PowerShell snippets in `build.ps1` to start and stop Docker container for tests.
4. Implement E2E tests using real SQLite DB and Docker network.
5. Integrate container lifecycle in test setup/teardown.
6. Run tests and refactor.

**Chunk H**

1. Write tests asserting README contains usage and command examples.
2. Draft `README.md` sections: Overview, Installation, Commands, Examples.
3. Populate usage examples and sample outputs.
4. Write CI pipeline script to build, test, package, and publish CLI to private NuGet.
5. Update `build.ps1` to include `PublishCli` step.
6. Run full suite and finalize blueprint.

---
