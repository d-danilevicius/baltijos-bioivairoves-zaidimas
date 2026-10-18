# Implementation Plan: Access control and error pages

**Feature:** `specs/003-access-control/spec.md`
**Branch:** `003-access-control`
**Status:** draft
**Depends on features:** none

## Summary

The plan describes implementation of the foundation scope for access control and error pages: role-based server authorization, session restore/check endpoint `GET /api/auth/me`, and frontend protection/error-state foundations.

## Technical Context

- **Language and version:** C# with .NET 10 (server); TypeScript (frontend).
- **Libraries:** as selected in `docs/architecture.md` (section 1): ASP.NET Core Identity, Entity Framework Core with Npgsql, AutoMapper for non-trivial mappings.
- **Frontend:** React with Vite and React Router.
- **Storage:** PostgreSQL through Entity Framework Core; schema changes only through migrations.
- **Testing:** xUnit (unit and integration, PostgreSQL in Testcontainers), Vitest and React Testing Library.
- **Target platform:** web application in current Chrome, Firefox, Edge and Safari; screens from 360 px width.
- **Constraints:** HTTPS only; secrets are not stored in the repository.

## Structure and Main Components

- Server:
  - role policies for Student, Teacher and Administrator, applied on protected controllers/actions;
  - centralized 403 response “Prieiga uždrausta” for role mismatch;
  - `GET /api/auth/me` endpoint that returns `{ id, userName, role }` for valid session and 401 otherwise.
- Frontend:
  - `ProtectedRoute` using `/api/auth/me` for session restore and access checks;
  - `AccessDeniedPage` (E19) and `NotFoundPage` (E20);
  - shared loading, error (“Bandyti dar kartą”) and empty-state components;
  - `ProfilePage` reading current user via `/api/auth/me` instead of router state.

## Server Request Changes

- `GET /api/auth/me`
  - 200: current user `{ id, userName, role }` when session is valid.
  - 401: when no valid session exists.
- Apply role policies (Student, Teacher, Administrator) to protected controllers/actions in this scope.
- Return 403 with “Prieiga uždrausta” for authenticated users with insufficient role permissions.

## Data Model Changes

- No data changes.
- No new migration.

## Testing Plan

- Backend unit tests:
  - role policy matrix for Student/Teacher/Administrator protected access foundations.
- Backend integration tests:
  - `GET /api/auth/me` valid session -> 200 with `{ id, userName, role }`;
  - `GET /api/auth/me` no/expired session -> 401;
  - because no feature endpoint with a role requirement exists yet, a small test-only controller is registered in `backend/tests/Project.IntegrationTests/AccessControl/` (not in `Project.Api`), with three actions (Student, Teacher, Administrator policies), and tests call them as student, teacher, administrator, and without a session;
  - role mismatch on those policy actions -> 403 with “Prieiga uždrausta”; same policies are then applied by real feature controllers in their own features.
- Frontend tests:
  - `ProtectedRoute` no session flow -> redirect to login with “Prisijunkite, kad galėtumėte tęsti.”;
  - protected screen expired session flow -> inactivity message “Sesija baigėsi dėl neveiklumo. Prisijunkite iš naujo.”;
  - role mismatch flow -> E19;
  - unknown URL flow -> E20 with “Į pradžią” button;
  - shared loading/error/empty-state components rendering and retry action.

## Technical Notes

- This feature is limited to access-control foundations only; ownership policy logic remains in dedicated data features defined by architecture.
- Because no feature endpoint with role requirements exists yet, role policies are integration-tested through a small test-only controller registered in `backend/tests/Project.IntegrationTests/AccessControl/` (not in `Project.Api`), with one action per policy and calls as each role plus no-session.
- Real feature controllers apply the same policies in their own features.
- Business rules are placed in services and policies; controllers only translate HTTP.

## Constitution Check

- **1. Code quality:** complies. Each class has one responsibility and stays below 200 lines; React file names follow the naming rules.
- **2. Architecture:** complies. Controllers and routes use role policies and service boundaries; ownership checks are intentionally excluded from this feature and implemented in their dedicated features; `ApplicationDbContext` is used directly (no Repository); DTOs are used at the API boundary and AutoMapper for non-trivial mappings; there is no schema change.
- **3. Testing:** complies. This feature covers FR-009, FR-059, NFR-003, NFR-027, NFR-028, NFR-029 and foundation-level VT-022 behavior.
- **4. Security:** complies. Server validates session and role permissions for protected actions, returns 401/403 correctly, validates input on server, and stores no secrets in Git.
- **5. Version control:** complies. The work is done in branch `003-access-control`, created from an updated `main`, and enters through a Pull Request.
- **6. Documentation:** complies. The specification is written before the code; README and `docs/` are updated if the behaviour changes.
- **7. AI assistance:** complies. The code is reviewed and tested by the student; the Pull Request gets a separate AI review.
- **8. Simplicity:** complies. No new libraries or layers are added.

**No exceptions. The plan complies with the constitution.**

