# Tasks: Confirmation dialog

**Specification:** `specs/006-confirmation-dialog/spec.md`
**Plan:** `specs/006-confirmation-dialog/plan.md`
**Depends on features:** none
**Tags:** `[US1]` – belongs to user story 1; no tag – setup or verification task.

Do the tasks in order. Each phase depends on the previous one. Tests are written before the implementation.

## Data

- [ ] T001 Check that no schema change is needed for this feature (see the plan)

## Tests (written first) (depends on: Data)

- [ ] T002 [US1] Test case: confirm – `frontend/src/tests/confirmDialog.test.tsx`
- [ ] T003 [US1] Test case: cancel – `frontend/src/tests/confirmDialog.test.tsx`
- [ ] T004 [US1] Test case: keyboard control – `frontend/src/tests/confirmDialog.test.tsx`

## Implementation (depends on: Tests)

- [ ] T005 [US1] Create the confirmation dialog component with the buttons “Patvirtinti” (Confirm) and “Atšaukti” (Cancel), keyboard control and an accessible name – `frontend/src/components/confirmDialog.tsx`

## Verification (depends on: Implementation)

- [ ] T006 Run `dotnet build`, `dotnet test`, `npm run lint`, `npm test` and `npm run build`
- [ ] T007 Update README and `docs/` if the behaviour, setup or configuration changed
- [ ] T008 Review `git diff` and open the Pull Request with the separate AI review evidence
