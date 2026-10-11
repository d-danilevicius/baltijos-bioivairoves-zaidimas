# Įgyvendinimo planas: Lygių valdymas

**Funkcija:** `specs/014-lygiu-valdymas/spec.md`  
**Šaka:** `014-lygiu-valdymas`  
**Būsena:** juodraštis  
**Priklauso nuo funkcijų:** 002, 005, 006

## Santrauka

Planas aprašo, kaip įgyvendinti funkciją „Lygių valdymas“ pagal patvirtintą specifikaciją.

## Techninis kontekstas

- **Kalba ir versija:** C# su .NET 10 (serveris); TypeScript (naršyklės dalis).
- **Bibliotekos:** Naujų bibliotekų nepridedama.
- **Naršyklės dalis:** React su Vite ir React Router.
- **Duomenų saugojimas:** PostgreSQL per Entity Framework Core; pakeitimai tik per migracijas.
- **Testavimas:** xUnit (serveris), Vitest ir React Testing Library (naršyklės dalis).
- **Tikslinė platforma:** žiniatinklio programa naujausiose Chrome, Firefox, Edge ir Safari versijose; ekranai nuo 360 px pločio.
- **Techniniai apribojimai:** tik HTTPS; puslapis atsidaro per 3 s; paslaptys nelaikomos saugykloje.

## Programos struktūra ir pagrindiniai komponentai

- Api: `AdminLevelsController`, tik administratoriaus rolei.
- Application: `LevelAdminService`, `LevelValidator` (VT-027: eilės numeris, 1+ situacija, 2–4 variantai, poveikio ribos).
- Infrastructure: `LevelRepository`.
- Naršyklės dalis: `AdminLevelsPage`, lygio forma, patvirtinimo langas.

## Serverio užklausų pakeitimai

- `GET /api/admin/levels`, `POST /api/admin/levels`, `PUT /api/admin/levels/{id}`, `DELETE /api/admin/levels/{id}` – tik administratoriui.

## Duomenų modelio pakeitimai

- `Levels` jau turi IsActive; trynimas žymi lygį kaip neaktyvų (rezultatai ir bandymai išlieka).
- Naudojamos `Situations` ir `DecisionOptions` (006).

## Testavimo planas

- xUnit: kiekviena VT-027 riba; ne administratorius atmetamas; trynimas nepašalina rezultatų; pradėtą bandymą galima užbaigti; redagavimas nekeičia išsaugotų rezultatų.
- Vitest: lygio forma.

## Techninės pastabos

Turinys keičiamas per sąsają, programos kodas nekeičiamas (NFR-020). Trynimas yra loginis, ne fizinis.

## Konstitucijos patikra

- **1. Kodo kokybė:** atitinka. Kodas tikrinamas `dotnet format`, ESLint ir Prettier; kompiliavimas be įspėjimų.
- **2. Architektūra:** atitinka. Verslo taisyklės yra tik Application sluoksnyje (aukščiau nurodytose paslaugose), Api tik priima užklausą ir tikrina rolę, Infrastructure tik skaito ir rašo duomenis, naršyklė taisyklių neskaičiuoja. Duomenų bazės pakeitimai tik per migracijas.
- **3. Testavimas:** atitinka. Kiekvienai susijusiai verslo taisyklei numatytas bent vienas automatinis testas (žr. testavimo planą).
- **4. Saugumas:** atitinka. Rolė ir nuosavybė tikrinamos serveryje, duomenys tikrinami serveryje, ryšys tik per HTTPS, slaptažodžiai ir paslaptys neįrašomi į žurnalus ar saugyklą.
- **5. Versijų valdymas:** atitinka. Darbas vyksta šakoje `014-lygiu-valdymas`, sukurtoje iš atnaujinto `main`, pakeitimai per sujungimo užklausą.
- **6. Dokumentacija:** atitinka. Specifikacija patvirtinta prieš kodą; elgsenos pakeitimai įrašomi į `docs/requirements.md`.
- **7. DI pagalba:** atitinka. Naujų bibliotekų nepridedama be pagrindimo; DI dirba tik pagal šią specifikaciją; kodą peržiūri žmogus.
- **8. Paprastumas:** atitinka. Naujos abstrakcijos ir bibliotekos nekuriamos, kol jų nereikia.

**Išimčių nėra. Planas atitinka konstituciją.**
