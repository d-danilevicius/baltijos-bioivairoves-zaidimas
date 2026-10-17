# Tasks: Task creation from GBIF observations

**Specification:** `specs/010-gbif-task-generation/spec.md`
**Plan:** `specs/010-gbif-task-generation/plan.md`
**Depends on features:** 003-access-control
**Tags:** `[US1]` – belongs to user story 1; no tag – setup or verification task.

Do the tasks in order. Each phase depends on the previous one. Tests are written before the implementation.

## Data

- [ ] T001 Add or change the entities described in the plan: Observation, ClassTask, Class (location, season) – `backend/src/Project.Api/Entities/`
- [ ] T002 Update `ApplicationDbContext` for these entities – `backend/src/Project.Api/Data/ApplicationDbContext.cs`
- [ ] T003 Create the migration `CreateObservationsAndTasksTables` – `backend/src/Project.Api/Migrations/`

## Tests (written first) (depends on: Data)

- [ ] T004 [US1] Test case: fewer than 10 observations – `backend/tests/Project.UnitTests/GbifTasks/GbifTaskServiceTests.cs`
- [ ] T005 [US1] Test case: GBIF error – `backend/tests/Project.UnitTests/GbifTasks/GbifTaskServiceTests.cs`
- [ ] T006 [US1] Test case: protected species – `backend/tests/Project.UnitTests/GbifTasks/GbifTaskServiceTests.cs`
- [ ] T007 [US1] Test case: not created a second time – `backend/tests/Project.UnitTests/GbifTasks/GbifTaskServiceTests.cs`
- [ ] T008 [US1] Test for NFR-002: GBIF client test; configuration check – `backend/tests/Project.IntegrationTests/GbifTasks/GbifTasksEndpointTests.cs`
- [ ] T009 [US1] Test for NFR-016: test with a fake GBIF response: tasks created within 30 s – `backend/tests/Project.IntegrationTests/GbifTasks/GbifTasksEndpointTests.cs`
- [ ] T010 [US1] Test for NFR-020: the fake GBIF does not respond or returns an error – `backend/tests/Project.IntegrationTests/GbifTasks/GbifTasksEndpointTests.cs`
- [ ] T011 [US1] Test for NFR-021: when GBIF data change, the tasks do not change – `backend/tests/Project.IntegrationTests/GbifTasks/GbifTasksEndpointTests.cs`
- [ ] T012 [US1] Frontend test: the screens of this feature (E13 Task creation from GBIF, E12 Class tasks) show the loading, empty, error and success states – `frontend/src/tests/CreateGbifTasksPage.test.tsx`

## Implementation (depends on: Tests)

- [ ] T013 [US1] Create the request and response DTOs and the validation attributes for: POST /api/classes/{id}/gbif-tasks – `backend/src/Project.Api/DTOs/`
- [ ] T014 [US1] Implement the business rules and server components: ClassTasksController (gbif-tasks), GbifTaskService, GbifClient (Integrations/Gbif) – `backend/src/Project.Api/Services/`
- [ ] T015 [US1] Add the endpoints: POST /api/classes/{id}/gbif-tasks – `backend/src/Project.Api/Controllers/`
- [ ] T016 [US1] Build the screens: CreateGbifTasksPage – `frontend/src/pages/, frontend/src/components/`
- [ ] T017 [US1] Add the API calls and types for the screens – `frontend/src/api/, frontend/src/types/`

## Verification (depends on: Implementation)

- [ ] T018 Run `dotnet build`, `dotnet test`, `npm run lint`, `npm test` and `npm run build`
- [ ] T019 Update README and `docs/` if the behaviour, setup or configuration changed
- [ ] T020 Review `git diff` and open the Pull Request with the separate AI review evidence
