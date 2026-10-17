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

.NET 10 and ASP.NET Core Web API, Entity Framework Core with PostgreSQL 17, React with TypeScript and Vite, xUnit, Vitest and React Testing Library. See [docs/architecture.md](docs/architecture.md) for the reasons.

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

There are no migrations yet. The first one is added by the first feature that needs a table.

## Run

Backend (listens on http://localhost:5080):

```
dotnet run --project backend/src/Project.Api
```

Check: http://localhost:5080/api/health returns `{"status":"ok"}`.

Frontend (http://localhost:5173, requests to `/api` are proxied to the backend):

```
cd frontend
npm install
npm run dev
```

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

## Deployment

Not defined yet.
