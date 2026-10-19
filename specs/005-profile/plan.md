# Implementation Plan: User profile

**Feature:** `specs/005-profile/spec.md`
**Branch:** `005-profile`
**Status:** draft
**Depends on features:** 003-access-control, 024-page-styling

## Summary

The endpoint `GET /api/auth/me` also returns the email of the logged-in user, and the profile screen shows it. The email is read from the session, so there is no new query and no schema change.

## Technical Context

- **Language and version:** C# with .NET 10 (server); TypeScript (frontend).
- **Libraries:** as selected in `docs/architecture.md` (section 1). No new library.
- **Frontend:** React with Vite and React Router; styles with Tailwind CSS (feature 024).
- **Storage:** no change.
- **Testing:** xUnit integration tests (PostgreSQL in Testcontainers), Vitest and React Testing Library.
- **Target platform:** web application in current Chrome, Firefox, Edge and Safari; screens from 360 px width.
- **Constraints:** HTTPS only; secrets are not stored in the repository.

## Structure and Main Components

- Server: `AuthController.Me` (existing) reads the email claim; `CurrentUserResponse` gets the field `Email`.
- Frontend: type `CurrentUser` gets the field `email`; `ProfilePage` shows it.

## Server Request Changes

- `GET /api/auth/me` – the response gets the field `email` (existing endpoint, existing 401 for no session).

## Data Model Changes

- No data changes.
- No new migration.

## Testing Plan

- Integration test: `GET /api/auth/me` with a valid session returns the email of the user.
- Existing integration tests of `GET /api/auth/me` (401 without a session, 401 after 31 minutes) stay unchanged.
- Frontend test: the profile screen shows the username, the email and the role.
- No unit test is added: there is no new business rule, the controller only reads the claims.

## Technical Notes

- ASP.NET Core Identity adds the email claim to the cookie at login, so `User.FindFirstValue(ClaimTypes.Email)` is used.
- A session created before this change has no email claim; `GET /api/auth/me` then returns 401 and the user logs in again (see the specification).

## Constitution Check

- **1. Code quality:** complies. No class grows above 200 lines; React file names follow the naming rules.
- **2. Architecture:** complies. The controller only translates the claims to the response DTO; there is no schema change and no Repository.
- **3. Testing:** complies. FR-001 and FR-002 are covered by the tests in the testing plan.
- **4. Security:** complies. The endpoint is protected with `[Authorize]`; a user gets only own data; no secret is stored in Git.
- **5. Version control:** complies. The work is done in branch `005-profile`, created from an updated `main`, and enters through a Pull Request.
- **6. Documentation:** complies. The specification is written before the code; README and `docs/` are updated.
- **7. AI assistance:** complies. The code is reviewed and tested by the student; the Pull Request gets a separate AI review.
- **8. Simplicity:** complies. No new libraries or layers are added.

**No exceptions. The plan complies with the constitution.**
