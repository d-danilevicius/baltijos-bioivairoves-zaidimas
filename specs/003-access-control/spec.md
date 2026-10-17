# Feature Specification: Access control and error pages

**Feature folder:** `specs/003-access-control`
**Status:** draft
**Related project requirements:** FR-009, FR-024, FR-052, FR-058, FR-059; NFR-003, NFR-027, NFR-028, NFR-029; VT-022
**Related screens:** E19 Access denied, E20 Page not found (`docs/ux.md`)

**Why this feature is needed:** “I want nobody else to reach my data and me not to reach other people’s data.” (user story 1).

## User Scenarios and Acceptance Scenarios

### User Story 1 – Access control and error pages (Priority: P1)

**User story:** As a user, I want nobody else to reach my data and me not to reach other people’s data.

**Independent verification:** A student who opens a teacher or administrator page sees “Prieiga uždrausta” (Access denied).

**Acceptance scenarios:**

1. **Verification:** A student who opens a teacher or administrator page sees “Prieiga uždrausta” (Access denied).
2. **Negative:** **Given** a teacher, **when** the teacher tries to open the class of another teacher, **then** access is rejected.
3. **Negative:** **Given** a student, **when** the student tries to open the results of another student, **then** access is rejected.
4. **Negative:** **Given** an administrator, **when** the administrator tries to view or change a student’s points, answers or evidence, **then** the action is rejected.
5. **Negative:** **Given** a student or a teacher, **when** the user tries to perform an administrator action, **then** the action is rejected.

## Edge Cases

- No additional edge cases beyond the validation rules below.

## Requirements

### Functional Requirements

- **FR-009:** The system MUST prevent a user who is not logged in from opening task or result screens.
- **FR-024:** The system MUST prevent a teacher from viewing other teachers’ classes and the results of their students.
- **FR-052:** The system MUST prevent a student from viewing or changing the results of another student.
- **FR-058:** The system MUST prevent an administrator from viewing or changing student points, answers and evidence.
- **FR-059:** The system MUST prevent a student or a teacher from accessing administrator functions.

### Non-functional Requirements

- **NFR-003:** The system MUST check the user’s role on the server before every protected action and, if the user has no permission, return the error “Prieiga uždrausta” (Access denied).
- **NFR-027:** When a screen does not exist, the system MUST show the message “Puslapis nerastas” (Page not found) and a button “Į pradžią” (To the home page).
- **NFR-028:** Every screen that loads data from the server MUST show a loading state until the data are received, and an error message with a button “Bandyti dar kartą” (Try again) if the data could not be received.
- **NFR-029:** Every list or table that may contain no records MUST show an empty-state message.

### Validation Rules

- **VT-022:** A student may view only own results, and a teacher only the results of students of own classes. An administrator does not see student results. Nobody can change saved results.

### Key Entities

- No new entities.

## Success Criteria

- **SC-001:** Requirement FR-009 is verified by the automated tests of this feature.
- **SC-002:** Requirement FR-024 is verified by the automated tests of this feature.
- **SC-003:** Requirement FR-052 is verified by the automated tests of this feature.
- **SC-004:** Requirement FR-058 is verified by the automated tests of this feature.
- **SC-005:** Requirement FR-059 is verified by the automated tests of this feature.

## Clarifications

**Session 2026-10-17**

- No questions were raised.

No unresolved clarification markers remain.

## Assumptions

- The assumptions and constraints of `docs/requirements.md` (section 11) apply.

## Dependencies

- Features: none.

## Scope Boundaries

**In scope:**
- Access control and error pages.

**Out of scope:**
- Everything that belongs to the other features of the list in `docs/architecture.md` (section 6).

