# Tasks: Account deletion

**Specification:** `specs/022-account-deletion/spec.md`
**Plan:** `specs/022-account-deletion/plan.md`
**Depends on features:** 003-access-control, 006-confirmation-dialog
**Tags:** `[US1]` – belongs to user story 1; no tag – setup or verification task.

Do the tasks in order. Each phase depends on the previous one. Tests are written before the implementation.

## Data

- [ ] T001 Check that no schema change is needed for this feature (see the plan)

## Tests (written first) (depends on: Data)

- [ ] T002 [US1] Test case: deletion according to VT-008 and VT-009 – `backend/tests/Project.UnitTests/Accounts/AccountDeletionServiceTests.cs`
- [ ] T003 [US1] Test case: the last administrator – `backend/tests/Project.UnitTests/Accounts/AccountDeletionServiceTests.cs`
- [ ] T004 [US1] Test for NFR-007: integration test: related data are removed – `backend/tests/Project.IntegrationTests/Accounts/AccountsEndpointTests.cs`
- [ ] T005 [US1] Test for NFR-023: integration test – `backend/tests/Project.IntegrationTests/Accounts/AccountsEndpointTests.cs`
- [ ] T006 [US1] Authorization test for FR-059: a user without permission or a non-owner is rejected – `backend/tests/Project.IntegrationTests/Accounts/AccountsEndpointTests.cs`
- [ ] T007 [US1] Frontend test: the screens of this feature (E17 Account management) show the loading, empty, error and success states – `frontend/src/tests/AccountsPage.test.tsx`

## Implementation (depends on: Tests)

- [ ] T008 [US1] Create the request and response DTOs and the validation attributes for: DELETE /api/admin/accounts/{id} – `backend/src/Project.Api/DTOs/`
- [ ] T009 [US1] Implement the business rules and server components: AdminAccountsController (delete), AccountDeletionService – `backend/src/Project.Api/Services/`
- [ ] T010 [US1] Add the endpoints: DELETE /api/admin/accounts/{id} – `backend/src/Project.Api/Controllers/`
- [ ] T011 [US1] Ask for confirmation before deleting an account using the confirmation dialog (FR-053, VT-029) – `frontend/src/`
- [ ] T012 [US1] Build the screens: AccountsPage (delete action) – `frontend/src/pages/, frontend/src/components/`
- [ ] T013 [US1] Add the API calls and types for the screens – `frontend/src/api/, frontend/src/types/`

## Verification (depends on: Implementation)

- [ ] T014 Run `dotnet build`, `dotnet test`, `npm run lint`, `npm test` and `npm run build`
- [ ] T015 Update README and `docs/` if the behaviour, setup or configuration changed
- [ ] T016 Review `git diff` and open the Pull Request with the separate AI review evidence
