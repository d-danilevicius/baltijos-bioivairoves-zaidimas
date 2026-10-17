# Implementation Plan: Class creation and deletion

**Feature:** `specs/007-class-create-and-delete/spec.md`
**Branch:** `007-class-create-and-delete`
**Status:** draft
**Depends on features:** 003-access-control, 006-confirmation-dialog

## Summary

The plan describes how the feature “Class creation and deletion” is implemented according to the approved specification: as a teacher, I want to create a class and get its code so that I can bring my students together, and to delete it when it is no longer needed.

## Technical Context

- **Language and version:** C# with .NET 10 (server); TypeScript (frontend).
- **Libraries:** as selected in `docs/architecture.md` (section 1): ASP.NET Core Identity, Entity Framework Core with Npgsql, AutoMapper for non-trivial mappings.
- **Frontend:** React with Vite and React Router.
- **Storage:** PostgreSQL through Entity Framework Core; schema changes only through migrations.
- **Testing:** xUnit (unit and integration, PostgreSQL in Testcontainers), Vitest and React Testing Library.
- **Target platform:** web application in current Chrome, Firefox, Edge and Safari; screens from 360 px width.
- **Constraints:** HTTPS only; secrets are not stored in the repository.

## Structure and Main Components

- Server: ClassesController, ClassService (code generation, deletion).
- Frontend: ClassListPage, NewClassPage, ClassInfoPage.

## Server Request Changes

- GET /api/classes, POST /api/classes, DELETE /api/classes/{id}

## Data Model Changes

- Class (Name, Code, OwnerId).
- Migration `CreateClassesTable`.

## Testing Plan

- Test case: code uniqueness.
- Test case: a student has no permission.
- Test case: a non-owner is rejected.

## Technical Notes

- Business rules are placed in the services listed above; controllers only translate HTTP.
- How the related data of a deleted class are removed (explicit deletion in the service inside one transaction, or cascade constraints) is decided in this feature’s `data-model.md` before implementation; the students’ points remain (VT-009, VT-027).

## Constitution Check

- **1. Code quality:** complies. Each class has one responsibility and stays below 200 lines; React file names follow the naming rules.
- **2. Architecture:** complies. ClassesController only calls services; the rules are in the services; `ApplicationDbContext` is used directly (no Repository); DTOs are used at the API boundary and AutoMapper for non-trivial mappings; the schema is changed by the migration `CreateClassesTable`.
- **3. Testing:** complies. The requirements FR-013, FR-014, FR-015, FR-023 are covered by the tests in the testing plan.
- **4. Security:** complies. Authentication, role and ownership are checked on the server for the protected requests: GET /api/classes, POST /api/classes, DELETE /api/classes/{id}. All input is validated on the server; no secret is stored in Git.
- **5. Version control:** complies. The work is done in branch `007-class-create-and-delete`, created from an updated `main`, and enters through a Pull Request.
- **6. Documentation:** complies. The specification is written before the code; README and `docs/` are updated if the behaviour changes.
- **7. AI assistance:** complies. The code is reviewed and tested by the student; the Pull Request gets a separate AI review.
- **8. Simplicity:** complies. No new libraries or layers are added.

**No exceptions. The plan complies with the constitution.**

