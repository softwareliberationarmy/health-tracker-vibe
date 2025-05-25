## Chunk H: Final Refactoring, Documentation, and CI Publishing

This chunk focuses on project finalization, documentation, and automated publishing.

### Prompt H.1: Write Tests for README Content Requirements

```text
Context:
- Project needs comprehensive documentation.
- README should contain usage examples, installation instructions, and command reference.
- Test-driven approach: write tests that verify README contains required sections.

Task:
- In `HealthTracker.Tests`, create `DocumentationTests.cs`.
- **Test Case 1 (README Exists and Has Required Sections):**
  - Write a test that reads `README.md` from solution root.
  - Assert that README contains required section headers:
    - "# Health Tracker CLI & API"
    - "## Installation"
    - "## Usage"
    - "## Commands"
    - "## Examples"
  - Use string.Contains() or regex to verify sections exist.
- **Test Case 2 (Usage Examples Are Present):**
  - Assert README contains example commands like `health log weight` and `health view run --last 3`.
  - Assert it shows sample output tables.
- These tests will fail as README.md doesn't exist yet.
```

### Prompt H.2: Create Basic README Structure

```text
Context:
- Failing tests expect README.md with specific sections.
- README should provide clear guidance for users and developers.

Task:
- Create `README.md` in solution root:
  - Add project title and brief description.
  - Add "## Installation" section (placeholder for now).
  - Add "## Usage" section with basic CLI syntax.
  - Add "## Commands" section listing available commands.
  - Add "## Examples" section (placeholder for now).
  - Add "## Development" section for contributors.
- Keep content minimal initially to satisfy tests.
- Run tests from Prompt H.1 - they should now pass basic structure checks.
```

### Prompt H.3: Populate Installation Instructions

```text
Context:
- README has basic structure.
- Need detailed installation instructions for CLI tool and API setup.

Task:
- Update "## Installation" section in README.md:
  - **CLI Installation:**
    - Instructions for installing from private NuGet feed: `dotnet tool install -g healthcli --add-source <feed-url>`.
    - Prerequisites: .NET 8.0 or later.
  - **API Setup:**
    - Docker installation: `docker pull health-tracker-api:latest`.
    - Docker run command with port mapping: `docker run -p 50000:8080 health-tracker-api:latest`.
    - Alternative: local development setup instructions.
  - **Configuration:**
    - How to set API base URL for CLI (environment variable or config file).
- Ensure instructions are clear and actionable.
```

### Prompt H.4: Create Comprehensive Usage Examples

```text
Context:
- README needs practical examples showing CLI usage.
- Examples should demonstrate all major features with sample outputs.

Task:
- Update "## Examples" section in README.md:
  - **Weight Logging Examples:**
    - Command with all arguments: `health log weight 180.5 22.1 15.2 40.3 1800 12 05/15/2025`.
    - Interactive prompting: `health log weight` (show sample prompts).
  - **Run Logging Examples:**
    - With arguments: `health log run 5km 25:30`.
    - Interactive: `health log run`.
  - **Viewing Data Examples:**
    - Default last 5: `health view weight`.
    - Custom count: `health view run --last 3`.
    - Sample table outputs using markdown tables or code blocks.
  - **Error Handling Examples:**
    - Show validation error output.
    - Show verbose mode: `health log weight --verbose`.
- Include realistic sample data and formatted output tables.
```

### Prompt H.5: Document API Endpoints

```text
Context:
- README should include API documentation for developers.
- API endpoints should be clearly documented with request/response examples.

Task:
- Add "## API Reference" section to README.md:
  - **Base URL:** `http://localhost:50000` (Docker default).
  - **Endpoints:**
    - `POST /weight`: Request body (WeighIn JSON), response (201 Created).
    - `GET /weight/last/{count}`: Response (array of WeighIn objects).
    - `POST /run`: Request body (Run JSON), response (201 Created).
    - `GET /run/last/{count}`: Response (array of Run objects).
  - **Data Models:**
    - WeighIn JSON schema with field descriptions.
    - Run JSON schema with field descriptions.
  - **Error Responses:**
    - 400 Bad Request with validation error examples.
    - 500 Internal Server Error format.
- Include curl examples for each endpoint.
```

### Prompt H.6: Write Tests for Build Script CI Functionality

```text
Context:
- Build script should support CI/CD operations.
- Need automated build, test, package, and publish workflow.
- Test-driven: write tests for build script functionality.

Task:
- In `DocumentationTests.cs` or new `BuildScriptTests.cs`:
  - **Test Case 3 (Build Script Has Required Targets):**
    - Read `build.ps1` content.
    - Assert it contains functions/targets for: `Build`, `Test`, `Package`, `PublishCli`.
    - Assert it has parameter handling for different targets.
  - **Test Case 4 (CLI Package Configuration):**
    - Read CLI project file (`HealthTracker.Cli.csproj`).
    - Assert it has NuGet package metadata: `PackageId`, `Version`, `Authors`, etc.
    - Assert it's configured as a .NET tool: `<PackAsTool>true</PackAsTool>`.
- These tests will fail as enhanced build script doesn't exist yet.
```

### Prompt H.7: Enhance Build Script with CI/CD Targets

```text
Context:
- Build script needs comprehensive CI/CD functionality.
- Should handle build, test, packaging, and publishing workflows.

Task:
- Update `build.ps1` with additional functions:
  - **Build Target:**
    - `Build-Solution`: Clean and build all projects in release mode.
    - Include NuGet restore and dependency verification.
  - **Test Target:**
    - `Run-UnitTests`: Run all tests except E2E.
    - `Run-AllTests`: Run unit tests and E2E tests.
    - Generate test results and coverage reports.
  - **Package Target:**
    - `Package-Cli`: Pack CLI as NuGet tool package.
    - `Package-Api`: Build Docker image for API.
  - **Publish Target:**
    - `Publish-Cli`: Push CLI package to private NuGet feed.
    - `Publish-Api`: Push Docker image to registry.
- Add parameter validation and error handling.
- Run tests from Prompt H.6 - they should now pass.
```

### Prompt H.8: Configure CLI Project for NuGet Tool Publishing

```text
Context:
- CLI needs to be packaged as a global .NET tool.
- Project file needs proper metadata for NuGet publishing.

Task:
- Update `HealthTracker.Cli.csproj`:
  - Add package metadata:
    - `<PackageId>healthcli</PackageId>`
    - `<Version>0.1.0</Version>`
    - `<Authors>Your Name</Authors>`
    - `<Description>CLI tool for logging and viewing health data</Description>`
    - `<PackageTags>health;cli;tracking</PackageTags>`
  - Configure as tool:
    - `<PackAsTool>true</PackAsTool>`
    - `<ToolCommandName>health</ToolCommandName>`
  - Set output and package settings for clean builds.
- Test packaging locally: `dotnet pack HealthTracker.Cli --configuration Release`.
- Verify generated .nupkg file has correct structure.
```

### Prompt H.9: Create CI Pipeline Configuration

```text
Context:
- Project needs automated CI/CD pipeline.
- Should build, test, and publish on code changes.

Task:
- Create `.github/workflows/ci.yml` (or equivalent for your CI system):
  - **Build Stage:**
    - Checkout code, setup .NET 8.0.
    - Restore dependencies and build solution.
  - **Test Stage:**
    - Run unit tests with coverage reporting.
    - Run E2E tests (if Docker available in CI).
  - **Package Stage:**
    - Pack CLI tool package.
    - Build and tag Docker image.
  - **Publish Stage (on tag/release):**
    - Push CLI package to private NuGet feed.
    - Push Docker image to registry.
- Include proper secret management for feed URLs and credentials.
- Add status badges to README for build/test status.
```

### Prompt H.10: Final Code Review and Documentation Polish

```text
Context:
- All features are implemented.
- Need final review, cleanup, and documentation polish.

Task:
- **Code Review:**
  - Review all code for consistency, readability, and best practices.
  - Ensure error handling is comprehensive.
  - Verify test coverage is adequate.
  - Remove any TODO comments or dead code.
- **Documentation Polish:**
  - Review README for accuracy and completeness.
  - Add troubleshooting section for common issues.
  - Include development setup instructions.
  - Add contribution guidelines if open source.
- **Final Testing:**
  - Run complete test suite (unit + E2E).
  - Test CLI installation and usage end-to-end.
  - Test Docker deployment and API functionality.
  - Verify CI pipeline works correctly.
- **Version and Release:**
  - Tag initial release (v0.1.0).
  - Generate release notes.
  - Publish initial version to feeds.
- Commit final changes: "docs: Complete project documentation and CI setup".
```
