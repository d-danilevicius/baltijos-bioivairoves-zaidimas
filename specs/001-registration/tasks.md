# Tasks: Registration

**Specification:** `specs/001-registration/spec.md`
**Plan:** `specs/001-registration/plan.md`
**Depends on features:** none
**Tags:** `[US1]` – belongs to user story 1; no tag – setup or verification task.

Do the tasks in order. Each phase depends on the previous one. Tests are written before the implementation.

## Setup

- [ ] T001 Add the package `Microsoft.AspNetCore.Identity.EntityFrameworkCore` – `backend/src/Project.Api/Project.Api.csproj`
- [ ] T002 Add the package `Microsoft.EntityFrameworkCore.InMemory` – `backend/tests/Project.UnitTests/Project.UnitTests.csproj`
- [ ] T003 Add the package `Testcontainers.PostgreSql` – `backend/tests/Project.IntegrationTests/Project.IntegrationTests.csproj`
- [ ] T004 Add the dev dependency `@testing-library/user-event` – `frontend/package.json`

## Tests (written first) (depends on: Setup)

- [ ] T005 [US1] Test: a valid student is registered with the student role – `backend/tests/Project.UnitTests/Registration/RegistrationServiceTests.cs`
- [ ] T006 [US1] Test: a valid teacher is registered with the teacher role – `backend/tests/Project.UnitTests/Registration/RegistrationServiceTests.cs`
- [ ] T007 [US1] Test: a repeated email (different letter case) is rejected – `backend/tests/Project.UnitTests/Registration/RegistrationServiceTests.cs`
- [ ] T008 [US1] Test: a repeated username is rejected – `backend/tests/Project.UnitTests/Registration/RegistrationServiceTests.cs`
- [ ] T009 [US1] Test: the administrator role and an unknown role are rejected – `backend/tests/Project.UnitTests/Registration/RegistrationServiceTests.cs`
- [ ] T010 [US1] Test: the password is stored only as a hash – `backend/tests/Project.UnitTests/Registration/RegistrationServiceTests.cs`
- [ ] T011 [US1] Test: the password is not written to logs (NFR-024) – `backend/tests/Project.UnitTests/Registration/RegistrationServiceTests.cs`
- [ ] T012 [US1] Test: limits of email, username, password and role in the request – `backend/tests/Project.UnitTests/Registration/RegisterRequestValidationTests.cs`
- [ ] T013 [US1] Integration test: 201 and a stored hash; 409 for a repeated email and username; 400 for the administrator role and a short password – `backend/tests/Project.IntegrationTests/Registration/RegistrationEndpointTests.cs`
- [ ] T014 [US1] Frontend test: registration form messages and the successful result – `frontend/src/tests/RegisterPage.test.tsx`

## Implementation (depends on: Tests)

- [ ] T015 [US1] Create `ApplicationUser` and `RoleNames` – `backend/src/Project.Api/Entities/`
- [ ] T016 [US1] Make `ApplicationDbContext` an Identity context with seeded roles and a unique email index – `backend/src/Project.Api/Data/ApplicationDbContext.cs`
- [ ] T017 [US1] Create `RegisterRequest` with the field limits and `RegisteredUserResponse` – `backend/src/Project.Api/DTOs/`
- [ ] T018 [US1] Create `RegistrationResult` and `IRegistrationService` – `backend/src/Project.Api/Services/`
- [ ] T019 [US1] Create `RegistrationService` with the registration rules – `backend/src/Project.Api/Services/RegistrationService.cs`
- [ ] T020 [US1] Configure Identity (the email normalization matches the unique index on `NormalizedEmail`) and register the service – `backend/src/Project.Api/Services/ServiceCollectionExtensions.cs`, `backend/src/Project.Api/Program.cs`
- [ ] T021 [US1] Add `POST /api/auth/register` – `backend/src/Project.Api/Controllers/AuthController.cs`
- [ ] T022 [US1] Create the migration `CreateIdentityTables` – `backend/src/Project.Api/Migrations/`
- [ ] T023 [US1] Create the registration page with the form and the error display – `frontend/src/pages/RegisterPage.tsx`
- [ ] T024 [US1] Create the form components, the API call, the validation and the message helpers – `frontend/src/components/`, `frontend/src/api/`, `frontend/src/helpers/`, `frontend/src/types/`
- [ ] T025 [US1] Add the routes `/register` and `/login` and the temporary login page (replaced when feature 002 is implemented) – `frontend/src/app.tsx`, `frontend/src/pages/LoginPage.tsx`

## Verification (depends on: Implementation)

- [ ] T026 Run `dotnet build`, `dotnet test`, `npm run lint`, `npm test` and `npm run build`
- [ ] T027 Check the feature by hand according to `quickstart.md`, including SC-001 (a new user registers in no more than 1 minute, measured by hand)
- [ ] T028 Review `git diff` and open the Pull Request with the separate AI review evidence
