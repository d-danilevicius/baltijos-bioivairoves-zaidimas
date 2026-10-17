# Implementation Plan: Joining a class with a code

**Feature:** `specs/008-join-class/spec.md`
**Branch:** `008-join-class`
**Status:** draft
**Depends on features:** 003-access-control, 006-confirmation-dialog

## Summary

The plan describes how the feature “Joining a class with a code” is implemented according to the approved specification: as a student, I want to join a class with a code so that I see the tasks of my class.

## Technical Context

- **Language and version:** C# with .NET 10 (server); TypeScript (frontend).
- **Libraries:** as selected in `docs/architecture.md` (section 1): ASP.NET Core Identity, Entity Framework Core with Npgsql, AutoMapper for non-trivial mappings.
- **Frontend:** React with Vite and React Router.
- **Storage:** PostgreSQL through Entity Framework Core; schema changes only through migrations.
- **Testing:** xUnit (unit and integration, PostgreSQL in Testcontainers), Vitest and React Testing Library.
- **Target platform:** web application in current Chrome, Firefox, Edge and Safari; screens from 360 px width.
- **Constraints:** HTTPS only; secrets are not stored in the repository.

## Structure and Main Components

- Server: ClassesController (join), ClassMembershipService.
- Frontend: JoinClassPage.

## Server Request Changes

- POST /api/classes/join

## Data Model Changes

- ClassMembership.
- Migration `CreateClassMembershipsTable`.

## Testing Plan

- Test case: code normalization.
- Test case: code not found.
- Test case: class change.

## Technical Notes

- Business rules are placed in the services listed above; controllers only translate HTTP.

## Constitution Check

- **1. Code quality:** complies. Each class has one responsibility and stays below 200 lines; React file names follow the naming rules.
- **2. Architecture:** complies. ClassesController (join) only calls services; the rules are in the services; `ApplicationDbContext` is used directly (no Repository); DTOs are used at the API boundary and AutoMapper for non-trivial mappings; the schema is changed by the migration `CreateClassMembershipsTable`.
- **3. Testing:** complies. The requirements FR-016, FR-017 are covered by the tests in the testing plan.
- **4. Security:** complies. Role and ownership are checked on the server for: POST /api/classes/join; all input is validated on the server; no secret is stored in Git.
- **5. Version control:** complies. The work is done in branch `008-join-class`, created from an updated `main`, and enters through a Pull Request.
- **6. Documentation:** complies. The specification is written before the code; README and `docs/` are updated if the behaviour changes.
- **7. AI assistance:** complies. The code is reviewed and tested by the student; the Pull Request gets a separate AI review.
- **8. Simplicity:** complies. No new libraries or layers are added.

**No exceptions. The plan complies with the constitution.**

