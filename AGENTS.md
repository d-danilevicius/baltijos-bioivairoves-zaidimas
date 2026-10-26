# AGENTS.md

Practical instructions for the AI assistant in this repository. The principles are in the constitution: `.specify/memory/constitution.md`. They are not repeated here.

## References
- Constitution: `.specify/memory/constitution.md`
- Requirements (FR, NFR, VT): `docs/requirements.md`
- Screens and flows: `docs/ux.md`
- Architecture and feature list: `docs/architecture.md`
- Feature specifications: `specs/NNN-name/`

## Repository map
- `backend/src/Project.Api/` – server (Controllers, Services, Integrations/Gbif, DTOs, Mapping, Entities, Data, Migrations, Middleware).
- `backend/tests/Project.UnitTests/`, `backend/tests/Project.IntegrationTests/` – server tests.
- `frontend/src/` – frontend (pages, components, hooks, helpers, api, types, tests).
- `specs/`, `docs/`, `.specify/`, `.github/workflows/`.

## Commands
Backend (in the repository root):
- `dotnet restore backend/Project.slnx`
- `dotnet build backend/Project.slnx`
- `dotnet test backend/Project.slnx`
- `dotnet format backend/Project.slnx --verify-no-changes`
- `dotnet ef migrations add <NameDescribingTheChange> --project backend/src/Project.Api`

Frontend (in the `frontend/` folder):
- `npm ci`, `npm run lint`, `npm test`, `npm run build`, `npm run dev`

Local database: `docker compose up -d` (a `.env` file based on `.env.example` is required).

## Workflow
Feature workflow according to the general project rules: updated `main` → branch → specification → Clarify → plan → Checklist and tasks → Analyze → implementation → tests → verification → Pull Request → separate AI review → merge → branch deletion. The documents of each feature are in `specs/NNN-name/`.

## Verification before finishing
- Review `git diff` and remove debug code.
- Run the backend build and tests; if `frontend/` changed, run lint, tests and build.
- Check that the specification and tasks are fulfilled; update README and `docs/` if the behaviour changed.
- Before the Pull Request check DRY, KISS and YAGNI (constitution 1.1 and 8): the same rule written in one place only, no unused code, no abstraction without a real boundary; then run the review step.

## Do not touch
- `.env`, secrets, `bin/`, `obj/`, `node_modules/`, `dist/`.
- Migrations that are already merged (create a new migration instead) and `*.Designer.cs`, `*ModelSnapshot.cs` by hand.
- `.specify/memory/constitution.md` without an explicit instruction.

## The AI must not
- Create Repository, GenericRepository or UnitOfWork.
- Write directly to `main`.
- Disable or skip tests, or change CI so that a failing command is hidden.
- Create a feature without an approved specification, or add requirements that are not in `docs/requirements.md`.
