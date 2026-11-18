# Implementation Plan: Joining a class with a code

**Feature:** `specs/008-join-class/spec.md`
**Branch:** `008-join-class`
**Status:** draft
**Depends on features:** 003-access-control, 006-confirmation-dialog, 007-class-create-and-delete, 024-page-styling

## Summary

A student enters a class code, the server normalizes and checks it, and the student becomes a member of the class. When the student already belongs to another class, the server asks for confirmation first, and after the confirmation the previous membership ends. The frontend gets the join screen (E05), the confirmation dialog of feature 006 and a menu link for the student; after joining, the student stays on the profile screen with the success message.

## Technical Context

- **Language and version:** C# with .NET 10 (server); TypeScript (frontend).
- **Libraries:** as selected in `docs/architecture.md` (section 1). No new library. The response has one or two fields, so it is created in the service; AutoMapper is not added for it.
- **Frontend:** React with Vite, React Router and Tailwind CSS; the dialog of feature 006.
- **Storage:** PostgreSQL through Entity Framework Core; the schema change is made by the migration `CreateClassMembershipsTable`.
- **Testing:** xUnit (unit and integration, PostgreSQL in Testcontainers), Vitest and React Testing Library.
- **Target platform:** web application in current Chrome, Firefox, Edge and Safari; screens from 360 px width.
- **Constraints:** HTTPS only; POST uses the existing CSRF protection; secrets are not stored in the repository.

## Structure and Main Components

- Server:
  - `Entities/ClassMembership.cs` – the entity: `Id`, `StudentId`, `ClassId`, `StartedAtUtc`, `EndedAtUtc` (empty while the student belongs to the class).
  - `Data/Configurations/ClassMembershipConfiguration.cs` – table `ClassMemberships`; foreign keys to the user and to `Classes` with cascade delete (a deleted class or student account removes the memberships, VT-009); a unique index on `StudentId` where `EndedAtUtc` is empty, so a student has one active membership (VT-025).
  - `DTOs/Requests/JoinClassRequest.cs` – `Code` (spaces removed and letters converted to uppercase when the value is set; `Required` and a 6-character `A–Z0–9` pattern) and `ConfirmChange` (the student confirmed the class change).
  - `DTOs/Responses/JoinClassResponse.cs` – the name of the class and `AlreadyMember`; `DTOs/Responses/ClassChangeConfirmationResponse.cs` – the names of the current and the new class.
  - `Services/IClassMembershipService.cs`, `Services/ClassMembershipService.cs` – finds the class by the code; returns `ClassNotFound`, `AlreadyMember`, `ConfirmationRequired` or `Joined`; on `Joined` ends the active membership (if any) and adds the new one in one save; when the unique index rejects the save (two requests at the same time), it evaluates the request again once.
  - `Services/JoinClassResult.cs`, `Services/JoinClassOutcome.cs` – the result type, same style as `ClassResult`.
  - `Controllers/ClassMembershipController.cs` – route `api/classes/join`, `[Authorize(Policy = PolicyName.Student)]`; the student is taken from the session; translates the result to 200, 400 (error for the field `code` when the class is not found), 409 and the existing 401/403 responses.
  - `Services/ServiceCollectionExtensions.cs` – registers the service.
- Frontend:
  - `types/classMembership.ts`, `api/classMembershipApi.ts` – types and the join call (CSRF token as in the other POST calls); the result types use `HttpStatus`.
  - `pages/JoinClassPage.tsx` – the form, the error next to the field, the confirmation dialog; on success opens `/profile` with the message.
  - `helpers/validateClassCode.ts` – normalization (uppercase, spaces removed) and the two screen errors.
  - `helpers/joinClassMessage.ts` – the texts („Prisijungėte prie klasės „7a“.“, „Jau priklausote šiai klasei.“, the dialog question).
  - `helpers/navigationLink.ts` – a separate menu of a student (“Prisijungti prie klasės”, “Profilis”).
  - `pages/ProfilePage.tsx` – shows the success message received from the join screen.
  - `App.tsx` – route `/join` inside `ProtectedRoute allowedRoles={['Student']}`.

## Server Request Changes

- `POST /api/classes/join` – body `{ code, confirmChange }`:
  - 200 – joined (or already a member of this class); the body has the class name and `alreadyMember` (true when the student was already in this class);
  - 400 – empty or wrong code format, or the class is not found; the errors are per field `code`;
  - 409 – the student belongs to another class and `confirmChange` is false; the body has the current and the new class name;
  - 401 – visitor; 403 – teacher or administrator.

## Data Model Changes

- New table `ClassMemberships` (`Id`, `StudentId`, `ClassId`, `StartedAtUtc`, `EndedAtUtc`), foreign keys with cascade delete, a unique filtered index on `StudentId` for the active membership.
- New migration `CreateClassMembershipsTable` in the same branch.

## Testing Plan

- Unit tests (`ClassMembershipService`): joining a class creates a membership; a code of a missing class returns `ClassNotFound`; the same class returns `AlreadyMember` and changes nothing; another class without confirmation returns `ConfirmationRequired`; another class with confirmation ends the previous membership and starts the new one; a student never has two active memberships.
- Unit tests (`JoinClassRequest`): lowercase letters become uppercase; spaces are removed; an empty code, 5 characters, 7 characters and a code with a symbol are rejected; a valid code is accepted.
- Integration tests: a student joins and gets 200; a code with lowercase letters and spaces works; an unknown code gives 400 for the field `code`; a wrong format gives 400; a class change without `confirmChange` gives 409 and changes nothing; with `confirmChange` gives 200 and the old membership ends; the same class gives 200 and nothing changes; a teacher and an administrator get 403; a visitor gets 401; a deleted class gives 400; two requests of the same student at the same time leave one active membership.
- Frontend tests: the join form shows the empty and the format errors; the server error “class not found” is shown next to the field; the confirmation dialog opens on 409 with both class names, “Pakeisti” sends the request again with the confirmation and “Atšaukti” sends nothing; success opens the profile with the message; the message „Jau priklausote šiai klasei.“ is shown; the student menu shows the join link; a teacher opening `/join` sees “Prieiga uždrausta”; the status 401 opens the login page with the session message.

## Technical Notes

- The student and the time come from the session and `TimeProvider`; the request has no field for them.
- The code is normalized in the request DTO, the same way `CreateClassRequest` trims the name, so the validation applies to the normalized code; the frontend uses the same rules only for the usability of the form.
- `confirmChange` protects the student from an accidental change (VT-029); it is not a security boundary, because a student can change only own membership. The security is the role policy, the CSRF protection and taking the student from the session. The answer 409 gives the screen the name of the current class without a second request.
- Ended memberships are kept with `EndedAtUtc`; the active one is the row without it.
- The task list (feature 012) does not exist yet, so after joining the student stays on `/profile`.

## Constitution Check

- **1. Code quality:** complies. Each class has one responsibility and stays below 200 lines; React file names follow the naming rules.
- **2. Architecture:** complies. `ClassMembershipController` only calls `IClassMembershipService`; the rules are in the service; `ApplicationDbContext` is used directly (no Repository); DTOs are used at the API boundary; the schema change has a migration.
- **3. Testing:** complies. FR-016, FR-017, FR-053 and FR-001–FR-005 are covered by the tests in the testing plan, including the authorization tests.
- **4. Security:** complies. The role is checked on the server; the student is taken from the session; the code and the confirmation are validated on the server; no secret is stored in Git.
- **5. Version control:** complies. The work is done in branch `008-join-class`, created from an updated `main`, and enters through a Pull Request.
- **6. Documentation:** complies. The specification is written before the code; README and `docs/` are updated.
- **7. AI assistance:** complies. The code is reviewed and tested by the student; the Pull Request gets a separate AI review.
- **8. Simplicity:** complies. No new libraries or layers; one small service.

**No exceptions. The plan complies with the constitution.**
