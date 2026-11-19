# Implementation Plan: Teacher tasks

**Feature:** `specs/011-teacher-tasks/spec.md`
**Branch:** `011-teacher-tasks`
**Status:** draft
**Depends on features:** 003-access-control, 006-confirmation-dialog, 007-class-create-and-delete, 010-gbif-task-generation, 024-page-styling

## Summary

A teacher creates own task for own class (title, question, correct answer, difficulty group and the confirmation that the task does not require collecting or disturbing organisms) and deletes it after a confirmation. The class task list (feature 010) shows teacher tasks with the label “Mokytojo užduotis” and the type “Mokytojo užduotis”. The server enforces the roles, the ownership and the validation. The frontend gets the teacher task form (E14), the “Sukurti savo užduotį” button and the deletion on E12.

## Technical Context

- **Language and version:** C# with .NET 10 (server); TypeScript (frontend).
- **Libraries:** as selected in `docs/architecture.md` (section 1). No new library. The mapping from the entity to `ClassTaskResponse` is trivial (four fields) and stays in one extension method; AutoMapper is not added for it, as in features 007 and 010.
- **Frontend:** React with Vite, React Router and Tailwind CSS; the dialog of feature 006.
- **Storage:** PostgreSQL through Entity Framework Core; the schema change is made by the migration `AddTeacherTaskFields`.
- **Testing:** xUnit (unit and integration, PostgreSQL in Testcontainers), Vitest and React Testing Library.
- **Target platform:** web application in current Chrome, Firefox, Edge and Safari; screens from 360 px width.
- **Constraints:** HTTPS only; POST and DELETE use the existing CSRF protection; secrets are not stored in the repository.

## Structure and Main Components

- Server:
  - `Entities/TaskSource.cs` – new enum: `Gbif`, `Teacher`.
  - `Entities/TaskType.cs` – new value `TeacherTask`.
  - `Entities/ClassTask.cs` – new properties `Title` (nullable, used by teacher tasks) and `Source`.
  - `Data/Configurations/ClassTaskConfiguration.cs` – `Source` stored as text (default `Gbif` for existing rows), `Title` up to 100 characters.
  - `DTOs/Requests/CreateTeacherTaskRequest.cs` – title, question, correct answer (spaces at the beginning and end removed when the value is set), difficulty group 1–3, confirmation; `[Required]`, `[StringLength]`, `[Range]` rules with the codes `Required`, `TooLong`, `Invalid`, `NotConfirmed`.
  - `DTOs/Responses/ClassTaskResponse.cs` – unchanged (id, type, group, label).
  - `Services/ClassTaskExtensions.cs` – `ToResponse()` for `ClassTask`: the label is “GBIF” or “Mokytojo užduotis” by the source; one place for the list and the create response (DRY).
  - `Services/ClassTaskService.cs` – the list uses `ToResponse()` (the constant `GbifLabel` moves to the extension).
  - `Services/ITeacherTaskService.cs`, `Services/TeacherTaskService.cs` – creates a teacher task and deletes a teacher task of an owned class (one responsibility, separate from the list service).
  - `Services/TeacherTaskOutcome.cs`, `Services/TeacherTaskResult.cs` – outcomes `Created`, `Deleted`, `NotFound`, `Forbidden`, `NotTeacherTask` and the created task.
  - `Controllers/ClassTaskController.cs` – new actions `POST tasks` and `DELETE tasks/{taskId}`; translates the result to 201, 204, 400, 401, 403, 404 and 409.
  - `Services/ServiceCollectionExtensions.cs` – registers `ITeacherTaskService`.
- Frontend:
  - `types/classTask.ts` – `TeacherTaskRequest`; `TeacherTask` is added to `TaskType`.
  - `api/classTaskApi.ts` – `createTeacherTask` and `deleteClassTask` (CSRF token as in the existing POST).
  - `helpers/teacherTaskMessage.ts` – the texts of E14 and the deletion.
  - `helpers/validateTeacherTask.ts` – validation of the form in the browser (same rules as VT-018, VT-021).
  - `helpers/gbifTaskMessage.ts` – `taskTypeLabels` gets `TeacherTask`.
  - `components/difficultyChoice.tsx` – the choice of three groups (radio buttons, as `seasonChoice.tsx`).
  - `components/textAreaField.tsx` – a multi-line field with a label and an error (for the question); the existing `textField.tsx` is single-line.
  - `components/classTaskTable.tsx` – the task table with the “Ištrinti” button next to teacher tasks (moved out of `ClassTasksPage.tsx` to keep the page below about 200 lines).
  - `pages/CreateTeacherTaskPage.tsx` – E14.
  - `pages/ClassTasksPage.tsx` – shows the table component, the button “Sukurti savo užduotį”, and the deletion with `ConfirmDialog`.
  - `app.tsx` – route `/classes/:id/tasks/new` inside `ProtectedRoute allowedRoles={['Teacher']}`.

## Server Request Changes

- `POST /api/classes/{id}/tasks` – creates a teacher task (201 with the task, 400 with the errors per field, 401, 403, 404).
- `DELETE /api/classes/{id}/tasks/{taskId}` – deletes a teacher task of the owner (204, 401, 403, 404; 409 for a GBIF task).
- `GET /api/classes/{id}/tasks` – unchanged route; the teacher tasks have the label “Mokytojo užduotis”.

## Data Model Changes

- Table `ClassTasks`: new column `Title` (nullable, up to 100 characters) and new column `Source` (text, default `Gbif`). The column `CorrectAnswer` keeps the limit of 200 (GBIF answers); the limit of 100 for teacher tasks is checked in the request.
- New migration `AddTeacherTaskFields` in the same branch.

## Testing Plan

- Unit tests (`CreateTeacherTaskRequest`): a valid request passes; an empty title, question or correct answer, a title of 101, a question of 501 and an answer of 101 characters fail; the limits are accepted at 100, 500 and 100; a title of 100 letters with spaces around is accepted; only spaces count as empty; difficulty group 0 and 4 and a missing group fail; a missing or `false` confirmation fails.
- Unit tests (`TeacherTaskService`): a task is created with `Source = Teacher`, `Type = TeacherTask` and the entered values; a non-owner gets `Forbidden` and nothing is saved; a missing class gets `NotFound`; deleting an own teacher task removes it; deleting a task of another class gives `NotFound`; deleting a GBIF task gives `NotTeacherTask` and the task stays; a task already removed gives `NotFound`.
- Unit tests (`ClassTaskService`): the list labels a teacher task “Mokytojo užduotis” and a GBIF task “GBIF”.
- Integration tests: a teacher creates a task and gets 201; the task is in the list with the label “Mokytojo užduotis”; invalid fields and a missing confirmation give 400 with the errors per field and nothing is saved; a student gets 403 on POST and DELETE; another teacher gets 403 on POST and DELETE; a visitor gets 401; a missing class or task gives 404; deleting an own teacher task gives 204 and it disappears from the list; deleting a GBIF task gives 409; a task of another class cannot be deleted through own class (404); the GBIF tasks of the class still can be created after a teacher task (409 is returned only when the class already has a season).
- Frontend tests: E14 shows the validation errors next to the fields, no group is selected initially, the confirmation error is shown, a saved task opens E12 with „Užduotis išsaugota.“, the server error message is shown; E12 shows the label and the “Ištrinti” button only next to teacher tasks, deletes only after the confirmation (cancel changes nothing); a student opening the route sees “Prieiga uždrausta”; `validateTeacherTask` unit tests.

## Technical Notes

- The source and the type of a teacher task are set by the server; the request has no fields for them, for the class owner or for the identifier.
- The delete removes the loaded task and saves; when another request already removed it, `DbUpdateConcurrencyException` is caught and the result is `NotFound` (the same way as deleting a class in feature 007), so two requests deleting the same task at the same time give 204 and 404, not an error.
- The correct answer is stored as entered (after removing the spaces at the beginning and end); the comparison of student answers belongs to feature 014.
- The class tasks and their points: when feature 014–016 add answers and points, they MUST keep the points when a task is deleted (VT-018); the foreign key of the later results must not cascade from the task.
- The frontend wrappers keep the status 401; the pages open the login page with the session message through `SessionExpiredRedirect`.
- The task type “Mokytojo užduotis” is added by the student’s decision (Clarifications of the specification); `docs/architecture.md` is updated in this feature.

## Constitution Check

- **1. Code quality:** complies. Each class has one responsibility and stays below 200 lines; the table is moved out of `ClassTasksPage.tsx`; React file names follow the naming rules.
- **2. Architecture:** complies. The controller only calls the services; the rules are in `TeacherTaskService`; `ApplicationDbContext` is used directly (no Repository); DTOs are used at the API boundary; the schema change has a migration.
- **3. Testing:** complies. FR-031–FR-035, FR-053 and FR-001–FR-005 are covered by the tests in the testing plan, including the authorization tests.
- **4. Security:** complies. Role and ownership are checked on the server for both new endpoints; the owner and the source are set by the server; all input is validated on the server; POST and DELETE use the CSRF protection; no secret is stored in Git.
- **5. Version control:** complies. The work is done in branch `011-teacher-tasks`, created from an updated `main`, and enters through a Pull Request.
- **6. Documentation:** complies. The specification is written before the code; README and `docs/` are updated.
- **7. AI assistance:** complies. The code is reviewed and tested by the student; the Pull Request gets a separate AI review.
- **8. Simplicity:** complies. No new libraries; one new service next to the list service; AutoMapper is not added for a four-field mapping.

**No exceptions. The plan complies with the constitution.**
