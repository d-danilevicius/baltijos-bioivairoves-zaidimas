# Feature Specification: Class students and removal

**Feature folder:** `specs/009-class-students`
**Status:** draft
**Related project requirements:** FR-018, FR-022; VT-027
**Related screens:** E11 Class information (`docs/ux.md`)

**Why this feature is needed:** “I want to see the students of the class and remove one who joined by mistake so that only my students stay in the class.” (user story 1).

## User Scenarios and Acceptance Scenarios

### User Story 1 – Class students and removal (Priority: P2)

**User story:** As a teacher, I want to see the students of the class and remove one who joined by mistake so that only my students stay in the class.

**Independent verification:** A removed student disappears from the list.

**Acceptance scenarios:**

1. **Verification:** A removed student disappears from the list.

## Edge Cases

- No additional edge cases beyond the validation rules below.

## Requirements

### Functional Requirements

- **FR-018:** The system MUST allow a teacher to view the list of students of own class.
- **FR-022:** The system MUST allow a teacher to remove a student from own class.

### Validation Rules

- **VT-027:** When a student is removed from a class or a class is deleted, the student’s results are not deleted, and the teacher no longer sees them.

### Key Entities

- ClassMembership.

## Success Criteria

- **SC-001:** Requirement FR-018 is verified by the automated tests of this feature.
- **SC-002:** Requirement FR-022 is verified by the automated tests of this feature.

## Clarifications

**Session 2026-10-17**

- No questions were raised.

No unresolved clarification markers remain.

## Assumptions

- The assumptions and constraints of `docs/requirements.md` (section 11) apply.

## Dependencies

- Features: 003-access-control, 006-confirmation-dialog.

## Scope Boundaries

**In scope:**
- Class students and removal.

**Out of scope:**
- Everything that belongs to the other features of the list in `docs/architecture.md` (section 6).

