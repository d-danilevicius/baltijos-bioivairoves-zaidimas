# Feature Specification: Task completion

**Feature folder:** `specs/014-task-completion`
**Status:** draft
**Related project requirements:** FR-038, FR-039, FR-044, FR-045, FR-049; NFR-027; VT-015, VT-019, VT-030
**Related screens:** E06 Task, E07 Task result, E21 Leave the task (`docs/ux.md`)

**Why this feature is needed:** “I want to submit an answer and evidence so that I get points.” (user story 1). This feature adds the answer form and saving of the submission; the points come with feature 016 and the photos with feature 015.

## User Scenarios and Acceptance Scenarios

### User Story 1 – Task completion (Priority: P1)

**User story:** As a student, I want to submit an answer and evidence so that I get points.

**Independent verification:** After submitting an answer and evidence the result is shown; the same task cannot be submitted again.

**Acceptance scenarios:**

1. **Answer form (GBIF task):** **Given** a student and an unlocked GBIF task of the student’s class, **when** the student opens the task (E06), **then** below the question the form shows the given options to choose one, the evidence field with the counter “0 / 500”, the note „Pateikto atsakymo pakeisti nebus galima.“ and the button “Pateikti atsakymą”.
2. **Answer form (teacher task):** **Given** a teacher task, **when** the student opens it, **then** the answer is entered as text of up to 100 characters instead of chosen from options.
3. **Submission:** **Given** the answer form, **when** the student chooses or enters an answer, enters the evidence and presses “Pateikti atsakymą”, **then** the server saves the submission (the answer, the evidence, the time) and the task result (E07) opens.
4. **Task result:** **Given** a submitted task, **when** the result (E07) opens, **then** it shows the answer and the evidence submitted by the student under the label “Mokinio pateikti duomenys” (FR-044); the data are never shown as a confirmed observation (VT-019). The points and the correct answer are added to E07 by feature 016.
5. **Answer validation:** **Given** the form, **when** the answer is not chosen or is empty, **then** nothing is saved and the error is shown next to the field: „Pasirinkite arba įrašykite atsakymą.“; **when** the text answer of a teacher task is longer than 100 characters, **then** „Atsakymas negali būti ilgesnis nei 100 simbolių.“
6. **Evidence validation:** **Given** the form, **when** the evidence is empty (or only spaces), **then** nothing is saved and „Įrašykite įrodymą.“ is shown next to the field; **when** it is longer than 500 characters, **then** „Įrodymas negali būti ilgesnis nei 500 simbolių.“
7. **Option check:** **Given** a GBIF task, **when** the server receives an answer that is not one of the options of the task, **then** it rejects the request with 400 and an error for the answer field.
8. **Answer cannot be changed:** **Given** a task that the student already submitted, **when** the student submits it again (or opens it), **then** the server returns 409 for the new submission, the first answer stays unchanged (VT-015) and the task screen shows the saved answer and evidence instead of the form.
9. **Locked task:** **Given** a task in a locked group of the student’s class, **when** the student sends a submission, **then** the server rejects it with 403 and nothing is saved.
10. **Task not found:** **Given** a task that does not exist, was deleted (also while the student had it open) or belongs to another class, **when** the student sends a submission, **then** the server returns 404, nothing is saved and the screen shows “Puslapis nerastas” (NFR-027).
11. **Teacher and administrator:** **Given** a logged-in teacher or administrator, **when** the user calls the endpoint of this feature, **then** the server rejects it with 403; **Given** a visitor, **then** the server returns 401.
12. **Server failure:** **Given** the answer form, **when** saving fails, **then** the student stays in the same task, the entered data remain, and „Nepavyko išsaugoti. Bandykite dar kartą.“ with the button “Bandyti dar kartą” is shown.
13. **Task status:** **Given** a student who submitted a task, **when** the student opens the task list (E04), **then** the task has the status “Padaryta” and “Tęsti” opens the first task that is not done in an unlocked group (FR-049).
14. **Leaving the task:** **Given** the answer form in which the student has chosen or entered an answer or evidence, **when** the student leaves the task without submitting, **then** the confirmation dialog (E21, feature 006) „Išeiti iš užduoties? Neišsiųstas atsakymas nebus išsaugotas.“ with the buttons “Išeiti” and “Likti” is shown; **when** the form is empty, **then** the student leaves without the dialog.

## Edge Cases

- Spaces at the beginning and end of the answer and the evidence are removed before validation; the length limits apply to the text without them (VT-030).
- Two requests submit the same task of the same student at the same time: one is saved, the other returns 409 (the answer cannot be changed).
- The task is deleted after the student opened it: the submission returns 404 and nothing is saved.
- When the session has expired, the task screen opens the login page with the message „Sesija baigėsi dėl neveiklumo. Prisijunkite iš naujo.“ (NFR-004); the unsent answer is not saved.
- A student who changed the class cannot submit a task of the old class (404); the earlier submissions are not shown in the new class.
- The warning „Stebėkite gamtą iš atstumo, nerinkite ir netrikdykite organizmų, nefotografuokite žmonių.“ stays on the task screen (FR-045).

## Requirements

### Functional Requirements

- **FR-038:** The system MUST allow a student to open an unlocked task (the opening is provided by feature 013; this feature adds the form to the same screen).
- **FR-039:** The system MUST allow a student to submit an answer and text evidence in a task.
- **FR-044:** The system MUST label data submitted by students “Mokinio pateikti duomenys” (Student-submitted data) and never show them as confirmed observations.
- **FR-049:** The system MUST allow a student, after logging out and logging in again, to continue from the first unfinished unlocked task (the status “Padaryta” is added to the task list by this feature).
- **FR-001:** The server MUST provide `POST /api/my/tasks/{id}/submissions` for a student; the request contains the answer and the evidence, the response contains the saved submission (the answer, the evidence, the label, the time).
- **FR-002:** The server MUST allow only a student to call the endpoint (a teacher and an administrator get 403, a visitor 401). The student and the class are taken from the session, never from the request; POST uses the existing CSRF protection.
- **FR-003:** The server MUST return 404 when the task does not exist or does not belong to the student’s current class, and 403 when the task belongs to a locked group (the same rule as in feature 013).
- **FR-004:** The server MUST validate the answer and the evidence and return the errors per field (400); a GBIF task accepts only one of its options, a teacher task accepts a text of up to 100 characters.
- **FR-005:** The server MUST save at most one submission of a task by a student and return 409 for another one; the saved answer is never changed (VT-015).
- **FR-006:** The server MUST show the status “Padaryta” in `GET /api/my/tasks` for a task the student has submitted.

### Key Entities

- **Submission:** the identifier, the task, the student, the answer (up to 100 characters), the evidence (up to 500 characters), the submission time. One submission of a task per student. Points are added by feature 016 and photos by feature 015.

### Validation Rules

- **VT-015:** A task is considered completed when the student has submitted an answer and evidence. A submitted answer cannot be changed.
- **VT-019:** A confirmed external observation is only an observation provided by the GBIF Occurrence API. Student-submitted data are always labelled as student-submitted and never become confirmed observations. Tasks created by a teacher are not GBIF observations and are labelled as teacher tasks.
- **VT-030:** Evidence text – up to 500 characters (the answer of a teacher task – up to 100 characters, VT-018).

## Success Criteria

- **SC-001:** Requirements FR-039, FR-001–FR-006 and VT-015 are verified by backend unit and integration tests (saving, validation, the option check, the second submission, locked task, deleted task, other class, roles, the status in the task list).
- **SC-002:** Requirements FR-038, FR-044 and the screens E06, E07 and E21 are verified by frontend tests (the form, the validation messages, the counter, the label “Mokinio pateikti duomenys”, the saved answer shown instead of the form, the failure state, the leave dialog).

## Clarifications

**Session 2026-11-19**

- Q: Can a submitted answer be changed? → A: No (VT-015).
- Q: What happens if the task is deleted while the student had it open? → A: “Puslapis nerastas” (Page not found) is shown, no points are awarded (Work 2).
- Q: Where is the opening of a task (FR-038) verified? → A: The opening and the check “unlocked task of the student’s own class” are made by feature 013; this feature adds the form and checks the same rule on submission.
- Q: What does the task result (E07) show in this feature, because the points and the correctness (VT-016) belong to feature 016? → A: Only the submitted data under “Mokinio pateikti duomenys”; feature 016 adds the points and the correct answer to E07.
- Q: When is the leave dialog (E21) shown? → A: Only when the student has already chosen or entered an answer or evidence; an empty form is left without the dialog.

No unresolved clarification markers remain.

## Assumptions

- The assumptions and constraints of `docs/requirements.md` (section 11) apply.
- The answer of a GBIF task is saved as the text of the chosen option; the server compares it with the options of the task.
- The 1-second display of points (NFR-015) and saving points (NFR-018) belong to feature 016; this feature saves the submission before the response is returned.

## Dependencies

- Features: 003-access-control, 006-confirmation-dialog, 012-student-task-list, 013-observation-card.

## Scope Boundaries

**In scope:**
- The answer and evidence form on E06 and saving the submission (`POST /api/my/tasks/{id}/submissions`).
- The result screen E07 with the submitted data under “Mokinio pateikti duomenys”.
- The status “Padaryta” in the task list and continuing from the first unfinished task.
- The leave dialog E21.

**Out of scope:**
- Points, correctness and unlocking of groups (feature 016); photos (feature 015); repeating a task and the best result (feature 017); results of the student and the class (features 018, 019); the teacher’s view of answers and evidence (feature 020); moderation of student evidence.
