## Chunk A: Solution & Project Scaffolding + Version Command

### Prompt A.1: Create Solution and Projects

```text
Context:
- We are starting a new .NET project called HealthTracker.
- It will consist of a CLI, an API, a shared Core library, and a Tests project.

Task:
- Create a new .NET solution file named `HealthTracker.sln`.
- Create the following .NET class library projects within the solution structure:
  - `src/HealthTracker.Cli` (Console Application)
  - `src/HealthTracker.Api` (ASP.NET Core Web API)
  - `src/HealthTracker.Core` (Class Library)
  - `tests/HealthTracker.Tests` (xUnit Test Project)
- Add the projects to the solution.
```

### Prompt A.2: Establish Project References

```text
Context:
- The solution and four projects (`Cli`, `Api`, `Core`, `Tests`) have been created.
- `Cli` and `Api` will depend on `Core` for shared models and logic.
- `Tests` will need to reference all other projects to test them.

Task:
- Add a project reference from `HealthTracker.Cli` to `HealthTracker.Core`.
- Add a project reference from `HealthTracker.Api` to `HealthTracker.Core`.
- Add project references from `HealthTracker.Tests` to `HealthTracker.Cli`, `HealthTracker.Api`, and `HealthTracker.Core`.
- Verify the references are correctly added to the respective `.csproj` files.
```

### Prompt A.3: Write Failing Test for CLI Version Command

```text
Context:
- Project structure and references are set up.
- We are practicing TDD. The first CLI feature is `health --version`.
- The CLI project is `HealthTracker.Cli` and tests are in `HealthTracker.Tests`.
- We will use `System.CommandLine` for parsing and `FluentAssertions` for assertions.

Task:
- In `HealthTracker.Tests`, create a new test class (e.g., `VersionCommandTests`).
- Write a failing xUnit test method that:
  - Simulates running the CLI with the `--version` argument.
  - Asserts that the standard output of the command is exactly `0.1.0` followed by a newline.
  - (Initially, this will fail as the `Program.cs` in `Cli` is empty or default).
- Ensure the test fails with an assertion error indicating the output was not as expected.
```

### Prompt A.4: Implement Minimal Version Command

```text
Context:
- A failing xUnit test exists for the `health --version` command, expecting `0.1.0`.
- The `HealthTracker.Cli` project's `Program.cs` needs to be updated.
- `System.CommandLine` should be used to handle command-line arguments.

Task:
- Add the `System.CommandLine` NuGet package to the `HealthTracker.Cli` project.
- Modify `Program.cs` in `HealthTracker.Cli` to:
  - Create a root command.
  - Add a `--version` option to the root command.
  - Set a handler for the root command that, if the `--version` option is used, prints `0.1.0` to the console.
  - Invoke the command parser with `args`.
- Run the test from Prompt A.3 to confirm it now passes.
```

### Prompt A.5: Refactor and Confirm

```text
Context:
- The `health --version` command is implemented and its test is passing.

Task:
- Review the code in `Program.cs` of `HealthTracker.Cli` and the test in `VersionCommandTests` for clarity, correctness, and adherence to coding standards (e.g., C# conventions, TDD principles).
- Make any necessary refactorings (e.g., improve variable names, simplify logic if possible, ensure using statements are correct).
- Re-run all tests to ensure they still pass after refactoring.
- Commit the changes with a message like "feat: Implement initial --version command for CLI".
```
