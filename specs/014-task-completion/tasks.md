# Tasks: Task completion

**Specification:** `specs/014-task-completion/spec.md`
**Plan:** `specs/014-task-completion/plan.md`
**Depends on features:** 003-access-control, 006-confirmation-dialog
**Tags:** `[US1]` – belongs to user story 1; no tag – setup or verification task.

Do the tasks in order. Each phase depends on the previous one. Tests are written before the implementation.

## Data

- [ ] T001 Add or change the entities described in the plan: Submission – `backend/src/Project.Api/Entities/`
- [ ] T002 Update `ApplicationDbContext` for these entities – `backend/src/Project.Api/Data/ApplicationDbContext.cs`
- [ ] T003 Create the migration `CreateSubmissionsTable` – `backend/src/Project.Api/Migrations/`

## Tests (written first) (depends on: Data)

- [ ] T004 [US1] Test case: locked task – `backend/tests/Project.UnitTests/Submissions/SubmissionServiceTests.cs`
- [ ] T005 [US1] Test case: empty evidence – `backend/tests/Project.UnitTests/Submissions/SubmissionServiceTests.cs`
- [ ] T006 [US1] Test case: the answer cannot be changed – `backend/tests/Project.UnitTests/Submissions/SubmissionServiceTests.cs`
- [ ] T007 [US1] Test case: deleted task – `backend/tests/Project.UnitTests/Submissions/SubmissionServiceTests.cs`
- [ ] T008 [US1] Frontend test: the screens of this feature (E06 Task, E07 Task result) show the loading, empty, error and success states – `frontend/src/tests/TaskPage.test.tsx`

## Implementation (depends on: Tests)

- [ ] T009 [US1] Create the request and response DTOs and the validation attributes for: POST /api/my/tasks/{id}/submissions – `backend/src/Project.Api/DTOs/`
- [ ] T010 [US1] Implement the business rules and server components: SubmissionsController, SubmissionService – `backend/src/Project.Api/Services/`
- [ ] T011 [US1] Add the endpoints: POST /api/my/tasks/{id}/submissions – `backend/src/Project.Api/Controllers/`
- [ ] T012 [US1] Ask for confirmation before leaving an unfinished task using the confirmation dialog (FR-053, VT-029) – `frontend/src/`
- [ ] T013 [US1] Build the screens: TaskPage (answer form), TaskResultPage – `frontend/src/pages/, frontend/src/components/`
- [ ] T014 [US1] Add the API calls and types for the screens – `frontend/src/api/, frontend/src/types/`

## Verification (depends on: Implementation)

- [ ] T015 Run `dotnet build`, `dotnet test`, `npm run lint`, `npm test` and `npm run build`
- [ ] T016 Update README and `docs/` if the behaviour, setup or configuration changed
- [ ] T017 Review `git diff` and open the Pull Request with the separate AI review evidence
