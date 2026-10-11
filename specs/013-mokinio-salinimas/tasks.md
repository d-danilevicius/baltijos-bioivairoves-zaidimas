# Užduotys: Mokinio pašalinimas iš klasės

**Specifikacija:** `specs/013-mokinio-salinimas/spec.md`  
**Planas:** `specs/013-mokinio-salinimas/plan.md`  
**Priklauso nuo funkcijų:** 012  
**Žymos:** `[US1]` – priklauso naudotojo istorijai; be žymos – paruošimo užduotis.

Vykdyti pagal tvarką. Kiekviena fazė priklauso nuo ankstesnės. Testai rašomi prieš įgyvendinimą.

## Testai (rašomi pirma)

- [ ] T001 [US1] Testas: mokytojas pašalina savo klasės mokinį – `tests/backend/Classes/RemoveStudentTests.cs`
- [ ] T002 [US1] Testas: svetimos klasės mokinio pašalinimas atmetamas – `tests/backend/Classes/RemoveStudentTests.cs`
- [ ] T003 [US1] Testas: pašalinus mokinį jo rezultatai išlieka – `tests/backend/Classes/RemoveStudentTests.cs`
- [ ] T004 [US1] Testas: pašalintas mokinys gali vėl prisijungti kodu – `tests/backend/Classes/RemoveStudentTests.cs`

## Įgyvendinimas (priklauso nuo: Testai)

- [ ] T005 [US1] Sukurti ClassMembershipService.Remove – `backend/Application/Classes/ClassMembershipService.cs`
- [ ] T006 [US1] Pridėti DELETE /api/classes/{id}/students/{studentId} – `backend/Api/Controllers/ClassesController.cs`
- [ ] T007 [US1] Pridėti mygtuką „Pašalinti“ su patvirtinimo langu – `frontend/src/pages/ClassResultsPage.tsx`
- [ ] T008 [US1] Testas: patvirtinimo langas – `tests/frontend/RemoveStudent.test.tsx`

## Patikra (priklauso nuo: Įgyvendinimas)

- [ ] T009 [US1] Paleisti dotnet test ir npm test
