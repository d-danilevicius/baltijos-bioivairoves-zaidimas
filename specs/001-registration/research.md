# Research: Registration

Decisions for feature 001. They are proposals until the student confirms them.

| Question | Decision | Reason |
|----------|----------|--------|
| How are accounts and passwords managed? | ASP.NET Core Identity (`UserManager<ApplicationUser>`) | Required by the general rules; it hashes passwords with a slow salted function (NFR-001). |
| How is the role stored? | Identity roles `Student`, `Teacher`, `Administrator`; one role per user | VT-005. |
| Where are the limits checked? | Field limits (VT-030, VT-003) by validation attributes of the request DTO; uniqueness and the allowed roles in `RegistrationService` | Constitution 2.2, 4.3. |
| How are uniqueness errors reported? | `409 Conflict` with a ProblemDetails `errors` entry (`email: AlreadyRegistered`, `userName: AlreadyInUse`) | The frontend maps codes to the Lithuanian messages (NFR-011). |
| Race of two identical registrations | Unique indexes in the database (Identity normalized email and username); a database conflict is converted to the same `409` | Edge case in the specification. |
| Is AutoMapper needed? | Not in this feature | The only mapping is request → user (two fields) and user → response (three fields); it is trivial (general rules: AutoMapper for non-trivial mappings). |
| Frontend routing | React Router route `/register` | `docs/ux.md`, E02. |
