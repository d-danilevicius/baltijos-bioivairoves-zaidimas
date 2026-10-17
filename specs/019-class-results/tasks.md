# Tasks: Class results

**Specification:** `specs/019-class-results/spec.md`
**Plan:** `specs/019-class-results/plan.md`
**Depends on features:** 003-access-control
**Tags:** `[US1]` – belongs to user story 1; no tag – setup or verification task.

Do the tasks in order. Each phase depends on the previous one. Tests are written before the implementation.

## Data

- [ ] T001 Check that no schema change is needed for this feature (see the plan)

## Tests (written first) (depends on: Data)

- [ ] T002 [US1] Test case: average – `backend/tests/Project.UnitTests/ClassResults/ClassResultsServiceTests.cs`
- [ ] T003 [US1] Test case: ordering – `backend/tests/Project.UnitTests/ClassResults/ClassResultsServiceTests.cs`
- [ ] T004 [US1] Test case: another teacher’s class – `backend/tests/Project.UnitTests/ClassResults/ClassResultsServiceTests.cs`
- [ ] T005 [US1] Test case: administrator rejected – `backend/tests/Project.UnitTests/ClassResults/ClassResultsServiceTests.cs`
- [ ] T006 [US1] Authorization test for FR-024: a user without permission or a non-owner is rejected – `backend/tests/Project.IntegrationTests/ClassResults/ClassResultsEndpointTests.cs`
- [ ] T007 [US1] Authorization test for FR-058: an administrator gets “Prieiga uždrausta” (Access denied) – `backend/tests/Project.IntegrationTests/ClassResults/ClassResultsEndpointTests.cs`
- [ ] T008 [US1] Frontend test: the screens of this feature (E15 Class results) show the loading, empty, error and success states – `frontend/src/tests/ClassResultsPage.test.tsx`

## Implementation (depends on: Tests)

- [ ] T009 [US1] Create the request and response DTOs and the validation attributes for: GET /api/classes/{id}/results – `backend/src/Project.Api/DTOs/`
- [ ] T010 [US1] Implement the business rules and server components: ClassResultsController, ClassResultsService – `backend/src/Project.Api/Services/`
- [ ] T011 [US1] Add the endpoints: GET /api/classes/{id}/results – `backend/src/Project.Api/Controllers/`
- [ ] T012 [US1] Build the screens: ClassResultsPage – `frontend/src/pages/, frontend/src/components/`
- [ ] T013 [US1] Add the API calls and types for the screens – `frontend/src/api/, frontend/src/types/`

## Verification (depends on: Implementation)

- [ ] T014 Run `dotnet build`, `dotnet test`, `npm run lint`, `npm test` and `npm run build`
- [ ] T015 Update README and `docs/` if the behaviour, setup or configuration changed
- [ ] T016 Review `git diff` and open the Pull Request with the separate AI review evidence
