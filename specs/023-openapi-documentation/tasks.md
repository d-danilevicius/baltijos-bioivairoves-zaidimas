# Tasks: API documentation with OpenAPI

**Specification:** `specs/023-openapi-documentation/spec.md`
**Plan:** `specs/023-openapi-documentation/plan.md`
**Depends on features:** 001, 003
**Tags:** `[US1]` – belongs to user story 1; no tag – setup or verification task.

Do the tasks in order. Each phase depends on the previous one. Tests are written before the implementation.

## Data

- [x] T001 Check that no schema change is needed for this feature (see the plan)

## Tests (written first) (depends on: Data)

- [x] T002 [US1] Integration test: in the test (Development) host `GET /openapi/v1.json` -> 200 and the document contains the paths of all existing endpoints (`/api/auth/register`, `/api/auth/login`, `/api/auth/logout`, `/api/auth/csrf`, `/api/auth/me`, `/api/health`) and the status codes 401 and 403 where documented – `backend/tests/Project.IntegrationTests/OpenApi/OpenApiDocumentTests.cs`
- [x] T003 [US1] Integration test: in the Production environment `/openapi/v1.json` and the interactive page -> 404 – `backend/tests/Project.IntegrationTests/OpenApi/OpenApiProductionTests.cs`

## Implementation (depends on: Tests)

- [x] T004 [US1] Add the packages `Microsoft.AspNetCore.OpenApi` and `Scalar.AspNetCore` (latest stable versions compatible with .NET 10, no preview versions) – `backend/src/Project.Api/Project.Api.csproj`
- [x] T005 [US1] Register `AddOpenApi()` and map `/openapi/v1.json` and the Scalar page only in the Development environment – `backend/src/Project.Api/Program.cs`
- [x] T006 [US1] Add `ProducesResponseType` attributes (response DTOs, 400, 401, 403, 409 where they apply, and `ProblemDetails` for errors) to `POST /api/auth/register`, `POST /api/auth/login`, `POST /api/auth/logout`, `GET /api/auth/csrf` and `GET /api/auth/me` – `backend/src/Project.Api/Controllers/`
- [x] T007 [US1] Add `ProducesResponseType` attributes to `GET /api/health` – `backend/src/Project.Api/Controllers/`

## Verification (depends on: Implementation)

- [x] T008 Run `dotnet build` and `dotnet test`
- [x] T009 Update README with where to open the documentation (`/openapi/v1.json` and the interactive page, Development only) and `docs/` if the setup changed – `README.md`
- [ ] T010 Review `git diff` and open the Pull Request with the separate AI review evidence
