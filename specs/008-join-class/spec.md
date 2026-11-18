# Feature Specification: Joining a class with a code

**Feature folder:** `specs/008-join-class`
**Status:** draft
**Related project requirements:** FR-016, FR-017, FR-053; VT-025, VT-029, VT-030
**Related screens:** E05 Join a class, E21 Confirmation dialog (`docs/ux.md`)

**Why this feature is needed:** “I want to join a class with a code so that I see the tasks of my class.” (user story 008).

## User Scenarios and Acceptance Scenarios

### User Story 1 – Joining a class with a code (Priority: P1)

**User story:** As a student, I want to join a class with a code so that I see the tasks of my class.

**Independent verification:** After entering a valid code the student is a member of that class; a student who was in another class belongs only to the new one.

**Acceptance scenarios:**

1. **Joining:** **Given** a logged-in student who has no class, **when** the student enters the code of an existing class on the join screen (E05) and presses “Prisijungti”, **then** the student becomes a member of that class and the message „Prisijungėte prie klasės „7a“.“ is shown.
2. **Code normalization:** **Given** the join screen, **when** the student enters a code with lowercase letters or spaces (for example “ab c 123”), **then** the lowercase letters are converted to uppercase and the spaces are removed before the code is checked (VT-030).
3. **Empty code:** **Given** the join screen, **when** the code is empty, **then** nobody is joined and the error is shown next to the field: „Įveskite klasės kodą.“
4. **Wrong format:** **Given** the join screen, **when** the code (after normalization) is not 6 letters or digits, **then** nobody is joined and the error is shown next to the field: „Kodą sudaro 6 raidės ar skaičiai.“
5. **Class not found:** **Given** the join screen, **when** the code has a correct format but no class has it, **then** nobody is joined and the error is shown next to the field: „Klasė su tokiu kodu nerasta.“ (FR-017).
6. **Changing the class:** **Given** a student who already belongs to class “7b”, **when** the student enters the code of class “7a” and presses “Prisijungti”, **then** the confirmation dialog (feature 006) asks „Jau priklausote klasei „7b“. Pakeisti į „7a“?“ with the buttons “Pakeisti” and “Atšaukti”; **when** the student confirms, **then** the membership in “7b” ends and the student is a member of “7a” only (VT-025); **when** the student cancels, **then** nothing changes.
7. **Teacher and administrator:** **Given** a logged-in teacher or administrator, **when** the user calls the join endpoint, **then** the server rejects it with 403 and the screen shows “Prieiga uždrausta”.
8. **Not logged in:** **Given** a visitor, **when** the join endpoint is called, **then** the server returns 401.
9. **Same class:** **Given** a student who already belongs to class “7a”, **when** the student enters the code of class “7a”, **then** the message „Jau priklausote šiai klasei.“ is shown, no confirmation dialog is shown and the membership does not change.

## Edge Cases

- The code is checked on the server too; the screen checks only improve the usability of the form.
- A code of a class that was deleted is treated as a non-existent code.
- When the session has expired, the join screen opens the login page with the message „Sesija baigėsi dėl neveiklumo. Prisijunkite iš naujo.“ (NFR-004).
- A student cannot belong to two classes at the same time (VT-025); two requests of the same student at the same time still leave one membership.
- A student without a class can log in and view the profile but does not see tasks until joining a class (VT-026).
- After a failed join the student stays on the same page and the entered code remains.

## Requirements

### Functional Requirements

- **FR-016:** The system MUST allow a student to join a class by entering a valid class code.
- **FR-017:** The system MUST reject a non-existent class code and show an error message; the code is checked according to VT-030.
- **FR-053:** The system MUST ask the user for confirmation before changing a student’s class (VT-029).
- **FR-001:** The server MUST provide `POST /api/classes/join`; the request contains the class code, the student is taken from the session, never from the request.
- **FR-002:** Only a student can join a class; a teacher, an administrator and a visitor are rejected (403, 403 and 401).
- **FR-003:** The server MUST normalize the code (uppercase, spaces removed), validate it and return the error per field; POST uses the existing CSRF protection.
- **FR-004:** The server MUST keep one active membership per student: joining a new class ends the previous membership (VT-025).
- **FR-005:** The student menu MUST contain a link to the join screen (E05).

### Key Entities

- **Class membership:** the identifier, the student, the class, the time the membership started, the time it ended (empty while the student belongs to the class).

### Validation Rules

- **VT-025:** A student can belong to only one class at a time. When joining a new class, the previous membership ends.
- **VT-029:** The user is asked for confirmation before changing a student’s class.
- **VT-030:** Class code – 6 uppercase letters (A–Z) or digits; when entered, lowercase letters are converted to uppercase and spaces are removed.

## Success Criteria

- **SC-001:** Requirements FR-001–FR-004 are verified by backend unit and integration tests (code normalization, code not found, class change, a teacher is rejected, an administrator is rejected, a visitor gets 401, validation errors).
- **SC-002:** Requirements FR-016, FR-017, FR-053 and FR-005 are verified by frontend tests of the join screen (validation errors, confirmation dialog, success message) and by screenshots in the Pull Request.

## Clarifications

- Q: How is the class code format checked? → A: 6 uppercase letters or digits; lowercase letters are converted to uppercase and spaces are removed (VT-030). Otherwise the class is not found.
- Q: What happens when the student is already in another class? → A: Confirmation is requested; after confirmation the previous membership ends (VT-025, VT-029).
- Q: What does the confirmation dialog ask? → A: „Jau priklausote klasei „7b“. Pakeisti į „7a“?“ with the buttons “Pakeisti” and “Atšaukti”, using the dialog of feature 006 (`docs/ux.md`, 5.3).
- Q: Which screen opens after joining, when the task list (E04, feature 012) does not exist yet? → A: The student stays on the profile screen (`/profile`) and the message „Prisijungėte prie klasės „7a“.“ is shown there. The task list opens after joining when feature 012 is added.
- Q: What happens when the student enters the code of the class the student already belongs to? → A: The message „Jau priklausote šiai klasei.“ is shown, no confirmation is asked and nothing changes.

## Assumptions

- The assumptions and constraints of `docs/requirements.md` (section 11) apply.
- Classes exist (feature 007); tasks do not exist yet, so a student who joined sees only the message and the profile until features 010–012 are added.

## Dependencies

- Features: 003-access-control (role policies), 006-confirmation-dialog (class change), 007-class-create-and-delete (classes and codes), 024-page-styling (screens).

## Scope Boundaries

**In scope:**
- The class membership entity, its migration, the endpoint `POST /api/classes/join`, the join screen (E05), the confirmation before changing the class, the menu link of a student.

**Out of scope:**
- The student list of a class and removing a student (009), the task list and tasks (010–012), results (014–020).
- Leaving a class without joining another one.
- Everything that belongs to the other features of the list in `docs/architecture.md` (section 6).
