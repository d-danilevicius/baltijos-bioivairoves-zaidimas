# Tasks: Teacher tasks

**Specification:** `specs/011-teacher-tasks/spec.md`
**Plan:** `specs/011-teacher-tasks/plan.md`
**Depends on features:** 003-access-control, 006-confirmation-dialog, 007-class-create-and-delete, 010-gbif-task-generation, 024-page-styling
**Tags:** `[US1]` – belongs to user story 1; no tag – setup or verification task.

Do the tasks in order. Each phase depends on the previous one. Tests are written before the implementation.

## Data

- [ ] T001 Add the enum `TaskSource`, the value `TaskType.TeacherTask` and the properties `Title` and `Source` of `ClassTask`; update `ClassTaskConfiguration` (`Source` as text with the default `Gbif`, `Title` up to 100 characters) – `backend/src/Project.Api/Entities/TaskSource.cs`, `backend/src/Project.Api/Entities/TaskType.cs`, `backend/src/Project.Api/Entities/ClassTask.cs`, `backend/src/Project.Api/Data/Configurations/ClassTaskConfiguration.cs`
- [ ] T002 Create the migration `AddTeacherTaskFields` – `backend/src/Project.Api/Migrations/`

## Tests (written first) (depends on: Data)

- [ ] T003 [US1] Unit tests of `CreateTeacherTaskRequest`: required fields, limits 100 / 500 / 100, spaces removed, difficulty group 1–3, confirmation – `backend/tests/Project.UnitTests/TeacherTasks/CreateTeacherTaskRequestValidationTests.cs`
- [ ] T004 [US1] Unit tests of `TeacherTaskService`: create, non-owner, missing class, delete, task of another class, GBIF task, already removed, removed by another request before the save – `backend/tests/Project.UnitTests/TeacherTasks/TeacherTaskServiceTests.cs`, `TeacherTaskDeletionRaceTests.cs`
- [ ] T005 [US1] Unit tests of the list labels “Mokytojo užduotis” and “GBIF” – `backend/tests/Project.UnitTests/Gbif/ClassTaskServiceTests.cs`
- [ ] T006 [US1] Integration tests of `POST` and `DELETE` tasks: create, list label, validation 400, student 403, other teacher 403, visitor 401, 404, delete 204, GBIF task 409, task of another class 404 – `backend/tests/Project.IntegrationTests/TeacherTasks/TeacherTaskEndpointTests.cs`
- [ ] T007 [US1] Frontend tests of the teacher task form (validation, confirmation, success message, server error) and of the access of a student – `frontend/src/tests/CreateTeacherTaskPage.test.tsx`
- [ ] T008 [US1] Frontend tests of E12: the label, the “Ištrinti” button only for teacher tasks, the deletion with the confirmation and the cancel – `frontend/src/tests/ClassTasksPage.test.tsx`
- [ ] T009 [US1] Frontend tests of the validation of the form – `frontend/src/tests/validateTeacherTask.test.ts`

## Implementation (depends on: Tests)

- [ ] T010 [US1] Create `CreateTeacherTaskRequest` – `backend/src/Project.Api/DTOs/Requests/CreateTeacherTaskRequest.cs`
- [ ] T011 [US1] Add `ClassTaskExtensions.ToResponse()` and use it in `ClassTaskService` – `backend/src/Project.Api/Services/ClassTaskExtensions.cs`, `backend/src/Project.Api/Services/ClassTaskService.cs`
- [ ] T012 [US1] Implement `TeacherTaskOutcome`, `TeacherTaskResult`, `ITeacherTaskService` and `TeacherTaskService` and register them – `backend/src/Project.Api/Services/`
- [ ] T013 [US1] Add the actions `POST tasks` and `DELETE tasks/{taskId}` to `ClassTaskController` – `backend/src/Project.Api/Controllers/ClassTaskController.cs`
- [ ] T014 [US1] Add the types and the API calls `createTeacherTask` and `deleteClassTask` – `frontend/src/types/classTask.ts`, `frontend/src/api/classTaskApi.ts`
- [ ] T015 [US1] Add `teacherTaskMessage`, `validateTeacherTask` and the label of `TeacherTask` – `frontend/src/helpers/teacherTaskMessage.ts`, `frontend/src/helpers/validateTeacherTask.ts`, `frontend/src/helpers/gbifTaskMessage.ts`
- [ ] T016 [US1] Add the components `difficultyChoice`, `textAreaField` and `classTaskTable` – `frontend/src/components/`
- [ ] T017 [US1] Build `CreateTeacherTaskPage` (E14) – `frontend/src/pages/CreateTeacherTaskPage.tsx`
- [ ] T018 [US1] Update `ClassTasksPage` (table component, button “Sukurti savo užduotį”, deletion with the dialog) and add the route `/classes/:id/tasks/new` – `frontend/src/pages/ClassTasksPage.tsx`, `frontend/src/app.tsx`

## Verification (depends on: Implementation)

- [ ] T019 Run `dotnet build backend/Project.slnx`, `dotnet test backend/Project.slnx`, `npm run lint`, `npm test` and `npm run build`
- [ ] T020 Update README (the endpoints, the migration, the teacher task screen), `docs/architecture.md` (the entity `ClassTask`, the type “Mokytojo užduotis”) and check the OpenAPI document
- [ ] T021 Review `git diff` and open the Pull Request with screenshots and the separate AI review evidence
