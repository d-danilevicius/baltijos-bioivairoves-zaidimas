# Contract: Authentication endpoints

## POST /api/auth/login

- **Who can call:** everyone (not logged in).
- **Request (JSON):** `{ "email": "...", "password": "..." }`
- **200 OK:** `{ "id": "...", "userName": "...", "role": "Student" | "Teacher" | "Administrator" }`
- **400 Bad Request** (ProblemDetails with `errors`: field → list of codes):
  - `email`: `Required`
  - `password`: `Required`
- **401 Unauthorized** (ProblemDetails):
  - General invalid credentials response without revealing which field is wrong. A wrong email or wrong password always returns this 401, including when the account is blocked. The credentials check is performed before any blocked-account check. Example ProblemDetails body: `{"type":"https://tools.ietf.org/html/rfc7235#section-3.1","title":"Unauthorized","status":401,"detail":"Invalid credentials.","errors":{"": ["InvalidCredentials"]}}`
  - Lithuanian UI message to display: "Neteisingas el. paštas arba slaptažodis." (Incorrect email or password.)
- **403 Forbidden** (ProblemDetails):
  - Blocked account (returned only after a successful credential verification). Example ProblemDetails body: `{"type":"https://tools.ietf.org/html/rfc9110#section-15.5.4","title":"Forbidden","status":403,"detail":"Account is blocked.","errors":{"": ["AccountBlocked"]}}`
  - Lithuanian UI message to display: "Paskyra užblokuota. Kreipkitės į administratorių." (The account is blocked. Contact the administrator.)
- **500 Internal Server Error:** unexpected error, without technical details.

Notes:
- The 401 response MUST not indicate whether the email or the password was incorrect (FR-006).
- The server records `Login.Success` and `Login.Failed` in `ActionLog` (NFR-023) but MUST NOT log the password or its hash (NFR-024).

## POST /api/auth/logout

- **Who can call:** authenticated users.
- **Request:** empty body.
- **204 No Content:** success; the authentication cookie is removed.
- **401 Unauthorized:** when the request is not authenticated.

Notes:
- Logout is a state-changing request and must respect antiforgery protection when called from the browser (cookie + header) as described in research.md.
- On logout the server records `Logout` in `ActionLog` (NFR-023).
