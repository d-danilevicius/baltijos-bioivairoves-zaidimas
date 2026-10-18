# Tasks: Login and logout

**Specification:** `specs/002-login/spec.md`
**Plan:** `specs/002-login/plan.md`
**Depends on features:** none
**Tags:** `[US1]` – belongs to user story 1; no tag – setup or verification task.

Do the tasks in order. Each phase depends on the previous one. Tests are written before the implementation.

## Data

- [x] T001 Add or change the entities described in the plan: ApplicationUser.IsBlocked, ActionLog – `backend/src/Project.Api/Entities/`
- [x] T002 Update `ApplicationDbContext` for these entities – `backend/src/Project.Api/Data/ApplicationDbContext.cs`
- [x] T003 Create the migration `AddIsBlockedAndActionLogTable` – `backend/src/Project.Api/Migrations/`

## Tests (written first) (depends on: Data)

- [x] T004 [US1] Test case: incorrect data – `backend/tests/Project.UnitTests/Login/LoginServiceTests.cs`
- [x] T005 [US1] Test case: blocked account – `backend/tests/Project.UnitTests/Login/LoginServiceTests.cs`
- [x] T006 [US1] Test case: the session ends after 30 min – `backend/tests/Project.UnitTests/Login/LoginServiceTests.cs`
- [x] T007 [US1] Integration test: the session cookie has the HttpOnly, Secure and SameSite flags, and a request that changes data without a valid CSRF token is rejected – `backend/tests/Project.IntegrationTests/Login/LoginEndpointTests.cs`
- [x] T008 [US1] Test for NFR-004: integration test with a replaceable clock – `backend/tests/Project.IntegrationTests/Login/LoginEndpointTests.cs`
- [x] T009 [US1] Test for NFR-023: integration test – `backend/tests/Project.IntegrationTests/Login/LoginEndpointTests.cs`
- [x] T010 [US1] Test for NFR-024: the log has no password – `backend/tests/Project.IntegrationTests/Login/LoginEndpointTests.cs`
- [x] T011 [US1] Frontend test: the screens of this feature (E03 Login, E18 Profile (logout)) show the loading, empty, error and success states – `frontend/src/tests/LoginPage.test.tsx`

## Implementation (depends on: Tests)

- [x] T012 [US1] Configure the session cookie (HttpOnly, Secure, SameSite, 30-minute inactivity) and CSRF protection – `backend/src/Project.Api/Program.cs`
- [x] T013 [US1] Create the request and response DTOs and the validation attributes for: POST /api/auth/login, POST /api/auth/logout – `backend/src/Project.Api/DTOs/`
- [x] T014 [US1] Implement the business rules and server components: AuthController (login, logout), LoginService – `backend/src/Project.Api/Services/`
- [x] T015 [US1] Add the endpoints: POST /api/auth/login, POST /api/auth/logout – `backend/src/Project.Api/Controllers/`
- [x] T016 [US1] Build the screens: LoginPage – `frontend/src/pages/, frontend/src/components/`
- [x] T017 [US1] Add the API calls and types for the screens – `frontend/src/api/, frontend/src/types/`

## Verification (depends on: Implementation)

- [x] T018 Run `dotnet build`, `dotnet test`, `npm run lint`, `npm test` and `npm run build`
- [x] T019 Update README and `docs/` if the behaviour, setup or configuration changed
- [ ] T020 Review `git diff` and open the Pull Request with the separate AI review evidence
