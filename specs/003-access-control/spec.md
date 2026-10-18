# Feature Specification: Access control and error pages

**Feature folder:** `specs/003-access-control`
**Status:** draft
**Related project requirements:** FR-009, FR-059; NFR-003, NFR-027, NFR-028, NFR-029; VT-022
**Related screens:** E19 Access denied, E20 Page not found (`docs/ux.md`)

**Why this feature is needed:** “I want nobody else to reach my data and me not to reach other people’s data.” (user story 1).

## User Scenarios and Acceptance Scenarios

### User Story 1 – Access control and error pages (Priority: P1)

**User story:** As a user, I want nobody else to reach my data and me not to reach other people’s data.

**Independent verification:** A user without a valid session or without the required role cannot open protected screens and gets the correct server/frontend responses.

**Acceptance scenarios:**

1. **No session:** **Given** a user without a valid session, **when** the frontend checks a protected screen via `GET /api/auth/me`, **then** the endpoint returns status 401 and `ProtectedRoute` redirects to the login screen with the message “Prisijunkite, kad galėtumėte tęsti.”.
2. **Wrong role:** **Given** a logged-in user with a role that is not allowed for a protected action, **when** the user opens that screen or API endpoint, **then** the server returns status 403 with the error “Prieiga uždrausta” and the frontend shows E19 Access denied.
3. **Unknown URL:** **Given** any user, **when** the user opens a URL that does not exist, **then** the frontend shows E20 Page not found with the button “Į pradžią”.

## Edge Cases

- When a session expires while the user is on a protected screen, the user is redirected to login and sees “Sesija baigėsi dėl neveiklumo. Prisijunkite iš naujo.” (`docs/ux.md`, NFR-004).

## Requirements

### Functional Requirements

- **FR-009:** The system MUST prevent a user who is not logged in from opening task or result screens.
- **FR-059:** The system MUST prevent a student or a teacher from accessing administrator functions.
- The backend MUST provide `GET /api/auth/me` that returns current user `{ id, userName, role }` when the session is valid and returns 401 otherwise.
- The frontend MUST use `GET /api/auth/me` to restore session after page refresh and `ProfilePage` MUST load user data from this endpoint instead of router state.

### Non-functional Requirements

- **NFR-003:** The system MUST check the user’s role on the server before every protected action and, if the user has no permission, return the error “Prieiga uždrausta” (Access denied).
- **NFR-027:** When a screen does not exist, the system MUST show the message “Puslapis nerastas” (Page not found) and a button “Į pradžią” (To the home page).
- **NFR-028:** Every screen that loads data from the server MUST show a loading state until the data are received, and an error message with a button “Bandyti dar kartą” (Try again) if the data could not be received.
- **NFR-029:** Every list or table that may contain no records MUST show an empty-state message.

### Validation Rules

- **VT-022 (foundation coverage):** This feature covers only session and role access foundations for protected routes and endpoints; data ownership validation scenarios are implemented in their dedicated features.

### Key Entities

- **CurrentUser:** `id`, `userName`, `role` (response model for `GET /api/auth/me`).

## Success Criteria

- **SC-001:** Requirement FR-009 is verified by the automated tests of this feature (session missing/expired -> 401 and protected route redirect behavior).
- **SC-002:** Requirement FR-059 is verified by the automated tests of this feature (role mismatch -> 403 and E19 Access denied behavior).
- **SC-003:** Requirements NFR-027, NFR-028 and NFR-029 are verified by the automated tests of this feature for E20 and shared loading/error/empty states.

## Clarifications

**Session 2026-10-17**

- No questions were raised.

No unresolved clarification markers remain.

## Assumptions

- The assumptions and constraints of `docs/requirements.md` (section 11) apply.
- Ownership checks are not implemented in this feature and are moved to features where the data exist, as already defined in `docs/architecture.md`:
  - FR-024 -> features 007, 009, 011, 019, 020.
  - FR-052 -> features 018, 020.
  - FR-058 -> features 018, 019, 020.
- Ownership policies and tests for FR-024, FR-052 and FR-058 are implemented in those dedicated features.

## Dependencies

- Features: none.

## Scope Boundaries

**In scope:**
- Foundation-only access control: server role policies for Student, Teacher and Administrator; centralized 403 “Prieiga uždrausta”.
- `GET /api/auth/me` for session restore and protected-route session checks.
- Frontend foundations: E19 AccessDeniedPage, E20 NotFoundPage, `ProtectedRoute`, session-expiry inactivity message, `ProfilePage` integration with `/api/auth/me`, shared loading/error/empty-state components.

**Out of scope:**
- Ownership checks for FR-024, FR-052 and FR-058 in this feature; they are implemented with their policies and tests in the dedicated data features listed above.

