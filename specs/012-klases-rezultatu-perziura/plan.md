# Įgyvendinimo planas: Klasės mokinių rezultatų peržiūra

**Funkcija:** `specs/012-klases-rezultatu-perziura/spec.md`  
**Šaka:** `012-klases-rezultatu-perziura`  
**Būsena:** juodraštis  
**Priklauso nuo funkcijų:** 008, 011

## Santrauka

Planas aprašo, kaip įgyvendinti funkciją „Klasės mokinių rezultatų peržiūra“ pagal patvirtintą specifikaciją.

## Techninis kontekstas

- **Kalba ir versija:** C# su .NET 10 (serveris); TypeScript (naršyklės dalis).
- **Bibliotekos:** Naujų bibliotekų nepridedama.
- **Naršyklės dalis:** React su Vite ir React Router.
- **Duomenų saugojimas:** PostgreSQL per Entity Framework Core; pakeitimai tik per migracijas.
- **Testavimas:** xUnit (serveris), Vitest ir React Testing Library (naršyklės dalis).
- **Tikslinė platforma:** žiniatinklio programa naujausiose Chrome, Firefox, Edge ir Safari versijose; ekranai nuo 360 px pločio.
- **Techniniai apribojimai:** tik HTTPS; puslapis atsidaro per 3 s; paslaptys nelaikomos saugykloje.

## Programos struktūra ir pagrindiniai komponentai

- Api: `ClassResultsController`, tik mokytojo rolei.
- Application: `ClassAccessService` (klasė priklauso prisijungusiam mokytojui), `ClassStatsService` (vidurkis iš dabartinių narių geriausių rezultatų, VT-025).
- Naršyklės dalis: `ClassResultsPage`, klasės pasirinkimas.

## Serverio užklausų pakeitimai

- `GET /api/classes/{id}/students` – narių sąrašas.
- `GET /api/classes/{id}/students/{studentId}/results` – mokinio rezultatai.
- `GET /api/classes/{id}/summary` – lygių vidurkiai, išdėstyti nuo mažiausio.

## Duomenų modelio pakeitimai

- Duomenų modelio pakeitimų nėra (skaitoma `ClassMemberships`, `LevelResults`).

## Testavimo planas

- xUnit: vidurkis; vidurkis nerodomas, kai niekas nebaigė; svetima klasė atmetama; pašalinto nario rezultatų nėra; lygių eilės tvarka.
- Vitest: suvestinės ir tuščios būsenos rodymas.

## Techninės pastabos

Mokytojas rezultatų negali keisti: rezultatų rašymo kelių šiai rolei nėra.

## Konstitucijos patikra

- **1. Kodo kokybė:** atitinka. Kodas tikrinamas `dotnet format`, ESLint ir Prettier; kompiliavimas be įspėjimų.
- **2. Architektūra:** atitinka. Verslo taisyklės yra tik Application sluoksnyje (aukščiau nurodytose paslaugose), Api tik priima užklausą ir tikrina rolę, Infrastructure tik skaito ir rašo duomenis, naršyklė taisyklių neskaičiuoja. Duomenų bazės pakeitimai tik per migracijas.
- **3. Testavimas:** atitinka. Kiekvienai susijusiai verslo taisyklei numatytas bent vienas automatinis testas (žr. testavimo planą).
- **4. Saugumas:** atitinka. Rolė ir nuosavybė tikrinamos serveryje, duomenys tikrinami serveryje, ryšys tik per HTTPS, slaptažodžiai ir paslaptys neįrašomi į žurnalus ar saugyklą.
- **5. Versijų valdymas:** atitinka. Darbas vyksta šakoje `012-klases-rezultatu-perziura`, sukurtoje iš atnaujinto `main`, pakeitimai per sujungimo užklausą.
- **6. Dokumentacija:** atitinka. Specifikacija patvirtinta prieš kodą; elgsenos pakeitimai įrašomi į `docs/requirements.md`.
- **7. DI pagalba:** atitinka. Naujų bibliotekų nepridedama be pagrindimo; DI dirba tik pagal šią specifikaciją; kodą peržiūri žmogus.
- **8. Paprastumas:** atitinka. Naujos abstrakcijos ir bibliotekos nekuriamos, kol jų nereikia.

**Išimčių nėra. Planas atitinka konstituciją.**
