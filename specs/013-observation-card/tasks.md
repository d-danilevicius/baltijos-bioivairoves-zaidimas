# Tasks: Observation card and protected species

**Specification:** `specs/013-observation-card/spec.md`
**Plan:** `specs/013-observation-card/plan.md`
**Depends on features:** 003-access-control
**Tags:** `[US1]` – belongs to user story 1; no tag – setup or verification task.

Do the tasks in order. Each phase depends on the previous one. Tests are written before the implementation.

## Data

- [ ] T001 Check that no schema change is needed for this feature (see the plan)

## Tests (written first) (depends on: Data)

- [ ] T002 [US1] Test case: the coordinates of a protected species are not in the response – `backend/tests/Project.UnitTests/ObservationCard/ObservationCardServiceTests.cs`
- [ ] T003 [US1] Test case: the GBIF label – `backend/tests/Project.UnitTests/ObservationCard/ObservationCardServiceTests.cs`
- [ ] T004 [US1] Test case: licence and link are present – `backend/tests/Project.UnitTests/ObservationCard/ObservationCardServiceTests.cs`
- [ ] T005 [US1] Frontend test: the screens of this feature (E06 Task) show the loading, empty, error and success states – `frontend/src/tests/TaskPage.test.tsx`

## Implementation (depends on: Tests)

- [ ] T006 [US1] Create the request and response DTOs and the validation attributes for: GET /api/my/tasks/{id} – `backend/src/Project.Api/DTOs/`
- [ ] T007 [US1] Implement the business rules and server components: MyTasksController (GET {id}), ObservationCardService – `backend/src/Project.Api/Services/`
- [ ] T008 [US1] Add the endpoints: GET /api/my/tasks/{id} – `backend/src/Project.Api/Controllers/`
- [ ] T009 [US1] Build the screens: TaskPage, observationCard component – `frontend/src/pages/, frontend/src/components/`
- [ ] T010 [US1] Add the API calls and types for the screens – `frontend/src/api/, frontend/src/types/`

## Verification (depends on: Implementation)

- [ ] T011 Run `dotnet build`, `dotnet test`, `npm run lint`, `npm test` and `npm run build`
- [ ] T012 Update README and `docs/` if the behaviour, setup or configuration changed
- [ ] T013 Review `git diff` and open the Pull Request with the separate AI review evidence
