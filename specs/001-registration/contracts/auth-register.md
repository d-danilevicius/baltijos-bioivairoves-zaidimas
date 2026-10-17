# Contract: POST /api/auth/register

- **Who can call:** everyone (not logged in).
- **Request (JSON):** `{ "email": "...", "userName": "...", "password": "...", "role": "Student" | "Teacher" }`
- **201 Created:** `{ "id": "...", "userName": "...", "role": "Student" | "Teacher" }`
- **400 Bad Request** (ProblemDetails with `errors`: field → list of codes):
  - `email`: `Required`, `InvalidFormat`, `TooLong`
  - `userName`: `Required`, `InvalidFormat` (not 3–30 letters, digits or “_”)
  - `password`: `Required`, `InvalidLength` (not 8–128 characters)
  - `role`: `Required`, `NotAllowed` (anything other than `Student` or `Teacher`)
- **409 Conflict** (ProblemDetails with `errors`):
  - `email`: `AlreadyRegistered`
  - `userName`: `AlreadyInUse`
- **500:** unexpected error, without technical details.

The response never contains the password or its hash.
