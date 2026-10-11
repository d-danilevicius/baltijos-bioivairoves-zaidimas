# Funkcijos specifikacija: Klasės mokinių rezultatų peržiūra

**Funkcijos aplankas:** `specs/012-klases-rezultatu-perziura`  
**Būsena:** juodraštis  
**Susiję projekto reikalavimai:** FR-030, FR-031, FR-032, FR-034; VT-017, VT-025; NFR-023

**Kodėl ši funkcija reikalinga:** Mokytojas mato, kuriuos lygius klasė supranta prasčiausiai.

## Naudotojo istorijos ir priėmimo scenarijai

### 1 istorija – Mokytojas peržiūri klasės rezultatus (prioritetas: P1)

**Naudotojo istorija:** Kaip mokytojas, noriu peržiūrėti klasės rezultatus, kad matyčiau, kuriuos lygius klasė supranta prasčiausiai.  
**Kodėl šis prioritetas:** Tai pagrindinė mokytojo nauda.  
**Nepriklausomas patikrinimas:** Turėti klasę su keliais mokiniais, atidaryti klasės rezultatus.

**Priėmimo scenarijai:**

1. **Duota** mokytojas turi klasę su mokiniais, **kai** jis atidaro klasę, **tada** mato mokinių sąrašą.
2. **Duota** mokytojas pasirenka mokinį, **kai** jis atidaro jo rezultatus, **tada** mato to mokinio baigtų lygių rezultatus.
3. **Duota** mokiniai baigė lygius, **kai** mokytojas atidaro klasės suvestinę, **tada** mato kiekvieno lygio vidutinį rezultatą ir lygius, išdėstytus nuo mažiausio vidurkio.
4. **Duota** mokytojas bando atidaryti kito mokytojo klasę, **kai** jis įveda jos adresą, **tada** prieiga atmetama.

## Ribiniai atvejai

- Klasėje nėra mokinių (tuščios būsenos pranešimas).
- Nė vienas narys nebaigė lygio (vidurkis nerodomas).
- Mokytojas bando atidaryti kito mokytojo klasę.
- Mokinys išėjo iš klasės (jo rezultatų nebematyti).

## Reikalavimai

### Funkciniai reikalavimai

- **FR-001:** Sistema PRIVALO leisti mokytojui peržiūrėti savo klasės mokinių sąrašą.
- **FR-002:** Sistema PRIVALO leisti mokytojui peržiūrėti kiekvieno savo klasės mokinio baigtų lygių rezultatus.
- **FR-003:** Sistema PRIVALO rodyti kiekvieno lygio klasės vidutinį rezultatą ir lygių tvarką pagal mažiausią vidurkį.
- **FR-004:** Sistema PRIVALO skaičiuoti vidurkį tik iš dabartinių klasės narių geriausių to lygio rezultatų.
- **FR-005:** Sistema NEPRIVALO rodyti vidurkio, jei lygio nebaigė nė vienas narys.
- **FR-006:** Sistema NEPRIVALO leisti mokytojui matyti kitų mokytojų klasių ir jų mokinių rezultatų.
- **FR-007:** Sistema NEPRIVALO leisti mokytojui keisti rezultatų.
- **FR-008:** Sistema PRIVALO leisti mokytojui, turinčiam kelias klases, pasirinkti klasę ir rodyti rezultatus tik tos klasės.

### Pagrindinės esybės

- Klasė.
- Klasės narystė.
- Lygio rezultatas.

## Sėkmės kriterijai

- **SC-001:** Klasės suvestinė atsidaro per ne daugiau kaip 3 sekundes.
- **SC-002:** 100 % bandymų pamatyti kito mokytojo klasę atmetama.
- **SC-003:** Mokytojas per ne daugiau kaip 2 paspaudimus randa lygį su mažiausiu vidurkiu.

## Paaiškinimai

**Sesija 2026-10-11**

- **Klausimas:** Ką mato mokytojas, turintis kelias klases?  
  **Atsakymas:** Jis pasirenka vieną savo klasę, o rezultatai ir vidurkiai rodomi atskirai kiekvienai klasei.

Neišspręstų paaiškinimo žymų nėra.

## Prielaidos

- Rezultatai rodomi tik apie baigtus lygius.

## Priklausomybės

- 010, 011, 008.

## Apimties ribos

**Įeina:**
- Klasės mokinių sąrašas, rezultatai, vidurkiai.

**Neįeina:**
- Rezultatų eksportas.
- Užduočių skyrimas.
