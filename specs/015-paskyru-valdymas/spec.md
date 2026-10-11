# Funkcijos specifikacija: Paskyrų valdymas

**Funkcijos aplankas:** `specs/015-paskyru-valdymas`  
**Būsena:** juodraštis  
**Susiję projekto reikalavimai:** FR-021, FR-022, FR-023, FR-024, FR-025, FR-039, FR-040; VT-006–VT-008; NFR-003, NFR-006, NFR-018

**Kodėl ši funkcija reikalinga:** Administratorius gali prižiūrėti paskyras ir apriboti piktnaudžiavimą.

## Naudotojo istorijos ir priėmimo scenarijai

### 1 istorija – Administratorius valdo paskyras (prioritetas: P1)

**Naudotojo istorija:** Kaip administratorius, noriu valdyti paskyras, kad galėčiau apriboti netinkamą naudojimą.  
**Kodėl šis prioritetas:** Reikalinga kontrolei ir duomenų tvarkymui.  
**Nepriklausomas patikrinimas:** Prisijungti kaip administratorius, peržiūrėti sąrašą, užblokuoti, atblokuoti ir ištrinti paskyrą.

**Priėmimo scenarijai:**

1. **Duota** prisijungęs administratorius, **kai** jis atidaro paskyrų sąrašą, **tada** mato visas paskyras.
2. **Duota** administratorius blokuoja paskyrą, **kai** jis patvirtina veiksmą, **tada** užblokuotas naudotojas nebegali prisijungti.
3. **Duota** paskyra užblokuota, **kai** administratorius ją atblokuoja, **tada** naudotojas vėl gali prisijungti.
4. **Duota** administratorius trina paskyrą, **kai** jis patvirtina veiksmą, **tada** paskyra pašalinama kartu su jos rezultatais.
5. **Duota** administratorius, **kai** jis bando užblokuoti savo paskyrą, **tada** veiksmas atmetamas.
6. **Duota** prisijungęs mokinys ar mokytojas, **kai** jis bando atidaryti paskyrų sąrašą, **tada** prieiga atmetama.

## Ribiniai atvejai

- Administratorius bando užblokuoti ar ištrinti save.
- Lieka tik vienas administratorius.
- Administratorius bando keisti mokinio rezultatą.
- Mokinys ar mokytojas bando atidaryti administratoriaus ekraną.

## Reikalavimai

### Funkciniai reikalavimai

- **FR-001:** Sistema PRIVALO leisti administratoriui peržiūrėti paskyrų sąrašą.
- **FR-002:** Sistema PRIVALO leisti administratoriui užblokuoti ir atblokuoti paskyrą.
- **FR-003:** Sistema PRIVALO leisti administratoriui ištrinti paskyrą kartu su jos rezultatais ir visais su ja susijusiais duomenimis.
- **FR-004:** Sistema PRIVALO prieš blokuojant ar trinant paskyrą paprašyti patvirtinimo.
- **FR-005:** Sistema NEPRIVALO leisti administratoriui užblokuoti ar ištrinti savo paties paskyros, o sistemoje visada turi likti bent vienas administratorius.
- **FR-006:** Sistema NEPRIVALO leisti administratoriui keisti mokinių rezultatų.
- **FR-007:** Sistema NEPRIVALO leisti mokiniui ar mokytojui pasiekti administratoriaus funkcijų.
- **FR-008:** Sistema PRIVALO registruoti administratoriaus veiksmus (data, laikas, naudotojas, veiksmas).
- **FR-009:** Sistema PRIVALO ištrynus mokytojo paskyrą ištrinti jo klases, bet NEPRIVALO ištrinti mokinių rezultatų.
- **FR-010:** Sistema PRIVALO ištrynus mokinio paskyrą pašalinti jį iš klasės ir perskaičiuoti klasės vidurkį.

### Pagrindinės esybės

- Naudotojas.
- Veiksmų žurnalo įrašas.

## Sėkmės kriterijai

- **SC-001:** 100 % užblokuotų naudotojų nebegali prisijungti.
- **SC-002:** 100 % bandymų pasiekti administratoriaus funkcijas kitomis rolėmis atmetama.
- **SC-003:** Kiekvienas administratoriaus veiksmas turi žurnalo įrašą.

## Paaiškinimai

**Sesija 2026-10-11**

- **Klausimas:** Kas nutinka klasėms, kai ištrinama mokytojo paskyra?  
  **Atsakymas:** Jo klasės ištrinamos, o mokinių rezultatai išlieka (VT-008, VT-024).
- **Klausimas:** Kas nutinka, kai ištrinama mokinio paskyra?  
  **Atsakymas:** Mokinys pašalinamas iš klasės, jo rezultatai ištrinami (VT-008), o klasės vidurkis perskaičiuojamas (VT-025).

Neišspręstų paaiškinimo žymų nėra.

## Prielaidos

- Administratoriaus paskyrą sukuria sistemos kūrėjas.

## Priklausomybės

- 002 Prisijungimas.

## Apimties ribos

**Įeina:**
- Paskyrų sąrašas, blokavimas, atblokavimas, trynimas.

**Neįeina:**
- Naudotojų rolių keitimas.
- Rezultatų redagavimas.
