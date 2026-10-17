# Feature Specification: Task completion

**Feature folder:** `specs/014-task-completion`
**Status:** draft
**Related project requirements:** FR-038, FR-039, FR-044; VT-015, VT-019
**Related screens:** E06 Task, E07 Task result (`docs/ux.md`)

**Why this feature is needed:** “I want to submit an answer and evidence so that I get points.” (user story 1).

## User Scenarios and Acceptance Scenarios

### User Story 1 – Task completion (Priority: P1)

**User story:** As a student, I want to submit an answer and evidence so that I get points.

**Independent verification:** After submitting an answer and evidence the result is shown.

**Acceptance scenarios:**

1. **Verification:** After submitting an answer and evidence the result is shown.
2. **Negative:** **Given** a student, **when** the student tries to submit an answer to a locked task, **then** the action is rejected.

## Edge Cases

- Can a submitted answer be changed?
- What happens if the task is deleted while the student had it open?

## Requirements

### Functional Requirements

- **FR-038:** The system MUST allow a student to open an unlocked task.
- **FR-039:** The system MUST allow a student to submit an answer and text evidence in a task.
- **FR-044:** The system MUST label data submitted by students “Mokinio pateikti duomenys” (Student-submitted data) and never show them as confirmed observations.

### Validation Rules

- **VT-015:** A task is considered completed when the student has submitted an answer and evidence. A submitted answer cannot be changed.
- **VT-019:** A confirmed external observation is only an observation provided by the GBIF Occurrence API. Student-submitted data are always labelled as student-submitted and never become confirmed observations. Tasks created by a teacher are not GBIF observations and are labelled as teacher tasks.

### Key Entities

- Submission.

## Success Criteria

- **SC-001:** Requirement FR-038 is verified by the automated tests of this feature.
- **SC-002:** Requirement FR-039 is verified by the automated tests of this feature.
- **SC-003:** Requirement FR-044 is verified by the automated tests of this feature.

## Clarifications

**Session 2026-10-17**

- Q: Can a submitted answer be changed? → A: No (VT-015).
- Q: What happens if the task is deleted while the student had it open? → A: “Puslapis nerastas” (Page not found) is shown, no points are awarded (Work 2).

No unresolved clarification markers remain.

## Assumptions

- The assumptions and constraints of `docs/requirements.md` (section 11) apply.

## Dependencies

- Features: 003-access-control, 006-confirmation-dialog.

## Scope Boundaries

**In scope:**
- Task completion.

**Out of scope:**
- Everything that belongs to the other features of the list in `docs/architecture.md` (section 6).

