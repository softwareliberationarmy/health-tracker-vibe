## Chunk F: Validation & Error Handling Enhancements

This chunk focuses on adding robust input validation and error handling across CLI and API.

### Prompt F.1: Write Failing Tests for CLI Input Validation (Future Dates)

```text
Context:
- The CLI currently accepts weight and run logging data.
- `spec.md` requires rejecting dates in the future.
- Validation errors should display in red text via `Spectre.Console`.
- Tests are in `HealthTracker.Tests`.

Task:
- Create `ValidationTests.cs` in `HealthTracker.Tests`.
- **Test Case 1 (Future Date in Weight Logging):**
  - Write a test for `health log weight 180 22 15 40 1800 12 12/31/2030` (future date).
  - Mock `IAnsiConsole` to capture error output.
  - Assert that a red error message is displayed (e.g., "Error: Date cannot be in the future").
  - Assert that the operation fails without calling the API.
- **Test Case 2 (Future Date in Run Logging):**
  - Write a test for `health log run 5km 25:30 12/31/2030`.
  - Assert similar red error output and no API call.
- These tests will fail as date validation is not implemented.
```

### Prompt F.2: Write Failing Tests for CLI Value Range Validation

```text
Context:
- `spec.md` defines validation rules: weight 100-300 lbs, fat/muscle 0-100%, etc.
- Out-of-range values should show red error messages.

Task:
- In `ValidationTests.cs`, add more test cases:
- **Test Case 3 (Weight Out of Range):**
  - Test `health log weight 50 22 15 40 1800 12` (weight < 100).
  - Assert red error message like "Error: Weight must be between 100 and 300 lbs".
  - Test `health log weight 350 22 15 40 1800 12` (weight > 300).
- **Test Case 4 (Fat Percentage Out of Range):**
  - Test `health log weight 180 22 150 40 1800 12` (fat > 100%).
  - Assert red error about fat percentage range (0-100%).
- **Test Case 5 (Resting Metabolism Too Low):**
  - Test `health log weight 180 22 15 40 500 12` (restingMetab < 1000).
  - Assert red error about minimum resting metabolism.
- These will fail as range validation is not implemented.
```

### Prompt F.3: Write Failing Tests for CLI Distance and Time Format Validation

```text
Context:
- Run logging requires proper distance units and time formats.
- Invalid formats should show red errors.

Task:
- In `ValidationTests.cs`, add run validation tests:
- **Test Case 6 (Missing Distance Unit):**
  - Test `health log run 5 25:30` (no unit like "km" or "mi").
  - Assert red error like "Error: Distance must include a unit (e.g., 5km, 3.1mi)".
- **Test Case 7 (Invalid Time Format):**
  - Test `health log run 5km 25:75` (invalid seconds > 59).
  - Assert red error about time format.
  - Test `health log run 5km 90:30` (invalid minutes > 59 in MM:SS).
- **Test Case 8 (Negative Distance):**
  - Test `health log run -5km 25:30`.
  - Assert red error about positive distance requirement.
- These will fail as format validation is not implemented.
```

### Prompt F.4: Implement Date Validation in CLI Commands

```text
Context:
- Failing tests expect date validation in weight and run logging.
- Both commands parse dates and should reject future dates.

Task:
- In `HealthTracker.Cli\Program.cs` (or create a shared `ValidationHelper.cs`):
  - Create a method `ValidateDate(DateTime? date, out string errorMessage)`.
  - Return `false` if date is in the future, set appropriate error message.
  - In `logWeightCommand` and `logRunCommand` handlers:
    - After parsing the date (from args or prompts), call `ValidateDate`.
    - If validation fails, use `AnsiConsole.MarkupLine($"[red]Error: {errorMessage}[/]")` to display red error.
    - Return early without making API call.
- Run tests from Prompt F.1. They should now pass.
```

### Prompt F.5: Implement Range Validation for Weight Data

```text
Context:
- Failing tests expect weight, BMI, fat, muscle, etc. to be validated against ranges.
- `spec.md` defines: weight 100-300 lbs, fat/muscle 0-100%, restingMetab > 1000, visceralFat 10-30.

Task:
- In `ValidationHelper.cs` (or inline in handler):
  - Create validation methods:
    - `ValidateWeight(double weight, out string errorMessage)`
    - `ValidatePercentage(double value, string fieldName, out string errorMessage)` (for fat/muscle)
    - `ValidateRestingMetab(int value, out string errorMessage)`
    - `ValidateVisceralFat(int value, out string errorMessage)`
- In `logWeightCommand` handler:
  - After gathering all weight data, validate each field.
  - If any validation fails, display red error and return early.
  - Only proceed to API call if all validations pass.
- Run tests from Prompt F.2. They should now pass.
```

### Prompt F.6: Implement Distance and Time Validation for Run Data

```text
Context:
- Failing tests expect distance unit and time format validation in run logging.

Task:
- In `ValidationHelper.cs`:
  - Create `ValidateDistanceWithUnit(string distanceWithUnit, out double distance, out string unit, out string errorMessage)`.
    - Parse numeric value and unit, ensure unit exists and distance > 0.
  - Create `ValidateTimeFormat(string timeStr, out int totalSeconds, out string errorMessage)`.
    - Parse MM:SS or HH:MM:SS, validate ranges (seconds < 60, minutes < 60).
- In `logRunCommand` handler:
  - Replace existing parsing with validation methods.
  - Display red errors if validation fails, return early.
  - Proceed to API call only if all validations pass.
- Run tests from Prompt F.3. They should now pass.
```

### Prompt F.7: Write Failing Tests for `--verbose` Exception Handling

```text
Context:
- CLI has a global `--verbose` option.
- `spec.md` requires full stack traces on exceptions when `--verbose` is used.
- Normal mode should show simple error messages.

Task:
- In `ValidationTests.cs` or new `ExceptionHandlingTests.cs`:
- **Test Case 9 (Exception Without Verbose):**
  - Mock an exception in a command handler (e.g., HTTP client throws).
  - Run command without `--verbose`.
  - Assert that only a simple error message is displayed (no stack trace).
- **Test Case 10 (Exception With Verbose):**
  - Same exception scenario.
  - Run command with `--verbose`.
  - Assert that full stack trace is displayed.
- These will fail as exception handling with verbose mode is not implemented.
```

### Prompt F.8: Implement Exception Handling with Verbose Mode

```text
Context:
- Failing tests expect different exception output based on `--verbose` flag.
- All CLI commands need consistent exception handling.

Task:
- In `HealthTracker.Cli\Program.cs`:
  - Create a helper method `HandleException(Exception ex, bool isVerbose)`.
    - If `isVerbose`: display `AnsiConsole.WriteException(ex)` (full stack trace).
    - If not verbose: display `AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]")`.
  - Wrap all command handlers in try-catch blocks.
  - Extract the `verbose` option value from `InvocationContext` and pass to `HandleException`.
- Alternative: Use `System.CommandLine`'s global exception handling if preferred.
- Run tests from Prompt F.7. They should now pass.
```

### Prompt F.9: Write Failing API Validation Tests

```text
Context:
- API endpoints should validate input and return HTTP 400 for invalid data.
- `spec.md` requires API validation with error messages.

Task:
- In `HealthTracker.Tests`, update `ApiEndpointsTests.cs` or create `ApiValidationTests.cs`.
- **Test Case 11 (Invalid Weight Data):**
  - POST to `/weight` with weight = 50 (out of range).
  - Assert HTTP 400 response with error message in body.
- **Test Case 12 (Invalid Run Data):**
  - POST to `/run` with negative distance.
  - Assert HTTP 400 response with appropriate error message.
- **Test Case 13 (Future Date):**
  - POST to `/weight` or `/run` with future date.
  - Assert HTTP 400 response.
- These will fail as API validation is not implemented.
```

### Prompt F.10: Implement Validation in API Endpoints

```text
Context:
- Failing tests expect API validation returning HTTP 400 for invalid data.
- Validation rules should match CLI validation.

Task:
- In `HealthTracker.Api`:
  - Create `ApiValidation.cs` with methods similar to CLI validation:
    - `ValidateWeighIn(WeighIn weighIn, out List<string> errors)`
    - `ValidateRun(Run run, out List<string> errors)`
  - In `Program.cs`, update endpoint handlers:
    - `POST /weight`: Call `ValidateWeighIn`, return `Results.BadRequest(errors)` if invalid.
    - `POST /run`: Call `ValidateRun`, return `Results.BadRequest(errors)` if invalid.
    - Only proceed to repository save if validation passes.
- Run tests from Prompt F.9. They should now pass.
```

### Prompt F.11: Refactor and Integration Testing

```text
Context:
- Validation is implemented in both CLI and API.
- Need to ensure consistent behavior and clean up any code duplication.

Task:
- **Refactoring:**
  - Move common validation logic to `HealthTracker.Core` if there's duplication between CLI and API.
  - Ensure error messages are consistent between CLI and API.
  - Clean up any code duplication in validation methods.
- **Integration Testing:**
  - Run all existing CLI and API tests to ensure validation doesn't break existing functionality.
  - Test end-to-end scenarios: CLI validation preventing invalid API calls.
  - Test API validation as backup when CLI validation is bypassed.
- **Final Testing:**
  - Run full test suite.
  - Manually test a few validation scenarios to ensure user experience is good.
- Commit changes: "feat: Implement comprehensive input validation and error handling".
```
