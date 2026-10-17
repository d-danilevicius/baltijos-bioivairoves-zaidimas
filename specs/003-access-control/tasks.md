# Tasks: Access control and error pages

**Specification:** `specs/003-access-control/spec.md`
**Plan:** `specs/003-access-control/plan.md`
**Depends on features:** none
**Tags:** `[US1]` – belongs to user story 1; no tag – setup or verification task.

Do the tasks in order. Each phase depends on the previous one. Tests are written before the implementation.

## Data

- [ ] T001 Check that no schema change is needed for this feature (see the plan)

## Tests (written first) (depends on: Data)

- [ ] T002 [US1] Test case: authorization table (student, teacher, administrator × protected actions); frontend route protection – `backend/tests/Project.UnitTests/AccessControl/AccessPolicyTests.cs`
- [ ] T003 [US1] Test for NFR-003: authorization tests for every protected action – `backend/tests/Project.IntegrationTests/AccessControl/AccessControlEndpointTests.cs`
- [ ] T004 [US1] Test for NFR-027: Vitest test – `frontend/src/tests/AccessDeniedPage.test.tsx`
- [ ] T005 [US1] Test for NFR-028: Vitest tests – `frontend/src/tests/AccessDeniedPage.test.tsx`
- [ ] T006 [US1] Test for NFR-029: Vitest tests – `frontend/src/tests/AccessDeniedPage.test.tsx`
- [ ] T007 [US1] Frontend test: the screens of this feature (E19 Access denied, E20 Page not found) show the loading, empty, error and success states – `frontend/src/tests/AccessDeniedPage.test.tsx`

## Implementation (depends on: Tests)

- [ ] T008 [US1] Create the authorization policies for the roles Student, Teacher and Administrator and apply them to the controllers – `backend/src/Project.Api/Program.cs, backend/src/Project.Api/Controllers/`
- [ ] T009 [US1] Create the ownership checks that the services call before changing or returning data of a class or a student – `backend/src/Project.Api/Services/`
- [ ] T010 [US1] Return the error “Prieiga uždrausta” (Access denied) with status 403 through the central error handling – `backend/src/Project.Api/Middleware/`
- [ ] T011 [US1] Build the screens: AccessDeniedPage, NotFoundPage, protectedRoute component – `frontend/src/pages/, frontend/src/components/`
- [ ] T012 [US1] Add the API calls and types for the screens – `frontend/src/api/, frontend/src/types/`

## Verification (depends on: Implementation)

- [ ] T013 Run `dotnet build`, `dotnet test`, `npm run lint`, `npm test` and `npm run build`
- [ ] T014 Update README and `docs/` if the behaviour, setup or configuration changed
- [ ] T015 Review `git diff` and open the Pull Request with the separate AI review evidence
