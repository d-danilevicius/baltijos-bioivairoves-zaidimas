# Įgyvendinimo planas: Naudotojo profilis

**Funkcija:** `specs/004-profilis/spec.md`  
**Šaka:** `004-profilis`  
**Būsena:** juodraštis  
**Priklauso nuo funkcijų:** 002 (klasės pavadinimas rodomas po 011)

## Santrauka

Planas aprašo, kaip įgyvendinti funkciją „Naudotojo profilis“ pagal patvirtintą specifikaciją.

## Techninis kontekstas

- **Kalba ir versija:** C# su .NET 10 (serveris); TypeScript (naršyklės dalis).
- **Bibliotekos:** Naujų bibliotekų nepridedama.
- **Naršyklės dalis:** React su Vite ir React Router.
- **Duomenų saugojimas:** PostgreSQL per Entity Framework Core; pakeitimai tik per migracijas.
- **Testavimas:** xUnit (serveris), Vitest ir React Testing Library (naršyklės dalis).
- **Tikslinė platforma:** žiniatinklio programa naujausiose Chrome, Firefox, Edge ir Safari versijose; ekranai nuo 360 px pločio.
- **Techniniai apribojimai:** tik HTTPS; puslapis atsidaro per 3 s; paslaptys nelaikomos saugykloje.

## Programos struktūra ir pagrindiniai komponentai

- Api: `ProfileController`.
- Application: `ProfileService`.
- Infrastructure: skaitymo užklausa.
- Naršyklės dalis: `ProfilePage`.

## Serverio užklausų pakeitimai

- `GET /api/profile` – grąžina naudotojo vardą, el. paštą, rolę ir (mokiniui) klasės pavadinimą. Visada grąžina tik prisijungusio naudotojo duomenis.

## Duomenų modelio pakeitimai

- Duomenų modelio pakeitimų nėra (skaitoma `Users`; klasė – iš `ClassMemberships`, kai ji atsiras).

## Testavimo planas

- xUnit: grąžinami tik prisijungusio naudotojo duomenys; mokinys be klasės.
- Vitest: profilio rodymas.

## Techninės pastabos

Kito naudotojo profilio užklausos kelio nėra, todėl atidaryti svetimo profilio neįmanoma.

## Konstitucijos patikra

- **1. Kodo kokybė:** atitinka. Kodas tikrinamas `dotnet format`, ESLint ir Prettier; kompiliavimas be įspėjimų.
- **2. Architektūra:** atitinka. Verslo taisyklės yra tik Application sluoksnyje (aukščiau nurodytose paslaugose), Api tik priima užklausą ir tikrina rolę, Infrastructure tik skaito ir rašo duomenis, naršyklė taisyklių neskaičiuoja. Duomenų bazės pakeitimai tik per migracijas.
- **3. Testavimas:** atitinka. Kiekvienai susijusiai verslo taisyklei numatytas bent vienas automatinis testas (žr. testavimo planą).
- **4. Saugumas:** atitinka. Rolė ir nuosavybė tikrinamos serveryje, duomenys tikrinami serveryje, ryšys tik per HTTPS, slaptažodžiai ir paslaptys neįrašomi į žurnalus ar saugyklą.
- **5. Versijų valdymas:** atitinka. Darbas vyksta šakoje `004-profilis`, sukurtoje iš atnaujinto `main`, pakeitimai per sujungimo užklausą.
- **6. Dokumentacija:** atitinka. Specifikacija patvirtinta prieš kodą; elgsenos pakeitimai įrašomi į `docs/requirements.md`.
- **7. DI pagalba:** atitinka. Naujų bibliotekų nepridedama be pagrindimo; DI dirba tik pagal šią specifikaciją; kodą peržiūri žmogus.
- **8. Paprastumas:** atitinka. Naujos abstrakcijos ir bibliotekos nekuriamos, kol jų nereikia.

**Išimčių nėra. Planas atitinka konstituciją.**
