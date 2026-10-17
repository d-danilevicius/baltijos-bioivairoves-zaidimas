# Feature Specification: Account list and blocking

**Feature folder:** `specs/021-account-blocking`
**Status:** draft
**Related project requirements:** FR-054, FR-055, FR-056; NFR-023; VT-006, VT-007
**Related screens:** E17 Account management (`docs/ux.md`)

**Why this feature is needed:** “I want to block and unblock accounts so that I can limit improper use.” (user story 1).

## User Scenarios and Acceptance Scenarios

### User Story 1 – Account list and blocking (Priority: P1)

**User story:** As an administrator, I want to block and unblock accounts so that I can limit improper use.

**Independent verification:** A blocked account cannot log in; after unblocking it can.

**Acceptance scenarios:**

1. **Verification:** A blocked account cannot log in; after unblocking it can.
2. **Negative:** **Given** an administrator, **when** the administrator tries to block their own account, **then** the action is rejected.

## Edge Cases

- No additional edge cases beyond the validation rules below.

## Requirements

### Functional Requirements

- **FR-054:** The system MUST allow an administrator to view the list of accounts.
- **FR-055:** The system MUST allow an administrator to block an account.
- **FR-056:** The system MUST allow an administrator to unblock an account.

### Non-functional Requirements

- **NFR-023:** The system MUST record in a log logins, failed logins and administrator actions (date, time, user, action).

### Validation Rules

- **VT-006:** A blocked user cannot log in until an administrator unblocks the account.
- **VT-007:** An administrator cannot block or delete their own account, and at least one administrator must always remain in the system.

### Key Entities

- ApplicationUser.IsBlocked, ActionLog.

## Success Criteria

- **SC-001:** Requirement FR-054 is verified by the automated tests of this feature.
- **SC-002:** Requirement FR-055 is verified by the automated tests of this feature.
- **SC-003:** Requirement FR-056 is verified by the automated tests of this feature.

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
- Account list and blocking.

**Out of scope:**
- Everything that belongs to the other features of the list in `docs/architecture.md` (section 6).

