# AGENTS.md

Praktinės instrukcijos DI pagalbininkui, dirbančiam šioje saugykloje. Principus žr. konstitucijoje.

## Privalomos nuorodos
- Projekto konstitucija: `.specify/memory/constitution.md`. Laikykis jos visuose pakeitimuose.
- Reikalavimai: `docs/requirements.md`.
- Ekranai ir naudotojo eigos: `docs/ux.md`.

## Saugyklos žemėlapis
- `backend/Api/` – užklausų priėmimas, prisijungimas, rolių patikra.
- `backend/Application/` – programos logika ir verslo taisyklės.
- `backend/Infrastructure/` – duomenų prieiga ir migracijos.
- `frontend/` – naršyklės dalis (React).
- `tests/backend/`, `tests/frontend/` – automatiniai testai.
- `docs/` – dokumentai.
- `specs/` – funkcijų specifikacijos (po vieną aplanką kiekvienai funkcijai).
- `.specify/` – Spec Kit failai ir konstitucija.
- `.github/` – automatinės patikros.

## Dažnos komandos
Serveris:
- `dotnet restore`
- `dotnet build`
- `dotnet test`
- `dotnet format`

Naršyklės dalis (aplanke `frontend/`):
- `npm install`
- `npm run dev`
- `npm run lint`
- `npm test`
- `npm run build`

## Funkcijos darbo eiga
1. Pradėk nuo atnaujinto `main`.
2. Sukurk funkcijos šaką.
3. Naudok Spec Kit eigą: specifikacija → planas → užduotys → įgyvendinimas.
4. Specifikaciją, planą ir užduotis laikyk `specs/<funkcijos-aplankas>/`.
5. Dirbk tik pagal aktyvią specifikaciją. Nieko neįtrauk nuo savęs.
6. Naujas verslo taisykles dėk į `backend/Application/`.

## Kaip patikrinti pakeitimus
Prieš siūlydamas commit:
1. Paleisk `dotnet build` ir `dotnet test`.
2. Paleisk `npm run lint`, `npm test`, `npm run build`, jei keitėsi `frontend/`.
3. Peržiūrėk `git diff` ir įsitikink, kad pakeitimai atitinka specifikaciją.
4. Jei pakeitėsi elgsena, atnaujink specifikaciją ir `docs/requirements.md`.

## Ko negalima keisti ar įrašyti
- Negalima įrašyti slaptažodžių, raktų ar `.env` failų.
- Negalima redaguoti `bin/`, `obj/`, `node_modules/`, `dist/`.
- Negalima keisti jau pritaikytų migracijų. Daryk naują migraciją.
- Negalima keisti `.specify/memory/constitution.md` be aiškaus nurodymo.
- Negalima rašyti tiesiai į `main`.

## Saugyklos taisyklės
- Visi tekstai naudotojui ir dokumentai rašomi lietuvių kalba.
- Commit pranešimai: `tipas(sritis): trumpas aprašas`.
- Reikalavimus žymėk numeriais: FR-xxx, NFR-xxx, VT-xxx. Nenaudok naujų, kol jie nepridėti į `docs/requirements.md`.
