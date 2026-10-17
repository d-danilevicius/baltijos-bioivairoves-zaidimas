# Feature Specification: Student answers and evidence for the teacher

**Feature folder:** `specs/020-student-answers`
**Status:** draft
**Related project requirements:** FR-020; VT-022
**Related screens:** E16 Student results (`docs/ux.md`)

**Why this feature is needed:** “I want to see a student’s answers and evidence so that I understand the student’s decisions.” (user story 1).

## User Scenarios and Acceptance Scenarios

### User Story 1 – Student answers and evidence for the teacher (Priority: P2)

**User story:** As a teacher, I want to see a student’s answers and evidence so that I understand the student’s decisions.

**Independent verification:** The answers are shown with the label “Mokinio pateikti duomenys” (Student-submitted data).

**Acceptance scenarios:**

1. **Verification:** The answers are shown with the label “Mokinio pateikti duomenys” (Student-submitted data).

## Edge Cases

- No additional edge cases beyond the validation rules below.

## Requirements

### Functional Requirements

- **FR-020:** The system MUST allow a teacher to view the answers and evidence submitted by students of own class.

### Validation Rules

- **VT-022:** A student may view only own results, and a teacher only the results of students of own classes. An administrator does not see student results. Nobody can change saved results.

### Key Entities

- No new entities.

## Success Criteria

- **SC-001:** Requirement FR-020 is verified by the automated tests of this feature.

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
- Student answers and evidence for the teacher.

**Out of scope:**
- Everything that belongs to the other features of the list in `docs/architecture.md` (section 6).

