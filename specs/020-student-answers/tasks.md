# Tasks: Student answers and evidence for the teacher

**Specification:** `specs/020-student-answers/spec.md`
**Plan:** `specs/020-student-answers/plan.md`
**Depends on features:** 003-access-control
**Tags:** `[US1]` – belongs to user story 1; no tag – setup or verification task.

Do the tasks in order. Each phase depends on the previous one. Tests are written before the implementation.

## Data

- [ ] T001 Check that no schema change is needed for this feature (see the plan)

## Tests (written first) (depends on: Data)

- [ ] T002 [US1] Test case: another teacher’s student rejected – `backend/tests/Project.UnitTests/StudentResults/StudentResultsServiceTests.cs`
- [ ] T003 [US1] Test case: administrator rejected – `backend/tests/Project.UnitTests/StudentResults/StudentResultsServiceTests.cs`
- [ ] T004 [US1] Authorization test for FR-024: a user without permission or a non-owner is rejected – `backend/tests/Project.IntegrationTests/StudentResults/StudentResultsEndpointTests.cs`
- [ ] T005 [US1] Authorization test for FR-058: an administrator gets “Prieiga uždrausta” (Access denied) – `backend/tests/Project.IntegrationTests/StudentResults/StudentResultsEndpointTests.cs`
- [ ] T006 [US1] Frontend test: the screens of this feature (E16 Student results) show the loading, empty, error and success states – `frontend/src/tests/StudentResultsPage.test.tsx`

## Implementation (depends on: Tests)

- [ ] T007 [US1] Create the request and response DTOs and the validation attributes for: GET /api/classes/{id}/students/{studentId}/results – `backend/src/Project.Api/DTOs/`
- [ ] T008 [US1] Implement the business rules and server components: ClassResultsController (student results), StudentResultsService – `backend/src/Project.Api/Services/`
- [ ] T009 [US1] Add the endpoints: GET /api/classes/{id}/students/{studentId}/results – `backend/src/Project.Api/Controllers/`
- [ ] T010 [US1] Build the screens: StudentResultsPage – `frontend/src/pages/, frontend/src/components/`
- [ ] T011 [US1] Add the API calls and types for the screens – `frontend/src/api/, frontend/src/types/`

## Verification (depends on: Implementation)

- [ ] T012 Run `dotnet build`, `dotnet test`, `npm run lint`, `npm test` and `npm run build`
- [ ] T013 Update README and `docs/` if the behaviour, setup or configuration changed
- [ ] T014 Review `git diff` and open the Pull Request with the separate AI review evidence
