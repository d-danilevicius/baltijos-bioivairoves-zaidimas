# Tasks: Class creation and deletion

**Specification:** `specs/007-class-create-and-delete/spec.md`
**Plan:** `specs/007-class-create-and-delete/plan.md`
**Depends on features:** 003-access-control, 006-confirmation-dialog, 024-page-styling
**Tags:** `[US1]` – belongs to user story 1; no tag – setup or verification task.

Do the tasks in order. Each phase depends on the previous one. Tests are written before the implementation.

## Data

- [ ] T001 Add the entity `SchoolClass` and its configuration (name, unique code, owner) to `ApplicationDbContext` – `backend/src/Project.Api/Entities/SchoolClass.cs`, `backend/src/Project.Api/Data/ApplicationDbContext.cs`
- [ ] T002 Create the migration `CreateClassesTable` – `backend/src/Project.Api/Migrations/`

## Tests (written first) (depends on: Data)

- [ ] T003 [US1] Unit tests of the code generator – `backend/tests/Project.UnitTests/Classes/ClassCodeGeneratorTests.cs`, `CreateClassRequestValidationTests.cs`
- [ ] T004 [US1] Unit tests of the class service: name rules, collision of the code, owner, not found – `backend/tests/Project.UnitTests/Classes/ClassServiceTests.cs`
- [ ] T005 [US1] Integration tests of the four endpoints: create, list, get, delete, student 403, other teacher 403, visitor 401, 404, validation 400 – `backend/tests/Project.IntegrationTests/Classes/ClassEndpointTests.cs`, `ClassesTestClient.cs`
- [ ] T006 [US1] Frontend tests of the class list, the new class form and the class information with the dialog – `frontend/src/tests/ClassListPage.test.tsx`, `frontend/src/tests/NewClassPage.test.tsx`, `frontend/src/tests/ClassDetailsPage.test.tsx`
- [ ] T007 [US1] Frontend tests of the starting screen by role, the teacher menu and the access of a student – `frontend/src/tests/startingScreen.test.ts`, `frontend/src/tests/navigationLinks.test.ts`, `frontend/src/tests/validateClassName.test.ts`, `frontend/src/tests/LoginPage.test.tsx`, `frontend/src/tests/HomePage.test.tsx`

## Implementation (depends on: Tests)

- [ ] T008 [US1] Create the request and response DTOs – `backend/src/Project.Api/DTOs/`
- [ ] T009 [US1] Implement `IClassCodeGenerator` and `ClassCodeGenerator` – `backend/src/Project.Api/Services/`
- [ ] T010 [US1] Implement `ClassResult`, `IClassService` and `ClassService` and register them – `backend/src/Project.Api/Services/`
- [ ] T011 [US1] Add `ClassesController` with the four endpoints – `backend/src/Project.Api/Controllers/ClassesController.cs`
- [ ] T012 [US1] Add the types and the API calls for classes – `frontend/src/types/classes.ts`, `frontend/src/api/classesApi.ts`
- [ ] T013 [US1] Add `validateClassName` and `classMessages` – `frontend/src/helpers/`
- [ ] T014 [US1] Build the screens `ClassListPage`, `NewClassPage`, `ClassDetailsPage` – `frontend/src/pages/`
- [ ] T015 [US1] Add the starting screen by role and the teacher menu, use them in `LoginPage`, `HomePage`, `ProfilePage` and add the routes – `frontend/src/helpers/startingScreen.ts`, `frontend/src/helpers/navigationLinks.ts`, `frontend/src/App.tsx`

## Verification (depends on: Implementation)

- [ ] T016 Run `dotnet build backend/Project.slnx`, `dotnet test backend/Project.slnx`, `npm run lint`, `npm test` and `npm run build`
- [ ] T017 Update README (the endpoints, the migration, the teacher screens), `docs/architecture.md` (the endpoint `GET /api/classes/{id}`, the entity name) and check the OpenAPI document
- [ ] T018 Review `git diff` and open the Pull Request with screenshots and the separate AI review evidence
