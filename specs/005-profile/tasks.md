# Tasks: User profile

**Specification:** `specs/005-profile/spec.md`
**Plan:** `specs/005-profile/plan.md`
**Depends on features:** 003-access-control, 024-page-styling
**Tags:** `[US1]` – belongs to user story 1; no tag – setup or verification task.

Do the tasks in order. Each phase depends on the previous one. Tests are written before the implementation.

## Data

- [ ] T001 Check that no schema change and no new migration is needed for this feature (see the plan)

## Tests (written first) (depends on: Data)

- [ ] T002 [US1] Integration test: `GET /api/auth/me` with a valid session returns the email – `backend/tests/Project.IntegrationTests/AccessControl/AuthMeEndpointTests.cs`
- [ ] T003 [US1] Frontend test: the profile screen shows the username, the email and the role – `frontend/src/tests/ProfilePage.test.tsx`

## Implementation (depends on: Tests)

- [ ] T004 [US1] Add the field `Email` to `CurrentUserResponse` and return it from `AuthController.Me` – `backend/src/Project.Api/DTOs/Responses/CurrentUserResponse.cs`, `backend/src/Project.Api/Controllers/AuthController.cs`
- [ ] T005 [US1] Add the field `email` to the type `CurrentUser` – `frontend/src/types/auth.ts`
- [ ] T006 [US1] Show the email on the profile screen – `frontend/src/pages/ProfilePage.tsx`

## Verification (depends on: Implementation)

- [ ] T007 Run `dotnet build`, `dotnet test`, `npm run lint`, `npm test` and `npm run build`
- [ ] T008 Update README (`GET /api/auth/me` returns the email) and `docs/architecture.md` (endpoint name `GET /api/auth/me`)
- [ ] T009 Review `git diff` and open the Pull Request with the separate AI review evidence
