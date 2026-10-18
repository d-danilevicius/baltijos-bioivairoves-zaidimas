# Tasks: Access control and error pages

**Specification:** `specs/003-access-control/spec.md`
**Plan:** `specs/003-access-control/plan.md`
**Depends on features:** none
**Tags:** `[US1]` – belongs to user story 1; no tag – setup or verification task.

Do the tasks in order. Each phase depends on the previous one. Tests are written before the implementation.

## Data

- [x] T001 Check that no schema change is needed for this feature (see the plan)

## Tests (written first) (depends on: Data)

- [x] T002 [US1] Unit tests for role policy matrix (Student, Teacher, Administrator × protected actions) – `backend/tests/Project.UnitTests/AccessControl/AccessPolicyTests.cs`
- [x] T003 [US1] Integration tests for `GET /api/auth/me`: valid session -> 200 with `{ id, userName, role }`; no session -> 401 – `backend/tests/Project.IntegrationTests/AccessControl/AuthMeEndpointTests.cs`
- [x] T004 [US1] Integration tests for role policies using a small test-only controller in `backend/tests/Project.IntegrationTests/AccessControl/` (not in `Project.Api`): three actions (Student, Teacher, Administrator) called as student, teacher, administrator and without a session; role mismatch -> 403 with “Prieiga uždrausta” – `backend/tests/Project.IntegrationTests/AccessControl/AccessControlEndpointTests.cs`
- [x] T005 [US1] Frontend tests for `ProtectedRoute`: no session redirect to login with “Prisijunkite, kad galėtumėte tęsti.” and expired session inactivity message – `frontend/src/tests/ProtectedRoute.test.tsx`
- [x] T006 [US1] Frontend tests for E19 AccessDeniedPage on wrong-role flow – `frontend/src/tests/AccessDeniedPage.test.tsx`
- [x] T007 [US1] Frontend tests for E20 NotFoundPage unknown URL flow with “Į pradžią” button – `frontend/src/tests/NotFoundPage.test.tsx`
- [x] T008 [US1] Frontend tests for shared loading/error (“Bandyti dar kartą”)/empty-state components – `frontend/src/tests/CommonStateComponents.test.tsx`
- [x] T009 [US1] Frontend test for `ProfilePage` loading current user via `/api/auth/me` instead of router state – `frontend/src/tests/ProfilePage.test.tsx`

## Implementation (depends on: Tests)

- [x] T010 [US1] Define server role policies for Student, Teacher and Administrator and apply them where an endpoint needs a role; `GET /api/auth/me` and logout stay available to every authenticated role – `backend/src/Project.Api/Program.cs, backend/src/Project.Api/Controllers/`
- [x] T011 [US1] Add `GET /api/auth/me` endpoint that returns current user `{ id, userName, role }` for valid session and 401 otherwise – `backend/src/Project.Api/Controllers/`
- [x] T012 [US1] Return 403 with “Prieiga uždrausta” (Access denied) through central error handling for insufficient-role access – `backend/src/Project.Api/Middleware/`
- [x] T013 [US1] Build frontend route-access foundations: `ProtectedRoute`, `AccessDeniedPage`, `NotFoundPage` – `frontend/src/components/, frontend/src/pages/`
- [x] T014 [US1] Add shared loading/error/empty-state components and integrate them into the protected-route flows – `frontend/src/components/`
- [x] T015 [US1] Update `ProfilePage` to use `/api/auth/me` instead of router state and adjust related API/types – `frontend/src/pages/, frontend/src/api/, frontend/src/types/`

## Verification (depends on: Implementation)

- [x] T016 Run `dotnet build`, `dotnet test`, `npm run lint`, `npm test` and `npm run build`
- [x] T017 Update README and `docs/` if the behaviour, setup or configuration changed
- [ ] T018 Review `git diff` and open the Pull Request with the separate AI review evidence
