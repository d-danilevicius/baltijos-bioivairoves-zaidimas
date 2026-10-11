# Funkcijos specifikacija: Prisijungimas prie klasės kodu

**Funkcijos aplankas:** `specs/011-prisijungimas-prie-klases`  
**Būsena:** juodraštis  
**Susiję projekto reikalavimai:** FR-028, FR-029, FR-040; VT-022, VT-023, VT-026

**Kodėl ši funkcija reikalinga:** Mokinys prisijungia prie savo mokytojo klasės, kad mokytojas matytų jo rezultatus.

## Naudotojo istorijos ir priėmimo scenarijai

### 1 istorija – Mokinys prisijungia prie klasės (prioritetas: P1)

**Naudotojo istorija:** Kaip mokinys, noriu prisijungti prie klasės kodu, kad mokytojas matytų mano rezultatus.  
**Kodėl šis prioritetas:** Tai būdas susieti mokinį su mokytoju.  
**Nepriklausomas patikrinimas:** Prisijungti kaip mokinys ir įvesti teisingą bei neteisingą kodą.

**Priėmimo scenarijai:**

1. **Duota** prisijungęs mokinys, nepriklausantis klasei, **kai** jis įveda galiojantį klasės kodą, **tada** jis tampa tos klasės nariu.
2. **Duota** mokinys įveda neegzistuojantį kodą, **kai** jis siunčia kodą, **tada** kodas atmetamas ir parodomas klaidos pranešimas.
3. **Duota** mokinys jau priklauso klasei, **kai** jis įveda kitos klasės kodą, **tada** prašoma patvirtinimo, o patvirtinus ankstesnė narystė pasibaigia.

## Ribiniai atvejai

- Kodas įvestas mažosiomis raidėmis ar su tarpais.
- Mokinys žaidžia nepriklausydamas klasei.
- Mokytojas ar administratorius bando prisijungti prie klasės.

## Reikalavimai

### Funkciniai reikalavimai

- **FR-001:** Sistema PRIVALO leisti mokiniui prisijungti prie klasės įvedus galiojantį klasės kodą.
- **FR-002:** Sistema PRIVALO atmesti neegzistuojantį kodą ir parodyti klaidos pranešimą.
- **FR-003:** Sistema PRIVALO leisti mokiniui priklausyti tik vienai klasei vienu metu.
- **FR-004:** Sistema PRIVALO prieš keičiant klasę paprašyti patvirtinimo.
- **FR-005:** Sistema PRIVALO leisti mokiniui žaisti ir nepriklausant klasei.
- **FR-006:** Sistema PRIVALO kodą kitokio formato nei 6 didžiosios raidės ar skaičiai atmesti kaip neegzistuojantį kodą.

### Pagrindinės esybės

- Klasės narystė.
- Klasė.

## Sėkmės kriterijai

- **SC-001:** Mokinys prisijungia prie klasės per ne daugiau kaip 30 sekundžių.
- **SC-002:** 100 % neegzistuojančių kodų atmetama.

## Paaiškinimai

**Sesija 2026-10-11**

- **Klausimas:** Kaip tikrinamas klasės kodo formatas?  
  **Atsakymas:** Kodą sudaro 6 didžiosios raidės (A–Z) arba skaičiai (VT-026). Kodas kitokio formato atmetamas tuo pačiu pranešimu kaip neegzistuojantis kodas.

Neišspręstų paaiškinimo žymų nėra.

## Prielaidos

- Mokinys gauna kodą iš mokytojo.

## Priklausomybės

- 010 Klasės sukūrimas ir ištrynimas.

## Apimties ribos

**Įeina:**
- Prisijungimas kodu, klasės keitimas.

**Neįeina:**
- Išėjimas iš klasės pačiam mokiniui.
