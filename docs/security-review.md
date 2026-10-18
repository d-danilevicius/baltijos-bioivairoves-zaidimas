# Security Review

**Date:** 2026-10-18
**Reviewed state:** `main` after features 001 (registration), 002 (login), 003 (access control) and 023 (OpenAPI documentation).
**Method:** manual review of the code, configuration and tests against the OWASP Top 10 (2025) categories and the relevant OWASP ASVS areas (authentication, session management, access control, input validation, error handling and logging). Dependencies were checked with `dotnet list backend/Project.slnx package --vulnerable --include-transitive` and `npm audit`.

Features 004–022 are not implemented yet. Where a risk belongs to a later feature (for example ownership of classes, tasks and uploaded photos), this document states it as a remaining limitation and names the feature that must add the control and its test.

## 1. Protected assets

| Asset | Why compromise matters |
|---|---|
| User accounts (student, teacher, administrator) | An attacker could act as the user. A teacher account could read student results; an administrator account could block or delete accounts. |
| Passwords | Reused passwords would let an attacker enter other services of the same person. |
| Authentication cookie and CSRF token | A stolen cookie is a full session of the user until it expires. A missing CSRF check would let another site send requests as the user. |
| Personal data (e-mail address, user name, role) | Exposure harms the privacy of students, who may be minors. |
| Student answers and results (later features) | Another user could read or change a student's work and points. |
| Uploaded photos (feature 015, not implemented) | Photos can contain GPS metadata and can expose the location of protected species. |
| Protected species locations (feature 013, not implemented) | Exact coordinates could lead to disturbing or collecting protected species. This is a core rule of the project. |
| Administrative functions (blocking, deleting accounts) | Misuse removes access of other users or deletes their data. |
| Application secrets (database connection string, database password) | They give full access to all data. |
| Action log | Changing or reading it would hide or expose who did what. |

## 2. Threat model

| Threat | Actor | Impact | Control | Verification |
|---|---|---|---|---|
| Calling a protected endpoint without a session | Anonymous visitor | Access to protected data | `[Authorize]` and role policies; API returns 401 | `GetAuthMe_NoSession_Returns401`, `TestPolicyEndpoint_NoSession_Returns401` |
| A user calls an endpoint of another role (for example a student calls a teacher endpoint) | Authenticated user | Privilege escalation | Role policies Student, Teacher, Administrator checked on the server; 403 with `AccessDenied` | `TestPolicyEndpoint_NonMatchingRole_Returns403WithAccessDenied`, `AuthorizeAsync_RoleAgainstPolicyMatrix_ReturnsExpectedResult` |
| Registering as administrator | Anonymous visitor | Full control of the application | `RegisterRequest.Role` accepts only `Student` or `Teacher` | `Post_Register_AdministratorRole_Returns400`, `RegisterRequest_RoleRequiredAndAllowed_InvalidWhenMissingOrAdministrator` |
| User A changes an identifier to read or edit data of user B | Authenticated user | Access to another user's data | Ownership checks in the services of features 007, 009, 011, 018, 019, 020 (not implemented yet) | Negative integration test in each of those features |
| Cross-site request forgery | Malicious web page | Actions performed as the logged-in user | SameSite=Lax cookie, antiforgery token header `X-CSRF-TOKEN` required for unsafe requests | `Logout_CsrfAndAuthenticationBehaviour` |
| Invalid or oversized input | Any client | Errors, bad data | Server-side validation with data annotations, 400 with error codes | `RegisterRequestValidationTests`, `Login_ScenarioEmptyEmailAndPassword_Returns400WithRequiredErrors` |
| Guessing passwords (credential attack) | Anonymous visitor | Account takeover | Passwords hashed by ASP.NET Core Identity; same error for unknown account and wrong password | `WrongPasswordAndUnknownEmail_Return401WithInvalidCredentials`. Limitation: no lockout or rate limiting (see 3.7) |
| Secrets in Git | Anyone with repository access | Database access | `.env` and local settings are ignored; connection string in user secrets; `.env.example` has placeholders only | `git ls-files` shows no secret files; see 3.2 |
| Vulnerable dependencies | Attacker using a known flaw | Remote exploitation | Dependency scan; patched `System.Security.Cryptography.Pkcs` version | Scan result in 3.3 |
| Stack trace or database error shown to a user | Any user | Information disclosure | `GlobalExceptionHandler` returns a generic ProblemDetails | `GlobalExceptionHandlerTests` |
| Password written to the log | Anyone with log access | Credential exposure | Logs contain only user id and action | `ActionLog_RecordsLoginFailedSuccessAndLogout_AndNoPasswordsInLogsOrActionLog`, `RegistrationServiceTests` |

## 3. Review by OWASP category

### 3.1 Broken Access Control
- **Risk:** vertical escalation (a student uses a teacher or administrator function) and horizontal escalation (user A reads user B's data).
- **Application area:** `Authorization/` (role policies), `AuthController`, later all feature controllers.
- **Existing protection:** the policies `Student`, `Teacher` and `Administrator` are enforced on the server. The API returns 401 without a session and 403 with `AccessDenied` for a wrong role (no redirects). The frontend `ProtectedRoute` only improves usability.
- **Remaining limitation:** ownership checks do not exist yet because the application has no owned data. Features 007, 009, 011, 018, 019 and 020 must check ownership in the service and include a test that uses another user's identifier.
- **Verification:** `AccessControlEndpointTests`, `AccessPolicyTests`, `AuthMeEndpointTests`.

### 3.2 Security Misconfiguration
- **Risk:** open CORS, unencrypted traffic, exposed documentation or secrets in configuration.
- **Application area:** `Program.cs`, `appsettings.json`, `.gitignore`, `docker-compose.yml`.
- **Existing protection:**
  - CORS allows only the origins from `Cors:AllowedOrigins` (the local frontend `http://localhost:5173`); there is no `AllowAnyOrigin`.
  - `UseHttpsRedirection` is enabled and `UseHsts` is used outside Development.
  - The authentication cookie is `HttpOnly`, `Secure` (always) and `SameSite=Lax`.
  - The OpenAPI document and the Scalar page are mapped only in Development (production returns 404).
  - `.env`, `appsettings.Development.json`, `appsettings.Local.json` and `secrets.json` are in `.gitignore`. The connection string is kept in .NET user secrets. `.env.example` contains only a placeholder.
- **Remaining limitation:** `AllowedHosts` is `*` in `appsettings.json`. The production origin and host names must be set when the application is deployed (no deployment exists yet).
- **Verification:** `GetOpenApiDocument_InProduction_ReturnsNotFound`, cookie flag assertions in `LoginEndpointTests`, `git ls-files | grep -i "env\|secret\|appsettings"` shows only `.env.example` and `appsettings.json`.

### 3.3 Software Supply Chain Failures
- **Risk:** known vulnerabilities, outdated or unnecessary packages.
- **Application area:** `backend/**/*.csproj`, `frontend/package.json`, `frontend/package-lock.json`.
- **Existing protection:**
  - On 2026-10-18 `dotnet list package --vulnerable --include-transitive` reported no vulnerable packages in `Project.Api`, `Project.UnitTests` and `Project.IntegrationTests`.
  - On the same date `npm audit` reported 0 vulnerabilities.
  - `System.Security.Cryptography.Pkcs` was raised to a patched version (commit `fix(backend): update System.Security.Cryptography.Pkcs to a patched version`).
  - The frontend has only three runtime dependencies (`react`, `react-dom`, `react-router-dom`); `package-lock.json` is committed.
  - AutoMapper is not added because the current mappings are trivial.
  - The packages added by feature 023 (`Microsoft.AspNetCore.OpenApi`, `Scalar.AspNetCore`) are documented in its plan.
- **Remaining limitation:** the scan is run by hand; CI does not run it. NuGet has no lock file.
- **Verification:** the two commands above; the build and tests pass without the removed or unused packages.

### 3.4 Cryptographic Failures
- **Risk:** passwords stored in plain text or with a weak algorithm; traffic and cookies readable on the network.
- **Application area:** ASP.NET Core Identity, cookie authentication.
- **Existing protection:** passwords are hashed by the ASP.NET Core Identity password hasher; there is no custom cryptography. The cookie is protected by ASP.NET Core Data Protection and sent only over HTTPS (`Secure`).
- **Remaining limitation:** where the Data Protection keys are stored in production is not configured because there is no deployment. This must be decided when the application is deployed.
- **Verification:** code review (`ServiceCollectionExtensions.cs`, `AuthenticationCookieExtensions.cs`); no password column other than Identity's `PasswordHash`.

### 3.5 Injection
- **Risk:** SQL injection or command injection through user input.
- **Application area:** all data access.
- **Existing protection:** all queries use Entity Framework Core and Identity (parameterized). The code has no `FromSql`, `ExecuteSql` or raw SQL concatenation and starts no processes.
- **Remaining limitation:** features that search or filter (for example GBIF parameters in feature 010) must validate values before they are used in queries and in calls to the external API.
- **Verification:** `grep -rn "FromSql\|ExecuteSql\|SqlQuery\|Process.Start" backend/src` returns nothing.

### 3.6 Insecure Design
- **Risk:** missing rules that cannot be fixed by code review later.
- **Application area:** requirements, specifications, plans.
- **Existing protection:** requirements are written before the code (`docs/requirements.md`, `specs/`). Roles are limited at registration. The antiforgery token is required for all unsafe requests that use the session. Protection of protected species locations and the separation of confirmed GBIF data from student data are specified for features 010 and 013.
- **Remaining limitation:** those two rules and the photo metadata removal (feature 015) are specified but not implemented. Each must have an automated test.
- **Verification:** the "tests" part of each `plan.md`.

### 3.7 Authentication Failures
- **Risk:** weak passwords, brute force attacks, sessions that do not end, information leakage in error messages.
- **Application area:** `LoginService`, `RegistrationService`, `AuthController`, cookie settings.
- **Existing protection:**
  - Identity is used; no custom authentication.
  - Unknown e-mail and wrong password return the same `401 InvalidCredentials`.
  - A blocked account is revealed only after a correct password (`403 AccountBlocked`).
  - The session is a cookie that is `HttpOnly`, `Secure` and `SameSite=Lax`, with a 30-minute sliding expiration.
  - Logout removes the cookie; the next protected request returns 401.
  - The password length is 8–128 characters.
- **Remaining limitation:**
  - `CheckPasswordSignInAsync` is called with `lockoutOnFailure: false`, so repeated wrong passwords are not limited. A lockout or rate limit is not implemented.
  - The password rule checks only length (as required by the specification), not complexity or a list of leaked passwords.
  - A request for an unknown e-mail can be faster than for a known one (timing difference).
- **Verification:** `LoginEndpointTests` (wrong password, blocked user, cookie flags, logout, session expiry after 29 and 31 minutes), `LoginContractTests`, `AuthMeEndpointTests`.

### 3.8 Software or Data Integrity Failures
- **Risk:** unreviewed or unverified code reaches `main`.
- **Application area:** GitHub repository and CI.
- **Existing protection:** the ruleset "Apsaugoti main" requires a Pull Request and the checks `build-and-test` and `lint-test-build`. Every Pull Request has a separate AI review recorded in its description. EF Core migrations are committed. `package-lock.json` is committed.
- **Remaining limitation:** no required human reviewer (single developer project); no signed commits.
- **Verification:** the GitHub ruleset and the history of Pull Requests #10–#14.

### 3.9 Security Logging and Alerting Failures
- **Risk:** attacks cannot be investigated, or logs leak secrets.
- **Application area:** `LoginService`, `ActionLogWriter`, `GlobalExceptionHandler`.
- **Existing protection:** `ILogger<T>` is used. Login success, login failure and logout are logged and saved in the `ActionLogs` table with a user id and time. Logs contain no passwords or e-mail addresses. Unhandled exceptions are logged with `LogError`.
- **Remaining limitation:** access denied (401, 403) is not logged. There is no alerting, and logs are not collected in a central place.
- **Verification:** `ActionLog_RecordsLoginFailedSuccessAndLogout_AndNoPasswordsInLogsOrActionLog` and `RegistrationServiceTests` assert that the password is not in the log; the `ActionLogs` rows can be read with `SELECT * FROM "ActionLogs"`.

### 3.10 Mishandling of Exceptional Conditions
- **Risk:** stack traces or database details shown to users; unhandled exceptions leave the system in an unknown state.
- **Application area:** `GlobalExceptionHandler`, ProblemDetails responses.
- **Existing protection:** the exception handler writes the error to the log and returns `500` with a generic message. Expected failures return `400`, `401`, `403` or `409` in one ProblemDetails format with error codes. The CSRF failure returns `400 InvalidCsrfToken`.
- **Remaining limitation:** none known for the current endpoints.
- **Verification:** `GlobalExceptionHandlerTests` (the response does not contain the secret detail of the exception).

## 4. Validation of input

All input of the current endpoints is validated on the server with data annotations: required fields, e-mail format and length (254), user name 3–30 characters (letters, digits, `_`), password length 8–128 and the allowed roles. Invalid data returns `400` with error codes instead of an exception. Features that accept identifiers, query parameters or files must add the same server-side checks (identifier range, file type and size).
**Verification:** `RegisterRequestValidationTests`, `RegistrationEndpointTests`, `LoginContractTests`.

## 5. Summary of remaining limitations

| Limitation | Where it will be handled |
|---|---|
| Ownership checks and negative tests | Features 007, 009, 011, 018, 019, 020 (each feature in its own branch) |
| No lockout or rate limiting for login | To be specified as a new security feature |
| Access denied events are not logged | To be specified as a new security feature |
| Production host names, HTTPS certificate and Data Protection keys | Deployment |
| Dependency scan is manual | To be added to CI when time allows |
| Photo metadata removal and protected species coordinates | Features 013 and 015 |
