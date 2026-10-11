# Užduotys: Naudotojo profilis

**Specifikacija:** `specs/004-profilis/spec.md`  
**Planas:** `specs/004-profilis/plan.md`  
**Priklauso nuo funkcijų:** 002 (klasės pavadinimas rodomas po 011)  
**Žymos:** `[US1]` – priklauso naudotojo istorijai; be žymos – paruošimo užduotis.

Vykdyti pagal tvarką. Kiekviena fazė priklauso nuo ankstesnės. Testai rašomi prieš įgyvendinimą.

## Testai (rašomi pirma)

- [ ] T001 [US1] Testas: grąžinami tik prisijungusio naudotojo duomenys – `tests/backend/Profile/ProfileServiceTests.cs`
- [ ] T002 [US1] Testas: mokinys be klasės gauna tuščią klasės lauką – `tests/backend/Profile/ProfileServiceTests.cs`
- [ ] T003 [US1] Testas: profilio puslapis rodo vardą, el. paštą, rolę – `tests/frontend/ProfilePage.test.tsx`

## Įgyvendinimas (priklauso nuo: Testai)

- [ ] T004 [US1] Sukurti ProfileService – `backend/Application/Profile/ProfileService.cs`
- [ ] T005 [US1] Pridėti GET /api/profile – `backend/Api/Controllers/ProfileController.cs`
- [ ] T006 [US1] Sukurti profilio puslapį – `frontend/src/pages/ProfilePage.tsx`

## Patikra (priklauso nuo: Įgyvendinimas)

- [ ] T007 [US1] Paleisti dotnet test ir npm test
