# Health Tracker CLI & API Specification

## Overview

This project is a **.NET-based command-line tool** for logging and viewing personal health data, including daily **weigh-ins** and **runs**. It uses a **SQLite-backed API hosted in Docker**, and is designed for use across multiple machines, but not for concurrent access. The system consists of:

- A **global CLI tool** (`health`) built with `.NET` and `Spectre.Console`
- A **REST API backend** using Minimal APIs and Dapper
- A **shared SQLite database**, created and managed by the API
- A **Docker container** running both API and DB
- Automated **unit and end-to-end tests**
- Local **PowerShell build script** for automation

## Features

### CLI Commands

- `health log weight <weight> <bmi> <fat> <muscle> <restingMetab> <visceralFat> [date]`

  - Logs a new weigh-in
  - Defaults `date` to today if omitted

- `health log run <distance> <time> [date]`

  - Logs a new run (e.g., `3.1mi` or `5km`)
  - Time can be `MM:SS` or `HH:MM:SS`
  - Defaults `date` to today if omitted

- `health view weight [--last 5]`

  - Displays last X weigh-ins in table format (default = 5)
  - optional parameter `--last` to change the number of entries

- `health view run [--last 5]`

  - Displays last X runs in table format (default = 5)
  - optional parameter `--last` to change the number of entries

- `health --version`

  - Shows current tool version

## Architecture

### CLI

- **Language**: C#
- **Libraries**:
  - `System.CommandLine` for parsing
  - `Spectre.Console` for formatting and color output
- **Distribution**: Global .NET Tool from a **private NuGet feed**
- **Packaging**:
  - SemVer versioning
  - Install via `dotnet tool install -g healthcli --add-source <private-feed-url>`

### API

- **Language**: C#
- **Framework**: .NET Minimal APIs
- **Data Access**: Dapper
- **Database**: SQLite (embedded in Docker container)
- **Endpoints**:
  - `POST /weight` — Create a new weigh-in (single record only)
  - `GET /weight/last/{count}` — Return last {count} weigh-ins
  - `POST /run` — Create a new run (single record only)
  - `GET /run/last/{count}` — Return last {count} runs

### Docker

- Single container hosts both API and SQLite DB
- Port mapping: `50000:8080`
- SQLite DB path configured via `ENV` variable in Dockerfile
- Data volume persists across restarts

## Data Model

### `weighins` Table

| Column       | Type    | Constraints                                                               |
| ------------ | ------- | ------------------------------------------------------------------------- |
| id           | INTEGER | Primary Key, Auto Increment                                               |
| date         | TEXT    | ISO 8601 with timezone offset (e.g., 2025-05-31T14:30:45-04:00), Required |
| weight       | REAL    | Required, `CHECK(weight BETWEEN 100 AND 300)`                             |
| bmi          | REAL    | Required                                                                  |
| fat          | REAL    | `CHECK(fat BETWEEN 0 AND 100)`                                            |
| muscle       | REAL    | `CHECK(muscle BETWEEN 0 AND 100)`                                         |
| restingMetab | INTEGER | `CHECK(restingMetab > 1000)`                                              |
| visceralFat  | INTEGER | `CHECK(visceralFat BETWEEN 10 AND 30)`                                    |

### `runs` Table

| Column       | Type    | Constraints                                                               |
| ------------ | ------- | ------------------------------------------------------------------------- |
| id           | INTEGER | Primary Key, Auto Increment                                               |
| date         | TEXT    | ISO 8601 with timezone offset (e.g., 2025-05-31T14:30:45-04:00), Required |
| distance     | REAL    | `CHECK(distance > 0)`                                                     |
| distanceUnit | TEXT    | e.g., `mi`, `km`; Required                                                |
| time         | INTEGER | Required (stored as number of seconds)                                    |

## Input Parsing and Validation

### Date Handling

- CLI accepts dates as `M/D`
- Automatically appends current year
- Rejects dates later than today
- Dates are stored with the user's local timezone offset (e.g., `-04:00`) rather than converted to UTC
- Display remains in local time (no conversion needed)

### Units and Formats

- **Weight**: lbs by default
- **Distance**: Must include unit (`mi`, `km`, etc.); error if missing
- **Time**: Accepts `MM:SS` or `HH:MM:SS`; default is `MM:SS`

### Validation Rules

| Field         | Rule                             |
| ------------- | -------------------------------- |
| Date          | Cannot be in the future          |
| Weight        | 100–300 lbs                      |
| Fat/Muscle    | 0–100%, 1 decimal place max      |
| Resting Metab | >1000                            |
| Visceral Fat  | 10–30                            |
| Distance      | Positive, up to 2 decimal places |
| Time          | Format `MM:SS` or `HH:MM:SS`     |

## Error Handling

### CLI

- **Validation errors**:
  - Displayed in **red**
  - Includes a helpful message
- **Exceptions**:
  - Basic message shown
  - Full stack trace shown only with `--verbose`

### API

- **400 Bad Request**: for validation failures, with message
- **500 Internal Server Error**: for unhandled errors, with simple message

## Testing Strategy

### CLI

- **Unit Tests** (TDD-first):
  - Input parsing
  - Command routing
  - Output formatting (mocking `Spectre.Console`)
  - Error handling
- **Framework**: `xUnit` + `FluentAssertions`

### API

- **Unit Tests** (TDD-first):
  - Input validation
  - Dapper parameter handling
- **Integration Tests**:
  - `TestServer` for testing endpoints in isolation
- **End-to-End Tests**:
  - Run against Docker container using real SQLite DB (no volume)
  - Post data and verify via `GET` requests

## Build & Automation

### PowerShell Build Script (`build.ps1`)

Supports:

- Building all projects
- Running all tests
- Publishing CLI as `.nupkg`
- Installing CLI from local NuGet folder
- Starting/stopping Docker container for API

Example usage:

```powershell
.\build.ps1 -Target RunTests
```

## Future Enhancements (Out of Scope for MVP)

- CSV export or monthly summaries
- Integration with fitness APIs (Strava, Apple Health, etc.)
- Support for custom units or metric/imperial toggling
- Data sync/merge for multi-machine setups beyond single-user
- Interactive CLI prompt rather than specifying all args at once
  - For example, I can just type `health log run` and the system will ask me what the distance run was, what my time was, and what the run date was (or enter for today)
