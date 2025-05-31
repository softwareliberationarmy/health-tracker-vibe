# Vertical Slice 5: View Runs (End-to-End) Prompts

This document contains the step-by-step prompts for implementing Vertical Slice 5, which adds the ability to view run data from the database through the API and display it in a formatted table via the CLI.

## Step 1: Add Run Retrieval Data Models

**Given** I need to retrieve and display run data  
**When** I define the models for run retrieval  
**Then** I should have complete data contracts for viewing run operations

**Prompt:**
Add models for run data retrieval in `HealthTracker.Shared`:

**Scenario:**

- **Given** a user wants to view their last 5 runs
- **When** the API returns run data
- **Then** the data should be properly structured for display

Implement:

1. `RunListResponse` model with properties:

   - `Runs` (List<RunResponse>)
   - `TotalCount` (int)
   - `RequestedCount` (int)

2. Update `RunResponse` to include display-friendly formatting:
   - Add `FormattedDate` property (computed from Date)
   - Add `FormattedTime` property for display (converts seconds to MM:SS or HH:MM:SS)
   - Add `FormattedDistance` property with unit
   - Add `CalculatedPace` property (time per distance unit)
   - Add constructor/factory method for easy creation

Write unit tests using TDD to verify:

- `RunListResponse` properly holds collections of runs
- `RunResponse` formatting methods work correctly
- Edge cases like empty lists are handled
- Date formatting matches expected display format
- Time formatting correctly shows MM:SS for < 1 hour, HH:MM:SS for >= 1 hour
- Pace calculation works correctly (minutes per mile/km)

---

## Step 2: Add Database Run Retrieval Methods

**Given** The API needs to retrieve run data from SQLite  
**When** I add run retrieval methods to the repository  
**Then** The database should support querying recent runs

**Prompt:**
Add run data retrieval methods to the database layer using TDD:

**Scenario:**

- **Given** the database contains run entries
- **When** a request is made for the last N runs
- **Then** the correct data should be returned in descending date order

Implement:

1. Add to `IRunRepository` interface:

   - `Task<List<RunResponse>> GetLastRunsAsync(int count)`
   - `Task<RunResponse?> GetRunByIdAsync(int id)`

2. Add to `RunRepository` class:
   - Implement methods using Dapper queries
   - Ensure proper SQL ordering (newest first)
   - Handle edge cases (count = 0, no data)
   - Convert time from seconds to proper display format

Write failing tests first for:

- Retrieving last N runs returns correct count
- Results are ordered by date descending (newest first)
- Method handles empty database gracefully
- Method handles count larger than available records
- SQL injection protection (parameterized queries)
- Date parsing from ISO 8601 storage format works correctly
- Time conversion from seconds storage to display format
- Distance and unit data is retrieved correctly

Then implement minimal code to make tests pass.

---

## Step 3: Implement API Run Retrieval Endpoint

**Given** The API needs to serve run data to the CLI  
**When** I implement the GET /run/last/{count} endpoint  
**Then** The API should return formatted run data

**Prompt:**
Add the run retrieval endpoint to the API using TDD:

**Scenario:**

- **Given** the API has run data in the database
- **When** a GET request is made to `/run/last/5`
- **Then** the last 5 runs should be returned in JSON format

Implement:

1. Add `GET /run/last/{count}` endpoint to `Program.cs`
2. Add endpoint handler that:

   - Validates the count parameter (1-50 range)
   - Calls `IRunRepository.GetLastRunsAsync()`
   - Returns `RunListResponse`
   - Handles edge cases and errors

3. Add parameter validation and error responses

Write failing integration tests first for:

- GET with valid count returns 200 OK with correct data
- GET with count=0 returns 400 Bad Request
- GET with count>50 returns 400 Bad Request (reasonable limit)
- GET with no data returns 200 OK with empty list
- Response structure matches `RunListResponse`
- Data is ordered correctly (newest first)
- Time formatting is consistent in API responses
- Distance and unit data is properly serialized

Then implement minimal code to make tests pass.

---

## Step 4: Add CLI Run Display Formatting

**Given** The CLI needs to display run data in a table format  
**When** I implement table formatting with Spectre.Console  
**Then** Run data should be displayed in a readable table

**Prompt:**
Implement run data display formatting using Spectre.Console and TDD:

**Scenario:**

- **Given** the CLI receives run data from the API
- **When** the view run command displays the data
- **Then** it should show a formatted table with columns for all run metrics

Implement:

1. `RunTableFormatter` class with methods:

   - `FormatRunTable(RunListResponse data)` - creates Spectre.Console table
   - `FormatDistanceValue(decimal distance, string unit)` - formats distance display
   - `FormatTimeValue(string timeString)` - formats time display
   - `FormatPaceValue(string pace)` - formats pace display (min/mile or min/km)
   - `FormatDate(DateTime date)` - formats date for display

2. Table formatting specifications:
   - Columns: Date, Distance, Time, Pace
   - Proper alignment and spacing (right-align numeric columns)
   - Color coding for headers
   - Handle empty data gracefully
   - Include units in headers

Write failing tests first for:

- Table formatting produces expected column structure
- Data formatting matches specifications (distance with units, time format)
- Empty data shows appropriate message
- Date formatting is consistent
- Color markup is properly applied
- Table width fits typical console
- Pace calculation and display works correctly
- Different distance units are handled properly

Then implement minimal code to make tests pass.

---

## Step 5: Add Run View Command to CLI

**Given** The CLI needs a `view run` command  
**When** I implement the run viewing command  
**Then** Users should be able to view their recent runs

**Prompt:**
Add the run viewing command to the CLI using TDD:

**Scenario:**

- **Given** the CLI is configured with run viewing capability
- **When** a user runs `health view run --last 5`
- **Then** the CLI should fetch and display the last 5 runs in a table

Implement:

1. Add `run` subcommand under existing `view` command group
2. Create `ViewRunCommand` class that:

   - Parses the `--last` parameter (default to 5)
   - Validates the count parameter (1-50 range)
   - Calls API client to fetch data
   - Uses `RunTableFormatter` to display results

3. Update `IApiClient` interface with `GetLastRunsAsync(int count)` method
4. Add mock implementation in `ApiClient` (return test data for now)

Write failing tests first for:

- Command routing recognizes `view run` correctly
- `--last` parameter is parsed correctly
- Default value of 5 is used when `--last` is omitted
- Invalid count values show validation errors
- API call is made with correct parameters
- Table formatting is applied to results
- Empty results show appropriate message
- Error handling when API is unavailable

---

## Step 6: Connect CLI to API for Run Viewing

**Given** The CLI and API both support run viewing operations  
**When** I connect them together  
**Then** Run data should flow from database through API to CLI display

**Prompt:**
Complete the integration between CLI and API for run viewing using TDD:

**Scenario:**

- **Given** both CLI and API are running with run data in database
- **When** a user executes `health view run --last 3`
- **Then** the last 3 runs should be retrieved and displayed in a table

Implement:

1. Complete `ApiClient.GetLastRunsAsync()` method to make actual HTTP GET to `/run/last/{count}`
2. Add proper error handling for HTTP failures
3. Add JSON deserialization for `RunListResponse`
4. Update CLI to handle API errors gracefully

Write failing tests first for:

- `ApiClient` makes correct HTTP GET with count parameter
- HTTP 200 response is deserialized correctly
- HTTP 400 response shows validation errors to user
- HTTP 404/500 responses show appropriate error messages
- Network failures are handled gracefully
- Empty response is handled correctly
- Time and distance data is preserved through JSON serialization

Integration test:

- End-to-end test that retrieves run data and verifies display
- Test using TestServer without Docker dependency

---

## Step 7: Add Pace Calculation and Display

**Given** Runners often want to see their pace per distance unit  
**When** I add pace calculation functionality  
**Then** The table should display pace information clearly

**Prompt:**
Implement pace calculation and display for run data using TDD:

**Scenario:**

- **Given** run data contains distance and time
- **When** the table is displayed
- **Then** it should show calculated pace in minutes per distance unit

Implement:

1. `PaceCalculator` class with methods:

   - `CalculatePace(decimal distance, int timeInSeconds, string unit)` - calculates pace
   - `FormatPace(decimal paceInMinutes, string unit)` - formats for display
   - `ConvertToMinutesPerMile(decimal pace, string fromUnit)` - standardizes pace

2. Update `RunTableFormatter` to include pace column
3. Handle different distance units for pace calculation
4. Add pace validation and edge case handling

Write failing tests first for:

- Pace calculation works correctly for miles (time/distance = min/mile)
- Pace calculation works correctly for kilometers
- Pace formatting shows appropriate precision (MM:SS per unit)
- Edge cases handled (very fast/slow paces, zero distance)
- Different units produce correct pace calculations
- Pace display is consistent and readable

---

## Step 8: Add Run Table Display Enhancements

**Given** The run table display needs to be user-friendly  
**When** I enhance the table formatting  
**Then** The display should be clear, readable, and informative

**Prompt:**
Enhance the run table display with better formatting and user experience using TDD:

**Scenario:**

- **Given** run data is being displayed
- **When** the table is rendered
- **Then** it should have clear headers, proper formatting, and helpful context

Implement:

1. Enhanced `RunTableFormatter` with:

   - Table title showing count and date range
   - Alternating row colors for readability
   - Right-aligned numeric columns
   - Units displayed in headers (Distance, Time, Pace)
   - Summary statistics (total distance, total time, average pace)

2. Add empty state handling:

   - Friendly message when no data exists
   - Suggestion to log run data first

3. Add data insights:
   - Highlight personal bests (fastest pace, longest distance)
   - Show recent trends if applicable

Write failing tests first for:

- Table title includes correct count and date range
- Numeric columns are right-aligned
- Units are displayed in headers
- Empty state shows helpful message
- Color formatting is applied consistently
- Summary statistics are calculated correctly
- Personal bests are highlighted appropriately

---

## Step 9: Add Parameter Validation and Error Handling

**Given** The view run command needs robust input validation  
**When** I implement comprehensive validation  
**Then** Invalid inputs should be caught with helpful error messages

**Prompt:**
Add comprehensive validation for run viewing parameters using TDD:

**Scenario:**

- **Given** a user provides invalid parameters to view runs
- **When** the system validates the input
- **Then** clear, helpful error messages should be displayed

Implement:

1. `ViewRunValidator` class with methods:

   - `ValidateCountParameter(int count)` - validates --last parameter
   - `ValidateApiResponse(RunListResponse response)` - validates API data

2. Enhanced error handling:

   - Count must be between 1 and 50
   - Clear error messages for each validation rule
   - Suggestions for valid usage

3. CLI integration to show validation errors in red with Spectre.Console

Write failing tests first for:

- Count validation with boundary testing (0, 1, 50, 51)
- Error messages are clear and actionable
- Valid parameters pass validation
- API response validation handles malformed data
- Edge cases (negative numbers, non-integers) are handled
- Helpful suggestions are provided for invalid input

---

## Step 10: Add Run Viewing End-to-End Test

**Given** The complete run viewing feature is implemented  
**When** I create end-to-end tests  
**Then** I should verify the complete flow works correctly

**Prompt:**
Create comprehensive end-to-end tests for run viewing using TDD:

**Scenario:**

- **Given** the API is running in Docker with run data in database
- **When** I execute run viewing commands via CLI
- **Then** the data should be retrieved and displayed correctly

Implement in `HealthTracker.Tests.E2E`:

1. `RunViewingE2ETests` class with setup/teardown for Docker container
2. Test scenarios:

   - View runs with default count (5)
   - View runs with custom count (3, 10)
   - View runs when no data exists
   - View runs with various distance units
   - Test parameter validation

3. Helper methods for:
   - Pre-populating database with test run data
   - Executing CLI commands and capturing output
   - Parsing table output for verification
   - Cleaning up test data between tests
   - Validating pace calculations in output

Write failing tests first for:

- CLI displays correct number of entries
- Table formatting is preserved in output
- Data is displayed in correct order (newest first)
- Empty database shows appropriate message
- Parameter validation errors are displayed
- Table headers and formatting are correct
- Pace calculations appear correctly in output
- Different distance units are displayed properly

---

## Step 11: Add Data Sorting and Filtering Options

**Given** Users may want different sorting and filtering options for runs  
**When** I add basic sorting capabilities  
**Then** Users should be able to view run data in different orders

**Prompt:**
Add sorting options to the run viewing feature using TDD:

**Scenario:**

- **Given** a user wants to see run data in different orders
- **When** they specify sorting options
- **Then** the data should be displayed in the requested order

Implement:

1. Add optional `--sort` parameter to `view run` command:

   - Options: `date-desc` (default), `date-asc`, `distance-desc`, `distance-asc`, `time-asc`, `time-desc`, `pace-asc`, `pace-desc`
   - Update API endpoint to accept sort parameter
   - Update repository methods to handle different sorting

2. Enhanced `RunTableFormatter` to show sort order in title
3. Add pace-based sorting logic

Write failing tests first for:

- Sort parameter parsing works correctly
- API correctly sorts data based on parameter
- Default sorting is date descending
- Invalid sort options show validation errors
- Table title reflects current sort order
- Pace-based sorting works correctly
- Distance and time sorting handle different units properly

Note: Keep this simple - just basic sorting options for now.

---

## Step 12: Add Performance Optimization for Large Datasets

**Given** The run viewing feature needs to handle larger datasets efficiently  
**When** I optimize for performance  
**Then** The system should handle data retrieval efficiently

**Prompt:**
Optimize the run viewing feature for performance using TDD:

**Scenario:**

- **Given** the system has many run entries
- **When** viewing operations are performed
- **Then** they should complete quickly with minimal resource usage

Implement:

1. Database query optimization:

   - Add proper indexing for date-based queries
   - Optimize SQL queries for large datasets
   - Add query result caching (if appropriate)

2. Memory optimization:

   - Stream large result sets efficiently
   - Proper disposal of database connections
   - Limit maximum retrievable count to prevent abuse

3. Calculation optimization:
   - Cache pace calculations
   - Optimize formatting operations

Write performance tests for:

- Run viewing completes within reasonable time
- Memory usage stays within bounds for large datasets
- Database queries are efficient (measure query time)
- Connection pooling works correctly
- Pace calculations don't significantly impact performance

Focus on measurable improvements while maintaining code clarity.

---

## Step 13: Documentation and Integration Verification

**Given** The run viewing feature is complete  
**When** I document and verify the integration  
**Then** The feature should be ready for production use

**Prompt:**
Complete documentation and final integration verification for run viewing:

**Documentation to update:**

1. Update `README.md` with run viewing examples
2. Add inline code documentation for new classes
3. Update API documentation with `/run/last/{count}` endpoint details
4. Add usage examples for different viewing scenarios
5. Document pace calculation methodology

**Final integration verification:**

1. Test complete workflow:
   - `.\build.ps1 -Target Build`
   - `.\build.ps1 -Target Test`
   - `.\build.ps1 -Target RunApi`
   - Log some run data using Slice 4
   - Test various run viewing scenarios
   - Verify table formatting and data accuracy

**Acceptance criteria verification:**

- [ ] All tests (unit, integration, E2E) are passing
- [ ] Run viewing works with different count parameters
- [ ] Table formatting is clear and readable
- [ ] Pace calculations are accurate and displayed properly
- [ ] Empty state is handled gracefully
- [ ] Parameter validation works correctly
- [ ] Different distance units are handled properly
- [ ] API handles requests efficiently
- [ ] CLI provides good user experience
- [ ] Documentation is complete and accurate
- [ ] Performance is acceptable

Create a final comprehensive test that exercises the entire run viewing feature end-to-end, including:

- Logging run data with various distance units
- Viewing with different parameters
- Verifying table formatting and data accuracy
- Testing pace calculations
- Testing edge cases and error scenarios
- Confirming integration with existing `/about` endpoint
