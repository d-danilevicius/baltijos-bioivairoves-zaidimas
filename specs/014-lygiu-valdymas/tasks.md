# Užduotys: Lygių valdymas

**Specifikacija:** `specs/014-lygiu-valdymas/spec.md`  
**Planas:** `specs/014-lygiu-valdymas/plan.md`  
**Priklauso nuo funkcijų:** 002, 005, 006  
**Žymos:** `[US1]` – priklauso naudotojo istorijai; be žymos – paruošimo užduotis.

Vykdyti pagal tvarką. Kiekviena fazė priklauso nuo ankstesnės. Testai rašomi prieš įgyvendinimą.

## Duomenys

- [ ] T001 Patikrinti, kad Level turi IsActive (loginis trynimas) – `backend/Application/Entities/Level.cs`

## Testai (rašomi pirma) (priklauso nuo: Duomenys)

- [ ] T002 [US1] Testas: lygio eilės numeris turi būti unikalus sveikas skaičius nuo 1 – `tests/backend/Admin/LevelValidatorTests.cs`
- [ ] T003 [US1] Testas: lygyje turi būti bent 1 situacija – `tests/backend/Admin/LevelValidatorTests.cs`
- [ ] T004 [US1] Testas: situacijoje 2–4 variantai – `tests/backend/Admin/LevelValidatorTests.cs`
- [ ] T005 [US1] Testas: poveikio ribos (būklė −50…50, rūšys −10…10) – `tests/backend/Admin/LevelValidatorTests.cs`
- [ ] T006 [US1] Testas: ne administratorius lygio nekuria – `tests/backend/Admin/AdminLevelsAuthorizationTests.cs`
- [ ] T007 [US1] Testas: ištrynus lygį rezultatai išlieka, o pradėtas bandymas gali būti užbaigtas – `tests/backend/Admin/LevelDeleteTests.cs`
- [ ] T008 [US1] Testas: redaguojant lygį išsaugoti rezultatai nesikeičia – `tests/backend/Admin/LevelEditTests.cs`
- [ ] T009 [US1] Testas: administratorius sukuria lygį su situacijomis ir variantais – `tests/backend/Admin/LevelAdminServiceTests.cs`

## Įgyvendinimas (priklauso nuo: Testai)

- [ ] T010 [US1] Sukurti LevelValidator – `backend/Application/Admin/LevelValidator.cs`
- [ ] T011 [US1] Sukurti LevelAdminService – `backend/Application/Admin/LevelAdminService.cs`
- [ ] T012 [US1] Pridėti administratoriaus lygių užklausas (GET, POST, PUT, DELETE) – `backend/Api/Controllers/AdminLevelsController.cs`
- [ ] T013 [US1] Apriboti kontrolerį administratoriaus rolei – `backend/Api/Controllers/AdminLevelsController.cs`
- [ ] T014 [US1] Sukurti administratoriaus lygių puslapį – `frontend/src/pages/AdminLevelsPage.tsx`
- [ ] T015 [US1] Sukurti lygio redagavimo formą – `frontend/src/components/LevelForm.tsx`
- [ ] T016 [US1] Testas: lygio forma – `tests/frontend/LevelForm.test.tsx`

## Patikra (priklauso nuo: Įgyvendinimas)

- [ ] T017 [US1] Paleisti dotnet test ir npm test
