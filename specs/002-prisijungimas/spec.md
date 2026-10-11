# Funkcijos specifikacija: Prisijungimas ir atsijungimas

**Funkcijos aplankas:** `specs/002-prisijungimas`  
**Būsena:** juodraštis  
**Susiję projekto reikalavimai:** FR-004–FR-007; VT-006; NFR-004, NFR-012, NFR-018, NFR-019

**Kodėl ši funkcija reikalinga:** Naudotojas turi įrodyti, kas jis yra, kad matytų tik jam skirtus duomenis.

## Naudotojo istorijos ir priėmimo scenarijai

### 1 istorija – Naudotojas prisijungia ir atsijungia (prioritetas: P1)

**Naudotojo istorija:** Kaip užsiregistravęs naudotojas, noriu prisijungti ir atsijungti, kad matyčiau tik savo duomenis.  
**Kodėl šis prioritetas:** Visos kitos funkcijos veikia tik prisijungus.  
**Nepriklausomas patikrinimas:** Prisijungti su teisingais ir neteisingais duomenimis, tada atsijungti.

**Priėmimo scenarijai:**

1. **Duota** užsiregistravęs naudotojas, **kai** jis įveda teisingą el. paštą ir slaptažodį, **tada** jis prisijungia ir patenka į savo pradinį ekraną.
2. **Duota** naudotojas įveda neteisingus duomenis, **kai** jis bando prisijungti, **tada** prisijungimas atmetamas, parodomas klaidos pranešimas ir naudotojas lieka prisijungimo ekrane.
3. **Duota** naudotojo paskyra užblokuota, **kai** jis bando prisijungti, **tada** prisijungimas atmetamas ir parodomas pranešimas apie blokavimą.
4. **Duota** naudotojas prisijungęs, **kai** jis pasirenka atsijungti, **tada** sesija baigiama ir jis grąžinamas į pradžios puslapį.

## Ribiniai atvejai

- Nėra veiksmų 30 minučių.
- Neprisijungęs lankytojas atidaro nuorodą į žaidimo ar rezultatų ekraną.
- Tuščias el. pašto ar slaptažodžio laukas.

## Reikalavimai

### Funkciniai reikalavimai

- **FR-001:** Sistema PRIVALO leisti užsiregistravusiam naudotojui prisijungti pateikus teisingą el. pašto adresą ir slaptažodį.
- **FR-002:** Sistema PRIVALO atmesti prisijungimą su neteisingais duomenimis ir parodyti klaidos pranešimą.
- **FR-003:** Sistema PRIVALO neleisti prisijungti užblokuotam naudotojui.
- **FR-004:** Sistema PRIVALO leisti prisijungusiam naudotojui atsijungti.
- **FR-005:** Sistema PRIVALO neleisti neprisijungusiam naudotojui pradėti žaidimo ar peržiūrėti rezultatų.
- **FR-006:** Sistema PRIVALO automatiškai atjungti naudotoją po 30 minučių neveiklumo.
- **FR-007:** Sistema PRIVALO registruoti prisijungimus ir nepavykusius prisijungimus, bet NEPRIVALO rašyti slaptažodžių.
- **FR-008:** Sistema PRIVALO neteisingų prisijungimo duomenų pranešime nenurodyti, kuris laukas klaidingas.

### Pagrindinės esybės

- Naudotojas – paskyra su role.
- Veiksmų žurnalo įrašas – užfiksuotas prisijungimas ar nepavykęs prisijungimas.

## Sėkmės kriterijai

- **SC-001:** Prisijungimas įvyksta per ne daugiau kaip 3 sekundes.
- **SC-002:** 100 % bandymų prisijungti su neteisingais duomenimis atmetama.
- **SC-003:** Neprisijungęs lankytojas negali pasiekti nė vieno apsaugoto ekrano.

## Paaiškinimai

**Sesija 2026-10-11**

- **Klausimas:** Ką rodyti, kai prisijungimo duomenys neteisingi?  
  **Atsakymas:** Bendrą pranešimą, nenurodantį, ar klaidingas el. paštas, ar slaptažodis. Užblokuotai paskyrai rodomas atskiras pranešimas.
- **Klausimas:** Ar ribojamas nepavykusių prisijungimų skaičius?  
  **Atsakymas:** Ne. Reikalavimuose tokios taisyklės nėra, todėl pirmoje versijoje ji neįeina (nepavykę bandymai tik registruojami žurnale).

Neišspręstų paaiškinimo žymų nėra.

## Prielaidos

- Užblokuoti paskyras gali administratorius (žr. 015).

## Priklausomybės

- 001 Registracija.

## Apimties ribos

**Įeina:**
- Prisijungimas ir atsijungimas.

**Neįeina:**
- Slaptažodžio atkūrimas.
