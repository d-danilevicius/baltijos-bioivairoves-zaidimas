# Feature Specification: Class creation and deletion

**Feature folder:** `specs/007-class-create-and-delete`
**Status:** draft
**Related project requirements:** FR-013, FR-014, FR-015, FR-023; VT-023, VT-024, VT-027, VT-030
**Related screens:** E09 Class list, E10 New class, E11 Class information (`docs/ux.md`)

**Why this feature is needed:** “I want to create a class and get its code so that I can bring my students together, and to delete it when it is no longer needed.” (user story 1).

## User Scenarios and Acceptance Scenarios

### User Story 1 – Class creation and deletion (Priority: P1)

**User story:** As a teacher, I want to create a class and get its code so that I can bring my students together, and to delete it when it is no longer needed.

**Independent verification:** After creating a class a 6-character code is shown; a deleted class disappears from the list.

**Acceptance scenarios:**

1. **Creation:** **Given** a logged-in teacher, **when** the teacher enters a class name on the new class screen and presses “Sukurti”, **then** the class is created, the server generates its 6-character code and the class information screen (E11) opens with the message „Klasė sukurta.“ and the code.
2. **Class list:** **Given** a logged-in teacher, **when** the teacher opens the class list (E09), **then** the page shows the teacher’s own classes with their names and codes; a class of another teacher is not shown.
3. **Empty list:** **Given** a teacher who has no class, **when** the teacher opens the class list, **then** the page shows „Dar nesukūrėte nė vienos klasės.“ and the button “Sukurti pirmą klasę”.
4. **Name validation:** **Given** the new class screen, **when** the name is empty (or only spaces) or longer than 50 characters, **then** nothing is created and the error is shown next to the field: „Įveskite klasės pavadinimą.“ or „Pavadinimas negali būti ilgesnis nei 50 simbolių.“
5. **Deletion:** **Given** the class information screen of the teacher’s own class, **when** the teacher presses „Ištrinti klasę“ and confirms in the confirmation dialog (feature 006), **then** the class is deleted and the class list opens with a message that the class was deleted; **when** the teacher cancels, **then** nothing changes.
6. **Student:** **Given** a logged-in student, **when** the student calls any class endpoint, **then** the server rejects it with 403 and the class screens show “Prieiga uždrausta”.
7. **Other teacher:** **Given** a class of teacher A, **when** teacher B opens or deletes it, **then** the server rejects it with 403 and the screen shows “Prieiga uždrausta”; **when** the class does not exist, **then** the server returns 404 and the screen shows “Puslapis nerastas”.
8. **Not logged in:** **Given** a visitor, **when** any class endpoint is called, **then** the server returns 401.
9. **Starting screen and menu:** **Given** a teacher who logs in (or opens the home page while logged in), **when** the starting screen opens, **then** it is the class list (`/classes`); the menu of a teacher shows “Klasės” and “Profilis”. Students keep `/profile` as their starting screen.

## Edge Cases

- Spaces at the beginning and end of the class name are removed; the name is not checked for uniqueness, because the class is identified by the code (VT-023).
- Two classes can get the same generated code only by chance; the server detects it (unique index) and generates another code, so the teacher never sees a duplicate.
- The class code contains only uppercase letters A–Z and digits (VT-030).
- Two teachers delete the same class at the same time: the second request returns 404.
- Class tasks do not exist yet; when the later features add them, they are deleted together with the class, and the points earned by the students remain (VT-009, VT-027).

## Requirements

### Functional Requirements

- **FR-013:** The system MUST allow a teacher to create a class by providing a class name of 1–50 characters (spaces at the beginning and end are removed) (VT-030).
- **FR-014:** The system MUST allow a teacher to view the list of own classes.
- **FR-015:** The system MUST generate a unique 6-character join code (uppercase letters A–Z and digits) for each class and show it to the teacher (VT-023).
- **FR-023:** The system MUST allow a teacher to delete own class; the teacher is asked for confirmation first (FR-053).
- **FR-001:** The server MUST provide `GET /api/classes`, `POST /api/classes`, `GET /api/classes/{id}` and `DELETE /api/classes/{id}`; the last two return 404 when the class does not exist.
- **FR-002:** Only a teacher can create a class, and the owner of a class is the teacher who created it (VT-024); the owner is taken from the session, never from the request.
- **FR-003:** The server MUST allow only the owner to view or delete a class; a student, another teacher and a visitor are rejected (403, 403 and 401).
- **FR-004:** A teacher’s starting screen MUST be the class list, and the teacher menu MUST contain “Klasės” (`docs/ux.md`).
- **FR-005:** The server MUST validate the class name and return the errors per field; POST and DELETE use the existing CSRF protection.

### Key Entities

- **Class:** the identifier, the name (1–50 characters), the unique code (6 characters), the owner (a teacher), the creation time.

### Validation Rules

- **VT-023:** A class join code is unique among all classes.
- **VT-024:** Only a teacher can create a class, and the owner of a class is the teacher who created it.
- **VT-027:** When a class is deleted, the results of the students are not deleted (applies when results exist, features 014–016).
- **VT-030:** Class name – 1–50 characters. Class code – 6 uppercase letters (A–Z) or digits.

## Success Criteria

- **SC-001:** Requirements FR-001–FR-003 and FR-005 are verified by backend unit and integration tests (code uniqueness, a student is rejected, another teacher is rejected, a visitor gets 401, validation errors).
- **SC-002:** Requirements FR-013–FR-015, FR-023 and FR-004 are verified by frontend tests of the class screens and by an end-to-end check in the Pull Request screenshots.

## Clarifications

- Q: What does the class delete? → A: In this feature only the class itself. The class tasks and the student memberships do not exist yet; when the later features add them, they are removed with the class and the student results remain (VT-009, VT-027).
- Q: How is the unique code generated? → A: The server generates 6 random characters (A–Z, 0–9); a unique index in the database guarantees uniqueness, and on a collision the server generates another code.
- Q: Why is `GET /api/classes/{id}` added to the endpoints of `docs/architecture.md`? → A: The class information screen (E11) can be opened by its address and must check the owner on the server; the list alone is not enough.
- Q: What is the starting screen of a teacher? → A: The class list. Feature 004-home-page kept `/profile` for every role until the role-specific screens exist; this feature adds the teacher’s one.
- Q: What does the confirmation dialog ask? → A: „Ištrinti klasę „{name}“?“ with the buttons “Ištrinti” and “Atšaukti”, using the dialog of feature 006.

No unresolved clarification markers remain.

## Assumptions

- The assumptions and constraints of `docs/requirements.md` (section 11) apply.
- Students, tasks and the class location and season do not exist yet; the class information screen shows only the name and the code (the later features add the rest).

## Dependencies

- Features: 003-access-control (role policies), 006-confirmation-dialog (deletion), 024-page-styling (screens).

## Scope Boundaries

**In scope:**
- The class entity, its migration, the four endpoints, the class list, new class and class information screens, the teacher’s starting screen and menu link.

**Out of scope:**
- Students joining a class (008), the student list and removal (009), tasks (010–011), results (015–019).
- Renaming a class.
- Everything that belongs to the other features of the list in `docs/architecture.md` (section 6).
