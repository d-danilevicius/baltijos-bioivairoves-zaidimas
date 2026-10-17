# Implementation Plan: Teacher tasks

**Feature:** `specs/011-teacher-tasks/spec.md`
**Branch:** `011-teacher-tasks`
**Status:** draft
**Depends on features:** 003-access-control, 006-confirmation-dialog

## Summary

The plan describes how the feature “Teacher tasks” is implemented according to the approved specification: as a teacher, I want to create my own task, for example about the school surroundings, so that I can add my own questions to the tasks.

## Technical Context

- **Language and version:** C# with .NET 10 (server); TypeScript (frontend).
- **Libraries:** as selected in `docs/architecture.md` (section 1): ASP.NET Core Identity, Entity Framework Core with Npgsql, AutoMapper for non-trivial mappings.
- **Frontend:** React with Vite and React Router.
- **Storage:** PostgreSQL through Entity Framework Core; schema changes only through migrations.
- **Testing:** xUnit (unit and integration, PostgreSQL in Testcontainers), Vitest and React Testing Library.
- **Target platform:** web application in current Chrome, Firefox, Edge and Safari; screens from 360 px width.
- **Constraints:** HTTPS only; secrets are not stored in the repository.

## Structure and Main Components

- Server: ClassTasksController, TeacherTaskService.
- Frontend: ClassTasksPage, TeacherTaskPage.

## Server Request Changes

- GET, POST /api/classes/{id}/tasks, DELETE /api/classes/{id}/tasks/{taskId}

## Data Model Changes

- ClassTask (Source = Teacher).
- No new migration.

## Testing Plan

- Test case: field limits.
- Test case: confirmation.
- Test case: another class’s task.
- Test case: points remain after deletion.

## Technical Notes

- Business rules are placed in the services listed above; controllers only translate HTTP.

## Constitution Check

- **1. Code quality:** complies. Each class has one responsibility and stays below 200 lines; React file names follow the naming rules.
- **2. Architecture:** complies. ClassTasksController only calls services; the rules are in the services; `ApplicationDbContext` is used directly (no Repository); DTOs are used at the API boundary and AutoMapper for non-trivial mappings; there is no schema change.
- **3. Testing:** complies. The requirements FR-031, FR-032, FR-033, FR-034, FR-035 are covered by the tests in the testing plan.
- **4. Security:** complies. Role and ownership are checked on the server for: GET, POST /api/classes/{id}/tasks, DELETE /api/classes/{id}/tasks/{taskId}; all input is validated on the server; no secret is stored in Git.
- **5. Version control:** complies. The work is done in branch `011-teacher-tasks`, created from an updated `main`, and enters through a Pull Request.
- **6. Documentation:** complies. The specification is written before the code; README and `docs/` are updated if the behaviour changes.
- **7. AI assistance:** complies. The code is reviewed and tested by the student; the Pull Request gets a separate AI review.
- **8. Simplicity:** complies. No new libraries or layers are added.

**No exceptions. The plan complies with the constitution.**

