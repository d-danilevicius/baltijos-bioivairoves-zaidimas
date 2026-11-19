# Feature Specification: Student task list

**Feature folder:** `specs/012-student-task-list`
**Status:** draft
**Related project requirements:** FR-033, FR-037, FR-042, FR-049; NFR-009; VT-013, VT-014, VT-026
**Related screens:** E04 Task list, E05 Join a class, E03 Login (`docs/ux.md`)

**Why this feature is needed:** “I want to see the class tasks by difficulty groups and continue from an uncompleted one so that I know what to do next.” (user story 012).

## User Scenarios and Acceptance Scenarios

### User Story 1 – Student task list (Priority: P1)

**User story:** As a student, I want to see the class tasks by difficulty groups and continue from an uncompleted one so that I know what to do next.

**Independent verification:** Tasks are shown in three groups; locked groups cannot be opened.

**Acceptance scenarios:**

1. **Groups:** **Given** a student who belongs to a class with tasks, **when** the student opens the task list (E04), **then** the tasks are shown in three difficulty groups (1, 2 and 3), each task with its type, its status and its label (VT-013, FR-037).
2. **Unlocked group:** **Given** the same student, **when** the list opens, **then** group 1 is unlocked and its tasks have the status “not done” (VT-014).
3. **Locked groups:** **Given** the same student who has earned no points and whose previous group has tasks, **when** the list opens, **then** groups 2 and 3 are locked: their tasks have the status “locked”, the message „Atrakinama surinkus 60 % ankstesnės grupės taškų.“ is shown, and the tasks cannot be opened (VT-014, FR-042).
4. **Labels:** **Given** a class with GBIF tasks and teacher tasks, **when** the student opens E04, **then** each task is labelled “GBIF” or “Mokytojo užduotis” and a teacher task is never shown as a GBIF observation (FR-033).
5. **Continue:** **Given** a student who belongs to a class with tasks, **when** the student presses “Tęsti”, **then** the first unfinished task of an unlocked group opens; after the student logs out and logs in again, the same button leads to the first unfinished unlocked task (FR-049). The task is reached in no more than two clicks from the task list (NFR-009).
6. **Starting screen:** **Given** a student who logs in or joins a class, **when** the login or the joining succeeds, **then** the task list (E04) opens; after joining, with the message „Prisijungėte prie klasės „7a“.“ (`docs/ux.md`, E03 and E05).
7. **No class:** **Given** a student who does not belong to a class, **when** the student opens E04, **then** no tasks are shown, the text „Dar nepriklausote klasei. Įveskite klasės kodą.“ and the button “Prisijungti prie klasės” are shown (VT-026).
8. **No tasks:** **Given** a student whose class has no tasks, **when** the student opens E04, **then** the text „Mokytojas dar nesukūrė užduočių.“ is shown.
9. **Server failure:** **Given** the task list, **when** the server fails, **then** the error state with the button “Bandyti dar kartą” is shown.
10. **Teacher and administrator:** **Given** a logged-in teacher or administrator, **when** the user calls the endpoint of this feature, **then** the server rejects it with 403 and the screen shows “Prieiga uždrausta”.
11. **Not logged in:** **Given** a visitor, **when** the endpoint is called, **then** the server returns 401.

## Edge Cases

- When the session has expired, the task list opens the login page with the message „Sesija baigėsi dėl neveiklumo. Prisijunkite iš naujo.“ (NFR-004).
- A student who changed the class (VT-025) sees only the tasks of the new class.
- A task the teacher deleted (feature 011) disappears from the list.
- The list never contains the question, the correct answer or the options of a task: only the type, the group, the status and the label.
- A group without tasks is not shown; the next group after it is unlocked at once (Clarifications).
- A student who has no points yet cannot unlock group 2 or group 3 when the previous group has tasks; points exist from features 014–016, so the unlocking itself is verified when they are added.

## Requirements

### Functional Requirements

- **FR-037:** The system MUST show a student the tasks of own class arranged by difficulty groups (VT-013).
- **FR-049:** The system MUST allow a student, after logging out and logging in again, to continue from the first unfinished unlocked task.
- **FR-033:** The system MUST label tasks created by a teacher “Mokytojo užduotis” (Teacher task) and never show them as GBIF observations.
- **FR-042:** The system MUST unlock the next difficulty group when the student reaches the share of points set by VT-014 (this feature shows the locked state; the points come with features 014–016).
- **FR-001:** The server MUST provide `GET /api/my/tasks` for a student; it returns the tasks of the student’s current class in three groups, the status of each group and task, and the task to continue from.
- **FR-002:** The server MUST allow only a student to call the endpoint (a teacher and an administrator get 403, a visitor 401). The class is taken from the student’s membership in the session, never from the request.
- **FR-003:** The server MUST return only the identifier, the type, the difficulty group, the label and the status of each task; the title, the question, the correct answer and the options MUST NOT be returned.
- **FR-004:** The server MUST decide the status of a task: “not done” in an unlocked group and “locked” in a locked group (VT-014). The status “done” is added by feature 014.

### Non-functional Requirements

- **NFR-009:** The system MUST allow a new student who already belongs to a class to start the first task in no more than 3 clicks after logging in.

### Key Entities

- **ClassTask (read):** the class tasks of feature 010 and feature 011.
- **ClassMembership (read):** the current class of the student (feature 008).
- **Submission (read):** does not exist yet; it is created by feature 014. Until then no task is done and the student has no points.

### Validation Rules

- **VT-013:** There are four GBIF task types, one additional teacher task type and three difficulty groups: group 1 – species group recognition and a question from an observation card; group 2 – features of a common local species; group 3 – comparison of seasonal observations.
- **VT-014:** The first difficulty group is unlocked for every student of the class. Each next group is unlocked when the student earns at least 60 % of the maximum possible points of the previous group.
- **VT-026:** A student without a class can log in and view the profile but does not see tasks until joining a class.

## Success Criteria

- **SC-001:** Requirements FR-037, FR-033, FR-001–FR-004 and VT-014, VT-026 are verified by backend unit and integration tests (groups, locked groups, a student without a class, a class without tasks, the first unfinished task, no question or answer in the response, a teacher and an administrator are rejected, a visitor gets 401).
- **SC-002:** Requirements FR-049 and NFR-009 and the screens of E04 are verified by frontend tests (groups, locked groups, the empty states, “Tęsti”, the starting screen, the error state).

## Clarifications

- Q: Which status does a task have before the table of submissions exists (feature 014)? → A: Until feature 014, no task is done and no points exist. All tasks in unlocked groups are “not done”. Groups 2 and 3 remain locked when the previous group contains tasks, but are unlocked when the previous group is empty. Features 014 and 016 add the status “done” and unlocking based on earned points. Decision of the student; `docs/` lists “Submission (read)” for this feature, but the entity is created by feature 014.
- Q: What does the button “Tęsti” do before the task screen (E06) exists? → A: The button is shown and opens the address of the task (`/tasks/{id}`) created by feature 013; before feature 013 is merged the address shows “Puslapis nerastas”. The tests of this feature check the address of the link. Decision of the student.
- Q: What happens when the previous group has no tasks (for example a class with only teacher tasks of group 2)? → A: The formula of VT-014 is applied as written: 0 points of 0 possible points is at least 60 %, so the next group is unlocked at once. A group without tasks is not shown to the student. Decision of the student; `docs/` does not describe this case.
- Q: Does the student task list show the title of a teacher task? → A: No. `docs/ux.md` (E04) lists the type, the status, the label and the best result. The title is decided in feature 013 (task screen E06). Decision of the student.
- Q: What does a student without a class see? → A: No tasks are shown, and joining a class is suggested (VT-026).

No unresolved clarification markers remain.

## Assumptions

- The assumptions and constraints of `docs/requirements.md` (section 11) apply.
- The student’s current class is the membership without an end date (feature 008).
- The best result of a task is shown by the task list when points exist (features 016 and 017).

## Dependencies

- Features: 003-access-control (role policies), 007-class-create-and-delete (class), 008-join-class (membership, the screen after joining), 010-gbif-task-generation and 011-teacher-tasks (ClassTask and the label), 024-page-styling (screens).

## Scope Boundaries

**In scope:**
- `GET /api/my/tasks`, the student task list (E04) with the groups, the statuses “not done” and “locked”, the labels, “Tęsti”, the empty states, E04 as the starting screen of a student and the screen after joining a class.

**Out of scope:**
- The task screen and the observation card (013), submitting an answer and the status “done” (014), points, the best result and group unlocking by points (016, 017).
- Everything that belongs to the other features of the list in `docs/architecture.md` (section 6).
