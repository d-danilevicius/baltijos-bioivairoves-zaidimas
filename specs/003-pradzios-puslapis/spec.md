# Funkcijos specifikacija: Pradžios puslapis

**Funkcijos aplankas:** `specs/003-pradzios-puslapis`  
**Būsena:** juodraštis  
**Susiję projekto reikalavimai:** FR-037; NFR-007, NFR-012, NFR-021

**Kodėl ši funkcija reikalinga:** Naujas lankytojas turi suprasti, kas yra žaidimas, ir rasti, kaip prisijungti.

## Naudotojo istorijos ir priėmimo scenarijai

### 1 istorija – Lankytojas mato pradžios puslapį (prioritetas: P1)

**Naudotojo istorija:** Kaip lankytojas, noriu matyti pradžios puslapį, kad suprasčiau, kas tai per žaidimas, ir rasčiau, kaip prisijungti.  
**Kodėl šis prioritetas:** Tai pirmas ekranas, kurį mato kiekvienas lankytojas.  
**Nepriklausomas patikrinimas:** Atidaryti svetainę neprisijungus ir patikrinti turinį.

**Priėmimo scenarijai:**

1. **Duota** neprisijungęs lankytojas, **kai** jis atidaro svetainę, **tada** jis mato projekto aprašymą ir nuorodas „Prisijungti“ bei „Registruotis“.
2. **Duota** lankytojas naudoja telefoną, **kai** jis atidaro pradžios puslapį, **tada** visas turinys telpa be horizontalaus slinkimo.
3. **Duota** lankytojas atidaro neegzistuojantį puslapio adresą, **kai** jis jį įveda naršyklėje, **tada** parodomas pranešimas „Puslapis nerastas“ ir mygtukas „Į pradžią“.

## Ribiniai atvejai

- Puslapis atidaromas lėtu ryšiu.
- Atidaromas neegzistuojantis adresas (rodomas „Puslapis nerastas“ su nuoroda į pradžią).

## Reikalavimai

### Funkciniai reikalavimai

- **FR-001:** Sistema PRIVALO neprisijungusiam lankytojui rodyti pradžios puslapį su projekto aprašymu ir nuorodomis „Prisijungti“ bei „Registruotis“.
- **FR-002:** Sistema PRIVALO rodyti pranešimą „Puslapis nerastas“ ir mygtuką „Į pradžią“, kai ekranas neegzistuoja.
- **FR-003:** Sistema PRIVALO prisijungusį naudotoją, atidariusį pradžios puslapį, nukreipti į jo pradinį ekraną pagal rolę.

### Pagrindinės esybės

- Jokių.

## Sėkmės kriterijai

- **SC-001:** Puslapis atsidaro per ne daugiau kaip 3 sekundes.
- **SC-002:** Lankytojas randa registracijos nuorodą su vienu paspaudimu.
- **SC-003:** Puslapis tinkamas ekranams nuo 360 px pločio.

## Paaiškinimai

**Sesija 2026-10-11**

- **Klausimas:** Ką mato prisijungęs naudotojas, atidaręs pradžios puslapį?  
  **Atsakymas:** Jis nukreipiamas į savo pradinį ekraną pagal rolę (žr. docs/ux.md, navigacija). Pradžios puslapis su aprašymu rodomas tik neprisijungusiems.

Neišspręstų paaiškinimo žymų nėra.

## Prielaidos

- Aprašymo tekstą pateikia projekto autorius.

## Priklausomybės

- Jokių.

## Apimties ribos

**Įeina:**
- Pradžios puslapis ir „Puslapis nerastas“.

**Neįeina:**
- Prisijungusio naudotojo pradinis ekranas.
