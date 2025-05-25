## Chunk D: Weight Feature (Log + View) in Full TDD Cycle

This chunk focuses on `health log weight` and `health view weight`.

### Prompt D.1: Write Failing Tests for `health log weight` Argument Parsing & Prompts

```text
Context:
- `health log weight` command structure is in place but has a placeholder handler.
- `spec.md` requires: `<weight> <bmi> <fat> <muscle> <restingMetab> <visceralFat> [date]` arguments.
- If no arguments, it should prompt for each field.
- `Spectre.Console` for prompts, `System.CommandLine` for args.
- Tests are in `HealthTracker.Tests`.

Task:
- Create `LogWeightCommandTests.cs` in `HealthTracker.Tests`.
- **Test Case 1 (All Arguments Provided):**
  - Write a test for `health log weight 180.5 22.1 15.2 40.3 1800 12 05/15/2025`.
  - The test should verify that the handler receives these exact values, correctly parsed (date as DateTime).
  - This will initially fail as arguments are not defined/parsed.
- **Test Case 2 (No Arguments - Prompting Flow):**
  - Write a test for `health log weight` (no args).
  - This test needs to mock `IAnsiConsole` (from Spectre.Console) to simulate user input for each prompt (weight, bmi, etc.).
  - Assert that the handler receives the values entered via mocked prompts.
  - This will fail as prompting logic and argument definitions are missing.
- Add `Spectre.Console` NuGet package to `HealthTracker.Cli` and `Spectre.Console.Testing` to `HealthTracker.Tests`.
```

### Prompt D.2: Implement Argument Definitions for `log weight`

```text
Context:
- Failing tests exist for `health log weight` argument parsing.
- `System.CommandLine` is used in `HealthTracker.Cli\Program.cs`.

Task:
- In `Program.cs` where `logWeightCommand` is defined:
  - Define `Argument<double>` for `weight`, `bmi`, `fat`, `muscle`.
  - Define `Argument<int>` for `restingMetab`, `visceralFat`.
  - Define `Argument<DateTime?>` for `date` (optional, custom parser might be needed for M/D format or use string and parse in handler).
  - Add these arguments to `logWeightCommand`.
  - Update the handler for `logWeightCommand` to accept these arguments.
  - For now, the handler can just store these values or print them so tests can assert they were received correctly.
- Run Test Case 1 from Prompt D.1. It should now pass for argument parsing.
```

### Prompt D.3: Implement Prompting Logic for `log weight`

```text
Context:
- Test Case 2 (prompting flow) for `health log weight` is failing.
- `Spectre.Console` should be used for prompting if arguments are not provided.

Task:
- In the handler for `logWeightCommand` in `Program.cs`:
  - Check if the required arguments (e.g., `weight`) were provided via command line. `ParseResult` can be used, or check for default values if arguments are optional and have defaults.
  - If not provided, use `AnsiConsole.Prompt()` from `Spectre.Console` to ask the user for each required field: `weight`, `bmi`, `fat`, `muscle`, `restingMetab`, `visceralFat`.
  - Use `AnsiConsole.Ask<DateTime?>("Date (MM/DD/YYYY, default today):")` for the date, defaulting to `DateTime.Today` if no input.
  - Ensure the handler is injectable with `IAnsiConsole` for testability.
- Run Test Case 2 from Prompt D.1. It should now pass.
```

### Prompt D.4: Write Failing Mock HTTP Client Test for POST /weight

```text
Context:
- The `log weight` CLI command can now gather data (either via args or prompts).
- This data needs to be POSTed to the `/weight` API endpoint.
- We need a test to verify the CLI constructs and sends the correct JSON payload.
- Use a mock HTTP client (e.g., using `RichardSzalay.MockHttp` or custom mock).

Task:
- Add `RichardSzalay.MockHttp` to `HealthTracker.Tests`.
- In `LogWeightCommandTests.cs`:
  - Write a new test method.
  - Configure `MockHttpMessageHandler` to expect a POST request to a configured API base URL + `/weight`.
  - The expected request should have a JSON body matching the `WeighIn` model from `HealthTracker.Core`, populated with sample data.
  - The test should simulate running `health log weight` (with args or mocked prompts) and assert that the mock HTTP client received the request as configured.
- This test will fail as the HTTP client call is not yet implemented in the CLI command handler.
```

### Prompt D.5: Implement HTTP Client Call for `log weight`

```text
Context:
- A failing test expects the `log weight` command to POST data to the API.
- The CLI needs an `HttpClient` to make this call.

Task:
- In `HealthTracker.Cli\Program.cs` (or a new service class):
  - Configure `HttpClient` for use by command handlers (e.g., via DI or a static instance, ensuring base address for the API is configurable).
  - In the `logWeightCommand` handler, after gathering all data:
    - Create a `WeighIn` object from `HealthTracker.Core.Models`.
    - Populate it with the gathered data (ensure date is UTC: `data.Date?.ToUniversalTime()`).
    - Serialize the `WeighIn` object to JSON.
    - Use the `HttpClient` to send a POST request to the `/weight` endpoint with the JSON payload.
    - Handle the API response (e.g., print success/failure message).
- Run the test from Prompt D.4. It should now pass.
- **API Side**: Ensure the `POST /weight` endpoint in `HealthTracker.Api` can deserialize the `WeighIn` object and save it using Dapper (for now, just returning OK is fine if DB persistence is for a later step, but ideally, it should try to save).
```

### Prompt D.6: Write Failing Tests for `health view weight --last N`

```text
Context:
- Weight logging CLI part is mostly done.
- Now, implement `health view weight [--last N]`.
- It should call `GET /weight/last/{N}` API and display results in a `Spectre.Console` table.

Task:
- Create `ViewWeightCommandTests.cs` in `HealthTracker.Tests`.
- **Test Case 1 (Default Last 5):**
  - Write a test for `health view weight`.
  - Mock the HTTP client to return a predefined list of 5 `WeighIn` objects when `GET /weight/last/5` is called.
  - Mock `IAnsiConsole` to capture table output.
  - Assert that `Spectre.Console.Table` is used and it renders the 5 weigh-ins correctly (check headers and a few data points).
- **Test Case 2 (`--last N`):**
  - Write a test for `health view weight --last 3`.
  - Mock HTTP client for `GET /weight/last/3` returning 3 weigh-ins.
  - Assert table output for 3 weigh-ins.
- These tests will fail as the command options, API call, and table rendering are not implemented.
```

### Prompt D.7: Implement `view weight` Command Options and API Call

```text
Context:
- Failing tests exist for `health view weight [--last N]`.

Task:
- In `Program.cs` where `viewWeightCommand` is defined:
  - Add an optional option `--last` of type `int` with a default value of 5.
  - Update the handler for `viewWeightCommand` to accept this option.
  - In the handler, use the `HttpClient` to call `GET /weight/last/{N}` API endpoint, where N is the value of the `--last` option.
  - Deserialize the JSON response into a `List<WeighIn>`.
- Run tests from Prompt D.6. They will still fail on table rendering but the API call part should be testable/working.
```

### Prompt D.8: Implement Table Rendering for `view weight`

```text
Context:
- `view weight` command can fetch data from the API.
- Failing tests expect this data to be rendered in a `Spectre.Console` table.

Task:
- In the `viewWeightCommand` handler in `Program.cs`:
  - After receiving the `List<WeighIn>` from the API:
    - Create a `Spectre.Console.Table`.
    - Add columns: "Date", "Weight (lbs)", "BMI", "Fat (%)", "Muscle (%)", "Resting Metab (kcal)", "Visceral Fat".
    - Iterate through the `List<WeighIn>` and add a row to the table for each entry.
    - Format data appropriately (e.g., date to local short date string, numbers to 1 decimal place where needed).
    - Render the table using the injected `IAnsiConsole`.
- Run tests from Prompt D.6. They should now pass.
- Refactor and commit: "feat: Implement log weight and view weight commands with API integration".
```

### Prompt D.9: API - Persist and Retrieve WeighIns

```text
Context:
- CLI can log and view weights, but API stubs don't persist data.
- `DbInitializer` creates the `weighins` table.
- Dapper should be used for data access in `HealthTracker.Api`.

Task:
- In `HealthTracker.Api`:
  - Create a `WeightRepository.cs` in the `Data` folder.
  - Implement methods in `WeightRepository`:
    - `Task AddAsync(WeighIn weighIn)`: Uses Dapper to `INSERT` a `WeighIn` into the `weighins` table. Ensure `date` is stored as ISO 8601 UTC TEXT, and other values match table schema.
    - `Task<IEnumerable<WeighIn>> GetLastNAsync(int count)`: Uses Dapper to `SELECT` the last `N` weigh-ins, ordered by date descending.
  - Register `WeightRepository` for DI in `Program.cs`.
  - Modify the `POST /weight` endpoint handler to use `WeightRepository.AddAsync`.
  - Modify the `GET /weight/last/{count}` endpoint handler to use `WeightRepository.GetLastNAsync`.
- Update `ApiEndpointsTests` (from Chunk B) for `/weight` endpoints to verify actual data persistence and retrieval using an in-memory or test-specific SQLite DB.
  - e.g., POST a weigh-in, then GET it back and assert contents.
- Run all API tests and CLI tests to ensure integration.
```
