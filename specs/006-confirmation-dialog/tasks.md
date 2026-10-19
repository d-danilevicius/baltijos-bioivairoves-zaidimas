# Tasks: Confirmation dialog

**Specification:** `specs/006-confirmation-dialog/spec.md`
**Plan:** `specs/006-confirmation-dialog/plan.md`
**Depends on features:** 024-page-styling
**Tags:** `[US1]` – belongs to user story 1; no tag – setup or verification task.

Do the tasks in order. Each phase depends on the previous one. Tests are written before the implementation.

## Data

- [ ] T001 Check that no server change, no schema change and no migration is needed for this feature (see the plan)

## Tests (written first) (depends on: Data)

- [ ] T002 [US1] Frontend tests of the dialog: texts, confirm, cancel, Esc key, focus, closed state, dialog role – `frontend/src/tests/ConfirmDialog.test.tsx`

## Implementation (depends on: Tests)

- [ ] T003 [US1] Build the component `ConfirmDialog` – `frontend/src/components/confirmDialog.tsx`

## Verification (depends on: Implementation)

- [ ] T004 Run `npm run lint`, `npm test` and `npm run build`
- [ ] T005 Update README and `docs/` if the behaviour, setup or configuration changed (expected: no change)
- [ ] T006 Review `git diff` and open the Pull Request with the separate AI review evidence
