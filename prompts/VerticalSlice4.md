# Vertical Slice 4: Log Run (End-to-End) Prompts

This document contains the step-by-step prompts for implementing Vertical Slice 4, which adds the ability to log run data from CLI through API to database with full validation and testing.

## Step 1: Add Run Data Models

**Given** I need to handle run logging requests and responses  
**When** I define the run-specific models  
**Then** I should have complete data contracts for run operations

**Prompt:**
Extend the shared models in `HealthTracker.Shared` for run logging:

**Scenario:**

- **Given** a user wants to log run data
- **When** the CLI sends run data to the API
- **Then** the data should be validated and structured correctly

Implement:

1. Complete the `RunRequest` model with properties:

   - `Date` (DateTime)
   - `Distance` (decimal)
   - `DistanceUnit` (string)
   - `Time` (int) - stored as total seconds

2. Add `RunResponse` model with properties:

   - `Id` (int)
   - All properties from `RunRequest`
   - `CreatedAt` (DateTime)
   - `FormattedTime` (string) - computed property for display (MM:SS or HH:MM:SS)
   - `FormattedDistance` (string) - computed property with unit

3. Add validation attributes following the spec constraints:
   - Distance: > 0, max 2 decimal places
   - DistanceUnit: must be provided (mi, km, etc.)
   - Time: > 0 seconds
   - Date: cannot be future

Write unit tests using TDD to verify:

- Valid data passes validation
- Invalid data fails with appropriate error messages
- Edge cases for each constraint
- Time formatting works correctly (converts seconds to MM:SS or HH:MM:SS)
- Distance formatting includes proper units

---

## Step 2: Implement CLI Run Input Parsing

**Given** The CLI needs to parse run logging command arguments  
**When** I implement input parsing and validation  
**Then** The CLI should validate input before sending to API

**Prompt:**
Implement run input parsing in the CLI using TDD:

**Scenario:**

- **Given** a user types `health log run 3.1mi 25:30` or `health log run 5km 1:05:22`
- **When** the CLI parses the arguments
- **Then** it should create a valid `RunRequest` object

Create:

1. A `RunInputParser` class with methods:

   - `ParseRunInput(string[] args)` - main parsing method
   - `ParseDistance(string distanceInput)` - handles "3.1mi", "5km" format
   - `ParseTime(string timeInput)` - handles "MM:SS" or "HH:MM:SS" format
   - `ParseDistanceUnit(string distanceInput)` - extracts unit from distance

2. A `RunValidator` class with method `ValidateRunInput(RunRequest request)`
3. Handle optional date parameter (defaults to today)
4. Convert date from `M/D` format to full DateTime with current year

Write failing tests first for:

- Valid distance parsing extracts number and unit correctly
- Time parsing handles both MM:SS and HH:MM:SS formats
- Missing unit in distance throws validation error
- Invalid time formats throw parsing errors
- Distance with more than 2 decimal places is rejected
- Future dates are rejected
- Date defaults to today when omitted
- Negative or zero values are rejected

Then implement the minimal code to make tests pass.

---

## Step 3: Add Run Command to CLI

**Given** The CLI needs a `log run` command  
**When** I implement the run logging command  
**Then** Users should be able to log run data via CLI

**Prompt:**
Add the run logging command to the CLI using TDD:

**Scenario:**

- **Given** the CLI is configured with run logging capability
- **When** a user runs `health log run 3.1mi 25:30`
- **Then** the CLI should parse, validate, and send the data to the API

Implement:

1. Add `run` subcommand under existing `log` command group
2. Create `LogRunCommand` class that:

   - Uses `RunInputParser` to parse arguments
   - Uses `RunValidator` to validate input
   - Calls API client to submit data
   - Displays success/error messages using Spectre.Console

3. Update `IApiClient` interface with `LogRunAsync(RunRequest request)` method
4. Add mock implementation in `ApiClient` (return success for now)

Write failing tests first for:

- Command routing recognizes `log run` correctly
- Valid arguments result in API call with correct data
- Invalid arguments display validation errors in red
- Missing distance unit shows specific error message
- Invalid time format shows helpful error message
- API errors are handled gracefully
- Success message is displayed in green

---

## Step 4: Add Database Schema for Runs

**Given** The API needs to store run data in SQLite  
**When** I add the runs table structure  
**Then** The database should support run data persistence

**Prompt:**
Add the runs table to the database infrastructure using TDD:

**Scenario:**

- **Given** the API starts up with an empty database
- **When** the database initialization runs
- **Then** the runs table should be created with proper constraints

Implement:

1. Add `runs` table creation SQL to `DatabaseService`:

   ```sql
   CREATE TABLE IF NOT EXISTS runs (
       id INTEGER PRIMARY KEY AUTOINCREMENT,
       date TEXT NOT NULL,
       distance REAL NOT NULL CHECK(distance > 0),
       distanceUnit TEXT NOT NULL,
       time INTEGER NOT NULL CHECK(time > 0)
   );
   ```

2. Add `IRunRepository` interface with methods:

   - `Task<int> InsertRunAsync(RunRequest request)`
   - `Task<int> GetRunCountAsync()`
   - `Task<DateTime?> GetLastRunDateAsync()`

3. Add `RunRepository` class implementing the interface using Dapper

Write failing tests first for:

- Table creation SQL is valid
- Insert operations work with valid data
- Database constraints reject invalid data (distance <= 0, time <= 0)
- Distance unit is properly stored and retrieved
- Time is stored as integer seconds
- Count and last date queries work correctly
- Repository methods handle empty database

---

## Step 5: Implement API Run Endpoint

**Given** The API needs to accept run logging requests  
**When** I implement the POST /run endpoint  
**Then** The API should validate and store run data

**Prompt:**
Add the run logging endpoint to the API using TDD:

**Scenario:**

- **Given** the API is running with database connectivity
- **When** a POST request is made to `/run` with valid run data
- **Then** the data should be validated, stored, and return success

Implement:

1. Add `POST /run` endpoint to `Program.cs`
2. Create run endpoint handler that:

   - Accepts `RunRequest` from request body
   - Validates the request using model validation
   - Calls `IRunRepository` to store data
   - Returns appropriate HTTP status codes

3. Update `IHealthService` to use `IRunRepository` for actual run counts
4. Register `IRunRepository` in DI container
5. Update `/about` endpoint to include run statistics

Write failing integration tests first for:

- POST with valid data returns 201 Created
- POST with invalid data returns 400 Bad Request with validation errors
- Database constraints are enforced (e.g., distance <= 0)
- Missing distance unit returns validation error
- Successful inserts update the count returned by `/about`
- Error handling for database failures
- Time is stored correctly as seconds

Then implement minimal code to make tests pass.

---

## Step 6: Connect CLI to API for Run Logging

**Given** The CLI and API both support run operations  
**When** I connect them together  
**Then** Run data should flow from CLI to database

**Prompt:**
Complete the integration between CLI and API for run logging using TDD:

**Scenario:**

- **Given** both CLI and API are running
- **When** a user executes `health log run 5km 22:15`
- **Then** the data should be stored in the database and success confirmed

Implement:

1. Complete `ApiClient.LogRunAsync()` method to make actual HTTP POST to `/run`
2. Add proper error handling for HTTP failures
3. Add JSON serialization/deserialization for request/response
4. Update CLI output formatting to show success/failure messages

Write failing tests first for:

- `ApiClient` makes correct HTTP POST with JSON body
- HTTP 201 response is handled as success
- HTTP 400 response shows validation errors to user
- HTTP 500 response shows generic error message
- Network failures are handled gracefully
- Time conversion from seconds works correctly in both directions

Integration test:

- End-to-end test that posts run data and verifies it's stored
- Test using TestServer without Docker dependency

---

## Step 7: Add Distance Unit Handling and Validation

**Given** Run entries need proper distance unit handling  
**When** I implement comprehensive unit validation  
**Then** Only valid distance units should be accepted

**Prompt:**
Implement comprehensive distance unit handling for run logging using TDD:

**Scenario:**

- **Given** a user logs run with distance "3.1mi" or "5km"
- **When** the system processes the distance
- **Then** it should validate the unit and store correctly

Implement:

1. `DistanceUnitHelper` class with methods:

   - `ValidateDistanceUnit(string unit)` - validates supported units
   - `NormalizeDistanceUnit(string unit)` - standardizes unit format
   - `GetSupportedUnits()` - returns list of valid units (mi, km, m, ft)

2. Update `RunInputParser` to use `DistanceUnitHelper`
3. Add unit validation to both CLI and API
4. Add helpful error messages for unsupported units

Write failing tests first for:

- Supported units (mi, km, m, ft) are accepted
- Unsupported units are rejected with helpful message
- Unit normalization handles case variations (MI, Mi, mi)
- Distance parsing rejects input without unit
- Error messages suggest valid alternatives
- Empty or whitespace units are handled

---

## Step 8: Add Time Format Validation and Conversion

**Given** Run entries need robust time handling  
**When** I implement time parsing and validation  
**Then** Time should be stored consistently and displayed properly

**Prompt:**
Add comprehensive time handling for run logging using TDD:

**Scenario:**

- **Given** a user logs run with time "25:30" or "1:05:22"
- **When** the system processes the time
- **Then** it should convert to seconds for storage and back for display

Implement:

1. `TimeHelper` class with methods:

   - `ParseTimeToSeconds(string timeInput)` - converts MM:SS or HH:MM:SS to seconds
   - `FormatSecondsToTime(int seconds)` - converts seconds back to display format
   - `ValidateTimeFormat(string timeInput)` - validates input format

2. Update `RunInputParser` to use `TimeHelper`
3. Add comprehensive time validation
4. Handle edge cases (00:00, negative times, etc.)

Write failing tests first for:

- MM:SS format parsing works correctly (25:30 = 1530 seconds)
- HH:MM:SS format parsing works correctly (1:05:22 = 3922 seconds)
- Invalid formats are rejected (25:99, 1:05, etc.)
- Zero time is rejected
- Negative time components are rejected
- Seconds to time conversion produces correct format
- Large times are handled correctly (marathon+ times)

---

## Step 9: Add Run Logging End-to-End Test

**Given** The complete run logging feature is implemented  
**When** I create end-to-end tests  
**Then** I should verify the complete flow works correctly

**Prompt:**
Create comprehensive end-to-end tests for run logging using TDD:

**Scenario:**

- **Given** the API is running in Docker with empty database
- **When** I execute run logging commands via CLI
- **Then** the data should be stored and retrievable

Implement in `HealthTracker.Tests.E2E`:

1. `RunLoggingE2ETests` class with setup/teardown for Docker container
2. Test scenarios:

   - Log run with miles and MM:SS time
   - Log run with kilometers and HH:MM:SS time
   - Log run with default date
   - Verify data appears in `/about` endpoint
   - Test validation error scenarios
   - Test multiple run entries

3. Helper methods for:
   - Executing CLI commands and capturing output
   - Making direct API calls to verify data
   - Database cleanup between tests
   - Validating time and distance conversions

Write failing tests first for:

- Successful run logging increases count in `/about`
- Invalid input shows validation errors without storing data
- Multiple entries are stored correctly
- CLI output matches expected format
- Time conversion works correctly end-to-end
- Distance units are preserved correctly
- Error scenarios are handled gracefully

---

## Step 10: Add Run Data Integrity and Error Recovery

**Given** The system needs robust error handling for runs  
**When** I implement error recovery and data integrity  
**Then** The system should handle failures gracefully

**Prompt:**
Add robust error handling and data integrity for run logging using TDD:

**Scenario:**

- **Given** various failure conditions occur during run logging
- **When** the system encounters errors
- **Then** it should handle them gracefully without data corruption

Implement:

1. Database transaction handling in `RunRepository`
2. Retry logic for transient failures
3. Data integrity validation before/after database operations
4. Graceful degradation when API is unavailable

Write failing tests first for:

- Database transaction rollback on validation failure
- Retry logic for transient database errors
- CLI handles API unavailability with helpful message
- Partial data doesn't get stored on validation failure
- Connection string validation and fallback
- Time and distance data integrity is maintained

---

## Step 11: Add Input Format Flexibility and User Experience

**Given** Users may enter run data in various formats  
**When** I enhance input parsing flexibility  
**Then** The system should handle common input variations

**Prompt:**
Enhance run input parsing for better user experience using TDD:

**Scenario:**

- **Given** a user enters run data in slightly different formats
- **When** the system parses the input
- **Then** it should handle common variations gracefully

Implement:

1. Enhanced `RunInputParser` with support for:

   - Distance with spaces ("3.1 mi" vs "3.1mi")
   - Time with various separators ("25:30" vs "25.30")
   - Case-insensitive units ("MI", "Mi", "mi")
   - Decimal vs integer distances ("3" vs "3.0")

2. Improved error messages with suggestions:
   - Show expected format when parsing fails
   - Suggest valid units when unit is invalid
   - Provide examples of valid time formats

Write failing tests first for:

- Various distance formats are parsed correctly
- Different time separators work
- Case variations in units are handled
- Helpful error messages include examples
- Edge cases (single digit times, etc.) work
- User-friendly validation messages

---

## Step 12: Documentation and Integration Verification

**Given** The run logging feature is complete  
**When** I document and verify the integration  
**Then** The feature should be ready for production use

**Prompt:**
Complete documentation and final integration verification for run logging:

**Documentation to update:**

1. Update `README.md` with run logging examples
2. Add inline code documentation for new classes
3. Update API documentation with `/run` endpoint details
4. Add troubleshooting guide for common validation errors
5. Include examples of supported distance units and time formats

**Final integration verification:**

1. Test complete workflow:
   - `.\build.ps1 -Target Build`
   - `.\build.ps1 -Target Test`
   - `.\build.ps1 -Target RunApi`
   - Test various run logging scenarios
   - Verify data persistence and retrieval

**Acceptance criteria verification:**

- [ ] All tests (unit, integration, E2E) are passing
- [ ] Run logging works with valid inputs
- [ ] Distance units are properly validated and stored
- [ ] Time formats are handled correctly (MM:SS and HH:MM:SS)
- [ ] Validation errors are clear and helpful
- [ ] Data persists correctly in database
- [ ] CLI provides good user experience
- [ ] API handles errors gracefully
- [ ] Documentation is complete and accurate
- [ ] Performance is acceptable

Create a final comprehensive test that exercises the entire run logging feature end-to-end, including:

- Logging runs with different distance units
- Testing both time formats
- Verifying data accuracy and persistence
- Testing edge cases and error scenarios
- Confirming integration with existing `/about` endpoint
