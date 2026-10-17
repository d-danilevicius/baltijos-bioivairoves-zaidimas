# Implementation Plan: Student answers and evidence for the teacher

**Feature:** `specs/020-student-answers/spec.md`
**Branch:** `020-student-answers`
**Status:** draft
**Depends on features:** 003-access-control

## Summary

The plan describes how the feature “Student answers and evidence for the teacher” is implemented according to the approved specification: as a teacher, I want to see a student’s answers and evidence so that I understand the student’s decisions.

## Technical Context

- **Language and version:** C# with .NET 10 (server); TypeScript (frontend).
- **Libraries:** as selected in `docs/architecture.md` (section 1): ASP.NET Core Identity, Entity Framework Core with Npgsql, AutoMapper for non-trivial mappings.
- **Frontend:** React with Vite and React Router.
- **Storage:** PostgreSQL through Entity Framework Core; schema changes only through migrations.
- **Testing:** xUnit (unit and integration, PostgreSQL in Testcontainers), Vitest and React Testing Library.
- **Target platform:** web application in current Chrome, Firefox, Edge and Safari; screens from 360 px width.
- **Constraints:** HTTPS only; secrets are not stored in the repository.

## Structure and Main Components

- Server: ClassResultsController (student results), StudentResultsService.
- Frontend: StudentResultsPage.

## Server Request Changes

- GET /api/classes/{id}/students/{studentId}/results

## Data Model Changes

- No data changes.
- No new migration.

## Testing Plan

- Test case: another teacher’s student rejected.
- Test case: administrator rejected.

## Technical Notes

- Business rules are placed in the services listed above; controllers only translate HTTP.

## Constitution Check

- **1. Code quality:** complies. Each class has one responsibility and stays below 200 lines; React file names follow the naming rules.
- **2. Architecture:** complies. ClassResultsController (student results) only calls services; the rules are in the services; `ApplicationDbContext` is used directly (no Repository); DTOs are used at the API boundary and AutoMapper for non-trivial mappings; there is no schema change.
- **3. Testing:** complies. The requirements FR-020 are covered by the tests in the testing plan.
- **4. Security:** complies. Role and ownership are checked on the server for: GET /api/classes/{id}/students/{studentId}/results; all input is validated on the server; no secret is stored in Git.
- **5. Version control:** complies. The work is done in branch `020-student-answers`, created from an updated `main`, and enters through a Pull Request.
- **6. Documentation:** complies. The specification is written before the code; README and `docs/` are updated if the behaviour changes.
- **7. AI assistance:** complies. The code is reviewed and tested by the student; the Pull Request gets a separate AI review.
- **8. Simplicity:** complies. No new libraries or layers are added.

**No exceptions. The plan complies with the constitution.**

