# Implementation Plan: Page styling

**Feature:** `specs/024-page-styling/spec.md`
**Branch:** `024-page-styling`
**Status:** draft
**Depends on features:** 001-registration, 002-login, 003-access-control, 004-home-page

## Summary

The plan describes how the existing screens get one shared look with Tailwind CSS without changing their behaviour.

## Technical Context

- **Language and version:** TypeScript (frontend); no backend change.
- **Libraries:** React with Vite and React Router (existing). New development dependencies: `tailwindcss` and `@tailwindcss/vite` (Tailwind CSS v4). They run at build time only. No other library is added.
- **Storage:** none.
- **Testing:** Vitest and React Testing Library.
- **Target platform:** web application in current Chrome, Firefox, Edge and Safari; screens from 360 px width.
- **Constraints:** contrast at least 4.5:1; errors shown by text, not only by colour.

## Structure and Main Components

- `frontend/vite.config.ts`: add the `@tailwindcss/vite` plugin.
- `frontend/src/styles.css`: `@import "tailwindcss";` and a `@theme` block with the colours (brand blue, amber accent, page background, error `#b00020`). The classes of feature 004 (`.home`, `.button`, `.error-text`) are removed.
- `components/pageLayout.tsx`: shared header with the title link and the menu, and a centered content column (used by every screen).
- `helpers/navigationLinks.ts`: the menu links of the login and registration screens (“Pagrindinis”, “Prisijungimas”, “Registracija”) and of the profile screen (“Profilis”). `PageLayout` shows a menu only when links are passed to it.
- `components/button.tsx`: button with the primary and secondary style.
- `components/linkButton.tsx`: React Router link with the same styles.
- `components/card.tsx`: card with a title for the forms and message screens.
- Pages: `HomePage`, `RegisterPage`, `LoginPage`, `ProfilePage` and the components `textField`, `roleChoice`, `errorState`, `loadingState`, `emptyState`, `messagePage` use the shared components and utility classes instead of inline styles. The registration, login and profile pages use `PageLayout`.

## Server Request Changes

- None.

## Data Model Changes

- No data changes.
- No new migration.

## Testing Plan

- New frontend test: the registration and login screens show the shared header (written first, fails before the change).
- The existing frontend tests must pass without changes of their expectations (FR-005).
- Visual check of every screen on desktop and on 360 px width; screenshots in the Pull Request.

## Technical Notes

- Inline `style` props are replaced by Tailwind utility classes.
- The error colour is defined once in the theme and used through the utility class `text-error`.

## Constitution Check

- **1. Code quality:** complies. Each component has one responsibility and stays below 200 lines; React file names follow the naming rules (pages in PascalCase, components in camelCase).
- **2. Architecture:** complies. No backend change.
- **3. Testing:** complies. The shared header is tested; the behaviour is covered by the existing tests.
- **4. Security:** complies. No change of data or authorization.
- **5. Version control:** complies. The work is done in branch `024-page-styling`, created from an updated `main`, and enters through a Pull Request.
- **6. Documentation:** complies. README and `docs/architecture.md` mention Tailwind CSS.
- **7. AI assistance:** complies. The code is reviewed and tested by the student; the Pull Request gets a separate AI review.
- **8. Simplicity:** complies with a documented exception: Tailwind CSS is a new library. The concrete need is in the specification (Clarifications). It is a development dependency and adds no runtime code.

**Exception: new development dependency Tailwind CSS. Justification: see the specification (Clarifications).**
