# Feature Specification: Observation card and protected species

**Feature folder:** `specs/013-observation-card`
**Status:** draft
**Related project requirements:** FR-038 (opening a task; the answer is submitted by feature 014), FR-043, FR-045, FR-046; NFR-027; VT-019, VT-020
**Related screens:** E06 Task, E04 Task list, E20 Page not found (`docs/ux.md`)

**Why this feature is needed:** “I want to see an observation card with a clear label and a warning so that I know where the data come from and how to observe nature safely.” (user story 013).

## User Scenarios and Acceptance Scenarios

### User Story 1 – Observation card and protected species (Priority: P1)

**User story:** As a student, I want to see an observation card with a clear label and a warning so that I know where the data come from and how to observe nature safely.

**Independent verification:** The card shows the label, licence, link and warning, and the coordinates of a protected species are not shown.

**Acceptance scenarios:**

1. **GBIF task:** **Given** a student and an unlocked GBIF task of the student’s class, **when** the student opens the task (E06), **then** the screen shows the warning, the question, the answer options (when the task has them) and the observation card with the label “Patvirtintas GBIF stebėjimas”, the dataset name, the licence and a link to the GBIF record (FR-043, FR-045, VT-019).
2. **Warning:** **Given** any task, **when** the student opens it, **then** the warning „Stebėkite gamtą iš atstumo, nerinkite ir netrikdykite organizmų, nefotografuokite žmonių.“ is shown (FR-045).
3. **Observation not protected:** **Given** a task whose observation has no IUCN Red List category of VU, EN or CR, **when** the student opens the task, **then** the card shows the place name and the coordinates.
4. **Protected species:** **Given** a task whose observation has the IUCN category VU, EN or CR, **when** the student opens the task, **then** the card shows only the place name and the note „Tikslios koordinatės paslėptos, nes rūšis saugoma.“, and the server response does not contain the coordinates (FR-046, VT-020).
5. **Teacher task:** **Given** a teacher task, **when** the student opens it, **then** the screen shows the warning, the title and the question with the label “Mokytojo užduotis”, no observation card is shown and the task is never shown as a GBIF observation (VT-019).
6. **No answer in the response:** **Given** any task, **when** the student opens it, **then** the server never returns the correct answer, and the card does not show the species name or the observation date.
7. **Link from the list:** **Given** the task list (E04), **when** the student presses “Tęsti”, **then** the task screen of the first unfinished unlocked task opens.
8. **Locked task:** **Given** a task in a locked group of the student’s class, **when** the student opens its address, **then** the server rejects it with 403 and the screen shows “Prieiga uždrausta”.
9. **Task not found:** **Given** a task that does not exist, was deleted or belongs to another class, **when** the student opens its address, **then** the server returns 404 and the screen shows “Puslapis nerastas” with the button “Į pradžią” (NFR-027).
10. **Teacher and administrator:** **Given** a logged-in teacher or administrator, **when** the user calls the endpoint of this feature, **then** the server rejects it with 403 and the screen shows “Prieiga uždrausta”.
11. **Not logged in:** **Given** a visitor, **when** the endpoint is called, **then** the server returns 401.
12. **Server failure:** **Given** the task screen, **when** the server fails, **then** the error state with the button “Bandyti dar kartą” is shown.

## Edge Cases

- When the session has expired, the task screen opens the login page with the message „Sesija baigėsi dėl neveiklumo. Prisijunkite iš naujo.“ (NFR-004).
- A student who changed the class (VT-025) cannot open the tasks of the old class (404).
- A task the teacher deleted (feature 011) while the student had it open shows “Puslapis nerastas”.
- A seasonal comparison task and a “month” question use the observation card too; the card never shows the species name or the observation date, because they would reveal the answer.
- When GBIF gave no IUCN category, the species is not treated as protected.
- When the place name of a protected species is not known, only the note about the hidden coordinates is shown.
- The task screen has no answer form: the answer and the evidence are submitted with feature 014.

## Requirements

### Functional Requirements

- **FR-043:** The system MUST show on each observation card the label “Patvirtintas GBIF stebėjimas” (Confirmed GBIF observation), the dataset name, the licence and a link to the GBIF record.
- **FR-045:** The system MUST show on each task a warning to observe nature from a distance, not to collect or disturb organisms and not to photograph people.
- **FR-046:** The system MUST NOT show a student the exact coordinates of an observation of a protected species (VT-020).
- **FR-001:** The server MUST provide `GET /api/my/tasks/{id}` for a student; it returns the task (identifier, type, difficulty group, label, title of a teacher task, question, answer options) and, for a GBIF task, the observation card.
- **FR-002:** The server MUST allow only a student to call the endpoint (a teacher and an administrator get 403, a visitor 401). The class is taken from the student’s membership in the session, never from the request.
- **FR-003:** The server MUST return 404 when the task does not exist or does not belong to the student’s current class, and 403 when the task belongs to a locked group (VT-014).
- **FR-004:** The server MUST NOT return the correct answer of a task, the species name or the observation date of the card.
- **FR-005:** The server MUST NOT include the latitude and the longitude in the response when the species is protected; the decision is made on the server, not in the browser.

### Key Entities

- **Observation (read):** the stored GBIF observation (dataset name, licence, GBIF key, place name, coordinates, IUCN category) of feature 010.
- **ClassTask (read):** the class tasks of features 010 and 011.
- **ClassMembership (read):** the current class of the student (feature 008).

### Validation Rules

- **VT-019:** A confirmed external observation is only an observation provided by the GBIF Occurrence API. Student-submitted data are always labelled as student-submitted and never become confirmed observations. Tasks created by a teacher are not GBIF observations and are labelled as teacher tasks.
- **VT-020:** A species is considered protected if GBIF gives it an IUCN Red List category of VU, EN or CR. The exact coordinates of such a species are not shown to a student; only the place name is shown.

## Success Criteria

- **SC-001:** Requirements FR-043, FR-046 and FR-001–FR-005 and VT-019, VT-020 are verified by backend unit and integration tests (the label, the dataset, the licence and the link are present; the coordinates of a protected species are not in the response; no correct answer, species name or observation date; a locked task 403; another class’s or a missing task 404; a teacher and an administrator 403; a visitor 401).
- **SC-002:** Requirements FR-045 and NFR-027 and the screen E06 are verified by frontend tests (the warning, the card, the protected species note, the teacher task, the not found page, the error state).

## Clarifications

- Q: What does the observation card show? → A: Only what `docs/` lists: the label, the dataset name, the licence, the link to the GBIF record and, for a protected species, the place name and the note about the hidden coordinates. For a species that is not protected the place name and the coordinates are shown. The species name and the observation date are not shown, because they would reveal the answer of the common-species and the month tasks; where a task needs the species, its name is already in the question. Decision of the student.
- Q: What does the server return when the task is locked, belongs to another class or does not exist? → A: A task that does not exist or belongs to another class: 404 (“Puslapis nerastas”). A task of the student’s class in a locked group: 403 (“Prieiga uždrausta”). A teacher and an administrator: 403, a visitor: 401. `docs/` says only that the server checks the class and the unlocked state. Decision of the student.
- Q: What does the task screen show in this feature? → A: The warning, the question, the answer options (when the task has them), the label and the card (a teacher task: the title and the question). The correct answer is never returned. The answer form is added by feature 014. The title of a teacher task is shown on this screen (decision left open in feature 012). Decision of the student.
- Q: What is the text of the warning? → A: „Stebėkite gamtą iš atstumo, nerinkite ir netrikdykite organizmų, nefotografuokite žmonių.“ `docs/` gives the meaning (FR-045) but not the exact text. Decision of the student.
- Q: Where is the opening of a task (FR-038) verified? → A: The task screen and the check “unlocked task of the student’s own class” (`docs/ux.md`, E06) are made in this feature; `docs/architecture.md` assigns FR-038 to feature 014, so FR-038 is verified together with feature 014 when the answer is submitted.

No unresolved clarification markers remain.

## Assumptions

- The assumptions and constraints of `docs/requirements.md` (section 11) apply.
- The link to the GBIF record is built from the stored GBIF key of the observation and opens in a new tab.
- A group is unlocked by the rule of feature 012 until features 014–016 add points.

## Dependencies

- Features: 003-access-control (role policies, error pages), 008-join-class (membership), 010-gbif-task-generation (Observation, ClassTask), 011-teacher-tasks (teacher task and label), 012-student-task-list (the list, “Tęsti”, the unlocking rule), 024-page-styling (screens).

## Scope Boundaries

**In scope:**
- `GET /api/my/tasks/{id}`, the task screen (E06) at `/tasks/{id}` with the warning, the question, the options, the observation card, the protected species rule and the not found, forbidden, expired session and error states.

**Out of scope:**
- The answer and evidence form, the submission, the status “done” and the task result (014, E07).
- Photos in the evidence (015), points and unlocking by points (016), repeating a task (017).
- Everything that belongs to the other features of the list in `docs/architecture.md` (section 6).
