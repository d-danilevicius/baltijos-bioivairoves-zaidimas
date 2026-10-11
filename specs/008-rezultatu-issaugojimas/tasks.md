# Užduotys: Rezultatų išsaugojimas ir žaidimo tęsimas

**Specifikacija:** `specs/008-rezultatu-issaugojimas/spec.md`  
**Planas:** `specs/008-rezultatu-issaugojimas/plan.md`  
**Priklauso nuo funkcijų:** 007  
**Žymos:** `[US1]` – priklauso naudotojo istorijai; be žymos – paruošimo užduotis.

Vykdyti pagal tvarką. Kiekviena fazė priklauso nuo ankstesnės. Testai rašomi prieš įgyvendinimą.

## Duomenys

- [ ] T001 Sukurti LevelResult esybę su unikalumu (StudentId, LevelId) – `backend/Application/Entities/LevelResult.cs`
- [ ] T002 Sukurti migraciją – `backend/Infrastructure/Data/Migrations/`

## Testai (rašomi pirma) (priklauso nuo: Duomenys)

- [ ] T003 [US1] Testas: rezultatas išsaugomas baigus lygį – `tests/backend/Results/ResultServiceTests.cs`
- [ ] T004 [US1] Testas: prastesnis rezultatas nepakeičia geriausio – `tests/backend/Results/ResultServiceTests.cs`
- [ ] T005 [US1] Testas: geresnis rezultatas pakeičia geriausią – `tests/backend/Results/ResultServiceTests.cs`
- [ ] T006 [US1] Testas: nebaigtas lygis rezultato neišsaugo – `tests/backend/Results/ResultServiceTests.cs`
- [ ] T007 [US1] Testas: tęsimas po naujo prisijungimo grąžina paskutinį nebaigtą lygį – `tests/backend/Attempts/CurrentAttemptTests.cs`
- [ ] T008 [US1] Testas: bandymo baigimas ir rezultato išsaugojimas vyksta kartu (nelieka baigto bandymo be rezultato) – `tests/backend/Results/ResultServiceTests.cs`

## Įgyvendinimas (priklauso nuo: Testai)

- [ ] T009 [US1] Sukurti ResultService.Save, vykdomą vienoje duomenų bazės operacijoje su bandymo užbaigimu – `backend/Application/Results/ResultService.cs`
- [ ] T010 [US1] Sukurti ResultRepository – `backend/Infrastructure/Results/ResultRepository.cs`
- [ ] T011 [US1] Pridėti GET /api/attempts/current – `backend/Api/Controllers/AttemptsController.cs`
- [ ] T012 [US1] Pridėti mygtuką „Tęsti“ lygių sąraše – `frontend/src/pages/LevelListPage.tsx`
- [ ] T013 [US1] Testas: „Tęsti“ mygtukas – `tests/frontend/LevelListPage.test.tsx`

## Patikra (priklauso nuo: Įgyvendinimas)

- [ ] T014 [US1] Paleisti dotnet test ir npm test
