# Feature Specification: Photos with the evidence

**Feature folder:** `specs/015-photos`
**Status:** draft
**Related project requirements:** FR-040; NFR-006; VT-030
**Related screens:** E06 Task (`docs/ux.md`)

**Why this feature is needed:** “I want to add photos to the evidence so that I show what I noticed.” (user story 1).

## User Scenarios and Acceptance Scenarios

### User Story 1 – Photos with the evidence (Priority: P1)

**User story:** As a student, I want to add photos to the evidence so that I show what I noticed.

**Independent verification:** A JPEG photo is shown with the evidence; an oversized or wrong-format one is rejected.

**Acceptance scenarios:**

1. **Verification:** A JPEG photo is shown with the evidence; an oversized or wrong-format one is rejected.
2. **Negative:** **Given** a student, **when** the student tries to add more than 3 photos, **then** the submission is rejected.

## Edge Cases

- No additional edge cases beyond the validation rules below.

## Requirements

### Functional Requirements

- **FR-040:** The system MUST allow a student to attach photos to the evidence according to VT-030.

### Non-functional Requirements

- **NFR-006:** The system MUST remove location and other metadata (EXIF) from photos uploaded by students before saving them.

### Validation Rules

- **VT-030:** Email – up to 254 characters. Username – 3–30 characters (letters, digits, “\_”). Class name – 1–50 characters. Class code – 6 uppercase letters (A–Z) or digits; when entered, lowercase letters are converted to uppercase and spaces are removed. Evidence text – up to 500 characters. Photo – JPEG or PNG format, up to 5 MB, at most 3 per answer.

### Key Entities

- SubmissionPhoto.

## Success Criteria

- **SC-001:** Requirement FR-040 is verified by the automated tests of this feature.

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
- Photos with the evidence.

**Out of scope:**
- Everything that belongs to the other features of the list in `docs/architecture.md` (section 6).

