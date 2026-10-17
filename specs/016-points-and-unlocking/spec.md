# Feature Specification: Points and group unlocking

**Feature folder:** `specs/016-points-and-unlocking`
**Status:** draft
**Related project requirements:** FR-041, FR-042, FR-048; NFR-015, NFR-018; VT-014, VT-016
**Related screens:** E07 Task result, E04 Task list (`docs/ux.md`)

**Why this feature is needed:** “I want to get points and unlock harder tasks so that I see my progress.” (user story 1).

## User Scenarios and Acceptance Scenarios

### User Story 1 – Points and group unlocking (Priority: P1)

**User story:** As a student, I want to get points and unlock harder tasks so that I see my progress.

**Independent verification:** After a correct answer the points are shown within 1 s; after earning the required share the next group appears.

**Acceptance scenarios:**

1. **Verification:** After a correct answer the points are shown within 1 s; after earning the required share the next group appears.
2. **Successful:** **Given** a student, **when** the student earns 60 % of the points of the first group, **then** the second group is unlocked.

## Edge Cases

- No additional edge cases beyond the validation rules below.

## Requirements

### Functional Requirements

- **FR-041:** The system MUST automatically award points for a submitted answer according to VT-016.
- **FR-042:** The system MUST unlock the next difficulty group when the student reaches the share of points set by VT-014.
- **FR-048:** The system MUST save the student’s points after each submitted task.

### Non-functional Requirements

- **NFR-015:** The system MUST show the earned points within no more than 1 second after an answer is submitted.
- **NFR-018:** The system MUST save the student’s points even when the connection is lost immediately after the answer is submitted.

### Validation Rules

- **VT-014:** The first difficulty group is unlocked for every student of the class. Each next group is unlocked when the student earns at least 60 % of the maximum possible points of the previous group.
- **VT-016:** A correct answer earns 10 points in a group 1 task, 20 in a group 2 task and 30 in a group 3 task; an incorrect answer earns 0 points. Correctness is determined from the observation card data, and in a teacher’s task from the correct answer set by the teacher.

### Key Entities

- Submission.Points.

## Success Criteria

- **SC-001:** Requirement FR-041 is verified by the automated tests of this feature.
- **SC-002:** Requirement FR-042 is verified by the automated tests of this feature.
- **SC-003:** Requirement FR-048 is verified by the automated tests of this feature.

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
- Points and group unlocking.

**Out of scope:**
- Everything that belongs to the other features of the list in `docs/architecture.md` (section 6).

