# Feature Specification: API documentation with OpenAPI

**Feature folder:** `specs/023-openapi-documentation`
**Status:** draft
**Related project requirements:** none. This feature has no FR from `docs/requirements.md`; it comes from the course requirement (Independent Work 5, item 11).
**Related screens:** none (developer tooling, no screen in `docs/ux.md`)

**Why this feature is needed:** Course requirement (Independent Work 5, item 11): “Make the API discoverable through appropriate ASP.NET Core API documentation or OpenAPI tooling where applicable. Documentation must reflect the real endpoints. Outdated endpoint documentation is incorrect documentation and must be updated with the implementation.”

## User Scenarios and Acceptance Scenarios

### User Story 1 – API documentation with OpenAPI (Priority: P1)

**User story:** As a developer, I want to see the real API endpoints, their request and response data and status codes in one generated description, so that the frontend and tests use the correct API.

**Independent verification:** In the Development environment the developer opens the generated OpenAPI document and the interactive page and finds all existing endpoints with request DTOs, response DTOs, status codes and the ProblemDetails error format. In Production neither is served.

**Acceptance scenarios:**

1. **Document:** **Given** the backend runs in the Development environment, **when** the developer opens `GET /openapi/v1.json`, **then** the response is status 200 with an OpenAPI document that lists `POST /api/auth/register`, `POST /api/auth/login`, `POST /api/auth/logout`, `GET /api/auth/csrf`, `GET /api/auth/me` and `GET /api/health`.
2. **Status codes:** **Given** the OpenAPI document, **when** the developer reads an endpoint, **then** the endpoint shows its request DTO, its response DTO and every status code it can return (including 400, 401, 403 and 409 where they apply) with the ProblemDetails error format for errors.
3. **Interactive page:** **Given** the backend runs in the Development environment, **when** the developer opens the interactive documentation page, **then** the page shows the same document and lets the developer browse the endpoints.
4. **Production:** **Given** the backend runs in the Production environment, **when** anyone requests `/openapi/v1.json` or the interactive page, **then** the server returns status 404.
5. **New endpoints:** **Given** a later feature adds a controller action, **when** the backend is built and started in Development, **then** the action appears in the document automatically with its declared status codes.

## Edge Cases

- An action without declared status codes would be documented incompletely; therefore every action MUST declare its status codes (see Assumptions).
- The CSRF failure of `POST /api/auth/logout` is a 400 response with ProblemDetails and the error code `InvalidCsrfToken`, and it MUST be documented.
- Endpoints that require a session or a role (401, 403) MUST show these codes in the document, even though the interactive page does not log in (see Scope Boundaries).

## Requirements

### Functional Requirements

- The backend MUST generate an OpenAPI document with the built-in ASP.NET Core OpenAPI support (`Microsoft.AspNetCore.OpenApi`) and serve it at `/openapi/v1.json`.
- The backend MUST provide an interactive documentation page for this document (`Scalar.AspNetCore`).
- The OpenAPI document and the interactive page MUST be available only in the Development environment and MUST NOT be served in Production.
- The document MUST list all existing endpoints: `POST /api/auth/register`, `POST /api/auth/login`, `POST /api/auth/logout`, `GET /api/auth/csrf`, `GET /api/auth/me` and `GET /api/health`.
- For every endpoint the document MUST show the request DTOs, the response DTOs, the status codes the endpoint can return (including 400, 401, 403 and 409 where they apply) and the ProblemDetails error format.
- The descriptions MUST come from the code (attributes such as `ProducesResponseType`); there MUST be no hand-written copy of the API description, so it cannot get out of date.
- The 200 responses of `GET /api/auth/csrf` and `GET /api/health` MUST show a response body schema (named response DTOs `CsrfTokenResponse` and `HealthResponse`; the JSON shape is not changed).
- The README MUST explain where to open the documentation.

### Non-functional Requirements

- The documentation MUST reflect the real endpoints; outdated endpoint documentation is incorrect documentation and MUST be updated together with the implementation (course requirement).
- The documentation MUST NOT be exposed in Production.

### Validation Rules

- Not applicable. This feature does not add user input; it only describes the existing validation responses (400) of other endpoints.

### Key Entities

- **OpenAPI document:** generated JSON description of the API served at `/openapi/v1.json` (not stored; generated from the code at runtime).

## Success Criteria

- **SC-001:** An automated integration test verifies that in the Development (test) host `GET /openapi/v1.json` returns 200 and contains the paths of all existing endpoints and the status codes 401 and 403 where they are documented.
- **SC-002:** An automated integration test verifies that in the Production environment the document and the interactive page are not served (404).
- **SC-003:** A developer can open the documentation by following the README only.

## Clarifications

**Session 2026-10-17**

- No questions were raised.

**Session 2026-10-18**

- The AI review of the Pull Request found the logout status code (400, not 403) and the missing response schemas of csrf and health; the specification was updated accordingly.

No unresolved clarification markers remain.

## Assumptions

- The assumptions and constraints of `docs/requirements.md` (section 11) apply.
- There is no FR in `docs/requirements.md` for this feature; the source is the course requirement (Independent Work 5, item 11).
- Rule for later features: every new endpoint MUST appear in the OpenAPI document automatically and MUST declare its status codes (for example with `ProducesResponseType`, including 400, 401, 403, 404 and 409 where they apply) and its request and response DTOs. An endpoint that changes must update its attributes in the same Pull Request.

## Dependencies

- Features: 001 and 003 (the existing authentication, health and `GET /api/auth/me` endpoints are documented by this feature).

## Scope Boundaries

**In scope:**
- The generated OpenAPI document at `/openapi/v1.json`.
- The interactive documentation page.
- Status code and response type attributes on the existing controllers.
- README section that explains where to open the documentation.

**Out of scope:**
- Authentication in the interactive page.
- Client code generation.
- API versioning.
