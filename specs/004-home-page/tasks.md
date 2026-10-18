# Tasks: Home page

**Specification:** `specs/004-home-page/spec.md`
**Plan:** `specs/004-home-page/plan.md`
**Depends on features:** 003-access-control
**Tags:** `[US1]` – belongs to user story 1; no tag – setup or verification task.

Do the tasks in order. Each phase depends on the previous one. Tests are written before the implementation.

## Data

- [x] T001 Check that no schema change and no backend change is needed for this feature (see the plan)

## Tests (written first) (depends on: Data)

- [x] T002 [US1] Frontend test: a visitor sees the description and the links “Prisijungti” and “Registruotis” – `frontend/src/tests/HomePage.test.tsx`
- [x] T003 [US1] Frontend test: a logged-in user is redirected to `/profile` – `frontend/src/tests/HomePage.test.tsx`
- [x] T004 [US1] Frontend test: the loading state is shown while the session is checked – `frontend/src/tests/HomePage.test.tsx`
- [x] T005 [US1] Frontend test: the error state with the retry button is shown when the session check fails – `frontend/src/tests/HomePage.test.tsx`

## Implementation (depends on: Tests)

- [x] T006 [US1] Add the Lithuanian texts of the home page – `frontend/src/helpers/homeMessages.ts`
- [x] T007 [US1] Build HomePage with the session check and the redirection – `frontend/src/pages/HomePage.tsx`
- [x] T008 [US1] Add the basic styles (contrast at least 4.5:1, usable from 360 px) and import them – `frontend/src/styles.css`, `frontend/src/main.tsx`

## Verification (depends on: Implementation)

- [x] T009 Run `npm run lint`, `npm test` and `npm run build` (the backend is not changed; CI runs the backend checks)
- [x] T010 Update README and `docs/` if the behaviour, setup or configuration changed
- [x] T011 Fix the contrast of error messages after the AI review: shared class `error-text` (`#b00020`) instead of inline `red` – `frontend/src/styles.css` and the components that show errors
- [ ] T012 Review `git diff` and open the Pull Request with the separate AI review evidence
