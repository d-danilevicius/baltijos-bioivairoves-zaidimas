# Data Model: Login

## ApplicationUser (extends `IdentityUser`)

| Field | Type | Rule |
|-------|------|------|
| IsBlocked | bool | default false; when true the user cannot log in (VT-006, FR-007). |

This field is an addition to the `ApplicationUser` used in registration.

## ActionLog

A new entity to record authentication and administrator actions required by NFR-023.

| Field | Type | Rule |
|-------|------|------|
| Id | long (auto-increment) | primary key |
| OccurredAtUtc | DateTime (UTC) | when the action occurred |
| UserId | string (nullable) | FK to `AspNetUsers` or null for anonymous actions |
| Action | string | short code describing the action (see list below) |

Recommended action names:
- `Login.Success`
- `Login.Failed`
- `Logout`
- `Account.Blocked` (administrator action)
- `Account.Unblocked` (administrator action)

## Privacy and logging

- No password, password hash, authentication tokens or sensitive secrets are ever stored in ActionLog or in any other logs (NFR-024).
- ActionLog stores only the fact of an action and the (nullable) user identifier.

## Migration

Create a migration named `AddIsBlockedAndActionLogTable` that:
- Adds the `IsBlocked` boolean column to `AspNetUsers` with default `false`.
- Creates the `ActionLog` table with the columns described above and an index on `OccurredAtUtc` for efficient recent-activity queries.

```
Notes:
- Keep the Action column short (e.g., 50 chars) and avoid logging any user input or secrets.
- Use UTC for OccurredAtUtc.
```
