## Chunk C: CLI Command Setup (log/view) + Global Options

### Prompt C.1: Write Failing Tests for `log` and `view` Subcommands

```text
Context:
- The CLI currently only has a `--version` option.
- We need to add `log` and `view` subcommands as per `spec.md`.
- `System.CommandLine` is used for CLI parsing.
- Tests are in `HealthTracker.Tests`.

Task:
- In `HealthTracker.Tests`, create a new test class (e.g., `CliCommandRoutingTests`).
- Write failing xUnit test methods that:
  - Simulate running `health log` and assert it doesn't throw and perhaps prints basic help for `log` or a placeholder message.
  - Simulate running `health view` and assert it doesn't throw and perhaps prints basic help for `view` or a placeholder message.
  - Simulate running `health log weight` (without further args) and assert it reaches a placeholder handler for this specific command.
  - Simulate running `health view weight` (without further args) and assert it reaches a placeholder handler.
- These tests will fail as these subcommands and their handlers are not defined.
```

### Prompt C.2: Configure `log` and `view` Subcommands

```text
Context:
- Failing tests expect `log` and `view` subcommands to be recognized.
- `HealthTracker.Cli`'s `Program.cs` needs to be updated.

Task:
- In `Program.cs` of `HealthTracker.Cli`:
  - Create a `logCommand` of type `Command` (named "log") with a description.
  - Create a `viewCommand` of type `Command` (named "view") with a description.
  - Add `logCommand` and `viewCommand` to the `rootCommand`.
  - For now, set a simple handler for `logCommand` and `viewCommand` that prints e.g., "Log command invoked" or "View command invoked" to allow tests to pass by checking for this output or by ensuring no error occurs.
- Run the tests from Prompt C.1. Some might pass if they only check for subcommand recognition without specific output.
```

### Prompt C.3: Add `weight` and `run` Sub-subcommands

```text
Context:
- `log` and `view` subcommands are registered.
- `spec.md` defines `health log weight`, `health log run`, `health view weight`, `health view run`.

Task:
- In `Program.cs` of `HealthTracker.Cli`:
  - Create `logWeightCommand` ("weight"), `logRunCommand` ("run"). Add them to `logCommand`.
  - Create `viewWeightCommand` ("weight"), `viewRunCommand` ("run"). Add them to `viewCommand`.
  - Set placeholder handlers for these four new sub-subcommands (e.g., print "Log weight invoked").
- Update tests in `CliCommandRoutingTests` (from C.1) to specifically check that invoking `health log weight` and `health view weight` (and their `run` counterparts) reaches these new placeholder handlers (e.g., by checking console output for the placeholder messages).
- Run tests to ensure they pass.
```

### Prompt C.4: Write Failing Tests for Global `--verbose` Option

```text
Context:
- The CLI needs a global `--verbose` option as per `spec.md`.
- This option should be accessible by all commands and subcommands.

Task:
- In `CliCommandRoutingTests` or a new dedicated test class:
  - Write a failing xUnit test that runs `health --verbose log weight` and asserts that a handler context or a globally accessible flag indicates verbosity is enabled.
  - Write another test for `health log weight --verbose` (testing option placement).
  - (How to assert this might involve refactoring handlers to accept a `ParseResult` or `InvocationContext` from which the option value can be retrieved, or setting a static property for test inspection initially).
- These tests will fail as the `--verbose` option is not defined.
```

### Prompt C.5: Implement Global `--verbose` Option and Placeholder Handlers

```text
Context:
- Failing tests exist for a global `--verbose` option.

Task:
- In `Program.cs` of `HealthTracker.Cli`:
  - Define a global option `verboseOption = new Option<bool>("--verbose", "Enable verbose output.")`.
  - Add `verboseOption` to the `rootCommand` using `rootCommand.AddGlobalOption(verboseOption)`.
  - Modify the placeholder handlers for `logWeightCommand`, `logRunCommand`, `viewWeightCommand`, `viewRunCommand` to accept `InvocationContext` or to access the parsed value of `verboseOption`.
  - In the handlers, conditionally print something like "Verbose mode ON" if `verboseOption` is true, allowing tests to assert this.
- Run tests from Prompt C.4 to confirm they pass.
```

### Prompt C.6: Ensure Help and Invalid Command Handling

```text
Context:
- Basic command structure is in place.
- `System.CommandLine` provides default help generation.

Task:
- Write xUnit tests in `CliCommandRoutingTests` to:
  - Verify `health --help` shows help for root command, including `log` and `view`.
  - Verify `health log --help` shows help for `log` command, including `weight` and `run`.
  - Verify `health an-invalid-command` results in an error message and non-zero exit code (standard behavior of `System.CommandLine`).
- Manually run these commands to visually inspect help output if necessary.
- Most of these should pass due to `System.CommandLine` defaults. Add specific handlers or configurations if needed to meet `spec.md` requirements not covered by defaults.
- Refactor CLI command setup for clarity and organization (e.g., separate methods for creating commands if `Program.cs` is getting large).
- Commit changes: "feat: Implement CLI command structure for log/view and global verbose option".
```
