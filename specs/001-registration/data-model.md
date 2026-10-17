# Data Model: Registration

## ApplicationUser (extends `IdentityUser`)

| Field | Type | Rule |
|-------|------|------|
| Id | string (Identity) | primary key |
| Email | string, up to 254 | required, unique (VT-001, VT-030) |
| UserName | string, 3–30 | required, unique, letters, digits, “_” (VT-002, VT-030) |
| PasswordHash | string (Identity) | slow salted hash, never the password (NFR-001) |

## Roles

`Student`, `Teacher`, `Administrator`. A user has exactly one role (VT-005). Only `Student` and `Teacher` can be chosen at registration (VT-004).

## Database

Identity tables (`AspNetUsers`, `AspNetRoles`, `AspNetUserRoles`, …) created by the migration `CreateIdentityTables`. The unique index `UserNameIndex` on the normalized username is created by Identity. The unique index on the normalized email is added in the migration, because Identity creates only a non-unique email index. The Identity email normalization matches this index.

## Not stored

No other personal data is stored (NFR-005).
