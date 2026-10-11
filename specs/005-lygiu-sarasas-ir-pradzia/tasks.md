# Užduotys: Lygių sąrašas ir lygio pradžia

**Specifikacija:** `specs/005-lygiu-sarasas-ir-pradzia/spec.md`  
**Planas:** `specs/005-lygiu-sarasas-ir-pradzia/plan.md`  
**Priklauso nuo funkcijų:** 002  
**Žymos:** `[US1]` – priklauso naudotojo istorijai; be žymos – paruošimo užduotis.

Vykdyti pagal tvarką. Kiekviena fazė priklauso nuo ankstesnės. Testai rašomi prieš įgyvendinimą.

## Duomenys

- [ ] T001 Sukurti Level esybę – `backend/Application/Entities/Level.cs`
- [ ] T002 Sukurti LevelAttempt esybę – `backend/Application/Entities/LevelAttempt.cs`
- [ ] T003 Sukurti konfigūracijas (OrderNo unikalus) – `backend/Infrastructure/Data/Configurations/LevelConfiguration.cs`
- [ ] T004 Sukurti migraciją ir 3 pradinių lygių duomenis – `backend/Infrastructure/Data/Migrations/`

## Testai (rašomi pirma) (priklauso nuo: Duomenys)

- [ ] T005 [US1] Testas: pirmas lygis atrakintas naujam mokiniui – `tests/backend/Levels/LevelAccessServiceTests.cs`
- [ ] T006 [US1] Testas: antras lygis užrakintas, kol nebaigtas pirmas – `tests/backend/Levels/LevelAccessServiceTests.cs`
- [ ] T007 [US1] Testas: bandymas prasideda nuo lygio pradinės būklės – `tests/backend/Levels/AttemptServiceTests.cs`
- [ ] T008 [US1] Testas: užrakinto lygio pradžia atmetama – `tests/backend/Levels/AttemptServiceTests.cs`
- [ ] T009 [US1] Testas: sąraše yra bent 3 lygiai ir nėra neaktyvių lygių – `tests/backend/Levels/LevelListTests.cs`
- [ ] T010 [US1] Testas: lygio aprašymas grąžina pavadinimą, aprašymą, pradinę būklę ir situacijų skaičių – `tests/backend/Levels/LevelListTests.cs`
- [ ] T011 [US1] Testas: baigtą lygį galima pakartoti – `tests/backend/Levels/AttemptServiceTests.cs`

## Įgyvendinimas (priklauso nuo: Testai)

- [ ] T012 [US1] Sukurti LevelAccessService – `backend/Application/Levels/LevelAccessService.cs`
- [ ] T013 [US1] Sukurti AttemptService.Start – `backend/Application/Attempts/AttemptService.cs`
- [ ] T014 [US1] Sukurti LevelRepository ir AttemptRepository – `backend/Infrastructure/Levels/`
- [ ] T015 [US1] Pridėti GET /api/levels, GET /api/levels/{id}, POST /api/levels/{id}/attempts – `backend/Api/Controllers/LevelsController.cs`
- [ ] T016 [US1] Sukurti lygių sąrašo puslapį – `frontend/src/pages/LevelListPage.tsx`
- [ ] T017 [US1] Sukurti lygio aprašymo puslapį – `frontend/src/pages/LevelIntroPage.tsx`
- [ ] T018 [US1] Sukurti patvirtinimo langą išeinant iš nebaigto lygio – `frontend/src/components/ConfirmDialog.tsx`
- [ ] T019 [US1] Testas: lygių sąrašas ir patvirtinimo langas – `tests/frontend/LevelListPage.test.tsx`
- [ ] T020 [US1] Leisti pradėti naują bandymą jau baigtam lygiui – `backend/Application/Attempts/AttemptService.cs`

## Patikra (priklauso nuo: Įgyvendinimas)

- [ ] T021 [US1] Paleisti dotnet test ir npm test
