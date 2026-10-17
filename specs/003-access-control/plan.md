# Implementation Plan: Access control and error pages

**Feature:** `specs/003-access-control/spec.md`
**Branch:** `003-access-control`
**Status:** draft
**Depends on features:** none

## Summary

The plan describes how the feature “Access control and error pages” is implemented according to the approved specification: as a user, I want nobody else to reach my data and me not to reach other people’s data.

## Technical Context

- **Language and version:** C# with .NET 10 (server); TypeScript (frontend).
- **Libraries:** as selected in `docs/architecture.md` (section 1): ASP.NET Core Identity, Entity Framework Core with Npgsql, AutoMapper for non-trivial mappings.
- **Frontend:** React with Vite and React Router.
- **Storage:** PostgreSQL through Entity Framework Core; schema changes only through migrations.
- **Testing:** xUnit (unit and integration, PostgreSQL in Testcontainers), Vitest and React Testing Library.
- **Target platform:** web application in current Chrome, Firefox, Edge and Safari; screens from 360 px width.
- **Constraints:** HTTPS only; secrets are not stored in the repository.

## Structure and Main Components

- Server: authorization policies and ownership checks used by the services, centralized error responses.
- Frontend: AccessDeniedPage, NotFoundPage, protectedRoute component.

## Server Request Changes

- all controllers: role policies and ownership check

## Data Model Changes

- No data changes.
- No new migration.

## Testing Plan

- Test case: authorization table (student, teacher, administrator × protected actions); frontend route protection.

## Technical Notes

- Business rules are placed in the services listed above; controllers only translate HTTP.

## Constitution Check

- **1. Code quality:** complies. Each class has one responsibility and stays below 200 lines; React file names follow the naming rules.
- **2. Architecture:** complies. authorization policies and ownership checks used by the services only calls services; the rules are in the services; `ApplicationDbContext` is used directly (no Repository); DTOs are used at the API boundary and AutoMapper for non-trivial mappings; there is no schema change.
- **3. Testing:** complies. The requirements FR-009, FR-024, FR-052, FR-058, FR-059 are covered by the tests in the testing plan.
- **4. Security:** complies. Role and ownership are checked on the server for: all controllers: role policies and ownership check; all input is validated on the server; no secret is stored in Git.
- **5. Version control:** complies. The work is done in branch `003-access-control`, created from an updated `main`, and enters through a Pull Request.
- **6. Documentation:** complies. The specification is written before the code; README and `docs/` are updated if the behaviour changes.
- **7. AI assistance:** complies. The code is reviewed and tested by the student; the Pull Request gets a separate AI review.
- **8. Simplicity:** complies. No new libraries or layers are added.

**No exceptions. The plan complies with the constitution.**

