# Implementation Plan: Login and logout

**Feature:** `specs/002-login/spec.md`
**Branch:** `002-login`
**Status:** draft
**Depends on features:** none

## Summary

The plan describes how the feature “Login and logout” is implemented according to the approved specification: as a registered user, I want to log in and log out so that nobody else sees my data.

## Technical Context

- **Language and version:** C# with .NET 10 (server); TypeScript (frontend).
- **Libraries:** as selected in `docs/architecture.md` (section 1): ASP.NET Core Identity, Entity Framework Core with Npgsql, AutoMapper for non-trivial mappings.
- **Frontend:** React with Vite and React Router.
- **Storage:** PostgreSQL through Entity Framework Core; schema changes only through migrations.
- **Testing:** xUnit (unit and integration, PostgreSQL in Testcontainers), Vitest and React Testing Library.
- **Target platform:** web application in current Chrome, Firefox, Edge and Safari; screens from 360 px width.
- **Constraints:** HTTPS only; secrets are not stored in the repository.

## Structure and Main Components

- Server: AuthController (login, logout), LoginService.
- Frontend: LoginPage.

## Server Request Changes

- POST /api/auth/login, POST /api/auth/logout

## Data Model Changes

- ApplicationUser.IsBlocked, ActionLog.
- Migration `AddIsBlockedAndActionLogTable`.

## Testing Plan

- Test case: incorrect data.
- Test case: blocked account.
- Test case: the session ends after 30 min.
- Test case: the session cookie has the HttpOnly, Secure and SameSite flags, and a request that changes data without a valid CSRF token is rejected.

## Technical Notes

- Business rules are placed in the services listed above; controllers only translate HTTP.
- The failed-login limit is intentionally not part of the first version (see the clarification above); this is an accepted risk for the first version.

## Constitution Check

- **1. Code quality:** complies. Each class has one responsibility and stays below 200 lines; React file names follow the naming rules.
- **2. Architecture:** complies. AuthController (login only calls services; the rules are in the services; `ApplicationDbContext` is used directly (no Repository); DTOs are used at the API boundary and AutoMapper for non-trivial mappings; the schema is changed by the migration `AddIsBlockedAndActionLogTable`.
- **3. Testing:** complies. The requirements FR-005, FR-006, FR-007, FR-008 are covered by the tests in the testing plan.
- **4. Security:** complies. `POST /api/auth/login` is public and only validates the credentials and the blocked status; `POST /api/auth/logout` requires an authenticated user. The session cookie uses the HttpOnly, Secure and SameSite flags and CSRF protection. All input is validated on the server; no secret is stored in Git.
- **5. Version control:** complies. The work is done in branch `002-login`, created from an updated `main`, and enters through a Pull Request.
- **6. Documentation:** complies. The specification is written before the code; README and `docs/` are updated if the behaviour changes.
- **7. AI assistance:** complies. The code is reviewed and tested by the student; the Pull Request gets a separate AI review.
- **8. Simplicity:** complies. No new libraries or layers are added.

**No exceptions. The plan complies with the constitution.**

