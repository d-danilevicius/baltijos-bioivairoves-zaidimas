# Feature Specification: User profile

**Feature folder:** `specs/005-profile`
**Status:** draft
**Related project requirements:** FR-012; VT-005
**Related screens:** E18 Profile (`docs/ux.md`)

**Why this feature is needed:** “I want to view my profile so that I know what data about me are stored.” (user story 1).

## User Scenarios and Acceptance Scenarios

### User Story 1 – User profile (Priority: P2)

**User story:** As a logged-in user, I want to view my profile so that I know what data about me are stored.

**Independent verification:** The profile shows the username, email, role and class.

**Acceptance scenarios:**

1. **Verification:** The profile shows the username, email, role and class.

## Edge Cases

- No additional edge cases beyond the validation rules below.

## Requirements

### Functional Requirements

- **FR-012:** The system MUST allow a logged-in user to view own profile data: username, email, role and (for a student) class.

### Validation Rules

- **VT-005:** Every user has exactly one role: student, teacher or administrator.

### Key Entities

- No new entities.

## Success Criteria

- **SC-001:** Requirement FR-012 is verified by the automated tests of this feature.

## Clarifications

**Session 2026-10-17**

- No questions were raised.

No unresolved clarification markers remain.

## Assumptions

- The assumptions and constraints of `docs/requirements.md` (section 11) apply.

## Dependencies

- Features: 003-access-control.

## Scope Boundaries

**In scope:**
- User profile.

**Out of scope:**
- Everything that belongs to the other features of the list in `docs/architecture.md` (section 6).

