# Feature Specification: Student task list

**Feature folder:** `specs/012-student-task-list`
**Status:** draft
**Related project requirements:** FR-037, FR-049; NFR-009; VT-013, VT-026
**Related screens:** E04 Task list (`docs/ux.md`)

**Why this feature is needed:** “I want to see the class tasks by difficulty groups and continue from an uncompleted one so that I know what to do next.” (user story 1).

## User Scenarios and Acceptance Scenarios

### User Story 1 – Student task list (Priority: P1)

**User story:** As a student, I want to see the class tasks by difficulty groups and continue from an uncompleted one so that I know what to do next.

**Independent verification:** Tasks are shown in three groups; locked groups cannot be opened.

**Acceptance scenarios:**

1. **Verification:** Tasks are shown in three groups; locked groups cannot be opened.

## Edge Cases

- What does a student without a class see?

## Requirements

### Functional Requirements

- **FR-037:** The system MUST show a student the tasks of own class arranged by difficulty groups (VT-013).
- **FR-049:** The system MUST allow a student, after logging out and logging in again, to continue from the first unfinished unlocked task.

### Non-functional Requirements

- **NFR-009:** The system MUST allow a new student who already belongs to a class to start the first task in no more than 3 clicks after logging in.

### Validation Rules

- **VT-013:** There are four task types and three difficulty groups: group 1 – species group recognition and a question from an observation card; group 2 – features of a common local species; group 3 – comparison of seasonal observations.
- **VT-026:** A student without a class can log in and view the profile but does not see tasks until joining a class.

### Key Entities

- ClassTask, Submission (read).

## Success Criteria

- **SC-001:** Requirement FR-037 is verified by the automated tests of this feature.
- **SC-002:** Requirement FR-049 is verified by the automated tests of this feature.

## Clarifications

**Session 2026-10-17**

- Q: What does a student without a class see? → A: No tasks are shown, and joining a class is suggested (VT-026).

No unresolved clarification markers remain.

## Assumptions

- The assumptions and constraints of `docs/requirements.md` (section 11) apply.

## Dependencies

- Features: 003-access-control.

## Scope Boundaries

**In scope:**
- Student task list.

**Out of scope:**
- Everything that belongs to the other features of the list in `docs/architecture.md` (section 6).

