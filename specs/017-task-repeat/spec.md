# Feature Specification: Task repetition

**Feature folder:** `specs/017-task-repeat`
**Status:** draft
**Related project requirements:** FR-051; VT-017
**Related screens:** E07 Task result (`docs/ux.md`)

**Why this feature is needed:** “I want to repeat a task so that I improve my result.” (user story 1).

## User Scenarios and Acceptance Scenarios

### User Story 1 – Task repetition (Priority: P2)

**User story:** As a student, I want to repeat a task so that I improve my result.

**Independent verification:** After repeating, the higher result remains.

**Acceptance scenarios:**

1. **Verification:** After repeating, the higher result remains.

## Edge Cases

- Which result remains after a task is repeated?

## Requirements

### Functional Requirements

- **FR-051:** The system MUST allow a student to repeat a completed task.

### Validation Rules

- **VT-017:** When a task is repeated, the result of the task is the highest number of points achieved.

### Key Entities

- Submission.

## Success Criteria

- **SC-001:** Requirement FR-051 is verified by the automated tests of this feature.

## Clarifications

**Session 2026-10-17**

- Q: Which result remains after a task is repeated? → A: The highest achieved (VT-017).

No unresolved clarification markers remain.

## Assumptions

- The assumptions and constraints of `docs/requirements.md` (section 11) apply.

## Dependencies

- Features: 003-access-control.

## Scope Boundaries

**In scope:**
- Task repetition.

**Out of scope:**
- Everything that belongs to the other features of the list in `docs/architecture.md` (section 6).

