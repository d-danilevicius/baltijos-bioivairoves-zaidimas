# Užduotys: Pradžios puslapis

**Specifikacija:** `specs/003-pradzios-puslapis/spec.md`  
**Planas:** `specs/003-pradzios-puslapis/plan.md`  
**Priklauso nuo funkcijų:** 002  
**Žymos:** `[US1]` – priklauso naudotojo istorijai; be žymos – paruošimo užduotis.

Vykdyti pagal tvarką. Kiekviena fazė priklauso nuo ankstesnės. Testai rašomi prieš įgyvendinimą.

## Testai (rašomi pirma)

- [ ] T001 [US1] Testas: neprisijungusiam rodomas aprašymas ir nuorodos „Prisijungti“, „Registruotis“ – `tests/frontend/HomePage.test.tsx`
- [ ] T002 [US1] Testas: prisijungęs naudotojas nukreipiamas pagal rolę – `tests/frontend/HomePage.test.tsx`
- [ ] T003 [US1] Testas: neegzistuojantis adresas rodo „Puslapis nerastas“ – `tests/frontend/NotFoundPage.test.tsx`

## Įgyvendinimas (priklauso nuo: Testai)

- [ ] T004 [US1] Sukurti pradžios puslapį – `frontend/src/pages/HomePage.tsx`
- [ ] T005 [US1] Sukurti „Puslapis nerastas“ puslapį su mygtuku „Į pradžią“ – `frontend/src/pages/NotFoundPage.tsx`
- [ ] T006 [US1] Pridėti nukreipimą pagal rolę – `frontend/src/routes/RoleRedirect.tsx`
- [ ] T007 [US1] Patikrinti išdėstymą 360 px pločio ekrane (be horizontalaus slinkimo) – `frontend/src/styles/home.css`

## Patikra (priklauso nuo: Įgyvendinimas)

- [ ] T008 [US1] Paleisti npm test ir npm run build
