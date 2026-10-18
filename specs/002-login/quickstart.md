# Quickstart: Login

How to check the feature by hand.

1. Start the database: `docker compose up -d` (a `.env` file based on `.env.example` is required).
2. Apply the migrations: `dotnet ef database update --project backend/src/Project.Api`.
3. Start the server: `dotnet run --project backend/src/Project.Api`.
4. Start the frontend: in `frontend/` run `npm run dev` and open the address it prints, then go to `/login`.
5. Login with a registered account (valid email and password). The starting screen opens according to the role.
6. Attempt login with incorrect credentials: the login page remains and the message "Neteisingas el. paštas arba slaptažodis." is shown.
7. Attempt login with a blocked account (set `IsBlocked = true` for the user in the database): the login page remains and the message "Paskyra užblokuota. Kreipkitės į administratorių." is shown.
8. Logout from a logged-in account: use the UI "Atsijungti" (Log out) or call `POST /api/auth/logout`; the server returns 204 and the login page opens.
9. Check that passwords are not written to logs and that `ActionLog` entries are created for successful logins, failed logins and logout (NFR-023, NFR-024).
10. Run backend tests: `dotnet test backend/Project.slnx`.
