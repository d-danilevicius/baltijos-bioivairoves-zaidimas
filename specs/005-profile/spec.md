# Feature Specification: User profile

**Feature folder:** `specs/005-profile`
**Status:** draft
**Related project requirements:** FR-012; VT-005
**Related screens:** E18 Profile (`docs/ux.md`)

**Why this feature is needed:** “I want to view my profile so that I know what data about me are stored.” (user story 1).

## User Scenarios and Acceptance Scenarios

### User Story 1 – User profile (Priority: P2)

**User story:** As a logged-in user, I want to view my profile so that I know what data about me are stored.

**Independent verification:** The profile shows the username, email and role of the logged-in user.

**Acceptance scenarios:**

1. **Given** a logged-in user, **when** the user opens `/profile`, **then** the page shows the username, the email and the role.
2. **Given** a logged-in user, **when** the server returns the current user (`GET /api/auth/me`), **then** the response contains the id, the username, the email and the role of that user only.
3. **Given** a visitor who is not logged in, **when** `GET /api/auth/me` is called, **then** the server returns 401.

## Edge Cases

- A long username or a long email wraps inside the card and does not cause horizontal scrolling.

## Requirements

### Functional Requirements

- **FR-012:** The system MUST allow a logged-in user to view own profile data: username, email and role. The class of a student is shown by feature 008-join-class, because the classes do not exist yet.
- **FR-001:** `GET /api/auth/me` MUST return the email of the logged-in user in the field `email`, together with the existing fields `id`, `userName` and `role`.
- **FR-002:** The profile screen MUST show the email under the label “El. paštas”.

### Key Entities

- No new entities. The email is an existing data of the user (feature 001-registration).

### Validation Rules

- **VT-005:** Every user has exactly one role (already stored by feature 001-registration; the profile only shows it).

## Success Criteria

- **SC-001:** Requirements FR-001 and the scenarios 2 and 3 are verified by backend integration tests.
- **SC-002:** Requirement FR-002 and the scenario 1 are verified by a frontend test of the profile screen.

## Clarifications

- Q: Where does the email come from? → A: From the session (the email claim that ASP.NET Core Identity adds at login); there is no new query and no schema change.
- Q: What happens to sessions that were created before this change and have no email claim? → A: `GET /api/auth/me` returns 401 and the user logs in again.

No unresolved clarification markers remain.

## Assumptions

- The assumptions and constraints of `docs/requirements.md` (section 11) apply.

## Dependencies

- Features: 003-access-control, 024-page-styling.

## Scope Boundaries

**In scope:**
- Showing the email in `GET /api/auth/me` and on the profile screen.

**Out of scope:**
- Editing the profile, changing the email or the password.
- The class of a student (feature 008-join-class).
- Everything that belongs to the other features of the list in `docs/architecture.md` (section 6).
