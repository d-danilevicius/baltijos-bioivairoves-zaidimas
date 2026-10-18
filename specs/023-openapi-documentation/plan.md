# Implementation Plan: API documentation with OpenAPI

**Feature:** `specs/023-openapi-documentation/spec.md`
**Branch:** `023-openapi-documentation`
**Status:** draft
**Depends on features:** 001, 003

## Summary

The plan describes how the backend generates an OpenAPI document from the code and shows it in an interactive page, only in the Development environment. The existing controllers declare their status codes with attributes so that the document reflects the real endpoints.

## Technical Context

- **Language and version:** C# with .NET 10 (server).
- **Libraries:** as selected in `docs/architecture.md` (section 1), plus two new dependencies:
  - `Microsoft.AspNetCore.OpenApi` – built-in ASP.NET Core OpenAPI support; generates the OpenAPI document from controllers and attributes. Latest stable version compatible with .NET 10 (no preview versions).
  - `Scalar.AspNetCore` – interactive documentation page that reads the generated document. Latest stable version compatible with .NET 10 (no preview versions).
- **Frontend:** not changed.
- **Storage:** not changed.
- **Testing:** xUnit (integration, PostgreSQL in Testcontainers).
- **Target platform:** developer machine (Development environment only).
- **Constraints:** HTTPS only; the documentation is not served in Production; secrets are not stored in the repository.

## Structure and Main Components

- Server:
  - `AddOpenApi()` registration and Development-only mapping of `/openapi/v1.json` and the Scalar page in `backend/src/Project.Api/Program.cs`;
  - `ProducesResponseType` attributes (request/response DTOs, 200/201/204, 400, 401, 403, 409 and `ProblemDetails`) on `AuthController` and the health endpoint.
- Documentation: README section with the documentation address.

## Server Request Changes

- New (Development only): `GET /openapi/v1.json` – generated OpenAPI document.
- New (Development only): interactive documentation page (Scalar).
- Existing endpoints get status code attributes only; their behavior does not change:
  - `POST /api/auth/register`, `POST /api/auth/login`, `POST /api/auth/logout`, `GET /api/auth/csrf`, `GET /api/auth/me`, `GET /api/health`.

## Data Model Changes

- No data changes.
- No new migration.

## Testing Plan

- Backend integration tests:
  - in the test (Development) host `GET /openapi/v1.json` -> 200 and the document contains the paths of all existing endpoints (`/api/auth/register`, `/api/auth/login`, `/api/auth/logout`, `/api/auth/csrf`, `/api/auth/me`, `/api/health`) and the status codes 401 and 403 where documented;
  - in the Production environment `/openapi/v1.json` and the interactive page -> 404.

## Technical Notes

- The OpenAPI document is generated from the code, so there is no hand-written copy of the API that can get out of date.
- The mapping is wrapped in an `IsDevelopment()` check in `Program.cs`; Production does not register the endpoints.
- Later features must declare status codes on their actions; this is a rule in the spec (Assumptions).
- Business rules are not changed; controllers only get attributes.

## Constitution Check

- **1. Code quality:** complies. Changes are small attributes and a short registration; each class keeps one responsibility and stays below 200 lines.
- **2. Architecture:** complies. Controllers keep translating HTTP only; DTOs are used at the API boundary and shown in the document; there is no schema change.
- **3. Testing:** complies. Integration tests cover the document in Development and the 404 in Production.
- **4. Security:** complies. The documentation is available only in Development and not in Production; no secrets are stored in Git; authorization behavior is not changed.
- **5. Version control:** complies. The work is done in branch `023-openapi-documentation`, created from an updated `main`, and enters through a Pull Request.
- **6. Documentation:** complies. The specification is written before the code; README is updated with the documentation address.
- **7. AI assistance:** complies. The code is reviewed and tested by the student; the Pull Request gets a separate AI review.
- **8. Simplicity:** complies. Two new libraries are added and justified: `Microsoft.AspNetCore.OpenApi` is the built-in ASP.NET Core support and `Scalar.AspNetCore` is the interactive page required to read the document; no new layers are added.

**No exceptions. The plan complies with the constitution.**
