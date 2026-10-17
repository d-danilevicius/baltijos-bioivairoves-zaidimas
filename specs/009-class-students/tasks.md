# Tasks: Class students and removal

**Specification:** `specs/009-class-students/spec.md`
**Plan:** `specs/009-class-students/plan.md`
**Depends on features:** 003-access-control, 006-confirmation-dialog
**Tags:** `[US1]` – belongs to user story 1; no tag – setup or verification task.

Do the tasks in order. Each phase depends on the previous one. Tests are written before the implementation.

## Data

- [ ] T001 Check that no schema change is needed for this feature (see the plan)

## Tests (written first) (depends on: Data)

- [ ] T002 [US1] Test case: another teacher’s class is rejected – `backend/tests/Project.UnitTests/Classes/ClassMembershipServiceTests.cs`
- [ ] T003 [US1] Test case: results remain – `backend/tests/Project.UnitTests/Classes/ClassMembershipServiceTests.cs`
- [ ] T004 [US1] Authorization test for FR-024: a user without permission or a non-owner is rejected – `backend/tests/Project.IntegrationTests/Classes/ClassesEndpointTests.cs`
- [ ] T005 [US1] Frontend test: the screens of this feature (E11 Class information) show the loading, empty, error and success states – `frontend/src/tests/ClassInfoPage.test.tsx`

## Implementation (depends on: Tests)

- [ ] T006 [US1] Create the request and response DTOs and the validation attributes for: GET /api/classes/{id}/students, DELETE /api/classes/{id}/students/{studentId} – `backend/src/Project.Api/DTOs/`
- [ ] T007 [US1] Implement the business rules and server components: ClassesController (students), ClassMembershipService – `backend/src/Project.Api/Services/`
- [ ] T008 [US1] Add the endpoints: GET /api/classes/{id}/students, DELETE /api/classes/{id}/students/{studentId} – `backend/src/Project.Api/Controllers/`
- [ ] T009 [US1] Ask for confirmation before removing a student from a class using the confirmation dialog (FR-053, VT-029) – `frontend/src/`
- [ ] T010 [US1] Build the screens: ClassInfoPage (student list) – `frontend/src/pages/, frontend/src/components/`
- [ ] T011 [US1] Add the API calls and types for the screens – `frontend/src/api/, frontend/src/types/`

## Verification (depends on: Implementation)

- [ ] T012 Run `dotnet build`, `dotnet test`, `npm run lint`, `npm test` and `npm run build`
- [ ] T013 Update README and `docs/` if the behaviour, setup or configuration changed
- [ ] T014 Review `git diff` and open the Pull Request with the separate AI review evidence
