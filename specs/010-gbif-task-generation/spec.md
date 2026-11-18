# Feature Specification: Task creation from GBIF observations

**Feature folder:** `specs/010-gbif-task-generation`
**Status:** draft
**Related project requirements:** FR-025, FR-026, FR-027, FR-028, FR-029, FR-030, FR-036, FR-047; NFR-002, NFR-016, NFR-020, NFR-021; VT-010, VT-011, VT-012, VT-013, VT-021
**Related screens:** E13 Task creation from GBIF, E12 Class tasks (`docs/ux.md`)

**Why this feature is needed:** “I want to choose a location and a season so that the class gets tasks from real GBIF observations near the school.” (user story 1).

## User Scenarios and Acceptance Scenarios

### User Story 1 – Task creation from GBIF observations (Priority: P1)

**User story:** As a teacher, I want to choose a location and a season so that the class gets tasks from real GBIF observations near the school.

**Independent verification:** With a fake GBIF response tasks of four types are created.

**Acceptance scenarios:**

1. **Successful creation:** **Given** a logged-in teacher who owns a class without GBIF tasks, and GBIF returns at least 10 suitable observations, **when** the teacher chooses a point in Lithuania and a season and presses “Gauti stebėjimus ir sukurti užduotis”, **then** up to 10 tasks of the four types are created, the location and the season are saved for the class, a copy of the observations used is stored, and the class tasks screen (E12) opens with the message „Užduotys sukurtos.“
2. **Four task types:** **Given** enough observations, **when** the tasks are created, **then** the class gets “Atpažink rūšių grupę”, “Rask dažnos vietinės rūšies požymių”, “Palygink sezoninius stebėjimus” and “Atsakyk į klausimą” tasks, each in its difficulty group (VT-013).
3. **Too few observations:** **Given** a teacher, and GBIF returns fewer than 10 suitable observations, **when** the teacher tries to create tasks, **then** no task is created and the message „Šioje vietovėje šiuo metų laiku rasta per mažai stebėjimų. Pasirinkite kitą vietovę ar metų laiką.“ is shown (FR-036).
4. **GBIF error or timeout:** **Given** a teacher, and GBIF does not respond within 10 seconds or returns an error, **when** the teacher tries to create tasks, **then** the message „Nepavyko gauti stebėjimų iš GBIF. Bandykite dar kartą.“ and the button “Bandyti dar kartą” are shown, and no incomplete tasks are created (NFR-020).
5. **Loading:** **Given** the request is running, **then** „Kraunami stebėjimai iš GBIF…“ is shown.
6. **Validation:** **Given** the form, **when** the location is not chosen, the coordinates are not numbers, the point is outside Lithuania, or the season is not chosen, **then** nothing is created and the errors „Pasirinkite vietovę.“, „Platuma ir ilguma turi būti skaičiai.“, „Pasirinkite vietovę Lietuvos teritorijoje.“ or „Pasirinkite metų laiką.“ are shown next to the fields (VT-010, VT-012). The same checks are made on the server.
7. **Second creation:** **Given** a class that already has GBIF tasks, **when** the teacher opens the class tasks screen, **then** the button “Sukurti iš GBIF” is not shown, and the server rejects a second creation request (VT-010).
8. **Student:** **Given** a logged-in student, **when** the student calls the endpoint, **then** the server rejects it with 403 and the screens show “Prieiga uždrausta”.
9. **Other teacher:** **Given** a class of teacher A, **when** teacher B tries to create GBIF tasks for it, **then** the server rejects it with 403; **when** the class does not exist, **then** the server returns 404.
10. **Not logged in:** **Given** a visitor, **when** the endpoint is called, **then** the server returns 401.

## Edge Cases

- The location can be chosen on the map or entered as latitude and longitude, so the form can be used without a mouse (`docs/ux.md`, accessibility).
- Only observations that have coordinates and a species name and are no farther than 10 km from the chosen point are used (VT-011); the distance is checked on the server.
- The 10 km radius is fixed; the teacher cannot change it.
- A season is defined by months (VT-012): spring – March to May, summer – June to August, autumn – September to November, winter – December to February.
- The “other season” for the seasonal comparison is the opposite season: spring ↔ autumn, summer ↔ winter.
- If the other season has too few observations, the seasonal comparison task is skipped and the other tasks are still created.
- A task type that cannot be created from the available observations (for example, fewer than 4 species groups for recognition) is skipped; tasks are created only if at least 10 suitable observations are found.
- Observations of protected species (IUCN category VU, EN or CR) are stored with that mark; the hiding of their coordinates for students belongs to feature 013 (VT-020).
- No task text asks the student to collect, catch, touch or disturb organisms; the tasks ask only to recognise, name and observe from a distance (FR-047, VT-021).
- A request that fails in the middle (database error) creates nothing: either all tasks and observations are saved or none (NFR-020).
- Two requests for the same class at the same time: only one succeeds, the other is rejected as a second creation.
- When the session has expired, the screens open the login page with the message „Sesija baigėsi dėl neveiklumo. Prisijunkite iš naujo.“ (NFR-004).
- When the class was deleted in another browser tab, the screen shows „Puslapis nerastas“.

## Requirements

### Functional Requirements

- **FR-025:** The system MUST allow a teacher to choose a location in Lithuania and a season for own class.
- **FR-026:** The system MUST retrieve from the GBIF Occurrence API observations near the chosen location in the chosen season according to VT-011.
- **FR-027:** The system MUST create the task “Atpažink rūšių grupę” from a retrieved observation: the student chooses the species group (for example birds, mammals, insects, plants) from 4 options; the options come from the GBIF class of the retrieved observations.
- **FR-028:** The system MUST create the task “Rask dažnos vietinės rūšies požymių” from the retrieved observations: the question is “Kuri rūšis šioje vietoje pastebėta dažniausiai?”, and the correct answer is the species with the most observations.
- **FR-029:** The system MUST create the task “Palygink sezoninius stebėjimus” from observations of the same location in the chosen season and the opposite season.
- **FR-030:** The system MUST create the task “Atsakyk į klausimą” from an observation card: the question is in which month the observation was made, and the answer comes from the observation date.
- **FR-036:** The system MUST NOT create tasks and MUST show the teacher a message if GBIF returns fewer observations than VT-011 requires.
- **FR-047:** The system MUST NOT create tasks that require collecting, touching or disturbing organisms (VT-021).
- **FR-001:** The server MUST provide `POST /api/classes/{id}/gbif-tasks` with the request body `latitude`, `longitude` and `season`; the owner of the class is taken from the session, never from the request.
- **FR-002:** The server MUST allow only the owner of the class to create GBIF tasks; a student, another teacher and a visitor are rejected (403, 403 and 401); a class that does not exist returns 404.
- **FR-003:** The server MUST validate the location and the season and return the errors per field; the request uses the existing CSRF protection.
- **FR-004:** The server MUST create the tasks of one request in one transaction, so no incomplete tasks stay after a failure.
- **NFR-002:** All communication with GBIF is over HTTPS.
- **NFR-016:** The tasks are created within no more than 30 seconds after the location and the season are confirmed.
- **NFR-020:** If GBIF does not respond within 10 seconds or returns an error, the teacher sees an error message with the button “Bandyti dar kartą” and no incomplete tasks are created.
- **NFR-021:** The system MUST store a copy of the GBIF observations used for tasks, so that the tasks do not change when GBIF data change.

### Key Entities

- **Class (extended):** the location (latitude, longitude) and the season of the class; empty until GBIF tasks are created.
- **Observation:** a stored copy of a GBIF observation: GBIF record identifier, species name, GBIF class and family, coordinates, place name, observation date, dataset name, licence, link to the GBIF record, IUCN category, class it belongs to.
- **ClassTask:** a task of a class: type, difficulty group, label “GBIF”, question, correct answer, the observation it is based on, the class it belongs to.

### Validation Rules

- **VT-010:** The GBIF tasks of one class are created for one location within the territory of Lithuania and one season, both chosen by the teacher of the class.
- **VT-011:** Only GBIF observations that have coordinates and a species name and are no farther than 10 km from the chosen location are used. Tasks are created only if at least 10 such observations are found.
- **VT-012:** Seasons are defined by months: spring – March to May, summer – June to August, autumn – September to November, winter – December to February.
- **VT-013:** There are four task types and three difficulty groups: group 1 – species group recognition and a question from an observation card; group 2 – features of a common local species; group 3 – comparison of seasonal observations.
- **VT-021:** No task may require collecting, catching, touching or disturbing organisms; only observation from a distance is allowed.

## Success Criteria

- **SC-001:** Requirements FR-026–FR-030, FR-036, FR-047 and NFR-020, NFR-021 are verified by backend unit tests (observation selection, seasons, task creation) and by integration tests with a fake GBIF response (success, fewer than 10 observations, GBIF error and timeout, protected species marked, second creation rejected, nothing saved after a failure).
- **SC-002:** The access rules (student 403, another teacher 403, visitor 401, class not found 404) are verified by integration tests.
- **SC-003:** FR-025 and the screen behaviour are verified by frontend tests of the E13 screen (validation, loading, GBIF error with “Bandyti dar kartą”, too few observations, success) and by screenshots in the Pull Request.

## Clarifications

- Q: How many tasks are created? → A: Up to 10 tasks in total, divided between the four task types as evenly as possible; the plan fixes the exact split.
- Q: How are the answer options of “Atpažink rūšių grupę” made? → A: From the GBIF class (for example birds, mammals, insects); one correct group and three others taken from observations of the same location.
- Q: What does the card question ask? → A: In which month the observation was made.
- Q: How are “features” of a common local species asked, if GBIF has no features? → A: The question is which species is observed most often at the location.
- Q: Which season is compared? → A: The opposite one (spring ↔ autumn, summer ↔ winter); if there are too few observations in it, the task is skipped.
- Q: How does the teacher choose the location? → A: A point on the map (Leaflet and OpenStreetMap) or coordinates; the 10 km radius is fixed and observations at any distance up to 10 km are used.
- Q: Can GBIF tasks of a class be created a second time? → A: No (VT-010); the button is hidden and the server rejects the request.
- Q: Which species is protected? → A: A species with the IUCN category VU, EN or CR (VT-020); here it is only marked, the hiding of coordinates is in feature 013.
- Q: What does “too few observations” mean for the opposite season? → A: Not fixed in `docs/`; the same minimum of 10 observations as in VT-011 is assumed (see Assumptions).

No unresolved clarification markers remain.

## Assumptions

- The assumptions and constraints of `docs/requirements.md` (section 11) apply.
- The question of “Palygink sezoninius stebėjimus” compares the number of observations of one species in the chosen and the opposite season; the exact text is fixed in the plan. This is an assumption, because `docs/` only says that observations of two seasons are compared.
- The opposite season needs at least 10 suitable observations, as VT-011 requires for the chosen one. This is an assumption.
- The class tasks screen (E12) is added here only as far as needed to show the created tasks (type, group, label); own tasks and deleting a task belong to feature 011.
- The display of the observation card, the label and the hiding of protected coordinates belong to feature 013; this feature only stores the data they need.

## Dependencies

- Features: 003-access-control (role policies), 007-class-create-and-delete (class and ownership), 024-page-styling (screens).
- External: GBIF Occurrence API (https://techdocs.gbif.org/en/openapi/v1/occurrence) and OpenStreetMap map tiles.

## Scope Boundaries

**In scope:**
- The location and the season of a class, the observation and class task entities, their migration, the endpoint `POST /api/classes/{id}/gbif-tasks`, the GBIF client, the E13 screen, a minimal task list on E12.

**Out of scope:**
- Teacher’s own tasks and deleting tasks (011), the student task list (012), the observation card and hiding the coordinates of protected species (013), answering tasks and points (014–016).
- Changing the location, the season or the 10 km radius of a class, a search of places by name, a check of where the student is.
- Everything that belongs to the other features of the list in `docs/architecture.md` (section 6).
