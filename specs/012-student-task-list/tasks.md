# Tasks: Student task list

**Specification:** `specs/012-student-task-list/spec.md`
**Plan:** `specs/012-student-task-list/plan.md`
**Depends on features:** 003-access-control
**Tags:** `[US1]` – belongs to user story 1; no tag – setup or verification task.

Do the tasks in order. Each phase depends on the previous one. Tests are written before the implementation.

## Data

- [ ] T001 Check that no schema change is needed for this feature (see the plan)

## Tests (written first) (depends on: Data)

- [ ] T002 [US1] Test case: groups – `backend/tests/Project.UnitTests/StudentTasks/StudentTaskListServiceTests.cs`
- [ ] T003 [US1] Test case: locked groups – `backend/tests/Project.UnitTests/StudentTasks/StudentTaskListServiceTests.cs`
- [ ] T004 [US1] Test case: a student without a class – `backend/tests/Project.UnitTests/StudentTasks/StudentTaskListServiceTests.cs`
- [ ] T005 [US1] Test case: the first uncompleted task – `backend/tests/Project.UnitTests/StudentTasks/StudentTaskListServiceTests.cs`
- [ ] T006 [US1] Test for NFR-009: Vitest test: two clicks from the task list to the task – `frontend/src/tests/TaskListPage.test.tsx`
- [ ] T007 [US1] Frontend test: the screens of this feature (E04 Task list) show the loading, empty, error and success states – `frontend/src/tests/TaskListPage.test.tsx`

## Implementation (depends on: Tests)

- [ ] T008 [US1] Create the request and response DTOs and the validation attributes for: GET /api/my/tasks – `backend/src/Project.Api/DTOs/`
- [ ] T009 [US1] Implement the business rules and server components: MyTasksController, StudentTaskListService – `backend/src/Project.Api/Services/`
- [ ] T010 [US1] Add the endpoints: GET /api/my/tasks – `backend/src/Project.Api/Controllers/`
- [ ] T011 [US1] Build the screens: TaskListPage – `frontend/src/pages/, frontend/src/components/`
- [ ] T012 [US1] Add the API calls and types for the screens – `frontend/src/api/, frontend/src/types/`

## Verification (depends on: Implementation)

- [ ] T013 Run `dotnet build`, `dotnet test`, `npm run lint`, `npm test` and `npm run build`
- [ ] T014 Update README and `docs/` if the behaviour, setup or configuration changed
- [ ] T015 Review `git diff` and open the Pull Request with the separate AI review evidence
