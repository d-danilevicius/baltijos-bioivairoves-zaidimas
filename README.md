# Baltijos biologinės įvairovės iššūkių žaidimas

Educational field game for schools, nature centres and regional parks. A teacher chooses a location in Lithuania and a season. The application retrieves biodiversity observations near that place from the [GBIF Occurrence API](https://techdocs.gbif.org/en/openapi/v1/occurrence) and creates tasks from them: recognise a species group, find features of a common local species, compare seasonal observations, or answer a question from an observation card. Students submit evidence, earn points and unlock harder tasks. Confirmed external observations are always shown separately from student-submitted data, and the system does not encourage collecting or disturbing protected species.

**Status:** repository base. Features are specified and implemented one by one in `specs/`.

## Documentation

- [Requirements](docs/requirements.md) – scope, functional and non-functional requirements, business rules
- [User experience, flows and wireframes](docs/ux.md)
- [Architecture and feature list](docs/architecture.md)
- `specs/NNN-name/` – specification, plan and tasks of each feature
- [AGENTS.md](AGENTS.md) and [constitution](.specify/memory/constitution.md) – rules for AI-assisted development

## Technology

.NET 10 and ASP.NET Core Web API, Entity Framework Core with PostgreSQL 17, React with TypeScript, Vite and Tailwind CSS, xUnit, Vitest and React Testing Library. See [docs/architecture.md](docs/architecture.md) for the reasons.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) 22 or newer
- [Docker](https://www.docker.com/) (local PostgreSQL)
- `dotnet-ef` tool: `dotnet tool install --global dotnet-ef`

## Local setup

1. Clone the repository.
2. Copy `.env.example` to `.env` and set `POSTGRES_PASSWORD`.
3. Start the database: `docker compose up -d`
4. Give the API its connection string (replace the password with the one from `.env`):

   ```
   dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Port=5432;Database=project;Username=project;Password=<password>" --project backend/src/Project.Api
   ```

## Database and migrations

Schema changes are EF Core migrations stored in `backend/src/Project.Api/Migrations`.

- Create a migration: `dotnet ef migrations add <DescriptiveName> --project backend/src/Project.Api`
- Apply migrations: `dotnet ef database update --project backend/src/Project.Api`

The first migration CreateIdentityTables exists (ASP.NET Core Identity tables and the seeded roles Student, Teacher, Administrator) and is applied with `dotnet ef database update --project backend/src/Project.Api`.

## Run

Backend (listens on http://localhost:5080):

```
dotnet run --project backend/src/Project.Api
```

Check: http://localhost:5080/api/health returns `{"status":"ok"}`.

On Windows both parts can be started with a double click on `start-dev.bat` in the repository root (the database must already be running).

Frontend (http://localhost:5173, requests to `/api` are proxied to the backend):

```
cd frontend
npm install
npm run dev
```

The registration page is at http://localhost:5173/register and the endpoint is POST /api/auth/register.

The login page is at http://localhost:5173/login and the endpoint is POST /api/auth/login.
After login the user is taken to the profile page http://localhost:5173/profile, where "Atsijungti" calls POST /api/auth/logout.
Session uses cookie authentication (HttpOnly, Secure, SameSite=Lax) with 30 minutes of inactivity.
CSRF: POST requests that change data need the X-CSRF-TOKEN header; the token is returned by GET /api/auth/csrf (fetch it after login).

Access control: GET /api/auth/me returns the current user { id, userName, role } when the session is valid and 401 otherwise; the frontend uses it to restore the session after a page refresh.
Role policies Student, Teacher and Administrator exist on the server; a user with the wrong role gets 403 with the error code "AccessDenied" (shown in the UI as "Prieiga uždrausta").
Frontend: protected routes (ProtectedRoute) redirect to /login when there is no session; unknown addresses show the "Puslapis nerastas" page; wrong role shows "Prieiga uždrausta".
No new migration and no new configuration in this feature.

Apply the new migration with: dotnet ef database update --project backend/src/Project.Api

## API documentation

The backend generates the OpenAPI document from the code (controllers and `ProducesResponseType` attributes).
It is available only in the Development environment and is not served in Production.

With the backend running (http://localhost:5080, from `launchSettings.json`):

- OpenAPI document: http://localhost:5080/openapi/v1.json
- Interactive page (Scalar): http://localhost:5080/scalar

Every new endpoint must declare its status codes and DTO types with `ProducesResponseType` so that the documentation stays correct.

## Tests

```
dotnet test backend/Project.slnx
cd frontend && npm test
```

Other checks: `dotnet format backend/Project.slnx --verify-no-changes`, `npm run lint`, `npm run build`.

## Configuration

| Setting | Where | Purpose |
|---|---|---|
| `ConnectionStrings:Default` | user secrets or environment variable `ConnectionStrings__Default` | PostgreSQL connection |
| `Cors:AllowedOrigins` | `appsettings.json` or `Cors__AllowedOrigins__0` | Allowed browser origins |
| `POSTGRES_*` | `.env` | Local database container |
| `API_URL` | environment, optional | Backend address for the frontend dev proxy |

Secrets are never committed. `.env` and `appsettings.Development.json` are ignored by Git.

## Continuous integration

GitHub Actions run on pull requests to `main`: backend (restore, build, test) and frontend (install, lint, test, production build).

## Image credits

The background image of the frontend (`frontend/src/assets/background-muted.jpg`) is a photo from Pexels (Pexels License), changed to muted colours.

## Deployment

Not defined yet.
