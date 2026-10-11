# Įgyvendinimo planas: Registracija

**Funkcija:** `specs/001-registracija/spec.md`  
**Šaka:** `001-registracija`  
**Būsena:** juodraštis  
**Priklauso nuo funkcijų:** Jokių.

## Santrauka

Planas aprašo, kaip įgyvendinti funkciją „Registracija“ pagal patvirtintą specifikaciją.

## Techninis kontekstas

- **Kalba ir versija:** C# su .NET 10 (serveris); TypeScript (naršyklės dalis).
- **Bibliotekos:** ASP.NET Core Identity (slaptažodžių saugojimas ir rolės, NFR-001, NFR-003).
- **Naršyklės dalis:** React su Vite ir React Router.
- **Duomenų saugojimas:** PostgreSQL per Entity Framework Core; pakeitimai tik per migracijas.
- **Testavimas:** xUnit (serveris), Vitest ir React Testing Library (naršyklės dalis).
- **Tikslinė platforma:** žiniatinklio programa naujausiose Chrome, Firefox, Edge ir Safari versijose; ekranai nuo 360 px pločio.
- **Techniniai apribojimai:** tik HTTPS; puslapis atsidaro per 3 s; paslaptys nelaikomos saugykloje.

## Programos struktūra ir pagrindiniai komponentai

- Api: `AuthController` – tik priima užklausą ir grąžina atsakymą.
- Application: `RegistrationService` – tikrina rolę (administratoriaus rolė draudžiama), naudotojo vardo formatą (3–30), unikalumą.
- Infrastructure: Identity saugykla, `UserRepository`.
- Naršyklės dalis: `RegisterPage` su forma ir klaidų rodymu.

## Serverio užklausų pakeitimai

- `POST /api/auth/register` – priima el. paštą, naudotojo vardą, slaptažodį, rolę (mokinys arba mokytojas); grąžina 201 arba 400 su klaidų sąrašu.

## Duomenų modelio pakeitimai

- Lentelė `Users` (per ASP.NET Core Identity): Id, Email (unikalus), UserName (unikalus), PasswordHash, Role (mokinys / mokytojas / administratorius).

## Testavimo planas

- xUnit: pasikartojantis el. paštas; pasikartojantis vardas; slaptažodis < 8; vardas už ribų; bandymas registruotis administratoriumi; sėkminga registracija.
- Vitest: formos klaidų pranešimai.

## Techninės pastabos

Slaptažodžio taisyklės (bent 8 simboliai) nustatomos Identity nustatymuose. Pirmoji funkcija, todėl čia įdiegiamas ASP.NET Core Identity.

## Konstitucijos patikra

- **1. Kodo kokybė:** atitinka. Kodas tikrinamas `dotnet format`, ESLint ir Prettier; kompiliavimas be įspėjimų.
- **2. Architektūra:** atitinka. Verslo taisyklės yra tik Application sluoksnyje (aukščiau nurodytose paslaugose), Api tik priima užklausą ir tikrina rolę, Infrastructure tik skaito ir rašo duomenis, naršyklė taisyklių neskaičiuoja. Duomenų bazės pakeitimai tik per migracijas.
- **3. Testavimas:** atitinka. Kiekvienai susijusiai verslo taisyklei numatytas bent vienas automatinis testas (žr. testavimo planą).
- **4. Saugumas:** atitinka. Rolė ir nuosavybė tikrinamos serveryje, duomenys tikrinami serveryje, ryšys tik per HTTPS, slaptažodžiai ir paslaptys neįrašomi į žurnalus ar saugyklą.
- **5. Versijų valdymas:** atitinka. Darbas vyksta šakoje `001-registracija`, sukurtoje iš atnaujinto `main`, pakeitimai per sujungimo užklausą.
- **6. Dokumentacija:** atitinka. Specifikacija patvirtinta prieš kodą; elgsenos pakeitimai įrašomi į `docs/requirements.md`.
- **7. DI pagalba:** atitinka. Naujų bibliotekų nepridedama be pagrindimo; DI dirba tik pagal šią specifikaciją; kodą peržiūri žmogus. Išimtis: ASP.NET Core Identity pagrįsta NFR-001 ir NFR-003.
- **8. Paprastumas:** atitinka. Naujos abstrakcijos ir bibliotekos nekuriamos, kol jų nereikia.

**Išimčių nėra. Planas atitinka konstituciją.**
