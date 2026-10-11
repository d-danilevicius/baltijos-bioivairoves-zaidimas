# Projekto konstitucija

Šis dokumentas nustato nuolatines inžinerijos taisykles visam projektui. Jame nėra atskirų funkcijų aprašų, jie yra `specs/` aplanke.

## 1. Kodo kokybė

1.1. Kodas MUST būti suformatuotas ir patikrintas automatiniais įrankiais (.NET: `dotnet format`; naršyklės dalis: ESLint ir Prettier).
1.2. Kodas MUST sukompiliuotis be įspėjimų. Įspėjimas paliekamas tik su pagrįstu paaiškinimu kode.
1.3. Kintamųjų, klasių ir funkcijų pavadinimai MUST aiškiai nusakyti paskirtį. Sutrumpinimai MUST NOT būti naudojami, jei jie nėra visuotinai žinomi.
1.4. Komentarai MUST aiškinti, kodėl kažkas daroma, o ne ką daro kodas.
1.5. Nenaudojamas kodas ir užkomentuotas senas kodas MUST NOT būti paliekami saugykloje.

## 2. Programos architektūra

2.1. Serveris MUST būti suskirstytas į sluoksnius: Api, Application, Infrastructure.
2.2. Verslo taisyklės MUST būti tik Application sluoksnyje. Jos MUST NOT būti API valdikliuose, naršyklės komponentuose ar duomenų bazės kode.
2.3. Api sluoksnis MUST tik priimti užklausą, patikrinti prisijungimą ir rolę, iškviesti Application sluoksnį ir grąžinti atsakymą.
2.4. Infrastructure sluoksnis MUST tik skaityti ir rašyti duomenis. Jame MUST NOT būti verslo sprendimų.
2.5. Naršyklės dalis MUST NOT skaičiuoti verslo rezultatų. Ji rodo serverio pateiktus duomenis.
2.6. Duomenų bazės struktūra MUST būti keičiama tik per Entity Framework Core migracijas.

## 3. Testavimas

3.1. Kiekviena verslo taisyklė (VT-xxx) MUST turėti bent vieną automatinį testą.
3.2. Klaidos taisymas MUST prasidėti nuo testo, kuris klaidą parodo.
3.3. Prieš sujungiant pakeitimą į `main` visi testai MUST praeiti.
3.4. Testai MUST NOT priklausyti nuo išorinių servisų ar nuo kitų testų vykdymo tvarkos.
3.5. Serverio testai MUST būti `tests/backend/`, naršyklės testai – `tests/frontend/`.

## 4. Saugumas

4.1. Slaptažodžiai MUST būti saugomi tik užšifruoti (ASP.NET Core Identity). Jie MUST NOT būti rašomi į žurnalus.
4.2. Rolė MUST būti tikrinama serveryje prieš kiekvieną apsaugotą veiksmą. Tikrinimas tik naršyklėje MUST NOT būti laikomas apsauga.
4.3. Visi duomenys iš naršyklės MUST būti tikrinami serveryje.
4.4. Paslaptys (slaptažodžiai, raktai, duomenų bazės prisijungimo eilutės) MUST NOT būti saugomos saugykloje. Jos laikomos aplinkos kintamuosiuose.
4.5. Duomenys tarp naršyklės ir serverio MUST būti perduodami tik per HTTPS.
4.6. Renkami MUST būti tik būtini asmens duomenys.

## 5. Versijų valdymas

5.1. Kiekviena funkcija MUST būti kuriama atskiroje šakoje, sukurtoje iš atnaujinto `main`.
5.2. Tiesiogiai į `main` MUST NOT būti rašoma. Pakeitimai patenka per sujungimo užklausą (pull request).
5.3. Commit pranešimai MUST sekti formatą `tipas(sritis): trumpas aprašas` (pvz., `feat(auth): add login`).
5.4. Vienas commit MUST apimti vieną loginį pakeitimą.
5.5. Sugeneruoti failai, `bin/`, `obj/`, `node_modules/` ir vietiniai aplinkos failai MUST NOT būti įrašomi į saugyklą.

## 6. Dokumentacija

6.1. Funkcijos kodas MUST būti rašomas tik po to, kai `specs/` aplanke yra patvirtinta tos funkcijos specifikacija.
6.2. Jei pakeitimas keičia elgseną, specifikacija ir reikalavimai (`docs/requirements.md`) MUST būti atnaujinti tame pačiame pakeitime.
6.3. README.md MUST aprašyti, kaip projektą paleisti ir išbandyti.
6.4. Architektūros sprendimų pakeitimai MUST būti užfiksuoti `docs/` aplanke.

## 7. DI pagalba programavimui

7.1. DI sugeneruotas kodas MUST būti peržiūrėtas žmogaus prieš įrašant į saugyklą.
7.2. DI MUST NOT pridėti naujos bibliotekos be pagrįsto poreikio, užfiksuoto specifikacijoje ar plane.
7.3. DI MUST dirbti tik pagal aktyvią specifikaciją. Funkcijos, kurių joje nėra, MUST NOT būti pridedamos.
7.4. DI MUST NOT keisti šios konstitucijos be aiškaus žmogaus nurodymo.
7.5. Praktinės DI darbo instrukcijos MUST būti `AGENTS.md`, o ne šioje konstitucijoje.

## 8. Paprastumas ir priežiūra

8.1. Turi būti pasirenkamas paprasčiausias sprendimas, kuris tenkina reikalavimus.
8.2. Abstrakcija (sąsaja, bendra klasė) MUST būti kuriama tik tada, kai ji naudojama bent dviejose vietose.
8.3. Biblioteka MUST būti pridedama tik konkrečiam reikalavimui įvykdyti.
8.4. Funkcija SHOULD būti ne ilgesnė nei vienas ekranas. Viršijus – reikia pagrįsti.
8.5. Kodas MUST NOT būti kopijuojamas. Pasikartojanti logika išskiriama į vieną vietą.

## 9. Valdymas

### Pakeitimai
Konstitucija keičiama tik per sujungimo užklausą, kurioje nurodoma, ką keičiama ir kodėl.

### Versijavimas
Naudojamas formatas MAJOR.MINOR.PATCH:
- MAJOR – principas pašalintas arba pakeistas priešinga prasme;
- MINOR – pridėtas naujas principas;
- PATCH – pataisytas tekstas be prasmės pakeitimo.

### Atitikties tikrinimas
- Kiekvienos funkcijos plane (`plan.md`) MUST būti patikrinta atitiktis šiai konstitucijai.
- Peržiūrint sujungimo užklausą MUST būti patikrinta, ar kodas neprieštarauja principams.
- Automatinės patikros (formatavimas, kompiliavimas, testai) MUST vykti kiekvienoje sujungimo užklausoje per `.github/`.

### Išimtys
- Išimtis leidžiama tik tada, kai taisyklės laikymasis neįmanomas arba nepagrįstai sudėtingas.
- Išimtis MUST būti užrašyta funkcijos plane: kuri taisyklė, kodėl, kiek laiko galioja.
- Išimtis MUST NOT taikytis saugumo taisyklėms (4 skyrius).

**Versija:** 1.0.0 | **Patvirtinta:** 2026-10-11 | **Paskutinis pakeitimas:** 2026-10-11
