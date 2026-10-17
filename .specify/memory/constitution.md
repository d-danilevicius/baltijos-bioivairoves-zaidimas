# Project Constitution

This document sets the stable engineering rules for the whole project. It contains no descriptions of individual features; they are in the `specs/` folder. It applies together with the general project rules.

## 1. Code quality
1.1. The code MUST follow SOLID, DRY, KISS and YAGNI.
1.2. A production C# class MUST NOT exceed 200 lines. React pages and components SHOULD NOT exceed about 200 lines, and when they have several responsibilities they MUST be split.
1.3. Names MUST describe the purpose. Generic names (Manager, Helper, Utils) MUST NOT be used without a narrowly defined meaning.
1.4. React file names: pages PascalCase, components and helper files camelCase, hooks start with `use`.
1.5. The code MUST be formatted (`.editorconfig`, `dotnet format`, ESLint, Prettier). Warnings MUST be investigated, not hidden.

## 2. Architecture
2.1. The server MUST be in one project `Project.Api`. A request goes Controller → Service → ApplicationDbContext → database.
2.2. Controllers MUST be thin: HTTP, login and role check, DTOs, calling a service. Business rules and database queries MUST be only in services.
2.3. Repository, GenericRepository and UnitOfWork MUST NOT be created. Services use `ApplicationDbContext` directly.
2.4. Dependencies MUST be injected. Input and output operations MUST be asynchronous.
2.5. Entities MUST NOT be returned from the API. Request and response DTOs are used, and non-trivial mappings are done with AutoMapper.
2.6. Schema changes MUST be made by EF Core migrations, created in the same branch, with a name that describes the change.
2.7. The frontend MUST NOT contain business rules. Its checks are only for convenience.

## 3. Testing
3.1. The server business logic MUST have unit tests, and important API behaviour MUST have integration tests.
3.2. Protected actions MUST have authorization tests that prove that a user without permission or a non-owner cannot perform the action.
3.3. The frontend MUST have automated tests (forms, states, important interactions).
3.4. Test names MUST describe the behaviour and the scenario (`Method_Condition_Result`). Tests MUST be repeatable and MUST NOT depend on production data or execution order.
3.5. Every FR MUST have at least one test (NFR-025).
3.6. A failing CI MUST NOT be ignored: disabling a test or changing CI so that a command is not failing MUST NOT be done.

## 4. Security
4.1. Authentication MUST use ASP.NET Core Identity. Passwords MUST be stored only as values of a slow salted hash function.
4.2. The role and ownership MUST be checked on the server before every protected action.
4.3. All input MUST be validated on the server.
4.4. Secrets MUST NOT be stored in Git. They are kept in environment variables or user secrets.
4.5. HTTPS MUST be used in production, and CORS MUST be restricted to the required addresses.
4.6. Errors MUST be handled in one place, and the content of stack or database errors MUST NOT be shown to the user.
4.7. Logs MUST use `ILogger<T>` and MUST NOT contain passwords, keys or other secrets.
4.8. OWASP risks MUST be assessed in the security review.

## 5. Version control
5.1. Every Spec Kit feature MUST be in a separate branch created from an updated `main`.
5.2. `main` MUST NOT be written to directly. Changes enter through a Pull Request that states the specification, the tests and the migrations.
5.3. Commit messages MUST follow Conventional Commits: `type(scope): description`.
5.4. Before every commit and Pull Request the diff MUST be reviewed. Debug code, temporary files and secrets MUST NOT be committed.
5.5. A Pull Request MUST NOT be merged while CI fails. A merged branch MUST be deleted.

## 6. Documentation
6.1. The code of a feature MUST be written only after the specification of that feature in `specs/` is approved. New behaviour found during development MUST be added to the specification.
6.2. README and `docs/` MUST be updated in the same change as the code when the startup, configuration or behaviour changes.
6.3. Configuration that depends on the environment MUST NOT be written in the code.

## 7. AI assistance
7.1. The student MUST understand, review and test all AI-generated code.
7.2. Every Pull Request MUST get a separate AI review. Its evidence (tool, findings, changes, findings not applied) MUST be recorded in the Pull Request.
7.3. The AI MUST work only according to the active specification and MUST NOT change this constitution without an explicit instruction.
7.4. The practical AI instructions are in `AGENTS.md`, not here.

## 8. Simplicity and dependencies
8.1. The simplest solution that satisfies the specification is chosen.
8.2. A library or a layer MUST be added only when there is a concrete need recorded in the plan. Unused dependencies MUST be removed.
8.3. An abstraction MUST be created only when it gives a real boundary or testability.

## 9. Governance
- **Change:** through a Pull Request with a reason.
- **Versioning:** MAJOR.MINOR.PATCH. MAJOR – a principle removed or changed to the opposite; MINOR – a principle added; PATCH – wording only.
- **Compliance check:** the Constitution Check of every `plan.md` (concrete for each principle), the analysis step, the AI review of the Pull Request and CI.
- **Exceptions:** only if written in the plan (rule, reason, simpler alternative) and approved before implementation. An exception MUST NOT apply to section 4 and cannot violate the general project rules without the instructor’s consent.

**Version:** 2.0.0 | **Last change:** 2026-10-17
