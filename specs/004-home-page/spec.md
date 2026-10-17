# Feature Specification: Home page

**Feature folder:** `specs/004-home-page`
**Status:** draft
**Related project requirements:** FR-010, FR-011
**Related screens:** E01 Home page (`docs/ux.md`)

**Why this feature is needed:** “I want to see the home page so that I understand what this game is and find how to log in.” (user story 1).

## User Scenarios and Acceptance Scenarios

### User Story 1 – Home page (Priority: P1)

**User story:** As a visitor, I want to see the home page so that I understand what this game is and find how to log in.

**Independent verification:** A visitor sees the description and links; a logged-in user is redirected to the user’s starting screen.

**Acceptance scenarios:**

1. **Verification:** A visitor sees the description and links; a logged-in user is redirected to the user’s starting screen.

## Edge Cases

- What does a logged-in user see when opening the home page?

## Requirements

### Functional Requirements

- **FR-010:** The system MUST show a visitor who is not logged in a home page with the project description and the links “Prisijungti” (Log in) and “Registruotis” (Register).
- **FR-011:** The system MUST redirect a logged-in user who opens the home page to the user’s starting screen according to the role.

### Key Entities

- No new entities.

## Success Criteria

- **SC-001:** Requirement FR-010 is verified by the automated tests of this feature.
- **SC-002:** Requirement FR-011 is verified by the automated tests of this feature.

## Clarifications

**Session 2026-10-17**

- Q: What does a logged-in user see when opening the home page? → A: The user is redirected to the starting screen according to the role (FR-011).

No unresolved clarification markers remain.

## Assumptions

- The assumptions and constraints of `docs/requirements.md` (section 11) apply.

## Dependencies

- Features: none.

## Scope Boundaries

**In scope:**
- Home page.

**Out of scope:**
- Everything that belongs to the other features of the list in `docs/architecture.md` (section 6).

