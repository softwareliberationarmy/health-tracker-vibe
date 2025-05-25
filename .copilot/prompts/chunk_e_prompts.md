## Chunk E: Run Feature (Log + View) in Full TDD Cycle

This chunk focuses on `health log run` and `health view run`.

### Prompt E.1: Write Failing Tests for `health log run` Argument Parsing & Prompts

```text
Context:
- `health log run` command structure is in place with a placeholder handler.
- `spec.md` requires: `<distance> <time> [date]` arguments.
  - Distance: e.g., `3.1mi` or `5km`. Unit is mandatory.
  - Time: `MM:SS` or `HH:MM:SS`.
- If no arguments, it should prompt for each field.
- Tests are in `HealthTracker.Tests`.

Task:
- Create `LogRunCommandTests.cs` in `HealthTracker.Tests`.
- **Test Case 1 (All Arguments Provided):**
  - Write a test for `health log run 5km 25:30 05/16/2025`.
  - Verify handler receives distance (e.g., 5.0), unit ("km"), time (as TimeSpan or total seconds), and date (DateTime).
  - Write another test for `health log run 3.1mi 01:15:45` (using today's date by default).
  - These will fail as arguments/parsing are missing.
- **Test Case 2 (No Arguments - Prompting Flow):**
  - Write a test for `health log run` (no args).
  - Mock `IAnsiConsole` to simulate user input for distance (e.g., "10km"), time ("45:00"), and date.
  - Assert handler receives correctly parsed values.
  - This will fail as prompting logic is missing.
```

### Prompt E.2: Implement Argument Definitions for `log run`

```text
Context:
- Failing tests exist for `health log run` argument parsing.
- `HealthTracker.Cli\Program.cs` needs updating.

Task:
- In `Program.cs` where `logRunCommand` is defined:
  - Define `Argument<string>` for `distanceWithUnit`, `timeStr`.
  - Define `Argument<DateTime?>` for `date` (optional).
  - Add these to `logRunCommand`.
  - In the handler:
    - Parse `distanceWithUnit`: extract numeric value and unit (e.g., "5km" -> 5.0, "km"). Reject if no unit.
    - Parse `timeStr`: convert `MM:SS` or `HH:MM:SS` to total seconds (int) or `TimeSpan`. Handle invalid formats.
    - Parse `date` or default to `DateTime.Today`.
  - Store/print parsed values for tests to assert.
- Run Test Case 1 from Prompt E.1. It should now pass for argument parsing.
```

### Prompt E.3: Implement Prompting Logic for `log run`

```text
Context:
- Test Case 2 (prompting flow) for `health log run` is failing.

Task:
- In the `logRunCommand` handler in `Program.cs`:
  - If arguments like `distanceWithUnit` were not provided:
    - Prompt for "Distance (e.g., 5km, 3.1mi):" using `AnsiConsole.Ask<string>()`.
    - Prompt for "Time (MM:SS or HH:MM:SS):" using `AnsiConsole.Ask<string>()`.
    - Prompt for "Date (MM/DD/YYYY, default today):" using `AnsiConsole.Ask<DateTime?>()`.
  - Perform parsing for prompted inputs similar to argument parsing (distance/unit, time string).
- Run Test Case 2 from Prompt E.1. It should now pass.
```

### Prompt E.4: Write Failing Mock HTTP Client Test for POST /run

```text
Context:
- `log run` CLI command gathers data.
- Data needs to be POSTed to `/run` API endpoint.

Task:
- In `LogRunCommandTests.cs`:
  - Write a test using `MockHttpMessageHandler` for a POST to `/run`.
  - Expect a JSON body matching `Run` model (`HealthTracker.Core.Models`), populated with sample parsed run data (Distance, DistanceUnit, TimeInSeconds, Date).
  - Simulate `health log run` and assert mock HTTP client received the request.
- Fails as HTTP call is not implemented.
```

### Prompt E.5: Implement HTTP Client Call for `log run`

```text
Context:
- Failing test expects `log run` to POST data to API.

Task:
- In `logRunCommand` handler (`Program.cs`):
  - After gathering and parsing run data:
    - Create a `Run` object (`HealthTracker.Core.Models`).
    - Populate with parsed distance, unit, time in seconds, and UTC date.
    - Serialize to JSON and POST to `/run` endpoint using `HttpClient`.
    - Handle API response.
- Run test from Prompt E.4. It should pass.
- **API Side**: Ensure `POST /run` in `HealthTracker.Api` deserializes `Run` and (ideally) saves it via a new `RunRepository` (similar to `WeightRepository`).
```

### Prompt E.6: Write Failing Tests for `health view run --last N`

```text
Context:
- Run logging CLI is mostly done.
- Implement `health view run [--last N]`.
- Calls `GET /run/last/{N}` API, displays in `Spectre.Console` table.

Task:
- Create `ViewRunCommandTests.cs` in `HealthTracker.Tests`.
- **Test Case 1 (Default Last 5):**
  - Test `health view run`. Mock HTTP for `GET /run/last/5` returning 5 `Run` objects.
  - Mock `IAnsiConsole`, assert `Spectre.Console.Table` renders 5 runs (headers: "Date", "Distance", "Time").
- **Test Case 2 (`--last N`):**
  - Test `health view run --last 2`. Mock HTTP for `GET /run/last/2`.
  - Assert table output for 2 runs.
- Fails as options, API call, table rendering are missing.
```

### Prompt E.7: Implement `view run` Command Options and API Call

```text
Context:
- Failing tests for `health view run [--last N]`.

Task:
- In `Program.cs` (`viewRunCommand` definition):
  - Add optional `--last` option (`int`, default 5).
  - Handler accepts option, calls `GET /run/last/{N}` API using `HttpClient`.
  - Deserialize JSON to `List<Run>`.
- Tests from E.6 will fail on table rendering but API call should work.
```

### Prompt E.8: Implement Table Rendering for `view run`

```text
Context:
- `view run` fetches data, needs table rendering.

Task:
- In `viewRunCommand` handler (`Program.cs`):
  - After getting `List<Run>`:
    - Create `Spectre.Console.Table`.
    - Columns: "Date", "Distance", "Time".
    - Add rows, formatting distance (e.g., "5.0 km") and time (from seconds to MM:SS or HH:MM:SS string).
    - Render table via `IAnsiConsole`.
- Tests from E.6 should pass.
- Refactor and commit: "feat: Implement log run and view run commands with API integration".
```

### Prompt E.9: API - Persist and Retrieve Runs

```text
Context:
- CLI can log/view runs, API stubs don't persist.
- `DbInitializer` creates `runs` table.

Task:
- In `HealthTracker.Api`:
  - Create `RunRepository.cs` in `Data` folder.
  - Implement methods:
    - `Task AddAsync(Run run)`: Dapper `INSERT` into `runs` table (Distance, DistanceUnit, TimeInSeconds, Date as ISO UTC TEXT).
    - `Task<IEnumerable<Run>> GetLastNAsync(int count)`: Dapper `SELECT` last `N` runs, ordered by date descending.
  - Register `RunRepository` for DI.
  - Modify `POST /run` endpoint to use `RunRepository.AddAsync`.
  - Modify `GET /run/last/{count}` endpoint to use `RunRepository.GetLastNAsync`.
- Update `ApiEndpointsTests` for `/run` to verify data persistence/retrieval.
- Run all API and CLI tests.
```
