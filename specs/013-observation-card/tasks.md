# Tasks: Observation card and protected species

**Specification:** `specs/013-observation-card/spec.md`
**Plan:** `specs/013-observation-card/plan.md`
**Depends on features:** 003-access-control, 008-join-class, 010-gbif-task-generation, 011-teacher-tasks, 012-student-task-list, 024-page-styling
**Tags:** `[US1]` = User Story 1 (observation card and protected species)

## Data

- [ ] T001 There is no schema change and no migration; the observation data of feature 010 are read – no file

## Tests (written first)

- [ ] T002 [US1] Unit tests of `ProtectedSpecies`: VU, EN and CR are protected; other categories and an empty category are not – `backend/tests/Project.UnitTests/ObservationCards/ProtectedSpeciesTests.cs`
- [ ] T003 [US1] Unit tests of `ObservationCardExtensions`: the label, the dataset name, the licence and the link built from the GBIF key; the place name and the coordinates of a species that is not protected; the place name, no coordinates and the hidden flag of a protected species; no species name and no observation date – `backend/tests/Project.UnitTests/ObservationCards/ObservationCardExtensionsTests.cs`
- [ ] T004 [US1] Unit tests of `StudentTaskDetailsService`: a GBIF task returns the question, the options and the card; a teacher task returns the title, the question and no card; the correct answer is never returned; a task of a locked group gives `Locked`; a group after an empty group is unlocked; a task of another class, a missing task, a student without a class and a student whose membership ended give `NotFound`; a student who changed the class cannot open the tasks of the old class – `backend/tests/Project.UnitTests/ObservationCards/StudentTaskDetailsServiceTests.cs`
- [ ] T005 [US1] Integration tests of `GET /api/my/tasks/{id}`: a student gets 200 with the card; the JSON of a protected species has no coordinates; the JSON has no correct answer, species name or observation date; a locked task 403; a task of another class and a missing task 404; a teacher 403; an administrator 403; a visitor 401; a deleted task 404 – `backend/tests/Project.IntegrationTests/ObservationCards/StudentTaskDetailsEndpointTests.cs`
- [ ] T006 [US1] Frontend tests of E06: the warning, the question, the options and the card with the label, the dataset, the licence and the link; the protected species note and no coordinates; a teacher task with the title and no card; 404 “Puslapis nerastas”; 403 “Prieiga uždrausta”; the expired session; the error state; the route `/tasks/{id}` for a student and a teacher – `frontend/src/tests/StudentTaskPage.test.tsx`
- [ ] T007 [US1] Update the tests of the list so that `StudentTaskService` still passes after the lookup of the current class moves to `CurrentClassLookup` – `backend/tests/Project.UnitTests/StudentTasks/StudentTaskServiceTests.cs` (only if a test changes)

## Implementation (depends on: Tests)

- [ ] T008 [US1] Create `ProtectedSpecies` (rule VT-020) – `backend/src/Project.Api/Services/ProtectedSpecies.cs`
- [ ] T009 [US1] Create the response DTOs `ObservationCardResponse` and `StudentTaskDetailsResponse` – `backend/src/Project.Api/DTOs/Responses/`
- [ ] T010 [US1] Create `ObservationCardExtensions` (`ToCard()`) and add `ToDetails()` to `ClassTaskExtensions` – `backend/src/Project.Api/Services/ObservationCardExtensions.cs`, `backend/src/Project.Api/Services/ClassTaskExtensions.cs`
- [ ] T011 [US1] Create `CurrentClassLookup` and use it in `StudentTaskService` – `backend/src/Project.Api/Services/CurrentClassLookup.cs`, `backend/src/Project.Api/Services/StudentTaskService.cs`
- [ ] T012 [US1] Create `StudentTaskDetailsOutcome`, `StudentTaskDetailsResult`, `IStudentTaskDetailsService` and `StudentTaskDetailsService` and register the service – `backend/src/Project.Api/Services/`
- [ ] T013 [US1] Add the action `GET api/my/tasks/{id:guid}` – `backend/src/Project.Api/Controllers/StudentTaskController.cs`
- [ ] T014 [US1] Add the types and the API call `getMyTask` – `frontend/src/types/studentTaskDetails.ts`, `frontend/src/api/studentTaskApi.ts`
- [ ] T015 [US1] Add the texts of E06 – `frontend/src/helpers/taskScreenMessage.ts`
- [ ] T016 [US1] Build the component of the card – `frontend/src/components/observationCard.tsx`
- [ ] T017 [US1] Build `StudentTaskPage` (E06) and add the route `/tasks/:id` – `frontend/src/pages/StudentTaskPage.tsx`, `frontend/src/app.tsx`

## Verification

- [ ] T018 Run `dotnet build backend/Project.slnx`, `dotnet test backend/Project.slnx`, `npm run lint`, `npm run format:check` (only on the changed files), `npm test` and `npm run build`
- [ ] T019 Open E06 in the browser for a GBIF task, a protected species, a teacher task, a locked task and a missing task and take the screenshots
- [ ] T020 Update README (the endpoint and the task screen) and `docs/architecture.md` (the endpoint) and check the OpenAPI document
- [ ] T021 Review `git diff` and open the Pull Request with screenshots and the separate AI review evidence
