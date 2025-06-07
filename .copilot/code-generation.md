# Coding Standards

## Communication

- Any time you interact with me, you MUST address me as "Puddle Duck"

## We practice TDD. That means:

- Write one failing unit test for the functionality you want to implement
- Run the test and make sure it fails with the correct error message
- Only write enough code to make that failing test pass
- Run all other associated tests to make sure they still pass, too
- Review the test code. If there are maintainability issues like duplication or complex methods, refactor the test code. Re-run the tests after any refactorings to make sure the tests still pass.
- Review the code you wrote to make the tests pass. If there are maintainability issues like duplication or complex methods, refactor that code. Re-run the tests after any refactorings to make sure the tests still pass.

## Writing code

- We prefer simple, clean, maintainable solutions over clever or complex ones, even if the latter are more concise or performant. Readability and maintainability are primary concerns.
- Make the smallest reasonable changes to get to the desired outcome. You MUST ask permission before re-implementing features or systems from scratch instead of updating the existing implementation.
- When modifying code, match the style and formatting of surrounding code, even if it differs from standard style guides. Consistency within a file is more important than strict adherence to external standards.
- NEVER make code changes that aren't directly related to the task you're currently assigned. If you notice something that should be fixed but is unrelated to your current task, ask me if you should fix it.
- NEVER remove code comments unless you can prove that they are actively false. Comments are important documentation and should be preserved even if they seem redundant or unnecessary to you.
- When you are trying to fix a bug or compilation error or any other issue, YOU MUST NEVER throw away the old implementation and rewrite without explicit permission from the user. If you are going to do this, YOU MUST STOP and get explicit permission from the user.
- When you are troubleshooting something, always assume that the code was working before you touched it, and that the issue was introduced by your changes, not by code which wasn't touched recently.
- NEVER name things as 'improved' or 'new' or 'enhanced', etc. Code naming should be evergreen. What is new today will be "old" someday.

### C# coding standards

- Prefer curly braces for all conditional code blocks, even if they only have one statement
- Methods should not be allowed to grow beyond 50 lines. If a method has > 50 lines, try to find a way to break it up into smaller methods with high cohesion. Don't spend more than 60 seconds trying to refactor it. If you can't fix it in 60 seconds, report the issue to me and continue with what you were doing.
- Classes and .cs files should not be allowed to grow beyond 500 lines. If a .cs file has > 500 lines, try to find a way to refactor it into smaller classes with high cohesion and loose coupling. Don't spend more than 60 seconds trying to refactor it. If you can't fix it in 60 seconds, report the issue to me and continue with what you were doing.

## Linting and Cleanup

- Always attempt to resolve any warnings, formatting issues, and linter issues for the code you generate, but do not spend more than 60 seconds resolving issues. If you cannot resolve the issues within 60 seconds, pause and report the issues to me.

## Getting help

- ALWAYS ask for clarification rather than making assumptions.
- If you're having trouble with something, it's ok to stop and ask for help. Especially if it's something your human might be better at.
- If you disagree with a request I've given you, it's _good_ to push back, but we should cite evidence.
