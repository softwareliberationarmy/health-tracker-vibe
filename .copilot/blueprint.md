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
2. Add failing E2E test: start Docker container, call CLI `--about`, verify output
3. Add failing integration test for API `/about` endpoint using TestServer
4. (TDD) Implement CLI command routing and output formatting with `--about` command (calls mock API and displays result)
5. (TDD) Implement minimal API with `/about` endpoint returning actual version and DB data
6. Set up Dockerfile for API + SQLite, with data volume and ENV for DB path
7. Add PowerShell build script(s) to build, test, publish, and manage Docker
8. Document setup and usage in README.md, complete all Vertical Slice Acceptance Criteria

---

## Vertical Slice 2: Log Weight (End-to-End)

**Goal:** Implement logging a new weigh-in from CLI to DB, with validation and tests.

### Chunks:

1. Add failing E2E test: CLI logs weight, verify via API call to `/about` endpoint that number of weigh-ins has changed
2. Add failing integration test for `POST /weight` (TestServer, in-memory DB)
3. (TDD) Implement input parsing and validation in CLI (date, ranges, formats)
4. (TDD) Implement API-side validation and Dapper parameter handling (mirror CLI rules)
5. (TDD) Add `POST /weight` endpoint to API (accepts and validates payload, stores in DB)
6. Add DB migration/init logic for `weighins` table
7. (TDD) Implement CLI command: `health log weight <weight> <bmi> <fat> <muscle> <restingMetab> <visceralFat> [date]`

---

## Vertical Slice 3: View Weigh-Ins (End-to-End)

**Goal:** Implement viewing last X weigh-ins from CLI, with table formatting and tests.

### Chunks:

1. Add failing E2E test: log weights, view weights, verify output
2. Add failing integration test for `GET /weight/last/{count}`
3. (TDD) Implement CLI output formatting (Spectre.Console table)
4. (TDD) Implement `GET /weight/last/{count}` endpoint in API (returns last X weigh-ins)
5. (TDD) Implement CLI command: `health view weight [--last 5]`

---

## Vertical Slice 4: Log Run (End-to-End)

**Goal:** Implement logging a new run from CLI to DB, with validation and tests.

### Chunks:

1. Add failing E2E test: CLI logs run, verify via API call to `/about` endpoint that number of runs has changed
2. Add failing integration test for `POST /run`
3. (TDD) Add CLI command: `health log run <distance> <time> [date]`. Implement input parsing and validation in CLI (distance, time, date)
4. (TDD) Add `POST /run` endpoint to API (mock validator, mock db context)
5. (TDD) Implement API-side validation and data persistence logic (mirror CLI rules)
6. Add DB migration/init logic for `runs` table

---

## Vertical Slice 5: View Runs (End-to-End)

**Goal:** Implement viewing last X runs from CLI, with table formatting and tests.

### Chunks:

1. Add failing E2E test: log runs, view runs, verify output
2. Add failing integration test for `GET /run/last/{count}`
3. (TDD) Implement CLI output formatting using mock API (Spectre.Console table)
4. (TDD) Add `GET /run/last/{count}` endpoint to API (returns last X runs)
5. (TDD) Add CLI command: `health view run [--last 5]`

---

> **Note:** This blueprint ensures each feature is delivered end-to-end, with tests and validation at every layer, following TDD and thin vertical slice principles. Remember to update this document as needed to reflect the current state and requirements of the project.
