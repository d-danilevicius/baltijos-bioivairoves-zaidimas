# Tasks: Task creation from GBIF observations

**Specification:** `specs/010-gbif-task-generation/spec.md`
**Plan:** `specs/010-gbif-task-generation/plan.md`
**Depends on features:** 003-access-control, 007-class-create-and-delete, 024-page-styling
**Tags:** `[US1]` – belongs to user story 1; no tag – setup or verification task.

Do the tasks in order. Each phase depends on the previous one. Tests are written before the implementation.

## Data

- [ ] T001 Add the enums `Season` (with `Months()` and `Opposite()`) and `TaskType` – `backend/src/Project.Api/Entities/Season.cs`, `backend/src/Project.Api/Entities/TaskType.cs`
- [ ] T002 Add the entities `Observation` and `ClassTask`, add `Latitude`, `Longitude` and `Season` to `SchoolClass` – `backend/src/Project.Api/Entities/`
- [ ] T003 Add the configurations of `Observation` and `ClassTask` (foreign keys with cascade delete, unique index on class and GBIF key), `Season` as the concurrency token of the class, and the `DbSet`s – `backend/src/Project.Api/Data/Configurations/`, `backend/src/Project.Api/Data/ApplicationDbContext.cs`
- [ ] T004 Create the migration `CreateObservationsAndTasksTables` – `backend/src/Project.Api/Migrations/`

## Tests (written first) (depends on: Data)

- [ ] T005 [US1] Unit tests of `SeasonExtensions`: the months of every season, the opposite season – `backend/tests/Project.UnitTests/Gbif/SeasonExtensionsTests.cs`
- [ ] T006 [US1] Unit tests of `GeoDistance` and `GbifObservationSelector`: 100 m and 9.9 km kept, 10.1 km removed, no coordinates, no species name, duplicates – `backend/tests/Project.UnitTests/Gbif/GbifObservationSelectorTests.cs`
- [ ] T007 [US1] Unit tests of `GbifTaskFactory` and `SeasonalTaskFactory`: at most 10 tasks, four types and groups, options, answers, skipped seasonal task, no task asks to collect or touch organisms – `backend/tests/Project.UnitTests/Gbif/GbifTaskFactoryTests.cs`
- [ ] T008 [US1] Unit tests of `GbifTaskService` with a fake `IGbifClient`: success, fewer than 10 observations, GBIF failure saves nothing, second creation, other teacher, missing class, protected species mark, concurrency conflict – `backend/tests/Project.UnitTests/Gbif/GbifTaskServiceTests.cs`
- [ ] T009 [US1] Unit tests of `GbifClient` with a fake HTTP handler: request parameters, mapping, timeout, network error, status 500 – `backend/tests/Project.UnitTests/Gbif/GbifClientTests.cs`
- [ ] T010 [US1] Unit tests of `CreateGbifTasksRequest`: outside Lithuania, borders, undefined season – `backend/tests/Project.UnitTests/Gbif/CreateGbifTasksRequestValidationTests.cs`
- [ ] T011 [US1] Integration tests of `POST /api/classes/{id}/gbif-tasks` and `GET /api/classes/{id}/tasks` with a fake GBIF client: 201, 422, 502, 409, two requests at the same time, 400 per field, student 403, other teacher 403, visitor 401, missing class 404 – `backend/tests/Project.IntegrationTests/Gbif/GbifTaskEndpointTests.cs`
- [ ] T012 [US1] Frontend tests of `validateGbifTasks`, `CreateGbifTasksPage` (validation errors, loading, GBIF error with “Bandyti dar kartą”, too few observations, success, coordinates without the map) and `ClassTasksPage` (empty, list, hidden “Sukurti iš GBIF”) – `frontend/src/tests/`
- [ ] T013 [US1] Frontend tests of the route access: a student sees “Prieiga uždrausta”, a 401 opens the login page with the session message – `frontend/src/tests/`

## Implementation (depends on: Tests)

- [ ] T014 [US1] Add `GbifOptions`, `GbifOccurrence`, `GbifUnavailableException`, `IGbifClient` and `GbifClient`, add the `Gbif` section to `appsettings.json` – `backend/src/Project.Api/Integrations/Gbif/`, `backend/src/Project.Api/appsettings.json`
- [ ] T015 [US1] Implement `GeoDistance` and `GbifObservationSelector` – `backend/src/Project.Api/Services/`
- [ ] T016 [US1] Implement `GbifTaskFactory` and `SeasonalTaskFactory` – `backend/src/Project.Api/Services/`
- [ ] T017 [US1] Create the DTOs: `CreateGbifTasksRequest`, `GbifTasksResponse`, `ClassTaskResponse`; add `Latitude`, `Longitude`, `Season` to `ClassResponse` – `backend/src/Project.Api/DTOs/`
- [ ] T018 [US1] Implement `GbifTaskOutcome`, `GbifTaskResult`, `IGbifTaskService`, `GbifTaskService`, `IClassTaskService` and `ClassTaskService`; register them and the `HttpClient` with the 10-second timeout – `backend/src/Project.Api/Services/`
- [ ] T019 [US1] Add `ClassTaskController` with `POST /api/classes/{id}/gbif-tasks` and `GET /api/classes/{id}/tasks` – `backend/src/Project.Api/Controllers/ClassTaskController.cs`
- [ ] T020 [US1] Add the types and the API calls – `frontend/src/types/classTask.ts`, `frontend/src/types/schoolClass.ts`, `frontend/src/api/classTaskApi.ts`
- [ ] T021 [US1] Add `validateGbifTasks`, `gbifTaskMessage` and `lithuaniaBounds` – `frontend/src/helpers/`
- [ ] T022 [US1] Add `leaflet` and `@types/leaflet`, build `locationMap` and `seasonChoice` – `frontend/package.json`, `frontend/src/components/`
- [ ] T023 [US1] Build the screens `CreateGbifTasksPage` (E13) and `ClassTasksPage` (E12), add the routes and the link and location in `ClassDetailsPage` – `frontend/src/pages/`, `frontend/src/app.tsx`

## Verification (depends on: Implementation)

- [ ] T024 Run `dotnet build backend/Project.slnx`, `dotnet test backend/Project.slnx`, `npm run lint`, `npm test` and `npm run build`
- [ ] T025 Update README (the endpoints, the `Gbif` configuration, the migration, the map library and the Lithuania rectangle), `docs/architecture.md` and check the OpenAPI document
- [ ] T026 Review `git diff` and open the Pull Request with screenshots and the separate AI review evidence
