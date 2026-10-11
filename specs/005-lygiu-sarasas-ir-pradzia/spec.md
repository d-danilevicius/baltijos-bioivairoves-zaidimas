# Funkcijos specifikacija: Lygių sąrašas ir lygio pradžia

**Funkcijos aplankas:** `specs/005-lygiu-sarasas-ir-pradzia`  
**Būsena:** juodraštis  
**Susiję projekto reikalavimai:** FR-008, FR-009, FR-038, FR-040; VT-009, VT-013; NFR-008

**Kodėl ši funkcija reikalinga:** Mokinys turi matyti, kokie lygiai yra, ir galėti pradėti tą, kuris jam atrakintas.

## Naudotojo istorijos ir priėmimo scenarijai

### 1 istorija – Mokinys pasirenka ir pradeda lygį (prioritetas: P1)

**Naudotojo istorija:** Kaip mokinys, noriu matyti lygių sąrašą ir pradėti atrakintą lygį, kad galėčiau pradėti žaisti.  
**Kodėl šis prioritetas:** Tai žaidimo pradžios taškas.  
**Nepriklausomas patikrinimas:** Prisijungti kaip naujas mokinys, atidaryti lygių sąrašą ir pradėti pirmą lygį.

**Priėmimo scenarijai:**

1. **Duota** prisijungęs mokinys, **kai** jis atidaro lygių sąrašą, **tada** jis mato bent 3 lygius, o pirmas lygis yra atrakintas.
2. **Duota** mokinys pasirenka atrakintą lygį, **kai** jis atidaro lygio aprašymą, **tada** mato lygio pavadinimą, aprašymą, pradinę ekosistemos būklę ir situacijų skaičių.
3. **Duota** mokinys nebaigė ankstesnio lygio, **kai** jis bando pradėti kitą lygį, **tada** lygis neatidaromas ir paaiškinama, kad jis dar užrakintas.

## Ribiniai atvejai

- Mokinys pradeda lygį, bet išeina nebaigęs (taikomas patvirtinimas).
- Lygis ištrintas administratoriaus.

## Reikalavimai

### Funkciniai reikalavimai

- **FR-001:** Sistema PRIVALO rodyti mokiniui sąrašą su ne mažiau kaip 3 žaidimo lygiais.
- **FR-002:** Sistema PRIVALO leisti mokiniui pradėti pasirinktą lygį, jei jis atrakintas.
- **FR-003:** Sistema PRIVALO prieš pradedant lygį parodyti pavadinimą, aprašymą, pradinę ekosistemos būklę ir situacijų skaičių.
- **FR-004:** Sistema PRIVALO pirmą lygį atrakinti kiekvienam mokiniui nuo registracijos.
- **FR-005:** Sistema PRIVALO kiekvieną lygį pradėti nuo to lygio aprašyme nustatytos pradinės būklės.
- **FR-006:** Sistema PRIVALO paprašyti patvirtinimo, kai mokinys išeina iš nebaigto lygio.
- **FR-007:** Sistema PRIVALO leisti mokiniui pakartoti baigtą lygį.

### Pagrindinės esybės

- Lygis.
- Lygio bandymas.

## Sėkmės kriterijai

- **SC-001:** Naujas mokinys pirmą lygį pradeda per ne daugiau kaip 3 paspaudimus po prisijungimo.
- **SC-002:** Lygių sąrašas atsidaro per ne daugiau kaip 3 sekundes.

## Paaiškinimai

**Sesija 2026-10-11**

- **Klausimas:** Ar galima kartoti jau baigtą lygį?  
  **Atsakymas:** Taip. Pakartojus lygį, lygio rezultatu lieka geriausias pasiektas rezultatas (VT-016).

Neišspręstų paaiškinimo žymų nėra.

## Prielaidos

- Bent 3 lygių turinį parengia projekto autorius.

## Priklausomybės

- 002 Prisijungimas.
- 014 Lygių valdymas (lygių turinys).

## Apimties ribos

**Įeina:**
- Lygių sąrašas, lygio aprašymas, lygio pradžia.

**Neįeina:**
- Situacijų sprendimas (006).
