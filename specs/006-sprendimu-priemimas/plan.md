# Įgyvendinimo planas: Sprendimų priėmimas lygyje

**Funkcija:** `specs/006-sprendimu-priemimas/spec.md`  
**Šaka:** `006-sprendimu-priemimas`  
**Būsena:** juodraštis  
**Priklauso nuo funkcijų:** 005

## Santrauka

Planas aprašo, kaip įgyvendinti funkciją „Sprendimų priėmimas lygyje“ pagal patvirtintą specifikaciją.

## Techninis kontekstas

- **Kalba ir versija:** C# su .NET 10 (serveris); TypeScript (naršyklės dalis).
- **Bibliotekos:** Naujų bibliotekų nepridedama.
- **Naršyklės dalis:** React su Vite ir React Router.
- **Duomenų saugojimas:** PostgreSQL per Entity Framework Core; pakeitimai tik per migracijas.
- **Testavimas:** xUnit (serveris), Vitest ir React Testing Library (naršyklės dalis).
- **Tikslinė platforma:** žiniatinklio programa naujausiose Chrome, Firefox, Edge ir Safari versijose; ekranai nuo 360 px pločio.
- **Techniniai apribojimai:** tik HTTPS; puslapis atsidaro per 3 s; paslaptys nelaikomos saugykloje.

## Programos struktūra ir pagrindiniai komponentai

- Api: `AttemptsController`.
- Application: `AttemptService.Choose` (viena parinktis situacijoje, būklės riba 0–100, rūšių skaičiaus pokytis).
- Infrastructure: `ChoiceRepository`.
- Naršyklės dalis: `SituationPage` su pokyčio rodymu.

## Serverio užklausų pakeitimai

- `GET /api/attempts/{id}/situation` – dabartinė situacija su variantais.
- `POST /api/attempts/{id}/choices` – pasirenka variantą; grąžina naują būklę ir rūšių skaičių; 409, jei situacijoje jau pasirinkta.

## Duomenų modelio pakeitimai

- Nauja `Situations`: Id, LevelId, OrderNo, Text.
- Nauja `DecisionOptions`: Id, SituationId, Text, StateEffect, SpeciesEffect.
- Nauja `Choices`: Id, AttemptId, SituationId, OptionId; unikalumas (AttemptId, SituationId).

## Testavimo planas

- xUnit: būklė apribojama 0 ir 100; antras pasirinkimas atmetamas; svetimas bandymas atmetamas; teisingas pokytis.
- Vitest: variantų pasirinkimas ir pokyčio rodymas.

## Techninės pastabos

Pokytį skaičiuoja serveris, naršyklė tik jį parodo. Atsakymas turi grįžti per 1 s (NFR-013).

## Konstitucijos patikra

- **1. Kodo kokybė:** atitinka. Kodas tikrinamas `dotnet format`, ESLint ir Prettier; kompiliavimas be įspėjimų.
- **2. Architektūra:** atitinka. Verslo taisyklės yra tik Application sluoksnyje (aukščiau nurodytose paslaugose), Api tik priima užklausą ir tikrina rolę, Infrastructure tik skaito ir rašo duomenis, naršyklė taisyklių neskaičiuoja. Duomenų bazės pakeitimai tik per migracijas.
- **3. Testavimas:** atitinka. Kiekvienai susijusiai verslo taisyklei numatytas bent vienas automatinis testas (žr. testavimo planą).
- **4. Saugumas:** atitinka. Rolė ir nuosavybė tikrinamos serveryje, duomenys tikrinami serveryje, ryšys tik per HTTPS, slaptažodžiai ir paslaptys neįrašomi į žurnalus ar saugyklą.
- **5. Versijų valdymas:** atitinka. Darbas vyksta šakoje `006-sprendimu-priemimas`, sukurtoje iš atnaujinto `main`, pakeitimai per sujungimo užklausą.
- **6. Dokumentacija:** atitinka. Specifikacija patvirtinta prieš kodą; elgsenos pakeitimai įrašomi į `docs/requirements.md`.
- **7. DI pagalba:** atitinka. Naujų bibliotekų nepridedama be pagrindimo; DI dirba tik pagal šią specifikaciją; kodą peržiūri žmogus.
- **8. Paprastumas:** atitinka. Naujos abstrakcijos ir bibliotekos nekuriamos, kol jų nereikia.

**Išimčių nėra. Planas atitinka konstituciją.**
