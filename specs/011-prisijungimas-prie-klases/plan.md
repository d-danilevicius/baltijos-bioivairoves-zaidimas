# Įgyvendinimo planas: Prisijungimas prie klasės kodu

**Funkcija:** `specs/011-prisijungimas-prie-klases/spec.md`  
**Šaka:** `011-prisijungimas-prie-klases`  
**Būsena:** juodraštis  
**Priklauso nuo funkcijų:** 010

## Santrauka

Planas aprašo, kaip įgyvendinti funkciją „Prisijungimas prie klasės kodu“ pagal patvirtintą specifikaciją.

## Techninis kontekstas

- **Kalba ir versija:** C# su .NET 10 (serveris); TypeScript (naršyklės dalis).
- **Bibliotekos:** Naujų bibliotekų nepridedama.
- **Naršyklės dalis:** React su Vite ir React Router.
- **Duomenų saugojimas:** PostgreSQL per Entity Framework Core; pakeitimai tik per migracijas.
- **Testavimas:** xUnit (serveris), Vitest ir React Testing Library (naršyklės dalis).
- **Tikslinė platforma:** žiniatinklio programa naujausiose Chrome, Firefox, Edge ir Safari versijose; ekranai nuo 360 px pločio.
- **Techniniai apribojimai:** tik HTTPS; puslapis atsidaro per 3 s; paslaptys nelaikomos saugykloje.

## Programos struktūra ir pagrindiniai komponentai

- Api: `ClassesController` (prisijungimas).
- Application: `ClassMembershipService` – kodo formatas, keitimas po patvirtinimo.
- Infrastructure: `MembershipRepository`.
- Naršyklės dalis: `JoinClassPage`, keitimo patvirtinimo langas.

## Serverio užklausų pakeitimai

- `POST /api/classes/join` – kodas ir požymis `confirmChange`; 404 neegzistuojančiam kodui; 409, jei mokinys jau turi klasę ir keitimas nepatvirtintas.

## Duomenų modelio pakeitimai

- Nauja `ClassMemberships`: Id, StudentId (unikalus – viena klasė vienu metu), ClassId, JoinedAt.

## Testavimo planas

- xUnit: teisingas kodas; neegzistuojantis kodas; blogas formatas; keitimas be patvirtinimo atmetamas; keitimas su patvirtinimu pakeičia klasę; mokinys be klasės gali žaisti.
- Vitest: forma ir patvirtinimas.

## Techninės pastabos

Unikalus ryšys `StudentId` užtikrina VT-022 net ir vienu metu siunčiant dvi užklausas.

## Konstitucijos patikra

- **1. Kodo kokybė:** atitinka. Kodas tikrinamas `dotnet format`, ESLint ir Prettier; kompiliavimas be įspėjimų.
- **2. Architektūra:** atitinka. Verslo taisyklės yra tik Application sluoksnyje (aukščiau nurodytose paslaugose), Api tik priima užklausą ir tikrina rolę, Infrastructure tik skaito ir rašo duomenis, naršyklė taisyklių neskaičiuoja. Duomenų bazės pakeitimai tik per migracijas.
- **3. Testavimas:** atitinka. Kiekvienai susijusiai verslo taisyklei numatytas bent vienas automatinis testas (žr. testavimo planą).
- **4. Saugumas:** atitinka. Rolė ir nuosavybė tikrinamos serveryje, duomenys tikrinami serveryje, ryšys tik per HTTPS, slaptažodžiai ir paslaptys neįrašomi į žurnalus ar saugyklą.
- **5. Versijų valdymas:** atitinka. Darbas vyksta šakoje `011-prisijungimas-prie-klases`, sukurtoje iš atnaujinto `main`, pakeitimai per sujungimo užklausą.
- **6. Dokumentacija:** atitinka. Specifikacija patvirtinta prieš kodą; elgsenos pakeitimai įrašomi į `docs/requirements.md`.
- **7. DI pagalba:** atitinka. Naujų bibliotekų nepridedama be pagrindimo; DI dirba tik pagal šią specifikaciją; kodą peržiūri žmogus.
- **8. Paprastumas:** atitinka. Naujos abstrakcijos ir bibliotekos nekuriamos, kol jų nereikia.

**Išimčių nėra. Planas atitinka konstituciją.**
