# Vertical Slice 3: View Weigh-Ins (End-to-End) Prompts

This document contains the step-by-step prompts for implementing Vertical Slice 3, which adds the ability to view weight data from the database through the API and display it in a formatted table via the CLI.

## Step 1: Add Weight Retrieval Data Models

**Given** I need to retrieve and display weight data  
**When** I define the models for weight retrieval  
**Then** I should have complete data contracts for viewing weight operations

**Prompt:**
Add models for weight data retrieval in `HealthTracker.Shared`:

**Scenario:**

- **Given** a user wants to view their last 5 weigh-ins
- **When** the API returns weight data
- **Then** the data should be properly structured for display

Implement:

1. `WeighInListResponse` model with properties:

   - `WeighIns` (List<WeighInResponse>)
   - `TotalCount` (int)
   - `RequestedCount` (int)

2. Update `WeighInResponse` to include display-friendly formatting:
   - Add `FormattedDate` property (computed from Date)
   - Add `FormattedWeight` property for display
   - Add constructor/factory method for easy creation

Write unit tests using TDD to verify:

- `WeighInListResponse` properly holds collections of weigh-ins
- `WeighInResponse` formatting methods work correctly
- Edge cases like empty lists are handled
- Date formatting matches expected display format

---

## Step 2: Add Database Retrieval Methods

**Given** The API needs to retrieve weight data from SQLite  
**When** I add weight retrieval methods to the repository  
**Then** The database should support querying recent weigh-ins

**Prompt:**
Add weight data retrieval methods to the database layer using TDD:

**Scenario:**

- **Given** the database contains weight entries
- **When** a request is made for the last N weigh-ins
- **Then** the correct data should be returned in descending date order

Implement:

1. Add to `IWeighInRepository` interface:

   - `Task<List<WeighInResponse>> GetLastWeighInsAsync(int count)`
   - `Task<WeighInResponse?> GetWeighInByIdAsync(int id)`

2. Add to `WeighInRepository` class:
   - Implement methods using Dapper queries
   - Ensure proper SQL ordering (newest first)
   - Handle edge cases (count = 0, no data)

Write failing tests first for:

- Retrieving last N weigh-ins returns correct count
- Results are ordered by date descending (newest first)
- Method handles empty database gracefully
- Method handles count larger than available records
- SQL injection protection (parameterized queries)
- Date parsing from ISO 8601 storage format works correctly

Then implement minimal code to make tests pass.

---

## Step 3: Implement API Weight Retrieval Endpoint

**Given** The API needs to serve weight data to the CLI  
**When** I implement the GET /weight/last/{count} endpoint  
**Then** The API should return formatted weight data

**Prompt:**
Add the weight retrieval endpoint to the API using TDD:

**Scenario:**

- **Given** the API has weight data in the database
- **When** a GET request is made to `/weight/last/5`
- **Then** the last 5 weigh-ins should be returned in JSON format

Implement:

1. Add `GET /weight/last/{count}` endpoint to `Program.cs`
2. Add endpoint handler that:

   - Validates the count parameter (1-50 range)
   - Calls `IWeighInRepository.GetLastWeighInsAsync()`
   - Returns `WeighInListResponse`
   - Handles edge cases and errors

3. Add parameter validation and error responses

Write failing integration tests first for:

- GET with valid count returns 200 OK with correct data
- GET with count=0 returns 400 Bad Request
- GET with count>50 returns 400 Bad Request (reasonable limit)
- GET with no data returns 200 OK with empty list
- Response structure matches `WeighInListResponse`
- Data is ordered correctly (newest first)

Then implement minimal code to make tests pass.

---

## Step 4: Add CLI Weight Display Formatting

**Given** The CLI needs to display weight data in a table format  
**When** I implement table formatting with Spectre.Console  
**Then** Weight data should be displayed in a readable table

**Prompt:**
Implement weight data display formatting using Spectre.Console and TDD:

**Scenario:**

- **Given** the CLI receives weight data from the API
- **When** the view weight command displays the data
- **Then** it should show a formatted table with columns for all weight metrics

Implement:

1. `WeightTableFormatter` class with methods:

   - `FormatWeightTable(WeighInListResponse data)` - creates Spectre.Console table
   - `FormatWeightValue(decimal weight)` - formats weight display
   - `FormatPercentage(decimal percentage)` - formats fat/muscle percentages
   - `FormatDate(DateTime date)` - formats date for display

2. Table formatting specifications:
   - Columns: Date, Weight, BMI, Fat%, Muscle%, Resting Metab, Visceral Fat
   - Proper alignment and spacing
   - Color coding for headers
   - Handle empty data gracefully

Write failing tests first for:

- Table formatting produces expected column structure
- Data formatting matches specifications (2 decimal places, etc.)
- Empty data shows appropriate message
- Date formatting is consistent
- Color markup is properly applied
- Table width fits typical console

Then implement minimal code to make tests pass.

---

## Step 5: Add Weight View Command to CLI

**Given** The CLI needs a `view weight` command  
**When** I implement the weight viewing command  
**Then** Users should be able to view their recent weigh-ins

**Prompt:**
Add the weight viewing command to the CLI using TDD:

**Scenario:**

- **Given** the CLI is configured with weight viewing capability
- **When** a user runs `health view weight --last 5`
- **Then** the CLI should fetch and display the last 5 weigh-ins in a table

Implement:

1. Add `view` command group to the CLI command structure
2. Add `weight` subcommand under `view` with optional `--last` parameter
3. Create `ViewWeightCommand` class that:

   - Parses the `--last` parameter (default to 5)
   - Validates the count parameter (1-50 range)
   - Calls API client to fetch data
   - Uses `WeightTableFormatter` to display results

4. Update `IApiClient` interface with `GetLastWeighInsAsync(int count)` method
5. Add mock implementation in `ApiClient` (return test data for now)

Write failing tests first for:

- Command routing recognizes `view weight` correctly
- `--last` parameter is parsed correctly
- Default value of 5 is used when `--last` is omitted
- Invalid count values show validation errors
- API call is made with correct parameters
- Table formatting is applied to results
- Empty results show appropriate message

---

## Step 6: Connect CLI to API for Weight Viewing

**Given** The CLI and API both support weight viewing operations  
**When** I connect them together  
**Then** Weight data should flow from database through API to CLI display

**Prompt:**
Complete the integration between CLI and API for weight viewing using TDD:

**Scenario:**

- **Given** both CLI and API are running with weight data in database
- **When** a user executes `health view weight --last 3`
- **Then** the last 3 weigh-ins should be retrieved and displayed in a table

Implement:

1. Complete `ApiClient.GetLastWeighInsAsync()` method to make actual HTTP GET to `/weight/last/{count}`
2. Add proper error handling for HTTP failures
3. Add JSON deserialization for `WeighInListResponse`
4. Update CLI to handle API errors gracefully

Write failing tests first for:

- `ApiClient` makes correct HTTP GET with count parameter
- HTTP 200 response is deserialized correctly
- HTTP 400 response shows validation errors to user
- HTTP 404/500 responses show appropriate error messages
- Network failures are handled gracefully
- Empty response is handled correctly

Integration test:

- End-to-end test that retrieves weight data and verifies display
- Test using TestServer without Docker dependency

---

## Step 7: Add Table Display Enhancements

**Given** The weight table display needs to be user-friendly  
**When** I enhance the table formatting  
**Then** The display should be clear, readable, and informative

**Prompt:**
Enhance the weight table display with better formatting and user experience using TDD:

**Scenario:**

- **Given** weight data is being displayed
- **When** the table is rendered
- **Then** it should have clear headers, proper formatting, and helpful context

Implement:

1. Enhanced `WeightTableFormatter` with:

   - Table title showing count and date range
   - Alternating row colors for readability
   - Right-aligned numeric columns
   - Units displayed in headers (lbs, %, etc.)
   - Summary statistics (average, trend indicators)

2. Add empty state handling:

   - Friendly message when no data exists
   - Suggestion to log weight data first

3. Add data validation display:
   - Highlight unusual values (if any)
   - Show data completeness indicators

Write failing tests first for:

- Table title includes correct count and date range
- Numeric columns are right-aligned
- Units are displayed in headers
- Empty state shows helpful message
- Color formatting is applied consistently
- Summary statistics are calculated correctly

---

## Step 8: Add Parameter Validation and Error Handling

**Given** The view weight command needs robust input validation  
**When** I implement comprehensive validation  
**Then** Invalid inputs should be caught with helpful error messages

**Prompt:**
Add comprehensive validation for weight viewing parameters using TDD:

**Scenario:**

- **Given** a user provides invalid parameters to view weight
- **When** the system validates the input
- **Then** clear, helpful error messages should be displayed

Implement:

1. `ViewWeightValidator` class with methods:

   - `ValidateCountParameter(int count)` - validates --last parameter
   - `ValidateApiResponse(WeighInListResponse response)` - validates API data

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

---

## Step 9: Add Weight Viewing End-to-End Test

**Given** The complete weight viewing feature is implemented  
**When** I create end-to-end tests  
**Then** I should verify the complete flow works correctly

**Prompt:**
Create comprehensive end-to-end tests for weight viewing using TDD:

**Scenario:**

- **Given** the API is running in Docker with weight data in database
- **When** I execute weight viewing commands via CLI
- **Then** the data should be retrieved and displayed correctly

Implement in `HealthTracker.Tests.E2E`:

1. `WeightViewingE2ETests` class with setup/teardown for Docker container
2. Test scenarios:

   - View weights with default count (5)
   - View weights with custom count (3, 10)
   - View weights when no data exists
   - View weights with various data scenarios
   - Test parameter validation

3. Helper methods for:
   - Pre-populating database with test weight data
   - Executing CLI commands and capturing output
   - Parsing table output for verification
   - Cleaning up test data between tests

Write failing tests first for:

- CLI displays correct number of entries
- Table formatting is preserved in output
- Data is displayed in correct order (newest first)
- Empty database shows appropriate message
- Parameter validation errors are displayed
- Table headers and formatting are correct

---

## Step 10: Add Data Sorting and Filtering Options

**Given** Users may want different sorting and filtering options  
**When** I add basic sorting capabilities  
**Then** Users should be able to view data in different orders

**Prompt:**
Add sorting options to the weight viewing feature using TDD:

**Scenario:**

- **Given** a user wants to see weight data in different orders
- **When** they specify sorting options
- **Then** the data should be displayed in the requested order

Implement:

1. Add optional `--sort` parameter to `view weight` command:

   - Options: `date-desc` (default), `date-asc`, `weight-desc`, `weight-asc`
   - Update API endpoint to accept sort parameter
   - Update repository methods to handle different sorting

2. Enhanced `WeightTableFormatter` to show sort order in title

Write failing tests first for:

- Sort parameter parsing works correctly
- API correctly sorts data based on parameter
- Default sorting is date descending
- Invalid sort options show validation errors
- Table title reflects current sort order

Note: Keep this simple - just basic sorting options for now.

---

## Step 11: Add Performance Optimization for Large Datasets

**Given** The weight viewing feature needs to handle larger datasets efficiently  
**When** I optimize for performance  
**Then** The system should handle data retrieval efficiently

**Prompt:**
Optimize the weight viewing feature for performance using TDD:

**Scenario:**

- **Given** the system has many weight entries
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

Write performance tests for:

- Weight viewing completes within reasonable time
- Memory usage stays within bounds for large datasets
- Database queries are efficient (measure query time)
- Connection pooling works correctly

Focus on measurable improvements while maintaining code clarity.

---

## Step 12: Documentation and Integration Verification

**Given** The weight viewing feature is complete  
**When** I document and verify the integration  
**Then** The feature should be ready for production use

**Prompt:**
Complete documentation and final integration verification for weight viewing:

**Documentation to update:**

1. Update `README.md` with weight viewing examples
2. Add inline code documentation for new classes
3. Update API documentation with `/weight/last/{count}` endpoint details
4. Add usage examples for different viewing scenarios

**Final integration verification:**

1. Test complete workflow:
   - `.\build.ps1 -Target Build`
   - `.\build.ps1 -Target Test`
   - `.\build.ps1 -Target RunApi`
   - Log some weight data using previous slice
   - Test various weight viewing scenarios
   - Verify table formatting and data accuracy

**Acceptance criteria verification:**

- [ ] All tests (unit, integration, E2E) are passing
- [ ] Weight viewing works with different count parameters
- [ ] Table formatting is clear and readable
- [ ] Empty state is handled gracefully
- [ ] Parameter validation works correctly
- [ ] API handles requests efficiently
- [ ] CLI provides good user experience
- [ ] Documentation is complete and accurate
- [ ] Performance is acceptable

Create a final comprehensive test that exercises the entire weight viewing feature end-to-end, including:

- Logging weight data
- Viewing with different parameters
- Verifying table formatting and data accuracy
- Testing edge cases and error scenarios
