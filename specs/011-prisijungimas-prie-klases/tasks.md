# Užduotys: Prisijungimas prie klasės kodu

**Specifikacija:** `specs/011-prisijungimas-prie-klases/spec.md`  
**Planas:** `specs/011-prisijungimas-prie-klases/plan.md`  
**Priklauso nuo funkcijų:** 010  
**Žymos:** `[US1]` – priklauso naudotojo istorijai; be žymos – paruošimo užduotis.

Vykdyti pagal tvarką. Kiekviena fazė priklauso nuo ankstesnės. Testai rašomi prieš įgyvendinimą.

## Duomenys

- [ ] T001 Sukurti ClassMembership esybę su unikaliu StudentId – `backend/Application/Entities/ClassMembership.cs`
- [ ] T002 Sukurti migraciją – `backend/Infrastructure/Data/Migrations/`

## Testai (rašomi pirma) (priklauso nuo: Duomenys)

- [ ] T003 [US1] Testas: prisijungimas su teisingu kodu – `tests/backend/Classes/MembershipServiceTests.cs`
- [ ] T004 [US1] Testas: neegzistuojantis kodas atmetamas – `tests/backend/Classes/MembershipServiceTests.cs`
- [ ] T005 [US1] Testas: kodas ne iš 6 didžiųjų raidžių ar skaičių atmetamas – `tests/backend/Classes/MembershipServiceTests.cs`
- [ ] T006 [US1] Testas: klasės keitimas be patvirtinimo atmetamas – `tests/backend/Classes/MembershipServiceTests.cs`
- [ ] T007 [US1] Testas: klasės keitimas su patvirtinimu pakeičia narystę – `tests/backend/Classes/MembershipServiceTests.cs`
- [ ] T008 [US1] Testas: mokinys be klasės gali pradėti lygį – `tests/backend/Classes/MembershipServiceTests.cs`
- [ ] T009 [US1] Testas: profilis rodo mokinio klasės pavadinimą – `tests/backend/Profile/ProfileServiceTests.cs`

## Įgyvendinimas (priklauso nuo: Testai)

- [ ] T010 [US1] Sukurti ClassMembershipService.Join – `backend/Application/Classes/ClassMembershipService.cs`
- [ ] T011 [US1] Sukurti MembershipRepository – `backend/Infrastructure/Classes/MembershipRepository.cs`
- [ ] T012 [US1] Pridėti POST /api/classes/join – `backend/Api/Controllers/ClassesController.cs`
- [ ] T013 [US1] Sukurti prisijungimo prie klasės puslapį su keitimo patvirtinimu – `frontend/src/pages/JoinClassPage.tsx`
- [ ] T014 [US1] Testas: forma ir patvirtinimas – `tests/frontend/JoinClassPage.test.tsx`
- [ ] T015 [US1] Pridėti klasės pavadinimą į profilio atsakymą – `backend/Application/Profile/ProfileService.cs`

## Patikra (priklauso nuo: Įgyvendinimas)

- [ ] T016 [US1] Paleisti dotnet test ir npm test
