# Užduotys: Registracija

**Specifikacija:** `specs/001-registracija/spec.md`  
**Planas:** `specs/001-registracija/plan.md`  
**Priklauso nuo funkcijų:** jokių  
**Žymos:** `[US1]` – priklauso naudotojo istorijai; be žymos – paruošimo užduotis.

Vykdyti pagal tvarką. Kiekviena fazė priklauso nuo ankstesnės. Testai rašomi prieš įgyvendinimą.

## Paruošimas

- [ ] T001 Sukurti .NET 10 sprendimą ir projektus Api, Application, Infrastructure – `backend/`
- [ ] T002 Sukurti React projektą su TypeScript naudojant Vite – `frontend/`
- [ ] T003 Sukurti xUnit testų projektą – `tests/backend/`
- [ ] T004 Nustatyti Vitest ir React Testing Library – `tests/frontend/`
- [ ] T005 Nuskaityti PostgreSQL ryšio eilutę iš aplinkos kintamojo (ne iš saugyklos failo) – `backend/Api/Program.cs`
- [ ] T006 Sukurti AppDbContext su ASP.NET Core Identity – `backend/Infrastructure/Data/AppDbContext.cs`
- [ ] T007 Nustatyti slaptažodžio taisyklę: bent 8 simboliai – `backend/Api/Program.cs`
- [ ] T008 Sukurti pradinę migraciją (naudotojų lentelės) – `backend/Infrastructure/Data/Migrations/`
- [ ] T009 Nustatyti .gitignore: bin/, obj/, node_modules/, dist/, .env – `.gitignore`
- [ ] T010 Įjungti HTTPS nukreipimą ir HSTS – `backend/Api/Program.cs`
- [ ] T011 Sukurti automatinę patikrą: dotnet format, build, test, npm run lint, npm test – `.github/workflows/ci.yml`
- [ ] T012 Aprašyti kasdienę duomenų atsarginę kopiją ir 7 dienų saugojimą – `docs/operations.md`

## Testai (rašomi pirma) (priklauso nuo: Paruošimas)

- [ ] T013 [US1] Testas: sėkminga registracija mokinio rolei – `tests/backend/Registration/RegistrationServiceTests.cs`
- [ ] T014 [US1] Testas: pasikartojantis el. paštas atmetamas – `tests/backend/Registration/RegistrationServiceTests.cs`
- [ ] T015 [US1] Testas: pasikartojantis naudotojo vardas atmetamas – `tests/backend/Registration/RegistrationServiceTests.cs`
- [ ] T016 [US1] Testas: slaptažodis trumpesnis nei 8 atmetamas – `tests/backend/Registration/RegistrationServiceTests.cs`
- [ ] T017 [US1] Testas: naudotojo vardas už 3–30 ribų arba su neleistinais simboliais atmetamas – `tests/backend/Registration/RegistrationServiceTests.cs`
- [ ] T018 [US1] Testas: registracija administratoriaus role atmetama – `tests/backend/Registration/RegistrationServiceTests.cs`

## Įgyvendinimas (priklauso nuo: Testai)

- [ ] T019 [US1] Sukurti užklausos ir rezultato klases – `backend/Application/Auth/RegistrationRequest.cs`
- [ ] T020 [US1] Sukurti RegistrationService su visomis registracijos taisyklėmis – `backend/Application/Auth/RegistrationService.cs`
- [ ] T021 [US1] Sukurti UserRepository naudojant Identity – `backend/Infrastructure/Users/UserRepository.cs`
- [ ] T022 [US1] Pridėti POST /api/auth/register – `backend/Api/Controllers/AuthController.cs`
- [ ] T023 [US1] Sukurti registracijos puslapį su forma ir klaidų rodymu – `frontend/src/pages/RegisterPage.tsx`
- [ ] T024 [US1] Testas: registracijos formos klaidų pranešimai – `tests/frontend/RegisterPage.test.tsx`

## Patikra (priklauso nuo: Įgyvendinimas)

- [ ] T025 [US1] Paleisti dotnet build, dotnet test, npm run lint, npm test
