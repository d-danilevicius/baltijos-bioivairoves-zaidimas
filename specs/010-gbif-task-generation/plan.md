# Implementation Plan: Task creation from GBIF observations

**Feature:** `specs/010-gbif-task-generation/spec.md`
**Branch:** `010-gbif-task-generation`
**Status:** draft
**Depends on features:** 003-access-control

## Summary

The plan describes how the feature “Task creation from GBIF observations” is implemented according to the approved specification: as a teacher, I want to choose a location and a season so that the class gets tasks from real GBIF observations near the school.

## Technical Context

- **Language and version:** C# with .NET 10 (server); TypeScript (frontend).
- **Libraries:** as selected in `docs/architecture.md` (section 1): ASP.NET Core Identity, Entity Framework Core with Npgsql, AutoMapper for non-trivial mappings.
- **Frontend:** React with Vite and React Router.
- **Storage:** PostgreSQL through Entity Framework Core; schema changes only through migrations.
- **Testing:** xUnit (unit and integration, PostgreSQL in Testcontainers), Vitest and React Testing Library.
- **Target platform:** web application in current Chrome, Firefox, Edge and Safari; screens from 360 px width.
- **Constraints:** HTTPS only; secrets are not stored in the repository.

## Structure and Main Components

- Server: ClassTasksController (gbif-tasks), GbifTaskService, GbifClient (Integrations/Gbif).
- Frontend: CreateGbifTasksPage.

## Server Request Changes

- POST /api/classes/{id}/gbif-tasks

## Data Model Changes

- Observation, ClassTask, Class (location, season).
- Migration `CreateObservationsAndTasksTables`.

## Testing Plan

- Test case: fewer than 10 observations.
- Test case: GBIF error.
- Test case: protected species.
- Test case: not created a second time.

## Technical Notes

- Business rules are placed in the services listed above; controllers only translate HTTP.

## Constitution Check

- **1. Code quality:** complies. Each class has one responsibility and stays below 200 lines; React file names follow the naming rules.
- **2. Architecture:** complies. ClassTasksController (gbif-tasks) only calls services; the rules are in the services; `ApplicationDbContext` is used directly (no Repository); DTOs are used at the API boundary and AutoMapper for non-trivial mappings; the schema is changed by the migration `CreateObservationsAndTasksTables`.
- **3. Testing:** complies. The requirements FR-025, FR-026, FR-027, FR-028, FR-029, FR-030, FR-036, FR-047 are covered by the tests in the testing plan.
- **4. Security:** complies. Role and ownership are checked on the server for: POST /api/classes/{id}/gbif-tasks; all input is validated on the server; no secret is stored in Git.
- **5. Version control:** complies. The work is done in branch `010-gbif-task-generation`, created from an updated `main`, and enters through a Pull Request.
- **6. Documentation:** complies. The specification is written before the code; README and `docs/` are updated if the behaviour changes.
- **7. AI assistance:** complies. The code is reviewed and tested by the student; the Pull Request gets a separate AI review.
- **8. Simplicity:** complies. No new library: the GBIF client uses `HttpClient` from `IHttpClientFactory` (docs/architecture.md, section 1).

**No exceptions. The plan complies with the constitution.**

