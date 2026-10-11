# Užduotys: Klasės mokinių rezultatų peržiūra

**Specifikacija:** `specs/012-klases-rezultatu-perziura/spec.md`  
**Planas:** `specs/012-klases-rezultatu-perziura/plan.md`  
**Priklauso nuo funkcijų:** 008, 011  
**Žymos:** `[US1]` – priklauso naudotojo istorijai; be žymos – paruošimo užduotis.

Vykdyti pagal tvarką. Kiekviena fazė priklauso nuo ankstesnės. Testai rašomi prieš įgyvendinimą.

## Testai (rašomi pirma)

- [ ] T001 [US1] Testas: vidurkis skaičiuojamas iš dabartinių narių geriausių rezultatų – `tests/backend/Classes/ClassStatsServiceTests.cs`
- [ ] T002 [US1] Testas: vidurkis nerodomas, kai lygio niekas nebaigė – `tests/backend/Classes/ClassStatsServiceTests.cs`
- [ ] T003 [US1] Testas: lygiai išdėstomi nuo mažiausio vidurkio – `tests/backend/Classes/ClassStatsServiceTests.cs`
- [ ] T004 [US1] Testas: svetimos klasės užklausa atmetama – `tests/backend/Classes/ClassAccessServiceTests.cs`
- [ ] T005 [US1] Testas: pašalinto nario rezultatų nėra – `tests/backend/Classes/ClassStatsServiceTests.cs`
- [ ] T006 [US1] Testas: mokytojas mato savo klasės narių sąrašą – `tests/backend/Classes/ClassResultsTests.cs`
- [ ] T007 [US1] Testas: mokytojas mato pasirinkto mokinio baigtų lygių rezultatus – `tests/backend/Classes/ClassResultsTests.cs`
- [ ] T008 [US1] Testas: mokytojas su dviem klasėmis mato rezultatus tik pasirinktos klasės – `tests/frontend/ClassResultsPage.test.tsx`

## Įgyvendinimas (priklauso nuo: Testai)

- [ ] T009 [US1] Sukurti ClassAccessService (klasės savininko patikra) – `backend/Application/Classes/ClassAccessService.cs`
- [ ] T010 [US1] Sukurti ClassStatsService – `backend/Application/Classes/ClassStatsService.cs`
- [ ] T011 [US1] Pridėti GET narių, mokinio rezultatų ir suvestinės užklausas – `backend/Api/Controllers/ClassResultsController.cs`
- [ ] T012 [US1] Sukurti klasės rezultatų puslapį su klasės pasirinkimu – `frontend/src/pages/ClassResultsPage.tsx`
- [ ] T013 [US1] Testas: suvestinės ir tuščios būsenos rodymas – `tests/frontend/ClassResultsPage.test.tsx`

## Patikra (priklauso nuo: Įgyvendinimas)

- [ ] T014 [US1] Paleisti dotnet test ir npm test
