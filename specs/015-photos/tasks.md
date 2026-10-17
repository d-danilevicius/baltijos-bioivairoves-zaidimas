# Tasks: Photos with the evidence

**Specification:** `specs/015-photos/spec.md`
**Plan:** `specs/015-photos/plan.md`
**Depends on features:** 003-access-control
**Tags:** `[US1]` – belongs to user story 1; no tag – setup or verification task.

Do the tasks in order. Each phase depends on the previous one. Tests are written before the implementation.

## Data

- [ ] T001 Add or change the entities described in the plan: SubmissionPhoto – `backend/src/Project.Api/Entities/`
- [ ] T002 Update `ApplicationDbContext` for these entities – `backend/src/Project.Api/Data/ApplicationDbContext.cs`
- [ ] T003 Create the migration `CreateSubmissionPhotosTable` – `backend/src/Project.Api/Migrations/`

## Tests (written first) (depends on: Data)

- [ ] T004 [US1] Test case: format – `backend/tests/Project.UnitTests/Photos/PhotoServiceTests.cs`
- [ ] T005 [US1] Test case: size – `backend/tests/Project.UnitTests/Photos/PhotoServiceTests.cs`
- [ ] T006 [US1] Test case: count – `backend/tests/Project.UnitTests/Photos/PhotoServiceTests.cs`
- [ ] T007 [US1] Test case: metadata removed – `backend/tests/Project.UnitTests/Photos/PhotoServiceTests.cs`
- [ ] T008 [US1] Test for NFR-006: the stored file has no metadata – `backend/tests/Project.IntegrationTests/Photos/PhotosEndpointTests.cs`
- [ ] T009 [US1] Frontend test: the screens of this feature (E06 Task) show the loading, empty, error and success states – `frontend/src/tests/photoInput.test.tsx`

## Implementation (depends on: Tests)

- [ ] T010 [US1] Create the request and response DTOs and the validation attributes for: POST /api/my/tasks/{id}/submissions (photos, multipart) – `backend/src/Project.Api/DTOs/`
- [ ] T011 [US1] Implement the business rules and server components: SubmissionsController, SubmissionService, PhotoService (SixLabors.ImageSharp) – `backend/src/Project.Api/Services/`
- [ ] T012 [US1] Add the endpoints: POST /api/my/tasks/{id}/submissions (photos, multipart) – `backend/src/Project.Api/Controllers/`
- [ ] T013 [US1] Build the screens: photoInput component – `frontend/src/pages/, frontend/src/components/`
- [ ] T014 [US1] Add the API calls and types for the screens – `frontend/src/api/, frontend/src/types/`

## Verification (depends on: Implementation)

- [ ] T015 Run `dotnet build`, `dotnet test`, `npm run lint`, `npm test` and `npm run build`
- [ ] T016 Update README and `docs/` if the behaviour, setup or configuration changed
- [ ] T017 Review `git diff` and open the Pull Request with the separate AI review evidence
