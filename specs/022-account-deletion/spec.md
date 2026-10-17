# Feature Specification: Account deletion

**Feature folder:** `specs/022-account-deletion`
**Status:** draft
**Related project requirements:** FR-057; NFR-007, NFR-023; VT-007, VT-008, VT-009
**Related screens:** E17 Account management (`docs/ux.md`)

**Why this feature is needed:** “I want to delete an account so that I remove data that are no longer needed.” (user story 1).

## User Scenarios and Acceptance Scenarios

### User Story 1 – Account deletion (Priority: P2)

**User story:** As an administrator, I want to delete an account so that I remove data that are no longer needed.

**Independent verification:** A deleted account cannot log in, and its data are removed.

**Acceptance scenarios:**

1. **Verification:** A deleted account cannot log in, and its data are removed.
2. **Successful:** **Given** an administrator, **when** the administrator deletes a teacher’s account, **then** the teacher’s classes and tasks are deleted and the students’ results remain.

## Edge Cases

- What happens when a teacher’s account is deleted?
- What happens when a student’s account is deleted?

## Requirements

### Functional Requirements

- **FR-057:** The system MUST allow an administrator to delete an account together with its results.

### Non-functional Requirements

- **NFR-007:** The system MUST, when an account is deleted, remove the data related to it according to VT-008 and VT-009.
- **NFR-023:** The system MUST record in a log logins, failed logins and administrator actions (date, time, user, action).

### Validation Rules

- **VT-007:** An administrator cannot block or delete their own account, and at least one administrator must always remain in the system.
- **VT-008:** When an account is deleted, all of its results, answers and photos are deleted with it.
- **VT-009:** When a teacher account is deleted, the teacher’s classes and their tasks are deleted, but student results remain. When a student account is deleted, the student is removed from the class and class averages are recalculated.

### Key Entities

- All related data.

## Success Criteria

- **SC-001:** Requirement FR-057 is verified by the automated tests of this feature.

## Clarifications

**Session 2026-10-17**

- Q: What happens when a teacher’s account is deleted? → A: Their classes and the classes’ tasks are deleted, the students’ results remain (VT-009).
- Q: What happens when a student’s account is deleted? → A: The results, answers and photos are deleted, the student is removed from the class, the averages are recalculated (VT-008, VT-009).

No unresolved clarification markers remain.

## Assumptions

- The assumptions and constraints of `docs/requirements.md` (section 11) apply.

## Dependencies

- Features: 003-access-control, 006-confirmation-dialog.

## Scope Boundaries

**In scope:**
- Account deletion.

**Out of scope:**
- Everything that belongs to the other features of the list in `docs/architecture.md` (section 6).

