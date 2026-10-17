# Implementation Plan: Account deletion

**Feature:** `specs/022-account-deletion/spec.md`
**Branch:** `022-account-deletion`
**Status:** draft
**Depends on features:** 003-access-control, 006-confirmation-dialog

## Summary

The plan describes how the feature “Account deletion” is implemented according to the approved specification: as an administrator, I want to delete an account so that I remove data that are no longer needed.

## Technical Context

- **Language and version:** C# with .NET 10 (server); TypeScript (frontend).
- **Libraries:** as selected in `docs/architecture.md` (section 1): ASP.NET Core Identity, Entity Framework Core with Npgsql, AutoMapper for non-trivial mappings.
- **Frontend:** React with Vite and React Router.
- **Storage:** PostgreSQL through Entity Framework Core; schema changes only through migrations.
- **Testing:** xUnit (unit and integration, PostgreSQL in Testcontainers), Vitest and React Testing Library.
- **Target platform:** web application in current Chrome, Firefox, Edge and Safari; screens from 360 px width.
- **Constraints:** HTTPS only; secrets are not stored in the repository.

## Structure and Main Components

- Server: AdminAccountsController (delete), AccountDeletionService.
- Frontend: AccountsPage (delete action).

## Server Request Changes

- DELETE /api/admin/accounts/{id}

## Data Model Changes

- All related data.
- No new migration.

## Testing Plan

- Test case: deletion according to VT-008 and VT-009.
- Test case: the last administrator.

## Technical Notes

- Business rules are placed in the services listed above; controllers only translate HTTP.

## Constitution Check

- **1. Code quality:** complies. Each class has one responsibility and stays below 200 lines; React file names follow the naming rules.
- **2. Architecture:** complies. AdminAccountsController (delete) only calls services; the rules are in the services; `ApplicationDbContext` is used directly (no Repository); DTOs are used at the API boundary and AutoMapper for non-trivial mappings; there is no schema change.
- **3. Testing:** complies. The requirements FR-057 are covered by the tests in the testing plan.
- **4. Security:** complies. Role and ownership are checked on the server for: DELETE /api/admin/accounts/{id}; all input is validated on the server; no secret is stored in Git.
- **5. Version control:** complies. The work is done in branch `022-account-deletion`, created from an updated `main`, and enters through a Pull Request.
- **6. Documentation:** complies. The specification is written before the code; README and `docs/` are updated if the behaviour changes.
- **7. AI assistance:** complies. The code is reviewed and tested by the student; the Pull Request gets a separate AI review.
- **8. Simplicity:** complies. No new libraries or layers are added.

**No exceptions. The plan complies with the constitution.**

