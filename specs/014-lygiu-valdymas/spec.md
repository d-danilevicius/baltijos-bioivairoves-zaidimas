# Funkcijos specifikacija: Lygių valdymas

**Funkcijos aplankas:** `specs/014-lygiu-valdymas`  
**Būsena:** juodraštis  
**Susiję projekto reikalavimai:** FR-019, FR-020, FR-040; VT-010, VT-018, VT-019, VT-027; NFR-020

**Kodėl ši funkcija reikalinga:** Administratorius gali kurti ir tvarkyti žaidimo turinį nekeisdamas programos.

## Naudotojo istorijos ir priėmimo scenarijai

### 1 istorija – Administratorius sukuria ir keičia lygį (prioritetas: P1)

**Naudotojo istorija:** Kaip administratorius, noriu kurti ir keisti lygius, kad žaidimo turinį galėčiau tvarkyti nekeisdamas programos.  
**Kodėl šis prioritetas:** Be lygių nėra žaidimo.  
**Nepriklausomas patikrinimas:** Prisijungti kaip administratorius, sukurti lygį su situacijomis, redaguoti ir ištrinti.

**Priėmimo scenarijai:**

1. **Duota** prisijungęs administratorius, **kai** jis sukuria lygį su pavadinimu, situacijomis ir sprendimo variantais, **tada** lygis išsaugomas ir tampa matomas mokiniams.
2. **Duota** lygis jau sukurtas, **kai** administratorius jį redaguoja, **tada** pakeitimai išsaugomi.
3. **Duota** administratorius ištrina lygį, **kai** jis patvirtina veiksmą, **tada** lygis nebepateikiamas naujiems žaidimams, o mokinių rezultatai išlieka.
4. **Duota** prisijungęs mokinys ar mokytojas, **kai** jis bando sukurti ar pakeisti lygį, **tada** veiksmas atmetamas.
5. **Duota** situacijoje nurodytas tik vienas sprendimo variantas, **kai** administratorius bando išsaugoti lygį, **tada** lygis neišsaugomas ir parodomas klaidos pranešimas.

## Ribiniai atvejai

- Situacija turi mažiau nei 2 arba daugiau nei 4 variantus.
- Varianto poveikis už leistinų ribų.
- Pasikartojantis lygio eilės numeris.
- Lygyje nėra nė vienos situacijos.
- Kitas naudotojas (ne administratorius) bando keisti lygį.

## Reikalavimai

### Funkciniai reikalavimai

- **FR-001:** Sistema PRIVALO leisti administratoriui sukurti lygį su pavadinimu, situacijomis ir sprendimo variantais.
- **FR-002:** Sistema PRIVALO leisti administratoriui redaguoti ir ištrinti lygį.
- **FR-003:** Sistema PRIVALO leisti kurti, redaguoti ir trinti lygius tik administratoriui.
- **FR-004:** Sistema PRIVALO reikalauti: lygio eilės numeris – unikalus sveikas skaičius nuo 1; bent 1 situacija; situacijoje 2–4 variantai; varianto poveikis būklei nuo −50 iki 50, rūšių skaičiui nuo −10 iki 10.
- **FR-005:** Sistema PRIVALO prieš ištrinant lygį paprašyti patvirtinimo.
- **FR-006:** Sistema NEPRIVALO ištrinti mokinių rezultatų, kai lygis ištrinamas.
- **FR-007:** Sistema PRIVALO leisti keisti lygių turinį nekeičiant programos kodo.
- **FR-008:** Sistema PRIVALO leisti užbaigti jau pradėtą ištrinto lygio bandymą.
- **FR-009:** Sistema NEPRIVALO keisti išsaugotų rezultatų redaguojant lygį.

### Pagrindinės esybės

- Lygis.
- Situacija.
- Sprendimo variantas.

## Sėkmės kriterijai

- **SC-001:** Administratorius naują lygį su situacijomis sukuria be programuotojo pagalbos.
- **SC-002:** 100 % bandymų keisti lygius ne administratoriaus teisėmis atmetama.

## Paaiškinimai

**Sesija 2026-10-11**

- **Klausimas:** Kas nutinka, kai ištrinamas lygis, kurį mokinys žaidžia?  
  **Atsakymas:** Lygis nebepateikiamas naujiems žaidimams (VT-019). Jau pradėtas bandymas gali būti užbaigtas, rezultatas išlieka.
- **Klausimas:** Kas nutinka rezultatams, kai lygis redaguojamas?  
  **Atsakymas:** Išsaugoti rezultatai nekeičiami (VT-017).

Neišspręstų paaiškinimo žymų nėra.

## Prielaidos

- Pirmą turinį gali įrašyti projekto autorius per tą pačią sąsają.

## Priklausomybės

- 002 Prisijungimas.

## Apimties ribos

**Įeina:**
- Lygių kūrimas, redagavimas, trynimas.

**Neįeina:**
- Lygių eilės pertvarkymas masiškai.
- Turinio importas.
