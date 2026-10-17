# Tasks: My results

**Specification:** `specs/018-my-results/spec.md`
**Plan:** `specs/018-my-results/plan.md`
**Depends on features:** 003-access-control
**Tags:** `[US1]` – belongs to user story 1; no tag – setup or verification task.

Do the tasks in order. Each phase depends on the previous one. Tests are written before the implementation.

## Data

- [ ] T001 Check that no schema change is needed for this feature (see the plan)

## Tests (written first) (depends on: Data)

- [ ] T002 [US1] Test case: sees only own – `backend/tests/Project.UnitTests/MyResults/ResultsServiceTests.cs`
- [ ] T003 [US1] Test case: administrator rejected – `backend/tests/Project.UnitTests/MyResults/ResultsServiceTests.cs`
- [ ] T004 [US1] Authorization test for FR-052: a user without permission or a non-owner is rejected – `backend/tests/Project.IntegrationTests/MyResults/MyResultsEndpointTests.cs`
- [ ] T005 [US1] Authorization test for FR-058: an administrator gets “Prieiga uždrausta” (Access denied) – `backend/tests/Project.IntegrationTests/MyResults/MyResultsEndpointTests.cs`
- [ ] T006 [US1] Frontend test: the screens of this feature (E08 My results) show the loading, empty, error and success states – `frontend/src/tests/MyResultsPage.test.tsx`

## Implementation (depends on: Tests)

- [ ] T007 [US1] Create the request and response DTOs and the validation attributes for: GET /api/my/results – `backend/src/Project.Api/DTOs/`
- [ ] T008 [US1] Implement the business rules and server components: MyResultsController, ResultsService – `backend/src/Project.Api/Services/`
- [ ] T009 [US1] Add the endpoints: GET /api/my/results – `backend/src/Project.Api/Controllers/`
- [ ] T010 [US1] Build the screens: MyResultsPage – `frontend/src/pages/, frontend/src/components/`
- [ ] T011 [US1] Add the API calls and types for the screens – `frontend/src/api/, frontend/src/types/`

## Verification (depends on: Implementation)

- [ ] T012 Run `dotnet build`, `dotnet test`, `npm run lint`, `npm test` and `npm run build`
- [ ] T013 Update README and `docs/` if the behaviour, setup or configuration changed
- [ ] T014 Review `git diff` and open the Pull Request with the separate AI review evidence
