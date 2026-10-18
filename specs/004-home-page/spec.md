# Feature Specification: Home page

**Feature folder:** `specs/004-home-page`
**Status:** draft
**Related project requirements:** FR-010, FR-011
**Related screens:** E01 Home page (`docs/ux.md`)

**Why this feature is needed:** “I want to see the home page so that I understand what this game is and find how to log in.” (user story 1).

## User Scenarios and Acceptance Scenarios

### User Story 1 – Home page (Priority: P1)

**User story:** As a visitor, I want to see the home page so that I understand what this game is and find how to log in.

**Independent verification:** A visitor sees the description and the links “Prisijungti” and “Registruotis”; a logged-in user who opens the home page is redirected to the starting screen.

**Acceptance scenarios:**

1. **Given** a visitor who is not logged in, **when** the visitor opens `/`, **then** the page shows the project title, the project description and the links “Prisijungti” (to `/login`) and “Registruotis” (to `/register`).
2. **Given** a logged-in user, **when** the user opens `/`, **then** the user is redirected to the starting screen (`/profile`, see Assumptions).
3. **Given** the session check is still running, **when** the user opens `/`, **then** the page shows the loading state.
4. **Given** the session check fails with a server error or a network error, **when** the user opens `/`, **then** the page shows the error state with a retry button; **when** the user presses the retry button, **then** the session check is repeated.

## Edge Cases

- What does a logged-in user see when opening the home page? The user is redirected to the starting screen (FR-011).
- The server answers 401 (no session or the session has expired) → the user is treated as a visitor and sees the home page of a visitor.
- The server is not available → the error state with the retry button is shown instead of the visitor content.
- The page is opened on a narrow screen (360 px) → the text and the links remain readable.

## Requirements

### Functional Requirements

- **FR-010:** The system MUST show a visitor who is not logged in a home page with the project description and the links “Prisijungti” (Log in) and “Registruotis” (Register).
- **FR-011:** The system MUST redirect a logged-in user who opens the home page to the user’s starting screen according to the role.
- **FR-012:** The home page MUST show the loading state while the session is checked and the error state with a retry button when the check fails (loading and server-error states of the project rules).
- **FR-013:** The home page MUST use a basic readable layout with a text-to-background contrast of at least 4.5:1 (NFR-010) and MUST be usable from 360 px width.

### Key Entities

- No new entities.

## Success Criteria

- **SC-001:** Requirement FR-010 is verified by the automated tests of this feature.
- **SC-002:** Requirement FR-011 is verified by the automated tests of this feature.
- **SC-003:** Requirement FR-012 is verified by the automated tests of this feature.

## Clarifications

**Session 2026-10-17**

- Q: What does a logged-in user see when opening the home page? → A: The user is redirected to the starting screen according to the role (FR-011).

**Session 2026-10-18**

- Q: Which API call tells whether the user is logged in? → A: The existing `GET /api/auth/me` (feature 003-access-control). No new backend endpoint is needed.
- Q: What is the starting screen of each role now? → A: The role-specific screens (task list, class list, account list) belong to later features. Until they exist, the starting screen of every role is the profile page `/profile`.
- Q: What does the page show when the session check fails with a server error? → A: The error state with a retry button (FR-012).

No unresolved clarification markers remain.

## Assumptions

- The assumptions and constraints of `docs/requirements.md` (section 11) apply.
- The starting screen of every role is `/profile` until the later features add the role-specific starting screens.

## Dependencies

- Features: 003-access-control (`GET /api/auth/me` and the frontend session check).

## Scope Boundaries

**In scope:**
- Home page for visitors and the redirection of logged-in users.
- Basic styling of the home page.

**Out of scope:**
- Backend changes (no new endpoint, no migration).
- Role-specific starting screens.
- Everything that belongs to the other features of the list in `docs/architecture.md` (section 6).
