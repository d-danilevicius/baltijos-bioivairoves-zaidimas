# Tasks: Login and logout

**Specification:** `specs/002-login/spec.md`
**Plan:** `specs/002-login/plan.md`
**Depends on features:** none
**Tags:** `[US1]` – belongs to user story 1; no tag – setup or verification task.

Do the tasks in order. Each phase depends on the previous one. Tests are written before the implementation.

## Data

- [ ] T001 Add or change the entities described in the plan: ApplicationUser.IsBlocked, ActionLog – `backend/src/Project.Api/Entities/`
- [ ] T002 Update `ApplicationDbContext` for these entities – `backend/src/Project.Api/Data/ApplicationDbContext.cs`
- [ ] T003 Create the migration `AddIsBlockedAndActionLogTable` – `backend/src/Project.Api/Migrations/`

## Tests (written first) (depends on: Data)

- [ ] T004 [US1] Test case: incorrect data – `backend/tests/Project.UnitTests/Login/LoginServiceTests.cs`
- [ ] T005 [US1] Test case: blocked account – `backend/tests/Project.UnitTests/Login/LoginServiceTests.cs`
- [ ] T006 [US1] Test case: the session ends after 30 min – `backend/tests/Project.UnitTests/Login/LoginServiceTests.cs`
- [ ] T007 [US1] Test for NFR-004: integration test with a replaceable clock – `backend/tests/Project.IntegrationTests/Login/LoginEndpointTests.cs`
- [ ] T008 [US1] Test for NFR-023: integration test – `backend/tests/Project.IntegrationTests/Login/LoginEndpointTests.cs`
- [ ] T009 [US1] Test for NFR-024: the log has no password – `backend/tests/Project.IntegrationTests/Login/LoginEndpointTests.cs`
- [ ] T010 [US1] Frontend test: the screens of this feature (E03 Login, E18 Profile (logout)) show the loading, empty, error and success states – `frontend/src/tests/LoginPage.test.tsx`

## Implementation (depends on: Tests)

- [ ] T011 [US1] Create the request and response DTOs and the validation attributes for: POST /api/auth/login, POST /api/auth/logout – `backend/src/Project.Api/DTOs/`
- [ ] T012 [US1] Implement the business rules and server components: AuthController (login, logout), LoginService – `backend/src/Project.Api/Services/`
- [ ] T013 [US1] Add the endpoints: POST /api/auth/login, POST /api/auth/logout – `backend/src/Project.Api/Controllers/`
- [ ] T014 [US1] Build the screens: LoginPage – `frontend/src/pages/, frontend/src/components/`
- [ ] T015 [US1] Add the API calls and types for the screens – `frontend/src/api/, frontend/src/types/`

## Verification (depends on: Implementation)

- [ ] T016 Run `dotnet build`, `dotnet test`, `npm run lint`, `npm test` and `npm run build`
- [ ] T017 Update README and `docs/` if the behaviour, setup or configuration changed
- [ ] T018 Review `git diff` and open the Pull Request with the separate AI review evidence
