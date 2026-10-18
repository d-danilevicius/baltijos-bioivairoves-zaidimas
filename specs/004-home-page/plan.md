# Implementation Plan: Home page

**Feature:** `specs/004-home-page/spec.md`
**Branch:** `004-home-page`
**Status:** draft
**Depends on features:** none

## Summary

The plan describes how the feature “Home page” is implemented according to the approved specification: as a visitor, I want to see the home page so that I understand what this game is and find how to log in.

## Technical Context

- **Language and version:** C# with .NET 10 (server); TypeScript (frontend).
- **Libraries:** as selected in `docs/architecture.md` (section 1): ASP.NET Core Identity, Entity Framework Core with Npgsql, AutoMapper for non-trivial mappings.
- **Frontend:** React with Vite and React Router.
- **Storage:** PostgreSQL through Entity Framework Core; schema changes only through migrations.
- **Testing:** xUnit (unit and integration, PostgreSQL in Testcontainers), Vitest and React Testing Library.
- **Target platform:** web application in current Chrome, Firefox, Edge and Safari; screens from 360 px width.
- **Constraints:** HTTPS only; secrets are not stored in the repository.

## Structure and Main Components

- Server: no change. The existing `GET /api/auth/me` is used.
- Frontend: `HomePage` uses the existing `useCurrentUserSession` hook, the `LoadingState` and `ErrorState` components and `Navigate` of React Router. The texts are in `helpers/homeMessages.ts`. The styles are in `styles.css`.

## Server Request Changes

- None.

## Data Model Changes

- No data changes.
- No new migration.

## Testing Plan

- Frontend (Vitest and React Testing Library, `getCurrentUser` is mocked): visitor view, redirection of a logged-in user, loading state, error state with retry.
- Backend: no change, so no new backend test. The backend checks still run in CI.

## Technical Notes

- Session states: `loading` → loading state; `ready` → `<Navigate to="/profile" replace />`; `unauthenticated` → visitor view; `error` → error state with retry.

## Constitution Check

- **1. Code quality:** complies. Each file has one responsibility and stays below 200 lines; React file names follow the naming rules (page `HomePage.tsx`, helper `homeMessages.ts`).
- **2. Architecture:** complies. No backend change.
- **3. Testing:** complies. The requirements FR-010, FR-011 and FR-012 are covered by the tests in the testing plan.
- **4. Security:** complies. The redirection is only a convenience; the access to protected data is still checked on the server (feature 003). No secret is stored in Git.
- **5. Version control:** complies. The work is done in branch `004-home-page`, created from an updated `main`, and enters through a Pull Request.
- **6. Documentation:** complies. The specification is written before the code; README and `docs/` are updated if the behaviour changes.
- **7. AI assistance:** complies. The code is reviewed and tested by the student; the Pull Request gets a separate AI review.
- **8. Simplicity:** complies. No new libraries or layers are added; plain CSS is used.

**No exceptions. The plan complies with the constitution.**
