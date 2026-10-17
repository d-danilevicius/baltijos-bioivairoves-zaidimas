# Feature Specification: Login and logout

**Feature folder:** `specs/002-login`
**Status:** draft
**Related project requirements:** FR-005, FR-006, FR-007, FR-008; NFR-004, NFR-023, NFR-024; VT-006
**Related screens:** E03 Login, E18 Profile (logout) (`docs/ux.md`)

**Why this feature is needed:** “I want to log in and log out so that nobody else sees my data.” (user story 1).

## User Scenarios and Acceptance Scenarios

### User Story 1 – Login and logout (Priority: P1)

**User story:** As a registered user, I want to log in and log out so that nobody else sees my data.

**Independent verification:** Correct data let the user in, incorrect data are rejected, after logout the screens are not accessible.

**Acceptance scenarios:**

1. **Verification:** Correct data let the user in, incorrect data are rejected, after logout the screens are not accessible.

## Edge Cases

- What to show when the login data are incorrect?
- Is the number of failed logins limited?

## Requirements

### Functional Requirements

- **FR-005:** The system MUST allow a registered user to log in by providing a correct email address and password.
- **FR-006:** The system MUST reject a login with incorrect credentials and show a general error message that does not indicate which field is incorrect.
- **FR-007:** The system MUST prevent a blocked user from logging in and show a message about the block.
- **FR-008:** The system MUST allow a logged-in user to log out.

### Non-functional Requirements

- **NFR-004:** The system MUST automatically log a user out after 30 minutes of inactivity.
- **NFR-023:** The system MUST record in a log logins, failed logins and administrator actions (date, time, user, action).
- **NFR-024:** The system MUST NOT write passwords to logs.

### Validation Rules

- **VT-006:** A blocked user cannot log in until an administrator unblocks the account.

### Key Entities

- ApplicationUser.IsBlocked, ActionLog.

## Success Criteria

- **SC-001:** Requirement FR-005 is verified by the automated tests of this feature.
- **SC-002:** Requirement FR-006 is verified by the automated tests of this feature.
- **SC-003:** Requirement FR-007 is verified by the automated tests of this feature.
- **SC-004:** Requirement FR-008 is verified by the automated tests of this feature.

## Clarifications

**Session 2026-10-17**

- Q: What to show when the login data are incorrect? → A: A general message that does not say which field is incorrect (FR-006). A blocked account gets a separate message (FR-007).
- Q: Is the number of failed logins limited? → A: No, the first version has no such rule.

No unresolved clarification markers remain.

## Assumptions

- The assumptions and constraints of `docs/requirements.md` (section 11) apply.

## Dependencies

- Features: none.

## Scope Boundaries

**In scope:**
- Login and logout.

**Out of scope:**
- Everything that belongs to the other features of the list in `docs/architecture.md` (section 6).

