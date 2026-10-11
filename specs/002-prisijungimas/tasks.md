# Užduotys: Prisijungimas ir atsijungimas

**Specifikacija:** `specs/002-prisijungimas/spec.md`  
**Planas:** `specs/002-prisijungimas/plan.md`  
**Priklauso nuo funkcijų:** 001  
**Žymos:** `[US1]` – priklauso naudotojo istorijai; be žymos – paruošimo užduotis.

Vykdyti pagal tvarką. Kiekviena fazė priklauso nuo ankstesnės. Testai rašomi prieš įgyvendinimą.

## Duomenys

- [ ] T001 Pridėti IsBlocked į naudotojo esybę – `backend/Application/Entities/AppUser.cs`
- [ ] T002 Sukurti ActionLog esybę – `backend/Application/Entities/ActionLogEntry.cs`
- [ ] T003 Sukurti ActionLog konfigūraciją – `backend/Infrastructure/Data/Configurations/ActionLogConfiguration.cs`
- [ ] T004 Sukurti migraciją – `backend/Infrastructure/Data/Migrations/`

## Testai (rašomi pirma) (priklauso nuo: Duomenys)

- [ ] T005 [US1] Testas: prisijungimas su teisingais duomenimis – `tests/backend/Login/LoginServiceTests.cs`
- [ ] T006 [US1] Testas: neteisingi duomenys grąžina bendrą pranešimą – `tests/backend/Login/LoginServiceTests.cs`
- [ ] T007 [US1] Testas: užblokuota paskyra neprisijungia – `tests/backend/Login/LoginServiceTests.cs`
- [ ] T008 [US1] Testas: slaptažodis neįrašomas į žurnalą – `tests/backend/Login/ActionLogTests.cs`
- [ ] T009 [US1] Testas: neprisijungęs negali pasiekti apsaugoto maršruto – `tests/backend/Login/AuthorizationTests.cs`
- [ ] T010 [US1] Testas: atsijungimas baigia sesiją – `tests/backend/Login/LoginServiceTests.cs`
- [ ] T011 [US1] Testas: sesija baigiasi po 30 min neveiklumo – `tests/backend/Login/SessionTests.cs`
- [ ] T012 [US1] Testas: sėkmingas ir nepavykęs prisijungimas įrašomi į žurnalą – `tests/backend/Login/ActionLogTests.cs`

## Įgyvendinimas (priklauso nuo: Testai)

- [ ] T013 [US1] Sukurti ActionLogWriter – `backend/Application/Logging/ActionLogWriter.cs`
- [ ] T014 [US1] Sukurti LoginService (blokavimo patikra, žurnalo įrašas) – `backend/Application/Auth/LoginService.cs`
- [ ] T015 [US1] Nustatyti slapuko sesiją: tik HTTPS, 30 min neveiklumo – `backend/Api/Program.cs`
- [ ] T016 [US1] Įjungti CSRF apsaugos raktą keičiančioms užklausoms – `backend/Api/Program.cs`
- [ ] T017 [US1] Pridėti POST /api/auth/login ir POST /api/auth/logout – `backend/Api/Controllers/AuthController.cs`
- [ ] T018 [US1] Sukurti prisijungimo puslapį – `frontend/src/pages/LoginPage.tsx`
- [ ] T019 [US1] Sukurti maršrutų apsaugą neprisijungusiems – `frontend/src/routes/ProtectedRoute.tsx`
- [ ] T020 [US1] Testas: nukreipimas į prisijungimą neprisijungusiam – `tests/frontend/ProtectedRoute.test.tsx`

## Patikra (priklauso nuo: Įgyvendinimas)

- [ ] T021 [US1] Paleisti dotnet test ir npm test
