# Feature Specification: Confirmation dialog

**Feature folder:** `specs/006-confirmation-dialog`
**Status:** draft
**Related project requirements:** FR-053; VT-029
**Related screens:** E21 Confirmation dialog (`docs/ux.md`)

**Why this feature is needed:** “I want to be asked for confirmation before an irreversible action so that I do not delete anything by accident.” (user story 1).

## User Scenarios and Acceptance Scenarios

### User Story 1 – Confirmation dialog (Priority: P1)

**User story:** As a user, I want to be asked for confirmation before an irreversible action so that I do not delete anything by accident.

**Independent verification:** The dialog has “Patvirtinti” (Confirm) and “Atšaukti” (Cancel); after cancelling nothing changes.

**Acceptance scenarios:**

1. **Verification:** The dialog has “Patvirtinti” (Confirm) and “Atšaukti” (Cancel); after cancelling nothing changes.

## Edge Cases

- No additional edge cases beyond the validation rules below.

## Requirements

### Functional Requirements

- **FR-053:** The system MUST ask the user for confirmation before the actions listed in VT-029.

### Validation Rules

- **VT-029:** The user is asked for confirmation before these actions: deleting a class, deleting a teacher’s task, removing a student from a class, changing a student’s class, blocking an account, deleting an account, leaving an unfinished task.

### Key Entities

- No new entities.

## Success Criteria

- **SC-001:** Requirement FR-053 is verified by the automated tests of this feature.

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
- Confirmation dialog.

**Out of scope:**
- Everything that belongs to the other features of the list in `docs/architecture.md` (section 6).

