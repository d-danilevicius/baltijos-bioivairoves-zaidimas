# Tasks: Points and group unlocking

**Specification:** `specs/016-points-and-unlocking/spec.md`
**Plan:** `specs/016-points-and-unlocking/plan.md`
**Depends on features:** 003-access-control
**Tags:** `[US1]` – belongs to user story 1; no tag – setup or verification task.

Do the tasks in order. Each phase depends on the previous one. Tests are written before the implementation.

## Data

- [ ] T001 Check that no schema change is needed for this feature (see the plan)

## Tests (written first) (depends on: Data)

- [ ] T002 [US1] Test case: points 10/20/30 – `backend/tests/Project.UnitTests/Scoring/ScoringServiceTests.cs`
- [ ] T003 [US1] Test case: the 60 % threshold – `backend/tests/Project.UnitTests/Scoring/ScoringServiceTests.cs`
- [ ] T004 [US1] Test case: saving when the connection is lost – `backend/tests/Project.UnitTests/Scoring/ScoringServiceTests.cs`
- [ ] T005 [US1] Test for NFR-015: integration test with time measurement – `backend/tests/Project.IntegrationTests/Scoring/ScoringEndpointTests.cs`
- [ ] T006 [US1] Test for NFR-018: points are written to the database before the response is returned; integration test – `backend/tests/Project.IntegrationTests/Scoring/ScoringEndpointTests.cs`
- [ ] T007 [US1] Frontend test: the screens of this feature (E07 Task result, E04 Task list) show the loading, empty, error and success states – `frontend/src/tests/TaskResultPage.test.tsx`

## Implementation (depends on: Tests)

- [ ] T008 [US1] Create `ScoringService` (points 10/20/30, VT-016) and `GroupUnlockService` (60 % threshold, VT-014) – `backend/src/Project.Api/Services/`
- [ ] T009 [US1] Call both services when an answer is submitted and save the points before the response is returned (NFR-018) – `backend/src/Project.Api/Services/SubmissionService.cs`
- [ ] T010 [US1] Return the group state in GET /api/my/tasks – `backend/src/Project.Api/Controllers/MyTasksController.cs`
- [ ] T011 [US1] Build the screens: TaskResultPage (points and unlocked group), TaskListPage (group state) – `frontend/src/pages/, frontend/src/components/`
- [ ] T012 [US1] Add the API calls and types for the screens – `frontend/src/api/, frontend/src/types/`

## Verification (depends on: Implementation)

- [ ] T013 Run `dotnet build`, `dotnet test`, `npm run lint`, `npm test` and `npm run build`
- [ ] T014 Update README and `docs/` if the behaviour, setup or configuration changed
- [ ] T015 Review `git diff` and open the Pull Request with the separate AI review evidence
