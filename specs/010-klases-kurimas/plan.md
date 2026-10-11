# Įgyvendinimo planas: Klasės sukūrimas ir ištrynimas

**Funkcija:** `specs/010-klases-kurimas/spec.md`  
**Šaka:** `010-klases-kurimas`  
**Būsena:** juodraštis  
**Priklauso nuo funkcijų:** 002

## Santrauka

Planas aprašo, kaip įgyvendinti funkciją „Klasės sukūrimas ir ištrynimas“ pagal patvirtintą specifikaciją.

## Techninis kontekstas

- **Kalba ir versija:** C# su .NET 10 (serveris); TypeScript (naršyklės dalis).
- **Bibliotekos:** Naujų bibliotekų nepridedama.
- **Naršyklės dalis:** React su Vite ir React Router.
- **Duomenų saugojimas:** PostgreSQL per Entity Framework Core; pakeitimai tik per migracijas.
- **Testavimas:** xUnit (serveris), Vitest ir React Testing Library (naršyklės dalis).
- **Tikslinė platforma:** žiniatinklio programa naujausiose Chrome, Firefox, Edge ir Safari versijose; ekranai nuo 360 px pločio.
- **Techniniai apribojimai:** tik HTTPS; puslapis atsidaro per 3 s; paslaptys nelaikomos saugykloje.

## Programos struktūra ir pagrindiniai komponentai

- Api: `ClassesController`, leidžiama tik mokytojo rolei.
- Application: `ClassService`, `ClassCodeGenerator` (kartoja, jei kodas jau yra).
- Infrastructure: `ClassRepository`.
- Naršyklės dalis: `TeacherClassesPage`, patvirtinimo langas ištrinant.

## Serverio užklausų pakeitimai

- `POST /api/classes` – sukuria klasę; grąžina pavadinimą ir kodą.
- `GET /api/classes` – mokytojo klasės.
- `DELETE /api/classes/{id}` – ištrina savo klasę.

## Duomenų modelio pakeitimai

- Nauja `Classes`: Id, TeacherId, Name (1–50), Code (6 simboliai, unikalus), CreatedAt.

## Testavimo planas

- xUnit: kodas unikalus ir 6 simbolių; pavadinimo ribos; mokinys negali kurti; mokytojas negali ištrinti svetimos klasės; ištrynus rezultatai neištrinami.
- Vitest: forma ir patvirtinimo langas.

## Techninės pastabos

Ištrynus klasę šalinamos tik narystės, rezultatų lentelės nekeičiamos.

## Konstitucijos patikra

- **1. Kodo kokybė:** atitinka. Kodas tikrinamas `dotnet format`, ESLint ir Prettier; kompiliavimas be įspėjimų.
- **2. Architektūra:** atitinka. Verslo taisyklės yra tik Application sluoksnyje (aukščiau nurodytose paslaugose), Api tik priima užklausą ir tikrina rolę, Infrastructure tik skaito ir rašo duomenis, naršyklė taisyklių neskaičiuoja. Duomenų bazės pakeitimai tik per migracijas.
- **3. Testavimas:** atitinka. Kiekvienai susijusiai verslo taisyklei numatytas bent vienas automatinis testas (žr. testavimo planą).
- **4. Saugumas:** atitinka. Rolė ir nuosavybė tikrinamos serveryje, duomenys tikrinami serveryje, ryšys tik per HTTPS, slaptažodžiai ir paslaptys neįrašomi į žurnalus ar saugyklą.
- **5. Versijų valdymas:** atitinka. Darbas vyksta šakoje `010-klases-kurimas`, sukurtoje iš atnaujinto `main`, pakeitimai per sujungimo užklausą.
- **6. Dokumentacija:** atitinka. Specifikacija patvirtinta prieš kodą; elgsenos pakeitimai įrašomi į `docs/requirements.md`.
- **7. DI pagalba:** atitinka. Naujų bibliotekų nepridedama be pagrindimo; DI dirba tik pagal šią specifikaciją; kodą peržiūri žmogus.
- **8. Paprastumas:** atitinka. Naujos abstrakcijos ir bibliotekos nekuriamos, kol jų nereikia.

**Išimčių nėra. Planas atitinka konstituciją.**
