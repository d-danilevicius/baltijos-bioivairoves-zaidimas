# Tasks: Task repetition

**Specification:** `specs/017-task-repeat/spec.md`
**Plan:** `specs/017-task-repeat/plan.md`
**Depends on features:** 003-access-control
**Tags:** `[US1]` – belongs to user story 1; no tag – setup or verification task.

Do the tasks in order. Each phase depends on the previous one. Tests are written before the implementation.

## Data

- [ ] T001 Check that no schema change is needed for this feature (see the plan)

## Tests (written first) (depends on: Data)

- [ ] T002 [US1] Test case: the highest result remains – `backend/tests/Project.UnitTests/Submissions/SubmissionServiceTests.cs`
- [ ] T003 [US1] Frontend test: the screens of this feature (E07 Task result) show the loading, empty, error and success states – `frontend/src/tests/TaskResultPage.test.tsx`

## Implementation (depends on: Tests)

- [ ] T004 [US1] Create the request and response DTOs and the validation attributes for: POST /api/my/tasks/{id}/submissions – `backend/src/Project.Api/DTOs/`
- [ ] T005 [US1] Implement the business rules and server components: SubmissionService (best result is kept) – `backend/src/Project.Api/Services/`
- [ ] T006 [US1] Add the endpoints: POST /api/my/tasks/{id}/submissions – `backend/src/Project.Api/Controllers/`
- [ ] T007 [US1] Build the screens: TaskResultPage (button “Pakartoti užduotį” (Repeat the task)) – `frontend/src/pages/, frontend/src/components/`
- [ ] T008 [US1] Add the API calls and types for the screens – `frontend/src/api/, frontend/src/types/`

## Verification (depends on: Implementation)

- [ ] T009 Run `dotnet build`, `dotnet test`, `npm run lint`, `npm test` and `npm run build`
- [ ] T010 Update README and `docs/` if the behaviour, setup or configuration changed
- [ ] T011 Review `git diff` and open the Pull Request with the separate AI review evidence
