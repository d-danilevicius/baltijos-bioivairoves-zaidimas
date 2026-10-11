# Užduotys: Sprendimų priėmimas lygyje

**Specifikacija:** `specs/006-sprendimu-priemimas/spec.md`  
**Planas:** `specs/006-sprendimu-priemimas/plan.md`  
**Priklauso nuo funkcijų:** 005  
**Žymos:** `[US1]` – priklauso naudotojo istorijai; be žymos – paruošimo užduotis.

Vykdyti pagal tvarką. Kiekviena fazė priklauso nuo ankstesnės. Testai rašomi prieš įgyvendinimą.

## Duomenys

- [ ] T001 Sukurti Situation esybę – `backend/Application/Entities/Situation.cs`
- [ ] T002 Sukurti DecisionOption esybę – `backend/Application/Entities/DecisionOption.cs`
- [ ] T003 Sukurti Choice esybę su unikalumu (AttemptId, SituationId) – `backend/Application/Entities/Choice.cs`
- [ ] T004 Sukurti migraciją – `backend/Infrastructure/Data/Migrations/`

## Testai (rašomi pirma) (priklauso nuo: Duomenys)

- [ ] T005 [US1] Testas: būklė apribojama ties 100 – `tests/backend/Attempts/ChooseTests.cs`
- [ ] T006 [US1] Testas: būklė apribojama ties 0 – `tests/backend/Attempts/ChooseTests.cs`
- [ ] T007 [US1] Testas: antras pasirinkimas toje pačioje situacijoje atmetamas – `tests/backend/Attempts/ChooseTests.cs`
- [ ] T008 [US1] Testas: svetimo bandymo pasirinkimas atmetamas – `tests/backend/Attempts/ChooseTests.cs`
- [ ] T009 [US1] Testas: grąžinamas teisingas būklės ir rūšių pokytis – `tests/backend/Attempts/ChooseTests.cs`

## Įgyvendinimas (priklauso nuo: Testai)

- [ ] T010 [US1] Sukurti AttemptService.Choose – `backend/Application/Attempts/AttemptService.cs`
- [ ] T011 [US1] Sukurti ChoiceRepository – `backend/Infrastructure/Attempts/ChoiceRepository.cs`
- [ ] T012 [US1] Pridėti GET /api/attempts/{id}/situation ir POST /api/attempts/{id}/choices – `backend/Api/Controllers/AttemptsController.cs`
- [ ] T013 [US1] Sukurti situacijos puslapį su pokyčio rodymu – `frontend/src/pages/SituationPage.tsx`
- [ ] T014 [US1] Testas: variantų pasirinkimas ir pokyčio rodymas – `tests/frontend/SituationPage.test.tsx`
- [ ] T015 [US1] Įrašyti pradines situacijas ir sprendimo variantus 3 pradiniams lygiams (migracijoje) – `backend/Infrastructure/Data/Migrations/`

## Patikra (priklauso nuo: Įgyvendinimas)

- [ ] T016 [US1] Paleisti dotnet test ir npm test; patikrinti, kad pokytis rodomas per 1 s
