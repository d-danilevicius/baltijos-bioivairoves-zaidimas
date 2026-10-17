# Feature Specification: Registration

**Feature folder:** `specs/001-registration`
**Status:** draft
**Related project requirements:** FR-001–FR-004; NFR-001, NFR-024; VT-001–VT-005, VT-030
**Related screens:** E02 Registration (`docs/ux.md`, section 5.1)

**Why this feature is needed:** Without an account nobody can play or manage a class. Registration lets a student or a teacher get their own account.

## User Scenarios and Acceptance Scenarios

### User Story 1 – A visitor registers (Priority: P1)

**User story:** As a visitor, I want to register as a student or a teacher so that I have my own account.
**Why this priority:** Without registration no other feature can be used.
**Independent verification:** One account is registered and the login page opens with the message „Paskyra sukurta. Prisijunkite.“ (Account created. Log in.)

**Acceptance scenarios:**

1. **Given** a visitor without an account, **when** the visitor enters a valid email, a username and a password and chooses the role student or teacher, **then** the account is created with the chosen role (FR-001).
2. **Given** an email address that is already assigned to another account, **when** the visitor tries to register with it, **then** the registration is rejected and the message „Šis el. pašto adresas jau užregistruotas.“ (This email address is already registered.) is shown (FR-002, VT-001).
3. **Given** a username that is already taken, **when** the visitor tries to register with it, **then** the registration is rejected and the message „Toks naudotojo vardas jau naudojamas.“ (This username is already in use.) is shown (FR-003, VT-002).
4. **Given** a password shorter than 8 or longer than 128 characters, **when** the visitor submits the form, **then** the registration is rejected and the message „Slaptažodį turi sudaryti nuo 8 iki 128 simbolių.“ (The password must consist of 8 to 128 characters.) is shown (FR-004, VT-003).
5. **Given** the administrator role is sent to the server, **when** the visitor submits the form, **then** the registration is rejected, because only the student and teacher roles can be chosen (VT-004).
6. **Given** an invalid field (email, username, role), **when** the visitor submits the form, **then** the registration is rejected and the invalid field is indicated (FR-004, VT-030).

## Edge Cases

- A field is left empty.
- The email is longer than 254 characters or has an invalid format.
- The username is shorter than 3 or longer than 30 characters, or contains characters other than letters, digits and “_”.
- No role is chosen.
- The password is never shown and never written to logs (NFR-024).
- Two registrations with the same email or username are sent at the same time: the database allows only one.

## Requirements

### Functional Requirements

- **FR-001:** The system MUST allow a visitor to register as a student or a teacher by providing an email address, a username, a password and a role.
- **FR-002:** The system MUST prevent registration when the email address is already associated with another account and show an error message.
- **FR-003:** The system MUST prevent registration when the username is already taken and show an error message.
- **FR-004:** The system MUST reject registration data that does not meet the limits of VT-003 and VT-030 and indicate which field is invalid.

### Non-functional Requirements

- **NFR-001:** Passwords MUST be stored only as slow, salted hash values (ASP.NET Core Identity).
- **NFR-024:** Passwords MUST NOT be written to logs.

### Validation Rules

- **VT-001:** One email address may be assigned to only one account.
- **VT-002:** A username must be unique among all accounts.
- **VT-003:** A password must consist of 8–128 characters.
- **VT-004:** A user may choose only the student or the teacher role when registering.
- **VT-005:** Every user has exactly one role.
- **VT-030:** Email – up to 254 characters. Username – 3–30 characters (letters, digits, “_”).

### Key Entities

- **User** – an account with an email, a username, a password hash and exactly one role.

## Success Criteria

- **SC-001:** A new user completes the registration in no more than 1 minute.
- **SC-002:** 100 % of attempts to register with an email or a username that is already used are rejected.
- **SC-003:** A password cannot be read from the stored data.

## Clarifications

**Session 2026-10-17**

- Q: What happens after a successful registration? → A: The login page opens with the message „Paskyra sukurta. Prisijunkite.“ (Account created. Log in.) The user is not logged in automatically (`docs/ux.md`, section 5.1).
- Q: How is the email compared for uniqueness? → A: Ignoring letter case.

No unresolved clarification markers remain.

## Assumptions

- Teachers register by themselves, and their identity is not verified.
- The administrator account is created by the system developer (seeded) and not through this feature.

## Dependencies

- None.

## Scope Boundaries

**In scope:**
- The registration form and the creation of the account.

**Out of scope:**
- Login and logout (feature 002).
- Creating an administrator account.
- Email confirmation and password reset.
