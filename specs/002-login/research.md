# Research: Login

Decisions for feature 002. They are proposals until the student confirms them.

| Question | Decision | Reason |
|----------|----------|--------|
| How is authentication performed? | Cookie authentication using ASP.NET Core Identity SignInManager and the authentication cookie. | Aligns with the registration feature (Identity) and simplifies frontend: the browser stores the cookie and the server is authoritative about the session. |
| How long is a session valid? | 30 minutes sliding inactivity (reset on each request that touches the session). | Matches NFR-004; sliding expiration improves usability while limiting exposure. |
| Which cookie flags? | HttpOnly, Secure (HTTPS only), SameSite=Lax. | Improves security against XSS and CSRF while allowing normal navigation and redirects. |
| How to protect against CSRF? | An antiforgery (CSRF) token is required for requests that change data (stateful endpoints). | Standard protection for cookie-based authentication. |
| API behaviour for unauthorized access | API endpoints return 401 (unauthenticated) or 403 (forbidden/blocked) status codes instead of redirecting to login pages. A wrong email or wrong password always returns 401 (the same response), including when the account is blocked; the blocked-account check happens after credentials are verified, so a 403 is only returned after successful credential verification. | Enables SPA/frontend to handle navigation and messages; avoids server-side redirects for API calls. |
| How to make the 30-minute timeout testable? | Use a replaceable clock abstraction (TimeProvider) injected into services and into the cookie/session configuration for tests. | Allows deterministic tests that advance time without waiting. |
| Failed-login limits? | No failed-login limit in the first version (accepted risk). | Decided in the spec clarifications; reduces scope for initial release. |

## Security notes

- The authentication cookie is used only over HTTPS and marked HttpOnly so JavaScript cannot read it.
- Antiforgery tokens are required for any state-changing endpoints when called from the browser; APIs used by the SPA receive tokens over the standard cookie + header pattern.
- API responses use 401/403 codes so frontend can display localized messages from `docs/ux.md` instead of relying on HTML redirects.

## Testability

- Replaceable TimeProvider enables unit and integration tests for inactivity expiry.
- Authentication behaviour (successful login, invalid credentials, blocked account) is covered by unit and integration tests.

## Migration / rollout

- No migration is required for authentication alone, but a data-model change (ApplicationUser.IsBlocked and ActionLog) is needed (see data-model.md).

## Assumptions

- The antiforgery token flow uses the standard ASP.NET Core approach (cookie + request header) and the frontend will send the header on state-changing requests.
- The application uses the Identity cookie scheme and the existing SignInManager for sign-in and sign-out operations.
- Cookie SameSite value Lax is acceptable for the SPA navigation patterns. If cross-site POSTs are required later, this will be revisited.
- Error code names for ProblemDetails: `InvalidCredentials` and `AccountBlocked` are used for 401 and 403 responses respectively (simple, see contracts/auth-login.md).
