# Feature Specification: Class results

**Feature folder:** `specs/019-class-results`
**Status:** draft
**Related project requirements:** FR-019, FR-021; VT-022, VT-028
**Related screens:** E15 Class results (`docs/ux.md`)

**Why this feature is needed:** “I want to view the class results so that I see which tasks the class understands worst.” (user story 1).

## User Scenarios and Acceptance Scenarios

### User Story 1 – Class results (Priority: P1)

**User story:** As a teacher, I want to view the class results so that I see which tasks the class understands worst.

**Independent verification:** Tasks are ordered from the lowest average.

**Acceptance scenarios:**

1. **Verification:** Tasks are ordered from the lowest average.

## Edge Cases

- What does a teacher with several classes see?
- How is the task average calculated?

## Requirements

### Functional Requirements

- **FR-019:** The system MUST allow a teacher to view the points and completed tasks of each student of own class.
- **FR-021:** The system MUST show a teacher the class average score of each task and the tasks ordered from the lowest average.

### Validation Rules

- **VT-022:** A student may view only own results, and a teacher only the results of students of own classes. An administrator does not see student results. Nobody can change saved results.
- **VT-028:** The average result of a class task is calculated only from the best results of the current class members for that task. If no member has completed the task, the average is not shown.

### Key Entities

- No new entities.

## Success Criteria

- **SC-001:** Requirement FR-019 is verified by the automated tests of this feature.
- **SC-002:** Requirement FR-021 is verified by the automated tests of this feature.

## Clarifications

**Session 2026-10-17**

- Q: What does a teacher with several classes see? → A: The teacher chooses one class, and the results are shown separately.
- Q: How is the task average calculated? → A: From the best results of the current class members; if nobody has completed the task, the average is not shown (VT-028).

No unresolved clarification markers remain.

## Assumptions

- The assumptions and constraints of `docs/requirements.md` (section 11) apply.

## Dependencies

- Features: 003-access-control.

## Scope Boundaries

**In scope:**
- Class results.

**Out of scope:**
- Everything that belongs to the other features of the list in `docs/architecture.md` (section 6).

