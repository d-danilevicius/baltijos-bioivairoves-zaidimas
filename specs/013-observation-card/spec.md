# Feature Specification: Observation card and protected species

**Feature folder:** `specs/013-observation-card`
**Status:** draft
**Related project requirements:** FR-043, FR-045, FR-046; VT-019, VT-020
**Related screens:** E06 Task (`docs/ux.md`)

**Why this feature is needed:** “I want to see an observation card with a clear label and a warning so that I know where the data come from and how to observe nature safely.” (user story 1).

## User Scenarios and Acceptance Scenarios

### User Story 1 – Observation card and protected species (Priority: P1)

**User story:** As a student, I want to see an observation card with a clear label and a warning so that I know where the data come from and how to observe nature safely.

**Independent verification:** The card shows the label, licence, link and warning, and the coordinates of a protected species are not shown.

**Acceptance scenarios:**

1. **Verification:** The card shows the label, licence, link and warning, and the coordinates of a protected species are not shown.
2. **Successful:** **Given** a student and a task with an observation of a protected species, **when** the student opens the task, **then** the exact coordinates are not shown and only the place name is shown.

## Edge Cases

- No additional edge cases beyond the validation rules below.

## Requirements

### Functional Requirements

- **FR-043:** The system MUST show on each observation card the label “Patvirtintas GBIF stebėjimas” (Confirmed GBIF observation), the dataset name, the licence and a link to the GBIF record.
- **FR-045:** The system MUST show on each task a warning to observe nature from a distance, not to collect or disturb organisms and not to photograph people.
- **FR-046:** The system MUST NOT show a student the exact coordinates of an observation of a protected species (VT-020).

### Validation Rules

- **VT-019:** A confirmed external observation is only an observation provided by the GBIF Occurrence API. Student-submitted data are always labelled as student-submitted and never become confirmed observations. Tasks created by a teacher are not GBIF observations and are labelled as teacher tasks.
- **VT-020:** A species is considered protected if GBIF gives it an IUCN Red List category of VU, EN or CR. The exact coordinates of such a species are not shown to a student; only the place name is shown.

### Key Entities

- Observation.

## Success Criteria

- **SC-001:** Requirement FR-043 is verified by the automated tests of this feature.
- **SC-002:** Requirement FR-045 is verified by the automated tests of this feature.
- **SC-003:** Requirement FR-046 is verified by the automated tests of this feature.

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
- Observation card and protected species.

**Out of scope:**
- Everything that belongs to the other features of the list in `docs/architecture.md` (section 6).

