# Quickstart: Registration

How to check the feature by hand.

1. Start the database: `docker compose up -d` (a `.env` file based on `.env.example` is required).
2. Apply the migrations: `dotnet ef database update --project backend/src/Project.Api`.
3. Start the server: `dotnet run --project backend/src/Project.Api`.
4. Start the frontend: in `frontend/` run `npm run dev` and open the address it prints, then go to `/register`.
5. Register a student with a valid email, a username, a password of at least 8 characters and the role student. The login page opens with the message „Paskyra sukurta. Prisijunkite.“ (Account created. Log in.)
6. Register again with the same email: the message „Šis el. pašto adresas jau užregistruotas.“ (This email address is already registered.) is shown.
7. Run the tests: `dotnet test backend/Project.slnx` and, in `frontend/`, `npm test`.
