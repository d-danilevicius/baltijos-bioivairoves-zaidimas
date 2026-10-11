# Funkcijos specifikacija: Klasės sukūrimas ir ištrynimas

**Funkcijos aplankas:** `specs/010-klases-kurimas`  
**Būsena:** juodraštis  
**Susiję projekto reikalavimai:** FR-026, FR-027, FR-035, FR-040; VT-020, VT-021, VT-024, VT-026

**Kodėl ši funkcija reikalinga:** Mokytojas gali sujungti savo mokinius į klasę ir stebėti jų pažangą.

## Naudotojo istorijos ir priėmimo scenarijai

### 1 istorija – Mokytojas sukuria klasę (prioritetas: P1)

**Naudotojo istorija:** Kaip mokytojas, noriu sukurti klasę, kad galėčiau sujungti savo mokinius.  
**Kodėl šis prioritetas:** Be klasės mokytojas negali matyti mokinių rezultatų.  
**Nepriklausomas patikrinimas:** Prisijungti kaip mokytojas, sukurti klasę ir gauti kodą.

**Priėmimo scenarijai:**

1. **Duota** prisijungęs mokytojas, **kai** jis įveda klasės pavadinimą ir ją sukuria, **tada** klasė sukuriama ir parodomas 6 simbolių prisijungimo kodas.
2. **Duota** mokytojas turi klasę, **kai** jis pasirenka ją ištrinti, **tada** prieš ištrinant prašoma patvirtinimo, o patvirtinus klasė pašalinama.
3. **Duota** prisijungęs mokinys, **kai** jis bando sukurti klasę, **tada** veiksmas atmetamas.
4. **Duota** mokytojas palieka tuščią pavadinimą, **kai** jis bando sukurti klasę, **tada** klasė nesukuriama ir parodomas klaidos pranešimas.

## Ribiniai atvejai

- Pavadinimas tuščias arba ilgesnis nei 50 simbolių.
- Mokytojas ištrina klasę su mokiniais (jų rezultatai neištrinami).
- Mokinys ar administratorius bando sukurti klasę.

## Reikalavimai

### Funkciniai reikalavimai

- **FR-001:** Sistema PRIVALO leisti mokytojui sukurti klasę, nurodžius klasės pavadinimą (1–50 simbolių).
- **FR-002:** Sistema PRIVALO kiekvienai klasei sugeneruoti unikalų 6 simbolių prisijungimo kodą ir rodyti jį mokytojui.
- **FR-003:** Sistema PRIVALO leisti mokytojui ištrinti savo klasę.
- **FR-004:** Sistema PRIVALO prieš ištrinant klasę paprašyti patvirtinimo.
- **FR-005:** Sistema NEPRIVALO ištrinti mokinių rezultatų, kai klasė ištrinama.
- **FR-006:** Sistema PRIVALO leisti kurti klasę tik mokytojui.

### Pagrindinės esybės

- Klasė.

## Sėkmės kriterijai

- **SC-001:** Mokytojas klasę sukuria per ne daugiau kaip 1 minutę.
- **SC-002:** Nė dvi klasės neturi to paties kodo.

## Paaiškinimai

**Sesija 2026-10-11**

- Neaiškumų nerasta.

Neišspręstų paaiškinimo žymų nėra.

## Prielaidos

- Kodą mokytojas mokiniams perduoda pats.

## Priklausomybės

- 002 Prisijungimas.

## Apimties ribos

**Įeina:**
- Klasės sukūrimas, kodas, klasės ištrynimas.

**Neįeina:**
- Kodo keitimas.
- Užduočių skyrimas klasei.
