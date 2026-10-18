@echo off
rem Starts the backend and the frontend in two windows. The database must already be running (docker compose up -d).
cd /d "%~dp0"
start "Backend" cmd /k dotnet run --project backend/src/Project.Api --launch-profile http
start "Frontend" cmd /k "cd frontend && npm run dev"
