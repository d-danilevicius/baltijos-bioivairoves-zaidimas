# Feature Specification: Confirmation dialog

**Feature folder:** `specs/006-confirmation-dialog`
**Status:** draft
**Related project requirements:** FR-053; VT-029
**Related screens:** E21 Confirmation dialog (`docs/ux.md`)

**Why this feature is needed:** “I want to be asked for confirmation before an irreversible action so that I do not delete anything by accident.” (user story 1). The dialog is one shared component; the later features (007, 008, 009, 011, 014, 021, 022) use it before the actions listed in VT-029.

## User Scenarios and Acceptance Scenarios

### User Story 1 – Confirmation dialog (Priority: P1)

**User story:** As a user, I want to be asked for confirmation before an irreversible action so that I do not delete anything by accident.

**Independent verification:** The dialog shows a question and two buttons; after confirming the confirm action is called, after cancelling nothing changes.

**Acceptance scenarios:**

1. **Given** a screen that asks for confirmation, **when** the dialog is opened, **then** it shows the question and two buttons: the confirm button (“Patvirtinti” by default) and the cancel button (“Atšaukti” by default). The screen that uses the dialog may set other button texts (for example “Pakeisti” and “Atšaukti”, or “Išeiti” and “Likti”, see `docs/ux.md`).
2. **Given** an open dialog, **when** the user presses the confirm button, **then** the confirm action of the screen is called once and the dialog closes.
3. **Given** an open dialog, **when** the user presses the cancel button, **then** the confirm action is not called, the dialog closes and nothing else changes.
4. **Given** an open dialog, **when** the user presses the Esc key, **then** it is the same as pressing the cancel button.
5. **Given** a dialog that is opened, **when** it appears, **then** the keyboard focus is on the cancel button (the safe choice); **when** the dialog closes, **then** the focus returns to the element that had it before the dialog opened.
6. **Given** a dialog that is closed, **when** the screen is shown, **then** the dialog is not shown.
7. **Given** an open dialog, **when** the user presses Tab on the last button or Shift+Tab on the first button, **then** the focus moves to the other end of the dialog and never to the page behind it.

## Edge Cases

- A long question wraps inside the dialog and does not cause horizontal scrolling on a screen 360 px wide.
- While the dialog is open, the page behind it is covered and cannot be used with the mouse or the keyboard.
- A question that is taller than the screen (a short mobile screen or enlarged text) does not move the buttons out of reach: the dialog is not higher than the screen and scrolls inside.

## Requirements

### Functional Requirements

- **FR-053:** The system MUST ask the user for confirmation before the actions listed in VT-029. This feature provides the dialog; every later feature that has one of these actions uses it.
- **FR-001:** The dialog MUST show the question, a confirm button and a cancel button; the button texts have the defaults “Patvirtinti” and “Atšaukti” and can be set by the screen that uses the dialog.
- **FR-002:** The dialog MUST call the confirm action only when the user presses the confirm button; cancelling (the cancel button or the Esc key) MUST NOT call it.
- **FR-003:** The dialog MUST be keyboard accessible: the focus is moved to the cancel button when it opens, Tab and Shift+Tab keep the focus inside the dialog, Esc cancels, and the focus returns to the previous element when it closes (NFR-013).
- **FR-004:** The dialog MUST be marked as a dialog for assistive technologies (`role="dialog"`, `aria-modal="true"`, labelled by the question).

### Key Entities

- No new entities. The dialog has no server part and stores no data.

## Success Criteria

- **SC-001:** Requirements FR-001–FR-004 and the scenarios 1–6 are verified by automated frontend tests of the dialog.
- **SC-002:** Requirement FR-053 is verified in each later feature that uses the dialog.

## Clarifications

- Q: Where is the dialog used in this feature? → A: Nowhere yet. The actions of VT-029 do not exist yet; the later features add them and use the dialog. This feature only provides and tests the component.
- Q: Which button has the focus when the dialog opens? → A: The cancel button, so that pressing Enter by accident does not confirm an irreversible action.
- Q: Does the dialog trap the focus inside? → A: Yes. `aria-modal` does not make the page behind inert, so Tab and Shift+Tab move only between the two buttons of the dialog (changed after the code review of the Pull Request).
- Q: What happens when the question is taller than the screen? → A: The dialog has a maximum height of 90 % of the screen and scrolls inside, so the question and both buttons can be reached (added after the code review of the Pull Request).

No unresolved clarification markers remain.

## Assumptions

- The assumptions and constraints of `docs/requirements.md` (section 11) apply.
- The dialog uses the shared button components and the colours of feature 024-page-styling.

## Dependencies

- Features: 024-page-styling (shared buttons and colours).

## Scope Boundaries

**In scope:**
- The shared dialog component and its tests.

**Out of scope:**
- Using the dialog on a screen: it is done in the features that have the actions of VT-029.
- Server changes.
- Everything that belongs to the other features of the list in `docs/architecture.md` (section 6).
