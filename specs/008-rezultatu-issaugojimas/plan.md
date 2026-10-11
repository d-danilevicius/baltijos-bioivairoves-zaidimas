# Įgyvendinimo planas: Rezultatų išsaugojimas ir žaidimo tęsimas

**Funkcija:** `specs/008-rezultatu-issaugojimas/spec.md`  
**Šaka:** `008-rezultatu-issaugojimas`  
**Būsena:** juodraštis  
**Priklauso nuo funkcijų:** 007

## Santrauka

Planas aprašo, kaip įgyvendinti funkciją „Rezultatų išsaugojimas ir žaidimo tęsimas“ pagal patvirtintą specifikaciją.

## Techninis kontekstas

- **Kalba ir versija:** C# su .NET 10 (serveris); TypeScript (naršyklės dalis).
- **Bibliotekos:** Naujų bibliotekų nepridedama.
- **Naršyklės dalis:** React su Vite ir React Router.
- **Duomenų saugojimas:** PostgreSQL per Entity Framework Core; pakeitimai tik per migracijas.
- **Testavimas:** xUnit (serveris), Vitest ir React Testing Library (naršyklės dalis).
- **Tikslinė platforma:** žiniatinklio programa naujausiose Chrome, Firefox, Edge ir Safari versijose; ekranai nuo 360 px pločio.
- **Techniniai apribojimai:** tik HTTPS; puslapis atsidaro per 3 s; paslaptys nelaikomos saugykloje.

## Programos struktūra ir pagrindiniai komponentai

- Application: `ResultService.Save` – bandymo užbaigimas ir rezultato išsaugojimas vienoje duomenų bazės operacijoje (NFR-015); saugomas geriausias rezultatas (VT-016).
- Infrastructure: `ResultRepository`.
- Naršyklės dalis: „Tęsti“ mygtukas lygių sąraše.

## Serverio užklausų pakeitimai

- `GET /api/attempts/current` – grąžina paskutinį nebaigtą lygį tęsimui.

## Duomenų modelio pakeitimai

- Nauja `LevelResults`: Id, StudentId, LevelId, BestScore, UpdatedAt; unikalumas (StudentId, LevelId).

## Testavimo planas

- xUnit: išsaugomas rezultatas; prastesnis rezultatas nepakeičia geriausio; geresnis pakeičia; nebaigtas lygis neišsaugomas; tęsimas po naujo prisijungimo.
- Vitest: „Tęsti“ mygtukas.

## Techninės pastabos

Nebaigto lygio tarpinė būsena nesaugoma, tęsiant lygis pradedamas iš naujo. Kito lygio atrakinimas lieka pagal baigtus bandymus (007); `LevelResults` naudojama tik geriausiam rezultatui.

## Konstitucijos patikra

- **1. Kodo kokybė:** atitinka. Kodas tikrinamas `dotnet format`, ESLint ir Prettier; kompiliavimas be įspėjimų.
- **2. Architektūra:** atitinka. Verslo taisyklės yra tik Application sluoksnyje (aukščiau nurodytose paslaugose), Api tik priima užklausą ir tikrina rolę, Infrastructure tik skaito ir rašo duomenis, naršyklė taisyklių neskaičiuoja. Duomenų bazės pakeitimai tik per migracijas.
- **3. Testavimas:** atitinka. Kiekvienai susijusiai verslo taisyklei numatytas bent vienas automatinis testas (žr. testavimo planą).
- **4. Saugumas:** atitinka. Rolė ir nuosavybė tikrinamos serveryje, duomenys tikrinami serveryje, ryšys tik per HTTPS, slaptažodžiai ir paslaptys neįrašomi į žurnalus ar saugyklą.
- **5. Versijų valdymas:** atitinka. Darbas vyksta šakoje `008-rezultatu-issaugojimas`, sukurtoje iš atnaujinto `main`, pakeitimai per sujungimo užklausą.
- **6. Dokumentacija:** atitinka. Specifikacija patvirtinta prieš kodą; elgsenos pakeitimai įrašomi į `docs/requirements.md`.
- **7. DI pagalba:** atitinka. Naujų bibliotekų nepridedama be pagrindimo; DI dirba tik pagal šią specifikaciją; kodą peržiūri žmogus.
- **8. Paprastumas:** atitinka. Naujos abstrakcijos ir bibliotekos nekuriamos, kol jų nereikia.

**Išimčių nėra. Planas atitinka konstituciją.**
