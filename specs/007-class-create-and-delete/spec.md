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

1. **Verification:** After creating a class a 6-character code is shown; a deleted class disappears from the list.
2. **Successful:** **Given** a logged-in teacher, **when** the teacher enters a class name and creates the class, **then** the class is created and a 6-character code is shown.
3. **Negative:** **Given** a logged-in student, **when** the student tries to create a class, **then** the action is rejected.

## Edge Cases

- What happens to the class tasks when the class is deleted?

## Requirements

### Functional Requirements

- **FR-013:** The system MUST allow a teacher to create a class by providing a class name.
- **FR-014:** The system MUST allow a teacher to view the list of own classes.
- **FR-015:** The system MUST generate a unique 6-character join code for each class and show it to the teacher.
- **FR-023:** The system MUST allow a teacher to delete own class.

### Validation Rules

- **VT-023:** A class join code is unique among all classes.
- **VT-024:** Only a teacher can create a class, and the owner of a class is the teacher who created it.
- **VT-027:** When a student is removed from a class or a class is deleted, the student’s results are not deleted, and the teacher no longer sees them.
- **VT-030:** Email – up to 254 characters. Username – 3–30 characters (letters, digits, “\_”). Class name – 1–50 characters. Class code – 6 uppercase letters (A–Z) or digits; when entered, lowercase letters are converted to uppercase and spaces are removed. Evidence text – up to 500 characters. Photo – JPEG or PNG format, up to 5 MB, at most 3 per answer.

### Requirements verified here and implemented in other features

- **FR-024** (implemented in `003-access-control`): The system MUST prevent a teacher from viewing other teachers’ classes and the results of their students.
- **FR-053** (implemented in `006-confirmation-dialog`): The system MUST ask the user for confirmation before the actions listed in VT-029.
- **VT-029** (implemented in `006-confirmation-dialog`): The user is asked for confirmation before these actions: deleting a class, deleting a teacher’s task, removing a student from a class, changing a student’s class, blocking an account, deleting an account, leaving an unfinished task.

### Key Entities

- Class (Name, Code, OwnerId).

## Success Criteria

- **SC-001:** Requirement FR-013 is verified by the automated tests of this feature.
- **SC-002:** Requirement FR-014 is verified by the automated tests of this feature.
- **SC-003:** Requirement FR-015 is verified by the automated tests of this feature.
- **SC-004:** Requirement FR-023 is verified by the automated tests of this feature.

## Clarifications

**Session 2026-10-17**

- Q: What happens to the class tasks when the class is deleted? → A: The class tasks are deleted together with the class, the points earned by the students remain (VT-009, VT-018, VT-027).

No unresolved clarification markers remain.

## Assumptions

- The assumptions and constraints of `docs/requirements.md` (section 11) apply.

## Dependencies

- Features: 003-access-control, 006-confirmation-dialog.

## Scope Boundaries

**In scope:**
- Class creation and deletion.

**Out of scope:**
- Everything that belongs to the other features of the list in `docs/architecture.md` (section 6).

