# Health Tracker Project Blueprint

This blueprint defines the step-by-step plan for building the Health Tracker CLI & API, using thin, vertical slices. Each slice delivers a complete, testable piece of functionality, from CLI to API to DB, with automated tests. All work is broken down into small, iterative chunks.

---

## Vertical Slice Acceptance Criteria (Definition of Done)

For each vertical slice, the following must be true:

- All code is refactored for maintainability and consistency as the slice is developed
- All relevant tests (unit, integration, E2E) are implemented and passing
- Code is linted and free of formatting issues
- Documentation and usage examples are updated as needed
- The feature is fully functional and meets the requirements of the slice

---

## Vertical Slice 1: Infrastructure & `--about` Command (End-to-End Skeleton)

**Goal:** Prove the architecture works end-to-end by implementing the minimal infrastructure and the `health --about` command, including integration and E2E tests.

### Chunks:

1. Create solution and projects: CLI, API, shared models, and test projects (unit, integration, E2E)
2. Add E2E test: start Docker container, call CLI `--about`, verify output
3. Add integration test for API `/about` endpoint using TestServer
4. Add unit tests for CLI command routing and output formatting (mock Spectre.Console)
5. Implement CLI skeleton with `--about` command (calls API and displays result)
6. Add unit tests for API `/about` endpoint
7. Implement minimal API with `/about` endpoint returning static/dummy data
8. Set up Dockerfile for API + SQLite, with data volume and ENV for DB path
9. Add PowerShell build script(s) to build, test, publish, and manage Docker
10. Document setup and usage in README.md

---

## Vertical Slice 2: Log Weight (End-to-End)

**Goal:** Implement logging a new weigh-in from CLI to DB, with validation and tests.

### Chunks:

1. Add E2E test: CLI logs weight, verify via API call to `/about` endpoint that number of weigh-ins has changed
2. Add integration test for `POST /weight` (TestServer, in-memory DB)
3. Add unit tests for CLI input parsing and validation
4. Implement input parsing and validation in CLI (date, ranges, formats)
5. Add unit tests for API validation and Dapper parameter handling
6. Implement API-side validation (mirror CLI rules)
7. Add `POST /weight` endpoint to API (accepts and validates payload, stores in DB)
8. Add DB migration/init logic for `weighins` table
9. Add CLI command: `health log weight <weight> <bmi> <fat> <muscle> <restingMetab> <visceralFat> [date]`

---

## Vertical Slice 3: View Weigh-Ins (End-to-End)

**Goal:** Implement viewing last X weigh-ins from CLI, with table formatting and tests.

### Chunks:

1. Add E2E test: log weights, view weights, verify output
2. Add integration test for `GET /weight/last/{count}`
3. Add unit tests for CLI output formatting (mock Spectre.Console)
4. Implement CLI output formatting (Spectre.Console table)
5. Add unit tests for API endpoint
6. Add `GET /weight/last/{count}` endpoint to API (returns last X weigh-ins)
7. Add CLI command: `health view weight [--last 5]`

---

## Vertical Slice 4: Log Run (End-to-End)

**Goal:** Implement logging a new run from CLI to DB, with validation and tests.

### Chunks:

1. Add E2E test: CLI logs run, verify via direct DB query or API
2. Add integration test for `POST /run`
3. Add unit tests for CLI input parsing and validation
4. Implement input parsing and validation in CLI (distance, time, date)
5. Add unit tests for API validation and Dapper parameter handling
6. Implement API-side validation (mirror CLI rules)
7. Add `POST /run` endpoint to API (accepts and validates payload, stores in DB)
8. Add DB migration/init logic for `runs` table
9. Add CLI command: `health log run <distance> <time> [date]`

---

## Vertical Slice 5: View Runs (End-to-End)

**Goal:** Implement viewing last X runs from CLI, with table formatting and tests.

### Chunks:

1. Add E2E test: log runs, view runs, verify output
2. Add integration test for `GET /run/last/{count}`
3. Add unit tests for CLI output formatting (mock Spectre.Console)
4. Implement CLI output formatting (Spectre.Console table)
5. Add unit tests for API endpoint
6. Add `GET /run/last/{count}` endpoint to API (returns last X runs)
7. Add CLI command: `health view run [--last 5]`

---

> **Note:** This blueprint ensures each feature is delivered end-to-end, with tests and validation at every layer, following TDD and thin vertical slice principles. Remember to update this document as needed to reflect the current state and requirements of the project.
