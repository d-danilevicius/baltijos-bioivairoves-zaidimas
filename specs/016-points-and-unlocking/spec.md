# Feature Specification: Points and group unlocking

**Feature folder:** `specs/016-points-and-unlocking`
**Status:** draft
**Related project requirements:** FR-041, FR-042, FR-048; NFR-015, NFR-018; VT-014, VT-016
**Related screens:** E07 Task result, E04 Task list (`docs/ux.md`)

**Why this feature is needed:** “I want to get points and unlock harder tasks so that I see my progress.” (user story 1).

## User Scenarios and Acceptance Scenarios

### User Story 1 – Points and group unlocking (Priority: P1)

**User story:** As a student, I want to get points and unlock harder tasks so that I see my progress.

**Independent verification:** After a correct answer the points are shown within 1 s; after earning the required share the next group appears.

**Acceptance scenarios:**

1. **Points for a correct answer:** **Given** a student who submits an answer to an unlocked task, **when** the answer is correct, **then** the server saves 10 points for a group 1 task, 20 for a group 2 task and 30 for a group 3 task, and the task result (E07) shows the points, for example „Gavote 10 taškų.“ (VT-016).
2. **Points for an incorrect answer:** **Given** a student who submits an answer, **when** the answer is incorrect, **then** the server saves 0 points and the task result shows „Gavote 0 taškų.“ and the correct answer.
3. **Task result:** **Given** a submitted task, **when** the task result (E07) is shown, **then** it shows the points earned, the correct answer and the data submitted by the student with the label “Mokinio pateikti duomenys”; the buttons “Kita užduotis” (Next task) and “Į užduočių sąrašą” (To the task list) are shown. The button “Pakartoti užduotį” belongs to feature 017.
4. **Next task:** **Given** the task result, **when** the student presses “Kita užduotis”, **then** the first task that is not done in an unlocked group opens (the same rule as “Tęsti”, FR-049); **when** there is no such task, **then** the button is not shown.
5. **Correct answer is hidden before submission:** **Given** a task that the student has not submitted, **when** the task is opened (E06), **then** the response contains no correct answer; it appears only together with the student’s submission.
6. **Reopening a submitted task:** **Given** a task that the student has already submitted, **when** the student opens it again, **then** the task result shows the saved points, the correct answer and the submitted data, and the points do not change.
7. **Unlocking:** **Given** a student of a class whose first group has tasks worth 50 points in total, **when** the student has earned 30 points (60 %) in that group, **then** the second group is unlocked: its status in the task list (E04) changes from “Užrakinta” to a list of tasks that are “Nepadaryta”, and the task result of the submission that caused it shows „Atrakinta 2 sudėtingumo grupė.“ (FR-042, VT-014).
8. **Not enough points:** **Given** the same student, **when** the student has earned 29 of 50 points (less than 60 %), **then** the second group stays locked and the task list shows „Atrakinama surinkus 60 % ankstesnės grupės taškų.“
9. **Third group:** **Given** a student, **when** the student has earned at least 60 % of the points of group 2, **then** group 3 is unlocked in the same way.
10. **Task list:** **Given** a student, **when** the task list (E04) is opened, **then** each done task shows its points (for example „10 tšk.“ next to „Padaryta“), each group shows its points as „30 iš 50“, and the status words and numbers are written as text (not by colour only).
11. **Saving:** **Given** a student who submits an answer, **when** the server answers, **then** the points are already saved in the database together with the answer; after the student logs out and logs in again the same points are shown (FR-048, NFR-018).
12. **Speed:** **Given** a student who submits an answer, **when** the answer is saved, **then** the points are in the response and are shown within 1 second (NFR-015).

**Access and validation scenarios (unchanged endpoints):**

13. **Student only:** **Given** a teacher or an administrator, **when** a student endpoint (`POST /api/my/tasks/{id}/submissions`, `GET /api/my/tasks`, `GET /api/my/tasks/{id}`) is called, **then** the server returns 403; **given** a visitor, **then** 401.
14. **Locked task:** **Given** a task of a locked group, **when** the student opens it or submits an answer, **then** the server returns 403 (unchanged from 013–014).
15. **Other class or no class:** **Given** a task of another class (or a student without a class), **when** the student opens it or submits an answer, **then** the server returns 404 (unchanged).
16. **Other student:** **Given** two students, **when** one of them calls the endpoints, **then** the server uses only the points and submissions of that student (VT-022).

## Edge Cases

- An answer is correct when it matches the correct answer ignoring letter case and spaces at the beginning and the end (the rule of VT-018, applied to every task, with or without options). Spaces inside the answer are not changed and numeric answers are not converted to numbers before the comparison. Example: the month „7“ is correct for 7; „07“ is not.
- The points of a task depend only on its difficulty group (10, 20 or 30), never on the number of attempts or the time.
- The maximum possible points of a group are the number of tasks that exist in the group now multiplied by the points of one task of that group. The share is calculated from the tasks that exist now: when the teacher adds or deletes a task, the maximum changes and the group may become locked or unlocked on the next opening of the task list.
- The points of a task that the teacher deleted do not count for unlocking (the task no longer exists in the group), but the saved submission and its points remain (VT-018, VT-027).
- 60 % is checked with whole numbers, without rounding: 3 of 5 points is exactly 60 % and unlocks; 29 of 50 does not.
- A group that has no tasks does not lock the next group (the rule of feature 012 is kept): a group is locked only while the previous group has tasks and the student has not yet earned 60 % of their points.
- The task submitted before this feature exists (feature 014 data) has 0 points after the migration; there is no production data yet.
- Two submissions of the same student for the same task at the same time: the first one keeps its points, the second returns 409 (unchanged from 014).
- A task deleted while the student has it open: “Puslapis nerastas” and no points are awarded (unchanged).

## Requirements

### Functional Requirements

- **FR-041:** The system MUST automatically award points for a submitted answer according to VT-016.
- **FR-042:** The system MUST unlock the next difficulty group when the student reaches the share of points set by VT-014.
- **FR-048:** The system MUST save the student’s points after each submitted task.
- **FR-001:** The server MUST determine the correctness and the points on the server when the answer is submitted; the client never sends points or correctness.
- **FR-002:** The response of `POST /api/my/tasks/{taskId}/submissions` MUST contain the points, the correct answer and, when this submission unlocked a group, the number of that group.
- **FR-003:** The correct answer MUST NOT be sent to the student before the student has submitted the task; after that it is part of the student’s own submission data in `GET /api/my/tasks/{id}`.
- **FR-004:** `GET /api/my/tasks` MUST return the points of each done task, the points and the maximum points of each group, and the unlocked state of each group according to VT-014.
- **FR-005:** The unlocking rule MUST have one implementation that the task list, the task opening and the submission use (feature 012 and 013 use the old rule `TaskGroupUnlocking`, which this feature replaces).
- **FR-006:** E07 MUST show the points, the correct answer, the submitted data with the label “Mokinio pateikti duomenys”, and the buttons “Kita užduotis” and “Į užduočių sąrašą”; E04 MUST show the points of done tasks and groups.

### Non-functional Requirements

- **NFR-015:** The system MUST show the earned points within no more than 1 second after an answer is submitted.
- **NFR-018:** The system MUST save the student’s points even when the connection is lost immediately after the answer is submitted.

### Key Entities

- **Submission.Points:** the whole number of points saved with the submission (0, 10, 20 or 30); set by the server.

### Validation Rules

- **VT-014:** The first difficulty group is unlocked for every student of the class. Each next group is unlocked when the student earns at least 60 % of the maximum possible points of the previous group.
- **VT-016:** A correct answer earns 10 points in a group 1 task, 20 in a group 2 task and 30 in a group 3 task; an incorrect answer earns 0 points. Correctness is determined from the observation card data, and in a teacher’s task from the correct answer set by the teacher.

## Success Criteria

- **SC-001:** Requirements FR-041, FR-001 and VT-016 are verified by backend unit tests (10/20/30 points, incorrect answer 0, option and typed answer matching) and by integration tests.
- **SC-002:** Requirements FR-042, FR-005 and VT-014 are verified by backend unit tests of the unlocking rule (exactly 60 %, below 60 %, group 2 and group 3, empty group, deleted task) and by integration tests of the task list.
- **SC-003:** Requirements FR-048, NFR-015 and NFR-018 are verified by integration tests (points are in the database when the response is returned; the response time is measured; the points remain after a new request).
- **SC-004:** Requirements FR-002–FR-004 and FR-006 are verified by integration tests (the correct answer is absent before submission, present after it) and by frontend tests of E04 and E07.

## Clarifications

- Q: Where are the points stored and when are they calculated? → A: `Submission.Points`, calculated by the server when the submission is saved and saved in the same database operation as the answer (`docs/architecture.md`, section 6, feature 016).
- Q: What is the maximum possible points of a group? → A: The number of tasks in the group multiplied by 10, 20 or 30 for group 1, 2 or 3. The example “30 iš 50” in `docs/ux.md` means 5 tasks of group 1.
- Q: Is the unlocking stored? → A: No. It is calculated from the current tasks and submissions each time (no new table or flag).
- Q: What happens to the button “Pakartoti užduotį”? → A: It belongs to feature 017 (task repetition) and is not added here; this feature has one submission per task (VT-015), so the best result of a task is its points.
- Q: How is an answer compared? → A: Ignoring letter case and spaces at the beginning and the end (the rule of VT-018, applied to every task); inner spaces are not changed and numbers are not converted (confirmed by the student, 2026-11-20).
- Q: What if the previous group has no tasks? → A: The next group is not locked; the rule of feature 012 is kept (confirmed 2026-11-20).
- Q: What are the points of submissions saved before this feature? → A: 0 (the migration default); they exist only in test databases (confirmed 2026-11-20). The data model has only `Points`, not a separate “incorrect” state, so 0 points is never shown as “incorrect”: E07 shows the points and the correct answer only.
- Q: When is “Kita užduotis” shown? → A: Only when another task is available to the student (the first task that is not done in an unlocked group); otherwise it is not shown (confirmed 2026-11-20).

## Assumptions

- The assumptions and constraints of `docs/requirements.md` (section 11) apply.
- Tasks are completed once (VT-015); repetition is feature 017, the results page is feature 018.
- The correct answer of a task already exists in `ClassTask.CorrectAnswer` (features 010–011).

## Dependencies

- Features: 003-access-control, 012-student-task-list, 013-observation-card, 014-task-completion.

## Scope Boundaries

**In scope:**
- Calculating and saving points, the correct answer in the student’s submission, the group unlocking rule (replacing `TaskGroupUnlocking`), the points in E04 and E07, the unlock message, the buttons “Kita užduotis” and “Į užduočių sąrašą”.

**Out of scope:**
- Repeating a task (017), the results page (018), the class results (019), the student answers for the teacher (020), photos (015).
- Everything that belongs to the other features of the list in `docs/architecture.md` (section 6).
