# Tasks: Class creation and deletion

**Specification:** `specs/007-class-create-and-delete/spec.md`
**Plan:** `specs/007-class-create-and-delete/plan.md`
**Depends on features:** 003-access-control, 006-confirmation-dialog
**Tags:** `[US1]` – belongs to user story 1; no tag – setup or verification task.

Do the tasks in order. Each phase depends on the previous one. Tests are written before the implementation.

## Data

- [ ] T001 Add or change the entities described in the plan: Class (Name, Code, OwnerId) – `backend/src/Project.Api/Entities/`
- [ ] T002 Update `ApplicationDbContext` for these entities – `backend/src/Project.Api/Data/ApplicationDbContext.cs`
- [ ] T003 Create the migration `CreateClassesTable` – `backend/src/Project.Api/Migrations/`

## Tests (written first) (depends on: Data)

- [ ] T004 [US1] Test case: code uniqueness – `backend/tests/Project.UnitTests/Classes/ClassServiceTests.cs`
- [ ] T005 [US1] Test case: a student has no permission – `backend/tests/Project.UnitTests/Classes/ClassServiceTests.cs`
- [ ] T006 [US1] Test case: a non-owner is rejected – `backend/tests/Project.UnitTests/Classes/ClassServiceTests.cs`
- [ ] T007 [US1] Authorization test for FR-024: a user without permission or a non-owner is rejected – `backend/tests/Project.IntegrationTests/Classes/ClassesEndpointTests.cs`
- [ ] T008 [US1] Frontend test: the screens of this feature (E09 Class list, E10 New class, E11 Class information) show the loading, empty, error and success states – `frontend/src/tests/ClassListPage.test.tsx`

## Implementation (depends on: Tests)

- [ ] T009 [US1] Create the request and response DTOs and the validation attributes for: GET /api/classes, POST /api/classes, DELETE /api/classes/{id} – `backend/src/Project.Api/DTOs/`
- [ ] T010 [US1] Implement the business rules and server components: ClassesController, ClassService (code generation, deletion) – `backend/src/Project.Api/Services/`
- [ ] T011 [US1] Add the endpoints: GET /api/classes, POST /api/classes, DELETE /api/classes/{id} – `backend/src/Project.Api/Controllers/`
- [ ] T012 [US1] Ask for confirmation before deleting a class using the confirmation dialog (FR-053, VT-029) – `frontend/src/`
- [ ] T013 [US1] Build the screens: ClassListPage, NewClassPage, ClassInfoPage – `frontend/src/pages/, frontend/src/components/`
- [ ] T014 [US1] Add the API calls and types for the screens – `frontend/src/api/, frontend/src/types/`

## Verification (depends on: Implementation)

- [ ] T015 Run `dotnet build`, `dotnet test`, `npm run lint`, `npm test` and `npm run build`
- [ ] T016 Update README and `docs/` if the behaviour, setup or configuration changed
- [ ] T017 Review `git diff` and open the Pull Request with the separate AI review evidence
