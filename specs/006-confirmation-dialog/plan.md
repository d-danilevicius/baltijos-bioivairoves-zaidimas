# Implementation Plan: Confirmation dialog

**Feature:** `specs/006-confirmation-dialog/spec.md`
**Branch:** `006-confirmation-dialog`
**Status:** draft
**Depends on features:** 024-page-styling

## Summary

A shared frontend component `ConfirmDialog` is added. It shows a question and two buttons, calls the confirm action only after the confirm button, and is keyboard accessible. It has no server part and is not used on a screen yet; the later features use it before the actions of VT-029.

## Technical Context

- **Language and version:** TypeScript (frontend). No server change.
- **Libraries:** none added. The dialog uses the shared `Button` component and Tailwind CSS (feature 024).
- **Frontend:** React with Vite and React Router.
- **Storage:** no change.
- **Testing:** Vitest and React Testing Library.
- **Target platform:** web application in current Chrome, Firefox, Edge and Safari; screens from 360 px width.
- **Constraints:** none beyond the project rules.

## Structure and Main Components

- Frontend: `frontend/src/components/confirmDialog.tsx` with the component `ConfirmDialog` and the properties `open`, `message`, `confirmLabel` (default “Patvirtinti”), `cancelLabel` (default “Atšaukti”), `onConfirm`, `onCancel`.
- The dialog is a block with `role="dialog"`, `aria-modal="true"` and `aria-labelledby` that points to the question, over a backdrop that covers the page.
- When it opens, the focus is moved to the cancel button; Tab and Shift+Tab keep the focus between the buttons of the dialog; the Esc key calls `onCancel`; when it closes, the focus returns to the element that had it before.
- The panel has a maximum height of 90 % of the screen and scrolls vertically inside.

## Server Request Changes

- none (frontend component)

## Data Model Changes

- No data changes.
- No new migration.

## Testing Plan

- Test case: shows the question and the default button texts.
- Test case: shows the button texts set by the screen.
- Test case: confirm button calls `onConfirm` once and does not call `onCancel`.
- Test case: cancel button calls `onCancel` and does not call `onConfirm`.
- Test case: Esc key calls `onCancel`.
- Test case: the focus is on the cancel button when the dialog opens and returns to the previous element when it closes.
- Test case: Tab on the last button and Shift+Tab on the first button keep the focus inside the dialog.
- Test case: a closed dialog shows nothing.
- Test case: the dialog has the role `dialog` and is labelled by the question.

## Technical Notes

- The native `<dialog>` element is not used, because the test environment (jsdom) does not support its methods; a block with `role="dialog"` is simple and testable.
- Business rules are not in the component: it only reports the choice of the user.

## Constitution Check

- **1. Code quality:** complies. The component has one responsibility and stays below 200 lines; the file name `confirmDialog.tsx` is camelCase.
- **2. Architecture:** complies. There is no server part; the component does not decide what happens after the confirmation.
- **3. Testing:** complies. FR-001–FR-004 are covered by the tests in the testing plan.
- **4. Security:** complies. The dialog is only a convenience; the server of the later features still checks every action (the dialog is not an authorization).
- **5. Version control:** complies. The work is done in branch `006-confirmation-dialog`, created from an updated `main`, and enters through a Pull Request.
- **6. Documentation:** complies. The specification is written before the code; README and `docs/` are updated if the behaviour changes.
- **7. AI assistance:** complies. The code is reviewed and tested by the student; the Pull Request gets a separate AI review.
- **8. Simplicity:** complies. No new libraries or layers are added.

**No exceptions. The plan complies with the constitution.**
