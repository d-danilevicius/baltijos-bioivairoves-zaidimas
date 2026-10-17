# Implementation Plan: Photos with the evidence

**Feature:** `specs/015-photos/spec.md`
**Branch:** `015-photos`
**Status:** draft
**Depends on features:** 003-access-control

## Summary

The plan describes how the feature “Photos with the evidence” is implemented according to the approved specification: as a student, I want to add photos to the evidence so that I show what I noticed.

## Technical Context

- **Language and version:** C# with .NET 10 (server); TypeScript (frontend).
- **Libraries:** as selected in `docs/architecture.md` (section 1): ASP.NET Core Identity, Entity Framework Core with Npgsql, AutoMapper for non-trivial mappings.
- **Frontend:** React with Vite and React Router.
- **Storage:** PostgreSQL through Entity Framework Core; schema changes only through migrations.
- **Testing:** xUnit (unit and integration, PostgreSQL in Testcontainers), Vitest and React Testing Library.
- **Target platform:** web application in current Chrome, Firefox, Edge and Safari; screens from 360 px width.
- **Constraints:** HTTPS only; secrets are not stored in the repository.

## Structure and Main Components

- Server: SubmissionsController, SubmissionService, PhotoService (SixLabors.ImageSharp).
- Frontend: photoInput component.

## Server Request Changes

- POST /api/my/tasks/{id}/submissions (photos, multipart)

## Data Model Changes

- SubmissionPhoto.
- Migration `CreateSubmissionPhotosTable`.

## Testing Plan

- Test case: format.
- Test case: size.
- Test case: count.
- Test case: metadata removed.

## Technical Notes

- Business rules are placed in the services listed above; controllers only translate HTTP.

## Constitution Check

- **1. Code quality:** complies. Each class has one responsibility and stays below 200 lines; React file names follow the naming rules.
- **2. Architecture:** complies. SubmissionsController only calls services; the rules are in the services; `ApplicationDbContext` is used directly (no Repository); DTOs are used at the API boundary and AutoMapper for non-trivial mappings; the schema is changed by the migration `CreateSubmissionPhotosTable`.
- **3. Testing:** complies. The requirements FR-040 are covered by the tests in the testing plan.
- **4. Security:** complies. Role and ownership are checked on the server for: POST /api/my/tasks/{id}/submissions (photos, multipart); all input is validated on the server; no secret is stored in Git.
- **5. Version control:** complies. The work is done in branch `015-photos`, created from an updated `main`, and enters through a Pull Request.
- **6. Documentation:** complies. The specification is written before the code; README and `docs/` are updated if the behaviour changes.
- **7. AI assistance:** complies. The code is reviewed and tested by the student; the Pull Request gets a separate AI review.
- **8. Simplicity:** complies. The library `SixLabors.ImageSharp` is already selected in docs/architecture.md (section 1) for format checking and removing metadata.

**No exceptions. The plan complies with the constitution.**

