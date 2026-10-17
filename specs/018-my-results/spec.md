# Feature Specification: My results

**Feature folder:** `specs/018-my-results`
**Status:** draft
**Related project requirements:** FR-050; VT-022
**Related screens:** E08 My results (`docs/ux.md`)

**Why this feature is needed:** “I want to view my results so that I see my progress.” (user story 1).

## User Scenarios and Acceptance Scenarios

### User Story 1 – My results (Priority: P1)

**User story:** As a student, I want to view my results so that I see my progress.

**Independent verification:** After a page reload my points and results are visible.

**Acceptance scenarios:**

1. **Verification:** After a page reload my points and results are visible.

## Edge Cases

- No additional edge cases beyond the validation rules below.

## Requirements

### Functional Requirements

- **FR-050:** The system MUST allow a student to view own points and the results of completed tasks.

### Validation Rules

- **VT-022:** A student may view only own results, and a teacher only the results of students of own classes. An administrator does not see student results. Nobody can change saved results.

### Key Entities

- No new entities.

## Success Criteria

- **SC-001:** Requirement FR-050 is verified by the automated tests of this feature.

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
- My results.

**Out of scope:**
- Everything that belongs to the other features of the list in `docs/architecture.md` (section 6).

