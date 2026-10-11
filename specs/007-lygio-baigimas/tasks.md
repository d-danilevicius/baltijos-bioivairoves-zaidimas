# Užduotys: Lygio baigimas ir kito lygio atrakinimas

**Specifikacija:** `specs/007-lygio-baigimas/spec.md`  
**Planas:** `specs/007-lygio-baigimas/plan.md`  
**Priklauso nuo funkcijų:** 006  
**Žymos:** `[US1]` – priklauso naudotojo istorijai; be žymos – paruošimo užduotis.

Vykdyti pagal tvarką. Kiekviena fazė priklauso nuo ankstesnės. Testai rašomi prieš įgyvendinimą.

## Duomenys

- [ ] T001 Pridėti FinalScore ir FinishedAt į LevelAttempt – `backend/Application/Entities/LevelAttempt.cs`
- [ ] T002 Sukurti migraciją – `backend/Infrastructure/Data/Migrations/`

## Testai (rašomi pirma) (priklauso nuo: Duomenys)

- [ ] T003 [US1] Testas: lygis baigiamas atsakius į visas situacijas – `tests/backend/Attempts/FinishTests.cs`
- [ ] T004 [US1] Testas: rezultatas lygus galutinei būklei – `tests/backend/Attempts/FinishTests.cs`
- [ ] T005 [US1] Testas: kitas lygis atrakinamas po baigimo – `tests/backend/Levels/LevelAccessServiceTests.cs`
- [ ] T006 [US1] Testas: rezultatas nerodomas, kol neatsakyta į visas situacijas – `tests/backend/Attempts/FinishTests.cs`

## Įgyvendinimas (priklauso nuo: Testai)

- [ ] T007 [US1] Sukurti AttemptService.Finish – `backend/Application/Attempts/AttemptService.cs`
- [ ] T008 [US1] Pridėti GET /api/attempts/{id}/result – `backend/Api/Controllers/AttemptsController.cs`
- [ ] T009 [US1] Sukurti lygio rezultato puslapį – `frontend/src/pages/LevelResultPage.tsx`
- [ ] T010 [US1] Testas: rezultato ekranas – `tests/frontend/LevelResultPage.test.tsx`

## Patikra (priklauso nuo: Įgyvendinimas)

- [ ] T011 [US1] Paleisti dotnet test ir npm test
