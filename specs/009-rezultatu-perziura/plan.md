# Įgyvendinimo planas: Savo rezultatų peržiūra

**Funkcija:** `specs/009-rezultatu-perziura/spec.md`  
**Šaka:** `009-rezultatu-perziura`  
**Būsena:** juodraštis  
**Priklauso nuo funkcijų:** 008

## Santrauka

Planas aprašo, kaip įgyvendinti funkciją „Savo rezultatų peržiūra“ pagal patvirtintą specifikaciją.

## Techninis kontekstas

- **Kalba ir versija:** C# su .NET 10 (serveris); TypeScript (naršyklės dalis).
- **Bibliotekos:** Naujų bibliotekų nepridedama.
- **Naršyklės dalis:** React su Vite ir React Router.
- **Duomenų saugojimas:** PostgreSQL per Entity Framework Core; pakeitimai tik per migracijas.
- **Testavimas:** xUnit (serveris), Vitest ir React Testing Library (naršyklės dalis).
- **Tikslinė platforma:** žiniatinklio programa naujausiose Chrome, Firefox, Edge ir Safari versijose; ekranai nuo 360 px pločio.
- **Techniniai apribojimai:** tik HTTPS; puslapis atsidaro per 3 s; paslaptys nelaikomos saugykloje.

## Programos struktūra ir pagrindiniai komponentai

- Api: `ResultsController`.
- Application: `ResultQueryService` – visada filtruoja pagal prisijungusį naudotoją.
- Naršyklės dalis: `MyResultsPage` su tuščia būsena.

## Serverio užklausų pakeitimai

- `GET /api/results` – prisijungusio mokinio rezultatai pagal lygius.

## Duomenų modelio pakeitimai

- Duomenų modelio pakeitimų nėra (skaitoma `LevelResults` ir `Levels`).

## Testavimo planas

- xUnit: grąžinami tik savo rezultatai; ištrinto lygio rezultatas išlieka.
- Vitest: tuščios būsenos pranešimas.

## Techninės pastabos

Užklausoje nėra mokinio identifikatoriaus, todėl kito mokinio rezultatų gauti neįmanoma.

## Konstitucijos patikra

- **1. Kodo kokybė:** atitinka. Kodas tikrinamas `dotnet format`, ESLint ir Prettier; kompiliavimas be įspėjimų.
- **2. Architektūra:** atitinka. Verslo taisyklės yra tik Application sluoksnyje (aukščiau nurodytose paslaugose), Api tik priima užklausą ir tikrina rolę, Infrastructure tik skaito ir rašo duomenis, naršyklė taisyklių neskaičiuoja. Duomenų bazės pakeitimai tik per migracijas.
- **3. Testavimas:** atitinka. Kiekvienai susijusiai verslo taisyklei numatytas bent vienas automatinis testas (žr. testavimo planą).
- **4. Saugumas:** atitinka. Rolė ir nuosavybė tikrinamos serveryje, duomenys tikrinami serveryje, ryšys tik per HTTPS, slaptažodžiai ir paslaptys neįrašomi į žurnalus ar saugyklą.
- **5. Versijų valdymas:** atitinka. Darbas vyksta šakoje `009-rezultatu-perziura`, sukurtoje iš atnaujinto `main`, pakeitimai per sujungimo užklausą.
- **6. Dokumentacija:** atitinka. Specifikacija patvirtinta prieš kodą; elgsenos pakeitimai įrašomi į `docs/requirements.md`.
- **7. DI pagalba:** atitinka. Naujų bibliotekų nepridedama be pagrindimo; DI dirba tik pagal šią specifikaciją; kodą peržiūri žmogus.
- **8. Paprastumas:** atitinka. Naujos abstrakcijos ir bibliotekos nekuriamos, kol jų nereikia.

**Išimčių nėra. Planas atitinka konstituciją.**
