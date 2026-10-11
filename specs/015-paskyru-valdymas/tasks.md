# Užduotys: Paskyrų valdymas

**Specifikacija:** `specs/015-paskyru-valdymas/spec.md`  
**Planas:** `specs/015-paskyru-valdymas/plan.md`  
**Priklauso nuo funkcijų:** 002, 008, 011  
**Žymos:** `[US1]` – priklauso naudotojo istorijai; be žymos – paruošimo užduotis.

Vykdyti pagal tvarką. Kiekviena fazė priklauso nuo ankstesnės. Testai rašomi prieš įgyvendinimą.

## Testai (rašomi pirma)

- [ ] T001 [US1] Testas: administratorius blokuoja ir atblokuoja paskyrą – `tests/backend/Admin/AccountAdminServiceTests.cs`
- [ ] T002 [US1] Testas: administratorius negali užblokuoti ar ištrinti savęs – `tests/backend/Admin/AccountAdminServiceTests.cs`
- [ ] T003 [US1] Testas: negalima ištrinti paskutinio administratoriaus – `tests/backend/Admin/AccountAdminServiceTests.cs`
- [ ] T004 [US1] Testas: ištrynus mokinį ištrinami jo rezultatai ir perskaičiuojamas klasės vidurkis – `tests/backend/Admin/AccountDeleteTests.cs`
- [ ] T005 [US1] Testas: ištrynus mokytoją ištrinamos jo klasės, bet mokinių rezultatai išlieka – `tests/backend/Admin/AccountDeleteTests.cs`
- [ ] T006 [US1] Testas: mokinys ir mokytojas negali pasiekti administratoriaus užklausų – `tests/backend/Admin/AdminUsersAuthorizationTests.cs`
- [ ] T007 [US1] Testas: kiekvienas administratoriaus veiksmas įrašomas į žurnalą – `tests/backend/Admin/AccountAdminServiceTests.cs`
- [ ] T008 [US1] Testas: administratorius mato paskyrų sąrašą – `tests/backend/Admin/AccountAdminServiceTests.cs`

## Įgyvendinimas (priklauso nuo: Testai)

- [ ] T009 [US1] Sukurti AccountAdminService (blokuoti, atblokuoti, trinti vienoje duomenų bazės operacijoje) – `backend/Application/Admin/AccountAdminService.cs`
- [ ] T010 [US1] Pridėti paskyrų užklausas (GET, block, unblock, DELETE) – `backend/Api/Controllers/AdminUsersController.cs`
- [ ] T011 [US1] Apriboti kontrolerį administratoriaus rolei – `backend/Api/Controllers/AdminUsersController.cs`
- [ ] T012 [US1] Sukurti paskyrų puslapį su patvirtinimo langais – `frontend/src/pages/AdminUsersPage.tsx`
- [ ] T013 [US1] Testas: patvirtinimo langai – `tests/frontend/AdminUsersPage.test.tsx`

## Patikra (priklauso nuo: Įgyvendinimas)

- [ ] T014 [US1] Paleisti dotnet test ir npm test

## Galutinė projekto patikra (priklauso nuo: Patikra)

- [ ] T015 Patikrinti, ar sistema išlaiko 50 vienu metu žaidžiančių naudotojų (NFR-014) – `tests/load/`
- [ ] T016 Patikrinti veikimą Chrome, Firefox, Edge ir Safari (NFR-017) – `docs/browser-check.md`
