# Vertical Slice 2: Log Weight (End-to-End) Prompts

This document contains the step-by-step prompts for implementing Vertical Slice 2, which adds the ability to log weight data from CLI through API to database with full validation and testing.

## Step 1: Add WeighIn Data Models

**Given** I need to handle weight logging requests and responses  
**When** I define the weight-specific models  
**Then** I should have complete data contracts for weight operations

**Prompt:**
Extend the shared models in `HealthTracker.Shared` for weight logging:

**Scenario:**

- **Given** a user wants to log weight data
- **When** the CLI sends weight data to the API
- **Then** the data should be validated and structured correctly

Implement:

1. Complete the `WeighInRequest` model with properties:

   - `Date` (DateTime)
   - `Weight` (decimal)
   - `Bmi` (decimal)
   - `Fat` (decimal)
   - `Muscle` (decimal)
   - `RestingMetab` (int)
   - `VisceralFat` (int)

2. Add `WeighInResponse` model with properties:

   - `Id` (int)
   - All properties from `WeighInRequest`
   - `CreatedAt` (DateTime)

3. Add validation attributes following the spec constraints:
   - Weight: 100-300 range
   - Fat/Muscle: 0-100 range
   - RestingMetab: > 1000
   - VisceralFat: 10-30 range
   - Date: cannot be future

Write unit tests using TDD to verify:

- Valid data passes validation
- Invalid data fails with appropriate error messages
- Edge cases for each constraint

---

## Step 2: Implement CLI Weight Input Parsing

**Given** The CLI needs to parse weight logging command arguments  
**When** I implement input parsing and validation  
**Then** The CLI should validate input before sending to API

**Prompt:**
Implement weight input parsing in the CLI using TDD:

**Scenario:**

- **Given** a user types `health log weight 180.5 25.2 15.0 45.0 1800 12`
- **When** the CLI parses the arguments
- **Then** it should create a valid `WeighInRequest` object

Create:

1. A `WeightInputParser` class with method `ParseWeightInput(string[] args)`
2. A `WeightValidator` class with method `ValidateWeightInput(WeighInRequest request)`
3. Handle optional date parameter (defaults to today)
4. Convert date from `M/D` format to full DateTime with current year

Write failing tests first for:

- Valid input parsing creates correct `WeighInRequest`
- Missing required arguments throws appropriate exceptions
- Invalid weight values (out of range) throw validation errors
- Invalid date formats throw parsing errors
- Future dates are rejected
- Date defaults to today when omitted

Then implement the minimal code to make tests pass.

---

## Step 3: Add Weight Command to CLI

**Given** The CLI needs a `log weight` command  
**When** I implement the weight logging command  
**Then** Users should be able to log weight data via CLI

**Prompt:**
Add the weight logging command to the CLI using TDD:

**Scenario:**

- **Given** the CLI is configured with weight logging capability
- **When** a user runs `health log weight 180.5 25.2 15.0 45.0 1800 12`
- **Then** the CLI should parse, validate, and send the data to the API

Implement:

1. Add `log` command group to the CLI command structure
2. Add `weight` subcommand under `log` with required parameters
3. Create `LogWeightCommand` class that:

   - Uses `WeightInputParser` to parse arguments
   - Uses `WeightValidator` to validate input
   - Calls API client to submit data
   - Displays success/error messages using Spectre.Console

4. Update `IApiClient` interface with `LogWeightAsync(WeighInRequest request)` method
5. Add mock implementation in `ApiClient` (return success for now)

Write failing tests first for:

- Command routing recognizes `log weight` correctly
- Valid arguments result in API call with correct data
- Invalid arguments display validation errors in red
- API errors are handled gracefully
- Success message is displayed in green

---

## Step 4: Add Database Schema for WeighIns

**Given** The API needs to store weight data in SQLite  
**When** I add the weighins table structure  
**Then** The database should support weight data persistence

**Prompt:**
Add the weighins table to the database infrastructure using TDD:

**Scenario:**

- **Given** the API starts up with an empty database
- **When** the database initialization runs
- **Then** the weighins table should be created with proper constraints

Implement:

1. Add `weighins` table creation SQL to `DatabaseService`:

   ```sql
   CREATE TABLE IF NOT EXISTS weighins (
       id INTEGER PRIMARY KEY AUTOINCREMENT,
       date TEXT NOT NULL,
       weight REAL NOT NULL CHECK(weight BETWEEN 100 AND 300),
       bmi REAL NOT NULL,
       fat REAL CHECK(fat BETWEEN 0 AND 100),
       muscle REAL CHECK(muscle BETWEEN 0 AND 100),
       restingMetab INTEGER CHECK(restingMetab > 1000),
       visceralFat INTEGER CHECK(visceralFat BETWEEN 10 AND 30)
   );
   ```

2. Add `IWeighInRepository` interface with methods:

   - `Task<int> InsertWeighInAsync(WeighInRequest request)`
   - `Task<int> GetWeighInCountAsync()`
   - `Task<DateTime?> GetLastWeighInDateAsync()`

3. Add `WeighInRepository` class implementing the interface using Dapper

Write failing tests first for:

- Table creation SQL is valid
- Insert operations work with valid data
- Database constraints reject invalid data
- Count and last date queries work correctly
- Repository methods handle empty database

---

## Step 5: Implement API Weight Endpoint

**Given** The API needs to accept weight logging requests  
**When** I implement the POST /weight endpoint  
**Then** The API should validate and store weight data

**Prompt:**
Add the weight logging endpoint to the API using TDD:

**Scenario:**

- **Given** the API is running with database connectivity
- **When** a POST request is made to `/weight` with valid weight data
- **Then** the data should be validated, stored, and return success

Implement:

1. Add `POST /weight` endpoint to `Program.cs`
2. Create `WeightController` logic or minimal API handler that:

   - Accepts `WeighInRequest` from request body
   - Validates the request using model validation
   - Calls `IWeighInRepository` to store data
   - Returns appropriate HTTP status codes

3. Update `IHealthService` to use `IWeighInRepository` for actual weight counts
4. Register `IWeighInRepository` in DI container

Write failing integration tests first for:

- POST with valid data returns 201 Created
- POST with invalid data returns 400 Bad Request with validation errors
- Database constraints are enforced (e.g., weight out of range)
- Successful inserts update the count returned by `/about`
- Error handling for database failures

Then implement minimal code to make tests pass.

---

## Step 6: Connect CLI to API for Weight Logging

**Given** The CLI and API both support weight operations  
**When** I connect them together  
**Then** Weight data should flow from CLI to database

**Prompt:**
Complete the integration between CLI and API for weight logging using TDD:

**Scenario:**

- **Given** both CLI and API are running
- **When** a user executes `health log weight 180.5 25.2 15.0 45.0 1800 12`
- **Then** the data should be stored in the database and success confirmed

Implement:

1. Complete `ApiClient.LogWeightAsync()` method to make actual HTTP POST to `/weight`
2. Add proper error handling for HTTP failures
3. Add JSON serialization/deserialization for request/response
4. Update CLI output formatting to show success/failure messages

Write failing tests first for:

- `ApiClient` makes correct HTTP POST with JSON body
- HTTP 201 response is handled as success
- HTTP 400 response shows validation errors to user
- HTTP 500 response shows generic error message
- Network failures are handled gracefully

Integration test:

- End-to-end test that posts weight data and verifies it's stored
- Test using TestServer without Docker dependency

---

## Step 7: Add Date Handling and Timezone Support

**Given** Weight entries need proper date handling with timezone  
**When** I implement date parsing and storage  
**Then** Dates should be stored with timezone offset as per spec

**Prompt:**
Implement comprehensive date handling for weight logging using TDD:

**Scenario:**

- **Given** a user logs weight with date "5/31" or omits date
- **When** the system processes the date
- **Then** it should be stored as ISO 8601 with timezone offset

Implement:

1. `DateTimeHelper` class with methods:

   - `ParseCliDate(string dateInput)` - handles M/D format
   - `ToIso8601WithOffset(DateTime dateTime)` - converts to storage format
   - `ValidateDateNotFuture(DateTime date)` - ensures date isn't future

2. Update `WeightInputParser` to use `DateTimeHelper`
3. Update database storage to use ISO 8601 format with timezone
4. Add timezone configuration support

Write failing tests first for:

- "5/31" parses to May 31 of current year
- Omitted date defaults to today
- Future dates are rejected with clear error
- Dates are stored with correct timezone offset
- Date parsing handles edge cases (leap years, invalid dates)

---

## Step 8: Add Comprehensive Input Validation

**Given** All weight inputs need thorough validation  
**When** I implement comprehensive validation  
**Then** Invalid inputs should be caught with helpful error messages

**Prompt:**
Add comprehensive validation for all weight logging inputs using TDD:

**Scenario:**

- **Given** a user provides invalid weight data
- **When** the system validates the input
- **Then** clear, helpful error messages should be displayed

Implement:

1. `ValidationResult` class to hold validation outcomes
2. Enhanced `WeightValidator` with detailed validation rules:

   - Weight: 100-300 lbs with helpful message
   - BMI: reasonable range validation
   - Fat/Muscle: 0-100% with decimal precision check
   - RestingMetab: > 1000 with context
   - VisceralFat: 10-30 with explanation

3. `ValidationErrorFormatter` for consistent error display
4. CLI integration to show validation errors in red with Spectre.Console

Write failing tests first for:

- Each validation rule with boundary testing
- Error messages are clear and actionable
- Multiple validation errors are collected and displayed
- Valid input passes all validation rules
- Edge cases (decimals, negative numbers, etc.)

---

## Step 9: Add Weight Logging End-to-End Test

**Given** The complete weight logging feature is implemented  
**When** I create end-to-end tests  
**Then** I should verify the complete flow works correctly

**Prompt:**
Create comprehensive end-to-end tests for weight logging using TDD:

**Scenario:**

- **Given** the API is running in Docker with empty database
- **When** I execute weight logging commands via CLI
- **Then** the data should be stored and retrievable

Implement in `HealthTracker.Tests.E2E`:

1. `WeightLoggingE2ETests` class with setup/teardown for Docker container
2. Test scenarios:

   - Log weight with all parameters
   - Log weight with default date
   - Verify data appears in `/about` endpoint
   - Test validation error scenarios
   - Test multiple weight entries

3. Helper methods for:
   - Executing CLI commands and capturing output
   - Making direct API calls to verify data
   - Database cleanup between tests

Write failing tests first for:

- Successful weight logging increases count in `/about`
- Invalid input shows validation errors without storing data
- Multiple entries are stored correctly
- CLI output matches expected format
- Error scenarios are handled gracefully

---

## Step 10: Add Weight Data Integrity and Error Recovery

**Given** The system needs robust error handling  
**When** I implement error recovery and data integrity  
**Then** The system should handle failures gracefully

**Prompt:**
Add robust error handling and data integrity for weight logging using TDD:

**Scenario:**

- **Given** various failure conditions occur
- **When** the system encounters errors
- **Then** it should handle them gracefully without data corruption

Implement:

1. Database transaction handling in `WeighInRepository`
2. Retry logic for transient failures
3. Data integrity validation before/after database operations
4. Graceful degradation when API is unavailable

Write failing tests first for:

- Database transaction rollback on validation failure
- Retry logic for transient database errors
- CLI handles API unavailability with helpful message
- Partial data doesn't get stored on validation failure
- Connection string validation and fallback

---

## Step 11: Performance and Optimization

**Given** The weight logging feature is functionally complete  
**When** I optimize for performance  
**Then** The system should handle operations efficiently

**Prompt:**
Optimize the weight logging feature for performance using TDD:

**Scenario:**

- **Given** the system needs to handle weight logging efficiently
- **When** operations are performed
- **Then** they should complete quickly with minimal resource usage

Implement:

1. Database connection pooling configuration
2. Async operation optimization
3. Input validation optimization
4. Memory usage optimization for CLI

Write performance tests for:

- Weight logging completes within reasonable time
- Memory usage stays within bounds
- Database connections are properly disposed
- Concurrent operations are handled correctly (for future use)

Focus on measurable improvements while maintaining code clarity.

---

## Step 12: Documentation and Integration Verification

**Given** The weight logging feature is complete  
**When** I document and verify the integration  
**Then** The feature should be ready for production use

**Prompt:**
Complete documentation and final integration verification for weight logging:

**Documentation to update:**

1. Update `README.md` with weight logging examples
2. Add inline code documentation for new classes
3. Update API documentation with `/weight` endpoint details
4. Add troubleshooting guide for common validation errors

**Final integration verification:**

1. Test complete workflow:
   - `.\build.ps1 -Target Build`
   - `.\build.ps1 -Target Test`
   - `.\build.ps1 -Target RunApi`
   - Test various weight logging scenarios
   - Verify data persistence and retrieval

**Acceptance criteria verification:**

- [ ] All tests (unit, integration, E2E) are passing
- [ ] Weight logging works with valid inputs
- [ ] Validation errors are clear and helpful
- [ ] Data persists correctly in database
- [ ] CLI provides good user experience
- [ ] API handles errors gracefully
- [ ] Documentation is complete and accurate
- [ ] Performance is acceptable

Create a final comprehensive test that exercises the entire weight logging feature end-to-end.
