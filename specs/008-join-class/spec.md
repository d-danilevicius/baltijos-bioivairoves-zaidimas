# Feature Specification: Joining a class with a code

**Feature folder:** `specs/008-join-class`
**Status:** draft
**Related project requirements:** FR-016, FR-017; VT-025, VT-030
**Related screens:** E05 Join a class (`docs/ux.md`)

**Why this feature is needed:** “I want to join a class with a code so that I see the tasks of my class.” (user story 1).

## User Scenarios and Acceptance Scenarios

### User Story 1 – Joining a class with a code (Priority: P1)

**User story:** As a student, I want to join a class with a code so that I see the tasks of my class.

**Independent verification:** After entering the code the student is visible in the class and sees its tasks.

**Acceptance scenarios:**

1. **Verification:** After entering the code the student is visible in the class and sees its tasks.
2. **Successful:** **Given** a student who already belongs to a class, **when** the student enters the code of another class and confirms, **then** the previous membership ends.

## Edge Cases

- How is the class code format checked?
- What happens when the student is already in another class?

## Requirements

### Functional Requirements

- **FR-016:** The system MUST allow a student to join a class by entering a valid class code.
- **FR-017:** The system MUST reject a non-existent class code and show an error message; the code is checked according to VT-030.

### Validation Rules

- **VT-025:** A student can belong to only one class at a time. When joining a new class, the previous membership ends.
- **VT-030:** Email – up to 254 characters. Username – 3–30 characters (letters, digits, “\_”). Class name – 1–50 characters. Class code – 6 uppercase letters (A–Z) or digits; when entered, lowercase letters are converted to uppercase and spaces are removed. Evidence text – up to 500 characters. Photo – JPEG or PNG format, up to 5 MB, at most 3 per answer.

### Key Entities

- ClassMembership.

## Success Criteria

- **SC-001:** Requirement FR-016 is verified by the automated tests of this feature.
- **SC-002:** Requirement FR-017 is verified by the automated tests of this feature.

## Clarifications

**Session 2026-10-17**

- Q: How is the class code format checked? → A: 6 uppercase letters or digits; lowercase letters are converted to uppercase and spaces are removed (VT-030). Otherwise the class is not found.
- Q: What happens when the student is already in another class? → A: Confirmation is requested; after confirmation the previous membership ends (VT-025, VT-029).

No unresolved clarification markers remain.

## Assumptions

- The assumptions and constraints of `docs/requirements.md` (section 11) apply.

## Dependencies

- Features: 003-access-control, 006-confirmation-dialog.

## Scope Boundaries

**In scope:**
- Joining a class with a code.

**Out of scope:**
- Everything that belongs to the other features of the list in `docs/architecture.md` (section 6).

