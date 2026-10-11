# Užduotys: Savo rezultatų peržiūra

**Specifikacija:** `specs/009-rezultatu-perziura/spec.md`  
**Planas:** `specs/009-rezultatu-perziura/plan.md`  
**Priklauso nuo funkcijų:** 008  
**Žymos:** `[US1]` – priklauso naudotojo istorijai; be žymos – paruošimo užduotis.

Vykdyti pagal tvarką. Kiekviena fazė priklauso nuo ankstesnės. Testai rašomi prieš įgyvendinimą.

## Testai (rašomi pirma)

- [ ] T001 [US1] Testas: grąžinami tik prisijungusio mokinio rezultatai – `tests/backend/Results/ResultQueryServiceTests.cs`
- [ ] T002 [US1] Testas: ištrinto lygio rezultatas išlieka sąraše – `tests/backend/Results/ResultQueryServiceTests.cs`
- [ ] T003 [US1] Testas: tuščias sąrašas rodo paaiškinantį pranešimą – `tests/frontend/MyResultsPage.test.tsx`

## Įgyvendinimas (priklauso nuo: Testai)

- [ ] T004 [US1] Sukurti ResultQueryService – `backend/Application/Results/ResultQueryService.cs`
- [ ] T005 [US1] Pridėti GET /api/results – `backend/Api/Controllers/ResultsController.cs`
- [ ] T006 [US1] Sukurti rezultatų puslapį su tuščia būsena – `frontend/src/pages/MyResultsPage.tsx`

## Patikra (priklauso nuo: Įgyvendinimas)

- [ ] T007 [US1] Paleisti dotnet test ir npm test
