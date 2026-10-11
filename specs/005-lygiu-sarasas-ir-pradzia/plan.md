# Įgyvendinimo planas: Lygių sąrašas ir lygio pradžia

**Funkcija:** `specs/005-lygiu-sarasas-ir-pradzia/spec.md`  
**Šaka:** `005-lygiu-sarasas-ir-pradzia`  
**Būsena:** juodraštis  
**Priklauso nuo funkcijų:** 002

## Santrauka

Planas aprašo, kaip įgyvendinti funkciją „Lygių sąrašas ir lygio pradžia“ pagal patvirtintą specifikaciją.

## Techninis kontekstas

- **Kalba ir versija:** C# su .NET 10 (serveris); TypeScript (naršyklės dalis).
- **Bibliotekos:** Naujų bibliotekų nepridedama.
- **Naršyklės dalis:** React su Vite ir React Router.
- **Duomenų saugojimas:** PostgreSQL per Entity Framework Core; pakeitimai tik per migracijas.
- **Testavimas:** xUnit (serveris), Vitest ir React Testing Library (naršyklės dalis).
- **Tikslinė platforma:** žiniatinklio programa naujausiose Chrome, Firefox, Edge ir Safari versijose; ekranai nuo 360 px pločio.
- **Techniniai apribojimai:** tik HTTPS; puslapis atsidaro per 3 s; paslaptys nelaikomos saugykloje.

## Programos struktūra ir pagrindiniai komponentai

- Api: `LevelsController`.
- Application: `LevelAccessService` (atrakinimo taisyklė VT-009), `AttemptService.Start` (pradinė būklė VT-013).
- Infrastructure: `LevelRepository`, `AttemptRepository`.
- Naršyklės dalis: `LevelListPage`, `LevelIntroPage`, patvirtinimo langas išeinant iš nebaigto lygio.

## Serverio užklausų pakeitimai

- `GET /api/levels` – lygių sąrašas su požymiu „atrakintas“.
- `GET /api/levels/{id}` – pavadinimas, aprašymas, pradinė būklė, situacijų skaičius.
- `POST /api/levels/{id}/attempts` – pradeda bandymą; 403, jei lygis užrakintas.

## Duomenų modelio pakeitimai

- Nauja `Levels`: Id, OrderNo (unikalus), Title, Description, StartState, StartSpeciesCount, IsActive.
- Nauja `LevelAttempts`: Id, StudentId, LevelId, StartedAt, Status (vyksta / baigtas), CurrentState, SpeciesCount.

## Testavimo planas

- xUnit: pirmas lygis atrakintas; antras užrakintas; pradžios būklė iš lygio aprašymo; užrakinto lygio pradžia atmetama.
- Vitest: lygių sąrašo rodymas, patvirtinimo langas.

## Techninės pastabos

Atrakinimas skaičiuojamas pagal baigtus bandymus. Kol 014 funkcija nesukurta, 3 pradiniai lygiai įrašomi pradiniais duomenimis (migracijoje).

## Konstitucijos patikra

- **1. Kodo kokybė:** atitinka. Kodas tikrinamas `dotnet format`, ESLint ir Prettier; kompiliavimas be įspėjimų.
- **2. Architektūra:** atitinka. Verslo taisyklės yra tik Application sluoksnyje (aukščiau nurodytose paslaugose), Api tik priima užklausą ir tikrina rolę, Infrastructure tik skaito ir rašo duomenis, naršyklė taisyklių neskaičiuoja. Duomenų bazės pakeitimai tik per migracijas.
- **3. Testavimas:** atitinka. Kiekvienai susijusiai verslo taisyklei numatytas bent vienas automatinis testas (žr. testavimo planą).
- **4. Saugumas:** atitinka. Rolė ir nuosavybė tikrinamos serveryje, duomenys tikrinami serveryje, ryšys tik per HTTPS, slaptažodžiai ir paslaptys neįrašomi į žurnalus ar saugyklą.
- **5. Versijų valdymas:** atitinka. Darbas vyksta šakoje `005-lygiu-sarasas-ir-pradzia`, sukurtoje iš atnaujinto `main`, pakeitimai per sujungimo užklausą.
- **6. Dokumentacija:** atitinka. Specifikacija patvirtinta prieš kodą; elgsenos pakeitimai įrašomi į `docs/requirements.md`.
- **7. DI pagalba:** atitinka. Naujų bibliotekų nepridedama be pagrindimo; DI dirba tik pagal šią specifikaciją; kodą peržiūri žmogus.
- **8. Paprastumas:** atitinka. Naujos abstrakcijos ir bibliotekos nekuriamos, kol jų nereikia.

**Išimčių nėra. Planas atitinka konstituciją.**
