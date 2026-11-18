# Tasks: Joining a class with a code

**Specification:** `specs/008-join-class/spec.md`
**Plan:** `specs/008-join-class/plan.md`
**Depends on features:** 003-access-control, 006-confirmation-dialog, 007-class-create-and-delete, 024-page-styling
**Tags:** `[US1]` – belongs to user story 1; no tag – setup or verification task.

Do the tasks in order. Each phase depends on the previous one. Tests are written before the implementation.

## Data

- [x] T001 Add the entity `ClassMembership` and its configuration (foreign keys with cascade delete, unique filtered index for the active membership) – `backend/src/Project.Api/Entities/ClassMembership.cs`, `backend/src/Project.Api/Data/Configurations/ClassMembershipConfiguration.cs`, `backend/src/Project.Api/Data/ApplicationDbContext.cs`
- [x] T002 Create the migration `CreateClassMembershipsTable` – `backend/src/Project.Api/Migrations/`

## Tests (written first) (depends on: Data)

- [x] T003 [US1] Unit tests of `JoinClassRequest`: normalization, empty code, wrong length, wrong symbol – `backend/tests/Project.UnitTests/Classes/JoinClassRequestValidationTests.cs`
- [x] T004 [US1] Unit tests of `ClassMembershipService`: join, class not found, same class, confirmation required, class change with confirmation, one active membership – `backend/tests/Project.UnitTests/Classes/ClassMembershipServiceTests.cs`
- [x] T005 [US1] Integration tests of `POST /api/classes/join`: join, normalized code, unknown code 400, wrong format 400, 409 without confirmation, change with confirmation, same class, deleted class, teacher 403, administrator 403, visitor 401, two requests at the same time – `backend/tests/Project.IntegrationTests/Classes/JoinClassEndpointTests.cs`
- [x] T006 [US1] Frontend tests of the join form and the dialog: validation errors, class not found, dialog with both class names, “Pakeisti”, “Atšaukti”, success message, same class message, 401 – `frontend/src/tests/JoinClassPage.test.tsx`, `frontend/src/tests/validateClassCode.test.ts`
- [x] T007 [US1] Frontend tests of the student menu, the profile message and the access of a teacher to `/join` – `frontend/src/tests/navigationLink.test.ts`, `frontend/src/tests/ProfilePage.test.tsx`, `frontend/src/tests/JoinClassPage.test.tsx`

## Implementation (depends on: Tests)

- [x] T008 [US1] Create the request and response DTOs: `JoinClassRequest`, `JoinClassResponse`, `ClassChangeConfirmationResponse` – `backend/src/Project.Api/DTOs/`
- [x] T009 [US1] Implement `JoinClassOutcome`, `JoinClassResult`, `IClassMembershipService` and `ClassMembershipService` and register them – `backend/src/Project.Api/Services/`
- [x] T010 [US1] Add `ClassMembershipController` with `POST /api/classes/join` – `backend/src/Project.Api/Controllers/ClassMembershipController.cs`
- [x] T011 [US1] Add the types and the API call for joining – `frontend/src/types/classMembership.ts`, `frontend/src/api/classMembershipApi.ts`
- [x] T012 [US1] Add `validateClassCode` and `joinClassMessage` – `frontend/src/helpers/`
- [x] T013 [US1] Build the screen `JoinClassPage` with the confirmation dialog – `frontend/src/pages/JoinClassPage.tsx`
- [x] T014 [US1] Add the student menu, show the success message in `ProfilePage` and add the route `/join` – `frontend/src/helpers/navigationLink.ts`, `frontend/src/pages/ProfilePage.tsx`, `frontend/src/App.tsx`

## Verification (depends on: Implementation)

- [x] T015 Run `dotnet build backend/Project.slnx`, `dotnet test backend/Project.slnx`, `npm run lint`, `npm test` and `npm run build`
- [x] T016 Update README (the endpoint, the migration, the student screen), `docs/architecture.md` and check the OpenAPI document
- [ ] T017 Review `git diff` and open the Pull Request with screenshots and the separate AI review evidence
