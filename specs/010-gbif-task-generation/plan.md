# Implementation Plan: Task creation from GBIF observations

**Feature:** `specs/010-gbif-task-generation/spec.md`
**Branch:** `010-gbif-task-generation`
**Status:** draft
**Depends on features:** 003-access-control, 007-class-create-and-delete, 024-page-styling

## Summary

A teacher chooses a point in Lithuania and a season for an own class. The server asks the GBIF Occurrence API for observations of that season and of the opposite season, keeps the suitable ones (VT-011), creates up to 10 tasks of four types, and saves the location, the season, the copy of the observations and the tasks in one transaction. The frontend gets the screen E13 (map or coordinates, season) and a minimal class task list (E12) that the teacher is sent to after success.

## Technical Context

- **Language and version:** C# with .NET 10 (server); TypeScript (frontend).
- **Libraries:** as selected in `docs/architecture.md` (section 1). New frontend library: `leaflet` with `@types/leaflet` (the map of `docs/architecture.md`, used only to choose a location, with the OpenStreetMap attribution). The server uses the built-in `HttpClient`; no GBIF library. The mappings to responses are trivial (a few fields), so AutoMapper is not added for them, as in feature 007.
- **Frontend:** React with Vite, React Router and Tailwind CSS; Leaflet is used directly (no wrapper library).
- **Storage:** PostgreSQL through Entity Framework Core; the schema change is made by the migration `CreateObservationsAndTasksTables`.
- **Testing:** xUnit (unit and integration, PostgreSQL in Testcontainers; GBIF is replaced by a fake client or a fake HTTP handler, never the real service), Vitest and React Testing Library.
- **Target platform:** web application in current Chrome, Firefox, Edge and Safari; screens from 360 px width.
- **Constraints:** HTTPS only (the GBIF address is `https://`); POST uses the existing CSRF protection; the GBIF address is configuration (`Gbif:BaseUrl`), not a secret; no secrets in the repository.

## Structure and Main Components

- Server:
  - `Entities/Season.cs` – enum `Spring`, `Summer`, `Autumn`, `Winter`, and `SeasonExtensions` with `Months()` (VT-012) and `Opposite()`; the season knows its own months.
  - `Entities/TaskType.cs` – enum `RecogniseSpeciesGroup`, `CommonSpecies`, `SeasonalComparison`, `ObservationQuestion`.
  - `Entities/Observation.cs` – the stored copy of an observation (see Data Model Changes).
  - `Entities/ClassTask.cs` – the task (type, difficulty group, question, correct answer, options, observation).
  - `Entities/SchoolClass.cs` – new nullable `Latitude`, `Longitude`, `Season`; `Data/Configurations/SchoolClassConfiguration.cs` – `Season` is a concurrency token.
  - `Data/Configurations/ObservationConfiguration.cs`, `ClassTaskConfiguration.cs`; `Data/ApplicationDbContext.cs` – `DbSet<Observation> Observations`, `DbSet<ClassTask> ClassTasks`.
  - `Integrations/Gbif/GbifOptions.cs` – `BaseUrl`, `TimeoutSeconds` (10).
  - `Integrations/Gbif/IGbifClient.cs`, `GbifClient.cs` – one call: observations of a country, with coordinates, in the months of a season, inside the square around the point; maps the JSON to `GbifOccurrence` and throws `GbifUnavailableException` on timeout, network error or a non-success status.
  - `Integrations/Gbif/GbifOccurrence.cs`, `GbifUnavailableException.cs`.
  - `Services/GeoDistance.cs` – the distance in km between two points (haversine); used by the selector only.
  - `Services/GbifObservationSelector.cs` – keeps occurrences with coordinates and a species name, no farther than 10 km from the point (VT-011); removes duplicates by GBIF key.
  - `Services/GbifTaskFactory.cs` – creates the tasks “Atpažink rūšių grupę”, “Rask dažnos vietinės rūšies požymių” and “Atsakyk į klausimą”; `Services/SeasonalTaskFactory.cs` – creates “Palygink sezoninius stebėjimus”. Task texts only ask to recognise, name and observe from a distance (VT-021).
  - `Services/IGbifTaskService.cs`, `GbifTaskService.cs` – checks the class and the owner, rejects a class that already has GBIF tasks, calls the GBIF client for the chosen and the opposite season (in parallel), selects, checks the minimum of 10, builds the tasks, saves everything in one `SaveChangesAsync`; returns `GbifTaskResult`.
  - `Services/GbifTaskResult.cs`, `Services/GbifTaskOutcome.cs` – `Created`, `NotFound`, `Forbidden`, `AlreadyCreated`, `TooFewObservations`, `GbifUnavailable`.
  - `Services/IClassTaskService.cs`, `ClassTaskService.cs` – lists the tasks of the teacher’s class for E12 (same ownership check).
  - `Controllers/ClassTaskController.cs` – `[Authorize(Policy = PolicyName.Teacher)]`, route `api/classes/{id}/...`; translates the results to status codes.
  - `DTOs/Requests/CreateGbifTasksRequest.cs` – `Latitude`, `Longitude`, `Season`; validates the range of Lithuania (latitude 53.9–56.5, longitude 20.9–26.9) and a defined season.
  - `DTOs/Responses/GbifTasksResponse.cs` (number of tasks created), `ClassTaskResponse.cs`; `ClassResponse` gets `Latitude`, `Longitude`, `Season`.
  - `Services/ServiceCollectionExtensions.cs` – registers the services, binds `GbifOptions`, registers `AddHttpClient<IGbifClient, GbifClient>` with the base address and the 10-second timeout.
  - `appsettings.json` – `Gbif` section.
- Frontend:
  - `types/classTask.ts`; `types/schoolClass.ts` – location and season of the class.
  - `api/classTaskApi.ts` – list tasks, create GBIF tasks (CSRF token as in the other POST calls).
  - `pages/CreateGbifTasksPage.tsx` (E13), `pages/ClassTasksPage.tsx` (E12, minimal).
  - `components/locationMap.tsx` – Leaflet map; a click sets the point; `components/seasonChoice.tsx` – the four seasons as radio buttons with labels.
  - `helpers/validateGbifTasks.ts`, `helpers/gbifTaskMessage.ts`, `helpers/lithuaniaBounds.ts` – the validation (frontend copy for the user only), the texts and the bounds.
  - `app.tsx` – routes `/classes/:id/tasks` and `/classes/:id/tasks/gbif` inside `ProtectedRoute allowedRoles={['Teacher']}`; `pages/ClassDetailsPage.tsx` – link to the class tasks and the location and season when set.

## Server Request Changes

- `POST /api/classes/{id}/gbif-tasks` – body `latitude`, `longitude`, `season`. Responses: 201 (`createdCount`), 400 (errors per field), 401, 403, 404, 409 (`AlreadyCreated`), 422 (`TooFewObservations`), 502 (`GbifUnavailable`).
- `GET /api/classes/{id}/tasks` – the tasks of the teacher’s class: 200, 401, 403, 404.
- `GET /api/classes` and `GET /api/classes/{id}` – the response also contains `latitude`, `longitude`, `season` (empty until GBIF tasks are created).

## Data Model Changes

- `Classes`: new nullable columns `Latitude`, `Longitude`, `Season`.
- New table `Observations`: `Id`, `ClassId` (cascade delete), `GbifKey`, `Species`, `GbifClass`, `Family`, `Latitude`, `Longitude`, `Locality` (nullable), `ObservedOn`, `DatasetName`, `License`, `IucnCategory` (nullable), `Season`; unique index on (`ClassId`, `GbifKey`).
- New table `ClassTasks`: `Id`, `ClassId` (cascade delete), `Type`, `DifficultyGroup` (1–3), `Question`, `CorrectAnswer`, `Options` (nullable list), `ObservationId` (nullable foreign key to `Observations`).
- New migration `CreateObservationsAndTasksTables` in the same branch.

## Testing Plan

- Unit tests (`SeasonExtensions`): the months of every season (winter December–February); the opposite season of each season.
- Unit tests (`GeoDistance`, `GbifObservationSelector`): a point 100 m and 9.9 km away is kept, 10.1 km is removed; an occurrence without coordinates or species name is removed; duplicates are removed.
- Unit tests (`GbifTaskFactory`, `SeasonalTaskFactory`): at most 10 tasks; the four types and the groups of VT-013; the recognition task has 4 options with one correct; the common-species answer is the species with the most observations; the card answer is the month of the observation; the seasonal task is skipped when the opposite season has fewer than 10 observations; no task text contains words that ask to collect or touch organisms.
- Unit tests (`GbifTaskService`, fake `IGbifClient`): success saves location, season, observations and tasks; fewer than 10 observations saves nothing; GBIF failure saves nothing; a second creation gives `AlreadyCreated`; another teacher gives `Forbidden`; a missing class gives `NotFound`; a protected species keeps its IUCN category; a concurrency conflict gives `AlreadyCreated`.
- Unit tests (`GbifClient`, fake HTTP handler): the request has the country, the coordinates filter, the months and the square; the response is mapped; a timeout, a network error and a 500 give `GbifUnavailableException`.
- Unit tests (`CreateGbifTasksRequest`): a point outside Lithuania, an undefined season, and values at the borders.
- Integration tests (GBIF replaced by a fake client): a teacher gets 201 and the tasks and observations are in the database; fewer than 10 observations gives 422 and nothing is saved; a GBIF error gives 502 and nothing is saved; a second request gives 409; two requests at the same time give one 201 and one 409; an invalid point or season gives 400 with errors per field; a student gets 403, another teacher 403, a visitor 401, a missing class 404 (also for the task list).
- Frontend tests: the E13 form shows the four validation errors; the loading text; the GBIF error with “Bandyti dar kartą”, which sends the request again; the too-few-observations message; success opens E12 with „Užduotys sukurtos.“; E12 shows the empty state and the list, and hides “Sukurti iš GBIF” when the class has GBIF tasks; the coordinates can be entered without the map; a student opening the pages sees “Prieiga uždrausta”; a 401 opens the login page with the session message.

## Technical Notes

- The owner of the class is taken from the session, never from the request. The location and the season are saved only after GBIF tasks are created, so a class without them has no GBIF tasks (this is how a second creation is detected).
- The two GBIF calls (chosen and opposite season) run in parallel; each has the 10-second timeout, so the whole request stays below the 30 seconds of NFR-016.
- GBIF has no search by distance in kilometres, so the client asks for the square around the point and the server removes what is farther than 10 km. The request parameters are checked against the GBIF Occurrence API documentation when the client tests are written.
- Winter spans two calendar years; the months of a season are sent as a list, not a range.
- The concurrency conflict of two requests for one class is detected by making `Season` of the class a concurrency token: both requests read the class with an empty season, the second update finds the season already set, fails, and the service returns `AlreadyCreated`. This works in PostgreSQL and in the in-memory test provider, and needs no extra column.
- Lithuania is checked as a rectangle (latitude 53.9–56.5, longitude 20.9–26.9); the server is the authority, the frontend repeats the check only for the message. The rectangle is an approximation of the territory; it is stated in the README.
- GBIF failures are logged with `ILogger` without the request body; the teacher gets a general message, never the technical details.
- The hiding of coordinates of protected species for students (VT-020) is not done here; the stored IUCN category is what feature 013 needs.

## Constitution Check

- **1. Code quality:** complies. Each class has one responsibility (client, selector, two factories, service) and stays below 200 lines; React file names follow the naming rules.
- **2. Architecture:** complies. The controller only calls the services; the rules are in the services; `ApplicationDbContext` is used directly (no Repository); the GBIF client is injected; DTOs are used at the API boundary; the schema change has a migration.
- **3. Testing:** complies. FR-025–FR-030, FR-036, FR-047, NFR-016, NFR-020, NFR-021 and the feature requirements are covered by the tests in the testing plan, including the authorization tests; tests do not call the real GBIF.
- **4. Security:** complies. Role and ownership are checked on the server; the location and the season are validated on the server; the GBIF address is configuration and HTTPS; errors do not show technical details.
- **5. Version control:** complies. The work is done in branch `010-gbif-task-generation`, created from an updated `main`, and enters through a Pull Request.
- **6. Documentation:** complies. The specification is written before the code; README (the `Gbif` configuration) and `docs/` are updated.
- **7. AI assistance:** complies. The code is reviewed and tested by the student; the Pull Request gets a separate AI review.
- **8. Simplicity:** complies. One new library (the map of the architecture), no AutoMapper for trivial mappings, no extra layers.

**No exceptions. The plan complies with the constitution.**
