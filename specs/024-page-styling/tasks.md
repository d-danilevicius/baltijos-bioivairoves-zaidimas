# Tasks: Page styling

**Specification:** `specs/024-page-styling/spec.md`
**Plan:** `specs/024-page-styling/plan.md`
**Depends on features:** 001-registration, 002-login, 003-access-control, 004-home-page
**Tags:** `[US1]` – belongs to user story 1; no tag – setup or verification task.

Do the tasks in order. Each phase depends on the previous one. Tests are written before the implementation.

## Data

- [x] T001 Check that no schema change and no backend change is needed for this feature (see the plan)

## Tests (written first) (depends on: Data)

- [x] T002 [US1] Frontend test: the registration and login screens show the shared header with the project title and the menu (home, login, registration), and the current screen is marked – `frontend/src/tests/PageLayout.test.tsx`

## Implementation (depends on: Tests)

- [x] T003 Install `tailwindcss` and `@tailwindcss/vite`, add the plugin and the theme (`frontend/vite.config.ts`, `frontend/src/styles.css`, `frontend/package.json`)
- [x] T004 [US1] Build the shared components `pageLayout`, `button`, `linkButton`, `card` and the helper `buttonStyles` – `frontend/src/components/`
- [x] T005 [US1] Style the registration and login screens and the components `textField` and `roleChoice` – `frontend/src/pages/`, `frontend/src/components/`
- [x] T006 [US1] Style the home, profile, access denied and page not found screens and the components `errorState`, `loadingState`, `emptyState`, `messagePage` – `frontend/src/pages/`, `frontend/src/components/`

## Verification (depends on: Implementation)

- [x] T007 Run `npm run lint`, `npm test` and `npm run build`; the existing tests pass without changes of their expectations
- [ ] T008 Check every screen on desktop and on 360 px width and take screenshots
- [x] T009 Update README and `docs/architecture.md` (Tailwind CSS in the frontend technologies)
- [ ] T010 Review `git diff` and open the Pull Request with the separate AI review evidence
