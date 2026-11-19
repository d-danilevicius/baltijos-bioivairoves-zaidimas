# Implementation Plan: Observation card and protected species

**Feature:** `specs/013-observation-card/spec.md`
**Branch:** `013-observation-card`
**Status:** draft
**Depends on features:** 003-access-control, 008-join-class, 010-gbif-task-generation, 011-teacher-tasks, 012-student-task-list, 024-page-styling

## Summary

A student opens a task (E06) from the task list and sees the warning, the question, the answer options and, for a GBIF task, the observation card with the label “Patvirtintas GBIF stebėjimas”, the dataset name, the licence and a link to the GBIF record. For a protected species (IUCN category VU, EN or CR) the card shows only the place name and the note about the hidden coordinates; the server does not send the coordinates at all. A teacher task shows the title and the question with the label “Mokytojo užduotis” and no card. The server never returns the correct answer, the species name or the observation date. A task of another class or a missing task gives 404, a task of a locked group gives 403. The answer form is added by feature 014.

## Technical Context

- **Language and version:** C# with .NET 10 (server); TypeScript (frontend).
- **Libraries:** as selected in `docs/architecture.md` (section 1). No new library. The mapping from a task and an observation to the response is trivial and is written as two small methods; AutoMapper is not added for it.
- **Frontend:** React with Vite, React Router and Tailwind CSS.
- **Storage:** PostgreSQL through Entity Framework Core; the tables `ClassTasks`, `Observations` and `ClassMemberships` are read; no schema change and no migration.
- **Testing:** xUnit (unit and integration, PostgreSQL in Testcontainers), Vitest and React Testing Library.
- **Target platform:** web application in current Chrome, Firefox, Edge and Safari; screens from 360 px width.
- **Constraints:** HTTPS only; the endpoint only reads; no secret is stored in the repository.

## Structure and Main Components

- Server:
  - `DTOs/Responses/ObservationCardResponse.cs` – the card: the label, the dataset name, the licence, the link to the GBIF record, the place name, the coordinates (empty for a protected species) and whether the coordinates are hidden. It has no species name and no observation date (FR-004).
  - `DTOs/Responses/StudentTaskDetailsResponse.cs` – identifier, type, difficulty group, label, title (a teacher task), question, answer options and the card (empty for a teacher task). It has no correct answer.
  - `Services/ProtectedSpecies.cs` – the rule VT-020: the IUCN category VU, EN or CR means a protected species (one place for the rule).
  - `Services/ObservationCardExtensions.cs` – `ToCard()` turns an `Observation` into the card; for a protected species it leaves out the coordinates; the GBIF link is built from the GBIF key.
  - `Services/ClassTaskExtensions.cs` – a new method `ToDetails()` builds the task response from a task and its card (the label comes from `ToLabel()` of feature 012).
  - `Services/CurrentClassLookup.cs` – the current class of a student (membership without an end date) found in one place; `StudentTaskService` of feature 012 is changed to use it (DRY: the same lookup is needed here).
  - `Services/IStudentTaskDetailsService.cs`, `Services/StudentTaskDetailsService.cs` – finds the current class, loads the task of that class and its observation, decides whether the task’s group is unlocked with `TaskGroupUnlocking` and builds the response.
  - `Services/StudentTaskDetailsOutcome.cs`, `Services/StudentTaskDetailsResult.cs` – the outcomes `Found`, `NotFound` and `Locked`, as in the other services.
  - `Services/ServiceCollectionExtensions.cs` – registers `IStudentTaskDetailsService`.
  - `Controllers/StudentTaskController.cs` – a new action `GET api/my/tasks/{id:guid}` (policy `Student`): 200, 403 for a locked task, 404 otherwise.
- Frontend:
  - `types/studentTaskDetails.ts` – `StudentTaskDetails` and `ObservationCard`.
  - `api/studentTaskApi.ts` – a new call `getMyTask(id)` with a result type per status (200, 401, 403, 404, failure).
  - `helpers/taskScreenMessage.ts` – the texts of E06: the warning, the captions of the card, the note about the hidden coordinates.
  - `components/observationCard.tsx` – the card: the label, the dataset, the licence, the link (opens in a new tab), the place name and the coordinates or the note.
  - `pages/StudentTaskPage.tsx` – E06: loading, error with “Bandyti dar kartą”, session expiry, access denied, not found, and the task (title or type, label, warning, question, options, card).
  - `app.tsx` – route `/tasks/:id` inside `ProtectedRoute allowedRoles={['Student']}`.

## Server Request Changes

- `GET /api/my/tasks/{id}` – returns one task of the student’s current class (200 with the task and the card; 401; 403 for a teacher, an administrator or a task of a locked group; 404 when the task does not exist or belongs to another class).

## Data Model Changes

- None. No migration. The observation data (dataset name, licence, GBIF key, place name, coordinates, IUCN category) were stored by feature 010.

## Testing Plan

- Unit tests (`ProtectedSpecies`): VU, EN and CR are protected; other categories and an empty category are not.
- Unit tests (`ObservationCardExtensions`): the card has the label, the dataset name, the licence and the link built from the GBIF key; a species that is not protected has the place name and the coordinates; a protected species has the place name and no coordinates and the hidden flag; the card has no species name and no observation date.
- Unit tests (`StudentTaskDetailsService`): a GBIF task of the student’s class returns the question, the options and the card; a teacher task returns the title, the question and no card; the correct answer is never in the response; a task of a locked group gives `Locked`; a task in group 2 is unlocked when group 1 has no tasks; a task of another class, a missing task, a student without a class and a student whose membership ended give `NotFound`; a student who changed the class cannot open the tasks of the old class.
- Integration tests: a student gets 200 with the card; the JSON of a protected species does not contain the coordinates; the JSON has no correct answer, species name or observation date; a locked task gives 403; a task of another class and a missing task give 404; a teacher gives 403; an administrator gives 403; a visitor gives 401; a task the teacher deleted gives 404.
- Frontend tests: E06 shows the warning, the question, the options and the card with the label, the dataset, the licence and the link; a protected species shows the place name and the note and no coordinates; a teacher task shows the title and no card; 404 shows “Puslapis nerastas”; 403 shows “Prieiga uždrausta”; the expired session opens the login; the error state shows “Bandyti dar kartą”; the route `/tasks/{id}` opens E06 for a student and “Prieiga uždrausta” for a teacher.

## Technical Notes

- Whether the coordinates are sent is decided on the server (`ObservationCardExtensions`); the browser only shows what it receives (FR-005).
- The response has no correct answer, species name or observation date, so the card cannot reveal the answer of a common-species or a month task (Clarifications).
- A task is unlocked by the same rule as in the list (`TaskGroupUnlocking`); feature 016 changes the rule in one place.
- The class comes from the membership of the logged-in student, never from the request; a task of another class is “not found”, not “forbidden”, so the existence of other classes’ tasks is not revealed.
- The address of the GBIF record is `https://www.gbif.org/occurrence/{GbifKey}`; the public address of the GBIF site is a constant, not an environment setting.
- `docs/` gives the label, the dataset, the licence, the link and the note; the other captions of the card (“Duomenų rinkinys”, “Licencija”, “GBIF įrašas”, “Vieta”, “Koordinatės”) are chosen in this feature.
- The answer options are shown as a list without controls; choosing an answer is added by feature 014.
- A non-GUID address `/tasks/abc` does not match the route and the screen shows “Puslapis nerastas”.

## Constitution Check

- **1. Code quality:** complies. Each class has one responsibility and stays below 200 lines; the protected species rule and the current class lookup exist in one place; React file names follow the naming rules.
- **2. Architecture:** complies. The controller only calls the service; the rules are in `StudentTaskDetailsService`, `ProtectedSpecies` and `TaskGroupUnlocking`; `ApplicationDbContext` is used directly (no Repository); DTOs are used at the API boundary and entities are not returned.
- **3. Testing:** complies. FR-043, FR-045, FR-046, FR-001–FR-005, NFR-027, VT-019 and VT-020 are covered by the tests in the testing plan, including the authorization tests.
- **4. Security:** complies. The role and the class are checked on the server; the coordinates of a protected species and the correct answer are never sent; no secret is stored in Git.
- **5. Version control:** complies. The work is done in branch `013-observation-card`, created from an updated `main`, and enters through a Pull Request.
- **6. Documentation:** complies. The specification is written before the code; README and `docs/` are updated.
- **7. AI assistance:** complies. The code is reviewed and tested by the student; the Pull Request gets a separate AI review.
- **8. Simplicity:** complies. No new libraries and no schema change; one new service, one rule class, one component and one page.

**No exceptions. The plan complies with the constitution.**
