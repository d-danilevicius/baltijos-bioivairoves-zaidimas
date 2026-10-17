# Feature Specification: Teacher tasks

**Feature folder:** `specs/011-teacher-tasks`
**Status:** draft
**Related project requirements:** FR-031, FR-032, FR-033, FR-034, FR-035; VT-018, VT-021
**Related screens:** E12 Class tasks, E14 Teacher task form (`docs/ux.md`)

**Why this feature is needed:** “I want to create my own task, for example about the school surroundings, so that I can add my own questions to the tasks.” (user story 1).

## User Scenarios and Acceptance Scenarios

### User Story 1 – Teacher tasks (Priority: P1)

**User story:** As a teacher, I want to create my own task, for example about the school surroundings, so that I can add my own questions to the tasks.

**Independent verification:** A saved task is visible in the class list with the label “Mokytojo užduotis” (Teacher task).

**Acceptance scenarios:**

1. **Verification:** A saved task is visible in the class list with the label “Mokytojo užduotis” (Teacher task).
2. **Successful:** **Given** a teacher, **when** the teacher fills in all task fields and ticks the confirmation, **then** the task is saved with the label “Mokytojo užduotis” (Teacher task).
3. **Negative:** **Given** a teacher, **when** the teacher does not tick the confirmation that the task does not require collecting or disturbing organisms, **then** the task is not saved.

## Edge Cases

- No additional edge cases beyond the validation rules below.

## Requirements

### Functional Requirements

- **FR-031:** The system MUST allow a teacher to create own task for own class by providing a title, a question, a correct answer and a difficulty group, for example about the school surroundings (VT-018).
- **FR-032:** The system MUST, before saving a teacher’s task, ask the teacher to confirm that the task does not require collecting, touching or disturbing organisms.
- **FR-033:** The system MUST label tasks created by a teacher “Mokytojo užduotis” (Teacher task) and never show them as GBIF observations.
- **FR-034:** The system MUST allow a teacher to delete a task the teacher created.
- **FR-035:** The system MUST allow a teacher to view the task list of own class, showing the type, difficulty group and label of each task.

### Validation Rules

- **VT-018:** A teacher’s task consists of a title (1–100 characters), a question (up to 500 characters), a correct answer (up to 100 characters) and a difficulty group chosen by the teacher. An answer is considered correct if it matches the correct answer, ignoring letter case and spaces at the beginning and end. A teacher may create and delete only tasks of own classes. When a task is deleted, the points already earned by students remain.
- **VT-021:** No task, including one created by a teacher, may require collecting, catching, touching or disturbing organisms; only observation from a distance is allowed.

### Key Entities

- ClassTask (Source = Teacher).

## Success Criteria

- **SC-001:** Requirement FR-031 is verified by the automated tests of this feature.
- **SC-002:** Requirement FR-032 is verified by the automated tests of this feature.
- **SC-003:** Requirement FR-033 is verified by the automated tests of this feature.
- **SC-004:** Requirement FR-034 is verified by the automated tests of this feature.
- **SC-005:** Requirement FR-035 is verified by the automated tests of this feature.

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
- Teacher tasks.

**Out of scope:**
- Everything that belongs to the other features of the list in `docs/architecture.md` (section 6).

