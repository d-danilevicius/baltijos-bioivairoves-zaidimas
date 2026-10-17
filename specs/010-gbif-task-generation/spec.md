# Feature Specification: Task creation from GBIF observations

**Feature folder:** `specs/010-gbif-task-generation`
**Status:** draft
**Related project requirements:** FR-025, FR-026, FR-027, FR-028, FR-029, FR-030, FR-036, FR-047; NFR-002, NFR-016, NFR-020, NFR-021; VT-010, VT-011, VT-012, VT-013, VT-021
**Related screens:** E13 Task creation from GBIF, E12 Class tasks (`docs/ux.md`)

**Why this feature is needed:** “I want to choose a location and a season so that the class gets tasks from real GBIF observations near the school.” (user story 1).

## User Scenarios and Acceptance Scenarios

### User Story 1 – Task creation from GBIF observations (Priority: P1)

**User story:** As a teacher, I want to choose a location and a season so that the class gets tasks from real GBIF observations near the school.

**Independent verification:** With a fake GBIF response tasks of four types are created.

**Acceptance scenarios:**

1. **Verification:** With a fake GBIF response tasks of four types are created.
2. **Successful:** **Given** a teacher, and GBIF returns at least 10 suitable observations, **when** the teacher chooses a location in Lithuania and a season, **then** tasks are created and a copy of the observations is stored.
3. **Negative:** **Given** a teacher, and GBIF returns fewer than 10 suitable observations, **when** the teacher tries to create tasks, **then** the tasks are not created and a message is shown.
4. **Negative:** **Given** a teacher, and GBIF does not respond within 10 seconds, **when** the teacher tries to create tasks, **then** an error with “Bandyti dar kartą” (Try again) is shown and incomplete tasks are not created.

## Edge Cases

- Can the GBIF tasks of a class be created a second time?
- Which species is considered protected?
- What happens when GBIF does not respond?

## Requirements

### Functional Requirements

- **FR-025:** The system MUST allow a teacher to choose a location in Lithuania (for example, the school surroundings) and a season for own class.
- **FR-026:** The system MUST retrieve from the GBIF Occurrence API biodiversity observations near the chosen location in the chosen season according to VT-011.
- **FR-027:** The system MUST create the task “Atpažink rūšių grupę” (Recognise the species group) from a retrieved observation.
- **FR-028:** The system MUST create the task “Rask dažnos vietinės rūšies požymių” (Find features of a common local species) from the retrieved observations of that location.
- **FR-029:** The system MUST create the task “Palygink sezoninius stebėjimus” (Compare seasonal observations) from observations of the same location in the chosen season and another season.
- **FR-030:** The system MUST create the task “Atsakyk į klausimą” (Answer a question) from an observation card.
- **FR-036:** The system MUST NOT create tasks and MUST show the teacher a message if GBIF returns fewer observations than VT-011 requires.
- **FR-047:** The system MUST NOT create tasks that require collecting, touching or disturbing organisms (VT-021).

### Non-functional Requirements

- **NFR-002:** The system MUST transfer data between the browser, the server and GBIF only over a secure HTTPS connection.
- **NFR-016:** The system MUST create the class tasks within no more than 30 seconds after the location and season are confirmed.
- **NFR-020:** If GBIF does not respond within 10 seconds or returns an error, the system MUST show the teacher an error message with a button “Bandyti dar kartą” (Try again) and MUST NOT create incomplete tasks.
- **NFR-021:** The system MUST store a copy of the GBIF observations used for tasks, so that the tasks do not change when GBIF data change.

### Validation Rules

- **VT-010:** The GBIF tasks of one class are created for one location within the territory of Lithuania and one season, both chosen by the teacher of the class.
- **VT-011:** Only GBIF observations that have coordinates and a species name and are no farther than 10 km from the chosen location are used. Tasks are created only if at least 10 such observations are found.
- **VT-012:** Seasons are defined by months: spring – March to May, summer – June to August, autumn – September to November, winter – December to February.
- **VT-013:** There are four task types and three difficulty groups: group 1 – species group recognition and a question from an observation card; group 2 – features of a common local species; group 3 – comparison of seasonal observations.
- **VT-021:** No task, including one created by a teacher, may require collecting, catching, touching or disturbing organisms; only observation from a distance is allowed.

### Key Entities

- Observation, ClassTask, Class (location, season).

## Success Criteria

- **SC-001:** Requirement FR-025 is verified by the automated tests of this feature.
- **SC-002:** Requirement FR-026 is verified by the automated tests of this feature.
- **SC-003:** Requirement FR-027 is verified by the automated tests of this feature.
- **SC-004:** Requirement FR-028 is verified by the automated tests of this feature.
- **SC-005:** Requirement FR-029 is verified by the automated tests of this feature.
- **SC-006:** Requirement FR-030 is verified by the automated tests of this feature.
- **SC-007:** Requirement FR-036 is verified by the automated tests of this feature.
- **SC-008:** Requirement FR-047 is verified by the automated tests of this feature.

## Clarifications

**Session 2026-10-17**

- Q: Can the GBIF tasks of a class be created a second time? → A: No. A class has one location and one season (VT-010), so the button “Sukurti iš GBIF” (Create from GBIF) is not shown.
- Q: Which species is considered protected? → A: A species for which GBIF gives the IUCN category VU, EN or CR (VT-020).
- Q: What happens when GBIF does not respond? → A: An error with “Bandyti dar kartą” (Try again) is shown, incomplete tasks are not created (NFR-020).

No unresolved clarification markers remain.

## Assumptions

- The assumptions and constraints of `docs/requirements.md` (section 11) apply.

## Dependencies

- Features: 003-access-control.

## Scope Boundaries

**In scope:**
- Task creation from GBIF observations.

**Out of scope:**
- Everything that belongs to the other features of the list in `docs/architecture.md` (section 6).

