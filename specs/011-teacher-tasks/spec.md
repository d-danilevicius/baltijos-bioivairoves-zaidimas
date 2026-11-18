# Feature Specification: Teacher tasks

**Feature folder:** `specs/011-teacher-tasks`
**Status:** draft
**Related project requirements:** FR-031, FR-032, FR-033, FR-034, FR-035, FR-053; VT-013, VT-018, VT-021
**Related screens:** E12 Class tasks, E14 Teacher task form, E21 Confirmation dialog (`docs/ux.md`)

**Why this feature is needed:** “I want to create my own task, for example about the school surroundings, so that I can add my own questions to the tasks.” (user story 011).

## User Scenarios and Acceptance Scenarios

### User Story 1 – Teacher tasks (Priority: P1)

**User story:** As a teacher, I want to create my own task, for example about the school surroundings, so that I can add my own questions to the tasks.

**Independent verification:** A saved task is visible in the class list with the label “Mokytojo užduotis” (Teacher task).

**Acceptance scenarios:**

1. **Creation:** **Given** a teacher on the teacher task form (E14) of own class, **when** the teacher fills in the title, the question, the correct answer, chooses a difficulty group, ticks the confirmation and presses “Išsaugoti”, **then** the task is saved with the label “Mokytojo užduotis”, and the class tasks screen (E12) opens with the message „Užduotis išsaugota.“ and shows the new task.
2. **Class task list:** **Given** a teacher who opens the class tasks screen (E12) of own class, **when** the class has tasks, **then** the list shows the type, the difficulty group and the label of each task (FR-035); **when** the class has no tasks, **then** the screen shows an empty state and the buttons to create tasks.
3. **Label:** **Given** a class with GBIF tasks and teacher tasks, **when** the teacher opens E12, **then** a teacher task is labelled “Mokytojo užduotis” and is never shown as a GBIF observation (FR-033).
4. **Field validation:** **Given** the teacher task form, **when** a field is empty or too long, or no difficulty group is chosen, **then** nothing is saved and the error is shown next to the field: „Įveskite užduoties pavadinimą.“, „Pavadinimas negali būti ilgesnis nei 100 simbolių.“, „Įveskite klausimą.“, „Klausimas negali būti ilgesnis nei 500 simbolių.“, „Įveskite teisingą atsakymą.“, „Atsakymas negali būti ilgesnis nei 100 simbolių.“ or „Pasirinkite sudėtingumo grupę.“ Initially no difficulty group is selected.
5. **Confirmation not ticked:** **Given** the teacher task form, **when** the teacher does not tick „Patvirtinu, kad užduotis nereikalauja rinkti, liesti ar trikdyti organizmų.“, **then** the task is not saved and the error „Pažymėkite, kad užduotis nereikalauja rinkti, liesti ar trikdyti organizmų.“ is shown (FR-032, VT-021).
6. **Server failure:** **Given** the teacher task form, **when** the server fails, **then** nothing is saved, the teacher stays on the form and the message „Nepavyko išsaugoti. Bandykite dar kartą.“ is shown.
7. **Deletion:** **Given** E12 of own class, **when** the teacher presses “Ištrinti” next to a teacher task and confirms in the confirmation dialog („Ištrinti užduotį? Mokinių jau gauti taškai išliks.“), **then** the task is deleted and disappears from the list; **when** the teacher cancels, **then** nothing changes. The button “Ištrinti” is shown only next to a task the teacher created.
8. **Student:** **Given** a logged-in student, **when** the student calls any of the task endpoints of this feature, **then** the server rejects it with 403 and the task screens show “Prieiga uždrausta”.
9. **Other teacher:** **Given** a class of teacher A, **when** teacher B lists, creates or deletes its tasks, **then** the server rejects it with 403 and the screen shows “Prieiga uždrausta”; **when** the class or the task does not exist, **then** the server returns 404 and the screen shows “Puslapis nerastas”.
10. **Not logged in:** **Given** a visitor, **when** any task endpoint is called, **then** the server returns 401.

## Edge Cases

- When the session has expired, the task screens open the login page with the message „Sesija baigėsi dėl neveiklumo. Prisijunkite iš naujo.“ (NFR-004).
- When the task was already deleted in another browser tab, the server returns 404 and the screen shows “Puslapis nerastas”.
- A task of another class cannot be deleted through the address of the teacher’s own class: the server returns 404.
- When a task is deleted, the points already earned by students remain (VT-018); points do not exist yet (features 014–016), so this is verified when they are added.
- The correct answer is stored as entered; the comparison of a student’s answer (ignoring letter case and spaces at the beginning and end) belongs to feature 014.
- A teacher may call the delete endpoint with the identifier of a GBIF task: the server returns 409 and nothing is deleted (Clarifications).

## Requirements

### Functional Requirements

- **FR-031:** The system MUST allow a teacher to create own task for own class by providing a title, a question, a correct answer and a difficulty group, for example about the school surroundings (VT-018).
- **FR-032:** The system MUST, before saving a teacher’s task, ask the teacher to confirm that the task does not require collecting, touching or disturbing organisms.
- **FR-033:** The system MUST label tasks created by a teacher “Mokytojo užduotis” (Teacher task) and never show them as GBIF observations.
- **FR-034:** The system MUST allow a teacher to delete a task the teacher created.
- **FR-035:** The system MUST allow a teacher to view the task list of own class, showing the type, difficulty group and label of each task.
- **FR-053:** The system MUST ask the user for confirmation before deleting a task (VT-029).
- **FR-001:** The server MUST provide `POST /api/classes/{id}/tasks` and `DELETE /api/classes/{id}/tasks/{taskId}`; `GET /api/classes/{id}/tasks` (feature 010) also returns the teacher tasks.
- **FR-002:** The server MUST allow only the owner of the class to list, create or delete its tasks; a student, another teacher and a visitor are rejected (403, 403 and 401). The owner is taken from the session, never from the request.
- **FR-003:** The server MUST validate the title, the question, the correct answer, the difficulty group and the confirmation (VT-018, VT-021) and return the errors per field; POST and DELETE use the existing CSRF protection.
- **FR-004:** The server MUST set the task source (GBIF or teacher) and the type of a teacher task itself; the client cannot send them.
- **FR-005:** The server MUST trim spaces at the beginning and end of the title, the question and the correct answer before validation, and MUST reject deleting a GBIF task with 409.

### Key Entities

- **ClassTask (Source = Teacher):** the identifier, the class, the title (1–100 characters), the question (up to 500 characters), the correct answer (up to 100 characters), the difficulty group (1–3), the type (a new type “Mokytojo užduotis”, Clarifications) and the source (teacher). It extends the ClassTask of feature 010.

### Validation Rules

- **VT-013:** There are three difficulty groups (1–3) (applies to the group chosen by the teacher). The four task types of VT-013 are GBIF types; a teacher task has its own type “Mokytojo užduotis” (Clarifications).
- **VT-018:** A teacher’s task consists of a title (1–100 characters), a question (up to 500 characters), a correct answer (up to 100 characters) and a difficulty group chosen by the teacher. An answer is considered correct if it matches the correct answer, ignoring letter case and spaces at the beginning and end. A teacher may create and delete only tasks of own classes. When a task is deleted, the points already earned by students remain.
- **VT-021:** No task, including one created by a teacher, may require collecting, catching, touching or disturbing organisms; only observation from a distance is allowed.

## Success Criteria

- **SC-001:** Requirements FR-031, FR-034 and FR-001–FR-005 are verified by backend unit and integration tests (field limits, confirmation, a student is rejected, another teacher is rejected, a visitor gets 401, another class’s task, deletion).
- **SC-002:** Requirements FR-032, FR-033, FR-035 and FR-053 are verified by frontend tests of E12 and E14 and by an end-to-end check in the Pull Request screenshots.

## Clarifications

- Q: Which text is the confirmation of FR-032? → A: A tick box on the form (E14) with the text „Patvirtinu, kad užduotis nereikalauja rinkti, liesti ar trikdyti organizmų.“; the task is not saved without it (`docs/ux.md`, 5.6).
- Q: What does the deletion dialog ask? → A: „Ištrinti užduotį? Mokinių jau gauti taškai išliks.“ using the dialog of feature 006 (`docs/ux.md`, 5.6).
- Q: Which task type does a teacher task have in the “type” column of E12? → A: A new type “Mokytojo užduotis” (Teacher task), so the type and the label match. The four GBIF types of VT-013 stay as they are. Decision of the student, because `docs/` gives only “ClassTask (Source = Teacher)”.
- Q: Are spaces at the beginning and end of the title, the question and the correct answer removed? → A: Yes, before validation, as for the class name (VT-030); the length limits apply to the text without these spaces.
- Q: What does the server return when a teacher deletes a GBIF task? → A: 409; the button is shown only next to teacher tasks, but the endpoint can be called directly.

No unresolved clarification markers remain.

## Assumptions

- The assumptions and constraints of `docs/requirements.md` (section 11) apply.
- Tasks of a teacher can be created before or after the GBIF tasks of the same class (`docs/ux.md`, E12 starts both E13 and E14).
- Student answers and points do not exist yet (features 014–016), so the statement that earned points remain is verified when they are added.

## Dependencies

- Features: 003-access-control (role policies), 006-confirmation-dialog (deletion), 007-class-create-and-delete (the class and its owner), 010-gbif-task-generation (ClassTask entity, E12, the task list endpoint), 024-page-styling (screens).

## Scope Boundaries

**In scope:**
- The title and the source of a task (migration), `POST` and `DELETE` task endpoints, the teacher label in the task list, the teacher task form (E14), the button to open it and the deletion with confirmation on E12.

**Out of scope:**
- The student task list (012), the observation card (013), answering tasks, the comparison of answers and points (014–016).
- Editing a teacher task.
- Everything that belongs to the other features of the list in `docs/architecture.md` (section 6).
