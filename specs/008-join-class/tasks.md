# Tasks: Joining a class with a code

**Specification:** `specs/008-join-class/spec.md`
**Plan:** `specs/008-join-class/plan.md`
**Depends on features:** 003-access-control, 006-confirmation-dialog
**Tags:** `[US1]` – belongs to user story 1; no tag – setup or verification task.

Do the tasks in order. Each phase depends on the previous one. Tests are written before the implementation.

## Data

- [ ] T001 Add or change the entities described in the plan: ClassMembership – `backend/src/Project.Api/Entities/`
- [ ] T002 Update `ApplicationDbContext` for these entities – `backend/src/Project.Api/Data/ApplicationDbContext.cs`
- [ ] T003 Create the migration `CreateClassMembershipsTable` – `backend/src/Project.Api/Migrations/`

## Tests (written first) (depends on: Data)

- [ ] T004 [US1] Test case: code normalization – `backend/tests/Project.UnitTests/Classes/ClassMembershipServiceTests.cs`
- [ ] T005 [US1] Test case: code not found – `backend/tests/Project.UnitTests/Classes/ClassMembershipServiceTests.cs`
- [ ] T006 [US1] Test case: class change – `backend/tests/Project.UnitTests/Classes/ClassMembershipServiceTests.cs`
- [ ] T007 [US1] Frontend test: the screens of this feature (E05 Join a class) show the loading, empty, error and success states – `frontend/src/tests/JoinClassPage.test.tsx`

## Implementation (depends on: Tests)

- [ ] T008 [US1] Create the request and response DTOs and the validation attributes for: POST /api/classes/join – `backend/src/Project.Api/DTOs/`
- [ ] T009 [US1] Implement the business rules and server components: ClassesController (join), ClassMembershipService – `backend/src/Project.Api/Services/`
- [ ] T010 [US1] Add the endpoints: POST /api/classes/join – `backend/src/Project.Api/Controllers/`
- [ ] T011 [US1] Ask for confirmation before changing a student’s class using the confirmation dialog (FR-053, VT-029) – `frontend/src/`
- [ ] T012 [US1] Build the screens: JoinClassPage – `frontend/src/pages/, frontend/src/components/`
- [ ] T013 [US1] Add the API calls and types for the screens – `frontend/src/api/, frontend/src/types/`

## Verification (depends on: Implementation)

- [ ] T014 Run `dotnet build`, `dotnet test`, `npm run lint`, `npm test` and `npm run build`
- [ ] T015 Update README and `docs/` if the behaviour, setup or configuration changed
- [ ] T016 Review `git diff` and open the Pull Request with the separate AI review evidence
