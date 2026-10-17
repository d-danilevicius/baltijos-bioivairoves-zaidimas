# Implementation Plan: Confirmation dialog

**Feature:** `specs/006-confirmation-dialog/spec.md`
**Branch:** `006-confirmation-dialog`
**Status:** draft
**Depends on features:** none

## Summary

The plan describes how the feature “Confirmation dialog” is implemented according to the approved specification: as a user, I want to be asked for confirmation before an irreversible action so that I do not delete anything by accident.

## Technical Context

- **Language and version:** C# with .NET 10 (server); TypeScript (frontend).
- **Libraries:** as selected in `docs/architecture.md` (section 1): ASP.NET Core Identity, Entity Framework Core with Npgsql, AutoMapper for non-trivial mappings.
- **Frontend:** React with Vite and React Router.
- **Storage:** PostgreSQL through Entity Framework Core; schema changes only through migrations.
- **Testing:** xUnit (unit and integration, PostgreSQL in Testcontainers), Vitest and React Testing Library.
- **Target platform:** web application in current Chrome, Firefox, Edge and Safari; screens from 360 px width.
- **Constraints:** HTTPS only; secrets are not stored in the repository.

## Structure and Main Components

- Server: none (frontend component).
- Frontend: confirmDialog component.

## Server Request Changes

- none (frontend component)

## Data Model Changes

- No data changes.
- No new migration.

## Testing Plan

- Test case: confirm.
- Test case: cancel.
- Test case: keyboard control.

## Technical Notes

- Business rules are placed in the services listed above; controllers only translate HTTP.

## Constitution Check

- **1. Code quality:** complies. Each class has one responsibility and stays below 200 lines; React file names follow the naming rules.
- **2. Architecture:** complies. none (frontend component) only calls services; the rules are in the services; `ApplicationDbContext` is used directly (no Repository); DTOs are used at the API boundary and AutoMapper for non-trivial mappings; there is no schema change.
- **3. Testing:** complies. The requirements FR-053 are covered by the tests in the testing plan.
- **4. Security:** complies. Role and ownership are checked on the server for: none (frontend component); all input is validated on the server; no secret is stored in Git.
- **5. Version control:** complies. The work is done in branch `006-confirmation-dialog`, created from an updated `main`, and enters through a Pull Request.
- **6. Documentation:** complies. The specification is written before the code; README and `docs/` are updated if the behaviour changes.
- **7. AI assistance:** complies. The code is reviewed and tested by the student; the Pull Request gets a separate AI review.
- **8. Simplicity:** complies. No new libraries or layers are added.

**No exceptions. The plan complies with the constitution.**

