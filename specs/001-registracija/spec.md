# Funkcijos specifikacija: Registracija

**Funkcijos aplankas:** `specs/001-registracija`  
**Būsena:** juodraštis  
**Susiję projekto reikalavimai:** FR-001, FR-002, FR-003; VT-001–VT-005, VT-026; NFR-001, NFR-011

**Kodėl ši funkcija reikalinga:** Be paskyros niekas negali žaisti ar valdyti klasės. Registracija leidžia mokiniui arba mokytojui gauti savo paskyrą.

## Naudotojo istorijos ir priėmimo scenarijai

### 1 istorija – Lankytojas užsiregistruoja (prioritetas: P1)

**Naudotojo istorija:** Kaip lankytojas, noriu užsiregistruoti kaip mokinys ar mokytojas, kad turėčiau savo paskyrą.  
**Kodėl šis prioritetas:** Be registracijos negalima naudotis jokia kita funkcija.  
**Nepriklausomas patikrinimas:** Atidaryti registracijos formą, užpildyti ją teisingais duomenimis ir patikrinti, ar paskyra sukurta.

**Priėmimo scenarijai:**

1. **Duota** lankytojas, neturintis paskyros, **kai** jis užpildo el. paštą, naudotojo vardą, slaptažodį ir pasirenka mokinio arba mokytojo rolę, **tada** paskyra sukuriama su pasirinkta role.
2. **Duota** el. pašto adresas jau priskirtas kitai paskyrai, **kai** lankytojas bando užsiregistruoti su juo, **tada** registracija atmetama ir parodomas aiškus pranešimas.
3. **Duota** lankytojas įveda trumpesnį nei 8 simbolių slaptažodį, **kai** jis siunčia formą, **tada** registracija atmetama ir nurodoma, kokio ilgio slaptažodis reikalingas.

## Ribiniai atvejai

- Kuris nors laukas paliktas tuščias.
- Naudotojo vardas jau užimtas.
- Naudotojo vardas trumpesnis nei 3 arba ilgesnis nei 30 simbolių, arba turi neleistinų simbolių.
- Neteisingas el. pašto formatas.
- Bandoma pasirinkti administratoriaus rolę.

## Reikalavimai

### Funkciniai reikalavimai

- **FR-001:** Sistema PRIVALO leisti lankytojui užsiregistruoti kaip mokiniui arba mokytojui, nurodžius el. pašto adresą, naudotojo vardą, slaptažodį ir rolę.
- **FR-002:** Sistema PRIVALO neleisti registracijos, jei el. pašto adresas jau priskirtas kitai paskyrai.
- **FR-003:** Sistema PRIVALO neleisti registracijos, jei naudotojo vardas jau užimtas.
- **FR-004:** Sistema PRIVALO reikalauti, kad slaptažodį sudarytų bent 8 simboliai.
- **FR-005:** Sistema PRIVALO reikalauti, kad naudotojo vardą sudarytų 3–30 simbolių (raidės, skaičiai, „_“).
- **FR-006:** Sistema PRIVALO neleisti registruotis administratoriaus role.
- **FR-007:** Kiekvienas klaidos pranešimas PRIVALO nurodyti, kas nepavyko ir ką daryti toliau.

### Pagrindinės esybės

- Naudotojas – paskyra su el. paštu, naudotojo vardu, slaptažodžiu ir viena role.

## Sėkmės kriterijai

- **SC-001:** Naujas naudotojas registraciją užbaigia per ne daugiau kaip 1 minutę.
- **SC-002:** 100 % bandymų registruotis su jau naudojamu el. paštu ar vardu atmetama.
- **SC-003:** Slaptažodžio negalima perskaityti iš išsaugotų duomenų.

## Paaiškinimai

**Sesija 2026-10-11**

- Neaiškumų nerasta.

Neišspręstų paaiškinimo žymų nėra.

## Prielaidos

- Mokytojai registruojasi patys, jų tapatybė netikrinama.
- Administratoriaus paskyrą sukuria sistemos kūrėjas.

## Priklausomybės

- Jokių.

## Apimties ribos

**Įeina:**
- Registracijos forma ir paskyros sukūrimas.

**Neįeina:**
- Prisijungimas.
- Administratoriaus paskyros kūrimas.
