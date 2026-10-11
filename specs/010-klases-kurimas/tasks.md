# Užduotys: Klasės sukūrimas ir ištrynimas

**Specifikacija:** `specs/010-klases-kurimas/spec.md`  
**Planas:** `specs/010-klases-kurimas/plan.md`  
**Priklauso nuo funkcijų:** 002  
**Žymos:** `[US1]` – priklauso naudotojo istorijai; be žymos – paruošimo užduotis.

Vykdyti pagal tvarką. Kiekviena fazė priklauso nuo ankstesnės. Testai rašomi prieš įgyvendinimą.

## Duomenys

- [ ] T001 Sukurti Class esybę (Name 1–50, Code unikalus) – `backend/Application/Entities/SchoolClass.cs`
- [ ] T002 Sukurti konfigūraciją su unikaliu Code – `backend/Infrastructure/Data/Configurations/ClassConfiguration.cs`
- [ ] T003 Sukurti migraciją – `backend/Infrastructure/Data/Migrations/`

## Testai (rašomi pirma) (priklauso nuo: Duomenys)

- [ ] T004 [US1] Testas: sukurta klasė gauna unikalų 6 simbolių kodą – `tests/backend/Classes/ClassServiceTests.cs`
- [ ] T005 [US1] Testas: tuščias ar per ilgas pavadinimas atmetamas – `tests/backend/Classes/ClassServiceTests.cs`
- [ ] T006 [US1] Testas: mokinys negali sukurti klasės – `tests/backend/Classes/ClassServiceTests.cs`
- [ ] T007 [US1] Testas: mokytojas negali ištrinti svetimos klasės – `tests/backend/Classes/ClassServiceTests.cs`
- [ ] T008 [US1] Testas: ištrynus klasę mokinių rezultatai išlieka – `tests/backend/Classes/ClassServiceTests.cs`
- [ ] T009 [US1] Testas: kodo pasikartojimo atveju sugeneruojamas kitas kodas – `tests/backend/Classes/ClassCodeGeneratorTests.cs`

## Įgyvendinimas (priklauso nuo: Testai)

- [ ] T010 [US1] Sukurti ClassCodeGenerator su pakartojimu, jei kodas užimtas – `backend/Application/Classes/ClassCodeGenerator.cs`
- [ ] T011 [US1] Sukurti ClassService (kurti, sąrašas, trinti) – `backend/Application/Classes/ClassService.cs`
- [ ] T012 [US1] Sukurti ClassRepository – `backend/Infrastructure/Classes/ClassRepository.cs`
- [ ] T013 [US1] Pridėti POST/GET /api/classes ir DELETE /api/classes/{id} – `backend/Api/Controllers/ClassesController.cs`
- [ ] T014 [US1] Sukurti mokytojo klasių puslapį su patvirtinimo langu trynimui – `frontend/src/pages/TeacherClassesPage.tsx`
- [ ] T015 [US1] Testas: forma ir patvirtinimo langas – `tests/frontend/TeacherClassesPage.test.tsx`

## Patikra (priklauso nuo: Įgyvendinimas)

- [ ] T016 [US1] Paleisti dotnet test ir npm test
