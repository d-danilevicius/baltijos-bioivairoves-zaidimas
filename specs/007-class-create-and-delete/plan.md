# Implementation Plan: Class creation and deletion

**Feature:** `specs/007-class-create-and-delete/spec.md`
**Branch:** `007-class-create-and-delete`
**Status:** draft
**Depends on features:** 003-access-control, 006-confirmation-dialog, 024-page-styling

## Summary

A teacher creates a class with a name, the server generates a unique 6-character code, the teacher sees the list of own classes and one class, and can delete the class after a confirmation. The server enforces the roles and the ownership. The frontend gets three screens (class list, new class, class information), the teacher’s starting screen and the menu link “Klasės”.

## Technical Context

- **Language and version:** C# with .NET 10 (server); TypeScript (frontend).
- **Libraries:** as selected in `docs/architecture.md` (section 1). No new library. The mapping from the entity to the response is trivial (three fields) and is done in the query with `Select`; AutoMapper is not added for it.
- **Frontend:** React with Vite, React Router and Tailwind CSS; the dialog of feature 006.
- **Storage:** PostgreSQL through Entity Framework Core; the schema change is made by the migration `CreateClassesTable`.
- **Testing:** xUnit (unit and integration, PostgreSQL in Testcontainers), Vitest and React Testing Library.
- **Target platform:** web application in current Chrome, Firefox, Edge and Safari; screens from 360 px width.
- **Constraints:** HTTPS only; POST and DELETE use the existing CSRF protection; secrets are not stored in the repository.

## Structure and Main Components

- Server:
  - `Entities/SchoolClass.cs` – the entity (named `SchoolClass` to avoid the keyword `class`; the table is `Classes`): `Id`, `Name`, `Code`, `OwnerId`, `CreatedAtUtc`.
  - `Data/ApplicationDbContext.cs` – `DbSet<SchoolClass> Classes`, name up to 50 characters, code of 6 characters with a unique index, the owner is a foreign key to the user (a deleted teacher deletes the classes, VT-009).
  - `Services/IClassCodeGenerator.cs`, `Services/ClassCodeGenerator.cs` – generates 6 random characters A–Z and 0–9 (one responsibility, replaceable in tests).
  - `Services/IClassService.cs`, `Services/ClassService.cs` – creates (validates and trims the name, generates the code, repeats with a new code when the unique index rejects it), lists, gets and deletes classes of the owner; returns a result with the outcome `Succeeded`, `ValidationFailed`, `NotFound` or `Forbidden`.
  - `Services/ClassResult.cs` – the result type.
  - `Controllers/ClassesController.cs` – `[Authorize(Policy = PolicyNames.Teacher)]`, translates the result to 200, 201, 204, 400, 403 and 404 (the 403 body is the existing `AccessDeniedResponse`).
  - `DTOs/Requests/CreateClassRequest.cs`, `DTOs/Responses/ClassResponse.cs`.
  - `Services/ServiceCollectionExtensions.cs` – registers the two services.
- Frontend:
  - `types/classes.ts`, `api/classesApi.ts` – types and calls (list, get, create, delete; CSRF token for POST and DELETE as in the logout call).
  - `pages/ClassListPage.tsx`, `pages/NewClassPage.tsx`, `pages/ClassDetailsPage.tsx`.
  - `helpers/validateClassName.ts`, `helpers/classMessages.ts` – the validation and the texts.
  - `helpers/startingScreen.ts` – the starting screen by role (teacher `/classes`, others `/profile`), used by `LoginPage` and `HomePage`.
  - `helpers/navigationLinks.ts` – the menu by role (teacher: “Klasės”, “Profilis”), used by the profile and class pages.
  - `App.tsx` – routes `/classes`, `/classes/new`, `/classes/:id` inside `ProtectedRoute allowedRoles={['Teacher']}`.

## Server Request Changes

- `GET /api/classes` – the classes of the logged-in teacher (200).
- `POST /api/classes` – creates a class (201, 400 with the errors per field, 401, 403).
- `GET /api/classes/{id}` – one class of the owner (200, 401, 403, 404).
- `DELETE /api/classes/{id}` – deletes a class of the owner (204, 401, 403, 404).

## Data Model Changes

- New table `Classes` (`Id`, `Name`, `Code`, `OwnerId`, `CreatedAtUtc`), unique index on `Code`, foreign key `OwnerId` to the users with cascade delete.
- New migration `CreateClassesTable` in the same branch.

## Testing Plan

- Unit tests (`ClassCodeGenerator`): the code has 6 characters; the characters are only A–Z and 0–9.
- Unit tests (`ClassService`): the name is trimmed; an empty name and a name of 51 characters are rejected; a name of 50 characters is accepted; the owner comes from the argument; when the code already exists another code is generated; a non-owner gets `Forbidden`; a missing class gets `NotFound`.
- Integration tests: a teacher creates a class and gets 201 with a 6-character code; the list shows only own classes; a student gets 403 on every endpoint; another teacher gets 403 on get and delete; a visitor gets 401; delete of an own class gives 204 and the class is no longer in the list; delete of a missing class gives 404; an invalid name gives 400 with the error for the field `name`.
- Frontend tests: the class list shows the classes and the empty state with “Sukurti pirmą klasę”; the new class form shows the two validation errors and the server error, and opens the class information after success; the class information shows the code and deletes only after the confirmation (cancel changes nothing); a teacher is sent to `/classes` after login and from the home page; a student is sent to `/profile`; the teacher menu shows “Klasės”; a student opening `/classes` sees “Prieiga uždrausta”.

## Technical Notes

- The code is generated by the server only; the request has no field for it or for the owner.
- A collision of the code is handled by the unique index: the service catches the database error of the index and tries another code (a small fixed number of times).
- The class tasks and the student memberships do not exist yet; their foreign keys will be added with cascade delete in the later features.

## Constitution Check

- **1. Code quality:** complies. Each class has one responsibility and stays below 200 lines; React file names follow the naming rules.
- **2. Architecture:** complies. `ClassesController` only calls `IClassService`; the rules are in the service; `ApplicationDbContext` is used directly (no Repository); DTOs are used at the API boundary; the schema change has a migration.
- **3. Testing:** complies. FR-013–FR-015, FR-023 and FR-001–FR-005 are covered by the tests in the testing plan, including the authorization tests.
- **4. Security:** complies. Role and ownership are checked on the server for all four endpoints; the owner is taken from the session; all input is validated on the server; no secret is stored in Git.
- **5. Version control:** complies. The work is done in branch `007-class-create-and-delete`, created from an updated `main`, and enters through a Pull Request.
- **6. Documentation:** complies. The specification is written before the code; README and `docs/` are updated.
- **7. AI assistance:** complies. The code is reviewed and tested by the student; the Pull Request gets a separate AI review.
- **8. Simplicity:** complies. No new libraries or layers; two small services.

**No exceptions. The plan complies with the constitution.**
