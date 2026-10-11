# Įgyvendinimo planas: Prisijungimas ir atsijungimas

**Funkcija:** `specs/002-prisijungimas/spec.md`  
**Šaka:** `002-prisijungimas`  
**Būsena:** juodraštis  
**Priklauso nuo funkcijų:** 001

## Santrauka

Planas aprašo, kaip įgyvendinti funkciją „Prisijungimas ir atsijungimas“ pagal patvirtintą specifikaciją.

## Techninis kontekstas

- **Kalba ir versija:** C# su .NET 10 (serveris); TypeScript (naršyklės dalis).
- **Bibliotekos:** Naujų bibliotekų nepridedama.
- **Naršyklės dalis:** React su Vite ir React Router.
- **Duomenų saugojimas:** PostgreSQL per Entity Framework Core; pakeitimai tik per migracijas.
- **Testavimas:** xUnit (serveris), Vitest ir React Testing Library (naršyklės dalis).
- **Tikslinė platforma:** žiniatinklio programa naujausiose Chrome, Firefox, Edge ir Safari versijose; ekranai nuo 360 px pločio.
- **Techniniai apribojimai:** tik HTTPS; puslapis atsidaro per 3 s; paslaptys nelaikomos saugykloje.

## Programos struktūra ir pagrindiniai komponentai

- Api: `AuthController` (prisijungimas / atsijungimas), apsaugos taisyklė visiems apsaugotiems maršrutams.
- Application: `LoginService` (blokavimo patikra, žurnalo įrašas), `ActionLogWriter`.
- Infrastructure: `ActionLogRepository`.
- Naršyklės dalis: `LoginPage`, maršrutų apsauga neprisijungusiems.

## Serverio užklausų pakeitimai

- `POST /api/auth/login` – el. paštas ir slaptažodis; 200 arba 401 (bendras pranešimas) arba 403 (užblokuota).
- `POST /api/auth/logout` – baigia sesiją.

## Duomenų modelio pakeitimai

- `Users` papildoma: IsBlocked (bool, numatyta false).
- Nauja lentelė `ActionLog`: Id, Time, UserId (gali būti tuščias), Action, Result.

## Testavimo planas

- xUnit: teisingi duomenys; neteisingi duomenys (bendras pranešimas); užblokuota paskyra; slaptažodis nepatenka į žurnalą.
- Vitest: nukreipimas neprisijungusiam į prisijungimo ekraną.

## Techninės pastabos

Sesija laikoma slapuke (tik HTTPS, nepasiekiama skriptams), nustatomas 30 min neveiklumo galiojimas. Kiekviena keičianti užklausa saugoma nuo užklausų klastojimo (CSRF) apsaugos raktu. Nepavykusių bandymų ribojimas neįeina (žr. specifikaciją).

## Konstitucijos patikra

- **1. Kodo kokybė:** atitinka. Kodas tikrinamas `dotnet format`, ESLint ir Prettier; kompiliavimas be įspėjimų.
- **2. Architektūra:** atitinka. Verslo taisyklės yra tik Application sluoksnyje (aukščiau nurodytose paslaugose), Api tik priima užklausą ir tikrina rolę, Infrastructure tik skaito ir rašo duomenis, naršyklė taisyklių neskaičiuoja. Duomenų bazės pakeitimai tik per migracijas.
- **3. Testavimas:** atitinka. Kiekvienai susijusiai verslo taisyklei numatytas bent vienas automatinis testas (žr. testavimo planą).
- **4. Saugumas:** atitinka. Rolė ir nuosavybė tikrinamos serveryje, duomenys tikrinami serveryje, ryšys tik per HTTPS, slaptažodžiai ir paslaptys neįrašomi į žurnalus ar saugyklą.
- **5. Versijų valdymas:** atitinka. Darbas vyksta šakoje `002-prisijungimas`, sukurtoje iš atnaujinto `main`, pakeitimai per sujungimo užklausą.
- **6. Dokumentacija:** atitinka. Specifikacija patvirtinta prieš kodą; elgsenos pakeitimai įrašomi į `docs/requirements.md`.
- **7. DI pagalba:** atitinka. Naujų bibliotekų nepridedama be pagrindimo; DI dirba tik pagal šią specifikaciją; kodą peržiūri žmogus.
- **8. Paprastumas:** atitinka. Naujos abstrakcijos ir bibliotekos nekuriamos, kol jų nereikia.

**Išimčių nėra. Planas atitinka konstituciją.**
