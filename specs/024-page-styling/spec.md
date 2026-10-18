# Feature Specification: Page styling

**Feature folder:** `specs/024-page-styling`
**Status:** draft
**Related project requirements:** NFR-008, NFR-010, NFR-013. This feature has no FR from `docs/requirements.md`; it comes from the need to make the existing screens look consistent and readable.
**Related screens:** E01 Home page, E02 Registration, E03 Login, Profile, Access denied, Page not found (`docs/ux.md`)

**Why this feature is needed:** The registration, login and profile screens have no design, and only the home page has basic styles. The screens must look like one application and meet the requirements for layout, contrast and keyboard use.

## User Scenarios and Acceptance Scenarios

### User Story 1 – Consistent look of the existing screens (Priority: P1)

**User story:** As a user, I want all screens to look the same and be easy to read, so that the application is pleasant and clear to use.

**Independent verification:** The home, registration, login, profile, access denied and page not found screens show the same header and the same content column; forms are shown in a card; buttons and links look the same on every screen; the behaviour of all screens is unchanged.

**Acceptance scenarios:**

1. **Header:** **Given** any existing screen, **when** the screen is opened, **then** it shows the same header with the project title.
2. **Forms:** **Given** the registration or login screen, **when** the screen is opened, **then** the form is shown in a card with labelled fields and a primary button.
3. **Errors:** **Given** a validation or server error, **when** it is shown, **then** it is shown as text (not only by colour) with the colour `#b00020` and is announced as an alert, as before.
4. **Keyboard:** **Given** any button or link, **when** the user moves with Tab, **then** a visible focus outline is shown.
5. **Narrow screen:** **Given** a screen 360 px wide, **when** any existing screen is opened, **then** there is no horizontal scrolling.
6. **Behaviour:** **Given** the existing screens, **when** they are used, **then** their behaviour is the same as before this feature (the existing frontend tests pass without changes of their expectations).
7. **Menu:** **Given** a visitor on the login or registration screen, **when** the screen is opened, **then** the header shows the menu “Pagrindinis”, “Prisijungimas” and “Registracija”, and the link of the current screen is marked as the current page; **when** the user selects a menu link or the project title, **then** the matching screen opens. The home page has no menu (it has the description and the buttons “Prisijungti” and “Registruotis”). A logged-in user on the profile screen sees the menu “Profilis” (the menu is extended by the later features).
8. **Centering:** **Given** any screen, **when** it is opened, **then** the header text and the menu and the card are centered horizontally and the card is centered in the space below the header.

## Edge Cases

- A long error text or a long field label wraps to the next line and does not overflow the card.
- A long username on the profile screen wraps inside the card and does not cause horizontal scrolling.
- The loading state and the error state of the home page and of protected pages use the same layout as the other screens.

## Requirements

### Functional Requirements

- **FR-001:** All existing screens MUST use one shared layout: a header with the project title and a centered content column.
- **FR-002:** The registration and login forms MUST be shown in a card with labelled fields and the shared primary button.
- **FR-003:** Buttons and links that look like buttons MUST use shared components with a primary and a secondary style and a visible focus outline (NFR-013).
- **FR-004:** The text-to-background contrast of all texts MUST be at least 4.5:1 (NFR-010) and the screens MUST be usable from 360 px width without horizontal scrolling (NFR-008).
- **FR-005:** The behaviour, the existing texts and the API calls of the screens MUST NOT change.
- **FR-006:** The shared header MUST show a menu with the links “Pagrindinis”, “Prisijungimas” and “Registracija” on the login and registration screens and “Profilis” on the profile screen, mark the current screen and link the project title to the home page, so that the user can move between the screens without typing an address. The home page, the “Page not found” screen and the “Access denied” screen have no menu.
- **FR-007:** The shared layout MUST center the header text, the menu and the content card horizontally, and the card vertically in the space below the header.
- **FR-008:** The login screen MUST show the text “Dar neturite paskyros?” with a link “Registruotis” to the registration screen.

### Key Entities

- No new entities.

## Success Criteria

- **SC-001:** Requirement FR-001 is verified by the automated tests of this feature (the shared header with the menu is shown on the registration and login screens).
- **SC-002:** Requirement FR-005 is verified by the existing frontend tests, which pass without changes of their expectations (two queries are changed from a text to the heading role, because the menu link “Prisijungimas” has the same text as the heading).
- **SC-004:** Requirement FR-006 is verified by the automated tests of this feature.
- **SC-005:** Requirement FR-008 is verified by an automated test of the login screen.
- **SC-003:** Requirements FR-002–FR-004 are checked visually with screenshots in the Pull Request.

## Clarifications

**Session 2026-10-18**

- Q: How does the user move between the screens? → A: The login and registration screens have a menu in the header (FR-006): “Pagrindinis”, “Prisijungimas”, “Registracija”; the profile screen has “Profilis”. The home page keeps the description and the two buttons and has no menu. The later features add their links to the menu according to the role.
- Q: How are the styles written? → A: With Tailwind CSS (utility classes) through the Vite plugin. The need: the screens share many repeated style values (spacing, colours, buttons); Tailwind keeps them in one place (the theme in `styles.css` and shared components) and replaces the inline styles. It is a build-time development dependency and adds no runtime library.
- Q: Which colour is used for error texts? → A: The theme colour `error` = `#b00020` (contrast 6.83:1 on the page background), replacing the class `error-text` of feature 004.
- Q: Is a dark theme needed? → A: No.

No unresolved clarification markers remain.

## Assumptions

- The assumptions and constraints of `docs/requirements.md` (section 11) apply.
- The design is simple: a dark blue header with an amber line, white cards and a muted (low-saturation) background photo of a reef, taken from Pexels (Pexels License). The text is always on the white cards or on the header, so the contrast of the texts does not depend on the photo.

## Dependencies

- Features: 001-registration, 002-login, 003-access-control, 004-home-page (the screens that are styled).

## Scope Boundaries

**In scope:**
- Tailwind CSS setup, shared layout, shared button components, forms in cards for the existing screens.

**Out of scope:**
- Changes of behaviour, texts, routes or API calls.
- Dark theme, animations and logo.
- Screens of the features 005–022 (they use the shared components when they are built).
