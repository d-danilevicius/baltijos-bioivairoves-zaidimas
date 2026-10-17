# Tasks: Teacher tasks

**Specification:** `specs/011-teacher-tasks/spec.md`
**Plan:** `specs/011-teacher-tasks/plan.md`
**Depends on features:** 003-access-control, 006-confirmation-dialog
**Tags:** `[US1]` – belongs to user story 1; no tag – setup or verification task.

Do the tasks in order. Each phase depends on the previous one. Tests are written before the implementation.

## Data

- [ ] T001 Check that no schema change is needed for this feature (see the plan)

## Tests (written first) (depends on: Data)

- [ ] T002 [US1] Test case: field limits – `backend/tests/Project.UnitTests/TeacherTasks/TeacherTaskServiceTests.cs`
- [ ] T003 [US1] Test case: confirmation – `backend/tests/Project.UnitTests/TeacherTasks/TeacherTaskServiceTests.cs`
- [ ] T004 [US1] Test case: another class’s task – `backend/tests/Project.UnitTests/TeacherTasks/TeacherTaskServiceTests.cs`
- [ ] T005 [US1] Test case: points remain after deletion – `backend/tests/Project.UnitTests/TeacherTasks/TeacherTaskServiceTests.cs`
- [ ] T006 [US1] Authorization test for FR-024: a user without permission or a non-owner is rejected – `backend/tests/Project.IntegrationTests/TeacherTasks/TeacherTasksEndpointTests.cs`
- [ ] T007 [US1] Frontend test: the screens of this feature (E12 Class tasks, E14 Teacher task form) show the loading, empty, error and success states – `frontend/src/tests/ClassTasksPage.test.tsx`

## Implementation (depends on: Tests)

- [ ] T008 [US1] Create the request and response DTOs and the validation attributes for: GET, POST /api/classes/{id}/tasks, DELETE /api/classes/{id}/tasks/{taskId} – `backend/src/Project.Api/DTOs/`
- [ ] T009 [US1] Implement the business rules and server components: ClassTasksController, TeacherTaskService – `backend/src/Project.Api/Services/`
- [ ] T010 [US1] Add the endpoints: GET, POST /api/classes/{id}/tasks, DELETE /api/classes/{id}/tasks/{taskId} – `backend/src/Project.Api/Controllers/`
- [ ] T011 [US1] Ask for confirmation before deleting a teacher task using the confirmation dialog (FR-053, VT-029) – `frontend/src/`
- [ ] T012 [US1] Build the screens: ClassTasksPage, TeacherTaskPage – `frontend/src/pages/, frontend/src/components/`
- [ ] T013 [US1] Add the API calls and types for the screens – `frontend/src/api/, frontend/src/types/`

## Verification (depends on: Implementation)

- [ ] T014 Run `dotnet build`, `dotnet test`, `npm run lint`, `npm test` and `npm run build`
- [ ] T015 Update README and `docs/` if the behaviour, setup or configuration changed
- [ ] T016 Review `git diff` and open the Pull Request with the separate AI review evidence
