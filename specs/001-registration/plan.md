# Implementation Plan: Registration

**Feature:** `specs/001-registration/spec.md`
**Branch:** `001-registration`
**Status:** draft
**Depends on features:** none

## Summary

The plan describes how the feature “Registration” is implemented according to the approved specification. A visitor sends an email, a username, a password and a role to `POST /api/auth/register`; the server validates the data, creates the account with ASP.NET Core Identity and returns the result. The frontend page `/register` shows the form and the messages of `docs/ux.md` (section 5.1).

## Technical Context

- **Language and version:** C# with .NET 10 (server); TypeScript (frontend).
- **Libraries:** ASP.NET Core Identity with EF Core stores (password hashing and roles, NFR-001). No new libraries on the server except the EF Core Identity package. The frontend test library `@testing-library/user-event` is added to simulate typing in the form tests.
- **Frontend:** React with Vite and React Router.
- **Storage:** PostgreSQL through Entity Framework Core; schema changes only through migrations.
- **Testing:** xUnit with the EF Core in-memory provider for the service unit tests; xUnit with `WebApplicationFactory` and PostgreSQL in Testcontainers for the API integration tests; Vitest and React Testing Library for the frontend.
- **Target platform:** web application in current Chrome, Firefox, Edge and Safari; screens from 360 px width.
- **Constraints:** HTTPS only; secrets are not stored in the repository.

## Structure and Main Components

- `Controllers/AuthController` – only receives the request, calls the service and translates the result to an HTTP response.
- `Services/RegistrationService` – checks the allowed role, the uniqueness of the email and the username, creates the user and assigns the role. Registered through `IRegistrationService`.
- `Services/ServiceCollectionExtensions` – one place for the Identity configuration (password length, unique email, allowed username characters), used by `Program.cs` and by the tests.
- `DTOs/Requests/RegisterRequest` – field limits as validation attributes (VT-030, VT-003); `DTOs/Responses/RegisteredUserResponse`.
- `Entities/ApplicationUser`, `Entities/RoleNames`; `Data/ApplicationDbContext` (Identity context, seeded roles, unique email index).
- Frontend: `pages/RegisterPage`, `components/textField`, `components/roleChoice`, `api/authApi`, `helpers/validateRegistration`, `helpers/registrationMessages`, `types/auth`. `pages/LoginPage` is a temporary page that only shows the notice until feature 002.

## Server Request Changes

- `POST /api/auth/register` – see `contracts/auth-register.md`. Returns 201, 400 or 409.

## Data Model Changes

- Identity tables and the seeded roles `Student`, `Teacher`, `Administrator`; the unique index `UserNameIndex` on the normalized username is created by Identity; the unique index on the normalized email is added in the migration. See `data-model.md`.
- Migration `CreateIdentityTables`.

## Testing Plan

- **Unit (xUnit):** successful registration of a student and of a teacher; repeated email (different letter case) and repeated username; administrator and unknown role rejected; password stored only as a hash; password not written to logs; limits of the request DTO (email, username, password, role).
- **Integration (xUnit, Testcontainers):** unique indexes of the email and the username are checked here, because the EF Core InMemory provider used in the unit tests does not enforce indexes; 201 and a stored hash; 409 for a repeated email and for a repeated username; 400 for the administrator role and for a short password.
- **Frontend (Vitest):** errors next to empty fields and no server call; password length message; the login page with the notice after success; the server message for a registered email; the server failure message.
- **Authorization tests:** not applicable, the endpoint is public by specification.

## Technical Notes

- The username and email are checked in the service before the insert to return a clear message; the unique indexes handle two identical requests sent at the same time.
- AutoMapper is not used: the only mappings (request → user, user → response) are trivial.

## Constitution Check

- **1. Code quality:** complies. Each class has one responsibility and is far below 200 lines; React files follow the naming rules (pages PascalCase, components and helpers camelCase).
- **2. Architecture:** complies. The controller only calls `IRegistrationService`; the rules are in `RegistrationService`; it uses `UserManager` (Identity) and no Repository layer; DTOs are used at the API boundary; the schema is changed by a migration; the frontend has only convenience checks.
- **3. Testing:** complies. Every FR of this feature (FR-001–FR-004) has unit, integration and frontend tests listed above.
- **4. Security:** complies. Passwords are hashed by Identity, all input is validated on the server, no password is written to a log, no secret is stored in Git.
- **5. Version control:** complies. The work is in branch `001-registration`; changes enter through a Pull Request.
- **6. Documentation:** complies. The specification is written before the code; README commands stay valid and the quickstart describes the check.
- **7. AI assistance:** complies. The code is reviewed and tested by the student; the Pull Request gets a separate AI review.
- **8. Simplicity:** complies. No new layers; AutoMapper is not added for trivial mappings. The one new frontend dev dependency (`@testing-library/user-event`) has a concrete need: typing in form tests.

**No exceptions. The plan complies with the constitution.**
