# Funkcijos specifikacija: Savo rezultatų peržiūra

**Funkcijos aplankas:** `specs/009-rezultatu-perziura`  
**Būsena:** juodraštis  
**Susiję projekto reikalavimai:** FR-017, FR-018; VT-017, VT-019; NFR-023

**Kodėl ši funkcija reikalinga:** Mokinys mato savo pažangą ir žino, kuriuos lygius gali tobulinti.

## Naudotojo istorijos ir priėmimo scenarijai

### 1 istorija – Mokinys peržiūri savo rezultatus (prioritetas: P2)

**Naudotojo istorija:** Kaip mokinys, noriu peržiūrėti savo rezultatus, kad matyčiau savo pažangą.  
**Kodėl šis prioritetas:** Rodo pažangą ir motyvuoja kartoti lygius.  
**Nepriklausomas patikrinimas:** Baigti kelis lygius ir atidaryti rezultatų ekraną.

**Priėmimo scenarijai:**

1. **Duota** mokinys baigė kelis lygius, **kai** jis atidaro rezultatus, **tada** mato kiekvieno baigto lygio rezultatą.
2. **Duota** mokinys dar nebaigė nė vieno lygio, **kai** jis atidaro rezultatus, **tada** mato tuščios būsenos pranešimą ir nuorodą į lygių sąrašą.
3. **Duota** mokinys bando atidaryti kito mokinio rezultatus, **kai** jis įveda jų adresą, **tada** prieiga atmetama.

## Ribiniai atvejai

- Lygis ištrintas administratoriaus (rezultatas išlieka).
- Mokinys bando atidaryti kito mokinio rezultatus.

## Reikalavimai

### Funkciniai reikalavimai

- **FR-001:** Sistema PRIVALO leisti mokiniui peržiūrėti savo baigtų lygių rezultatus.
- **FR-002:** Sistema NEPRIVALO leisti mokiniui peržiūrėti ar keisti kito mokinio rezultatų.
- **FR-003:** Sistema PRIVALO tuščiame sąraše rodyti paaiškinantį pranešimą.

### Pagrindinės esybės

- Lygio rezultatas.

## Sėkmės kriterijai

- **SC-001:** Rezultatų ekranas atsidaro per ne daugiau kaip 3 sekundes.
- **SC-002:** 100 % bandymų pamatyti kito mokinio rezultatus atmetama.

## Paaiškinimai

**Sesija 2026-10-11**

- Neaiškumų nerasta.

Neišspręstų paaiškinimo žymų nėra.

## Prielaidos

- Rezultatai rodomi pagal lygius.

## Priklausomybės

- 008 Rezultatų išsaugojimas.

## Apimties ribos

**Įeina:**
- Savo rezultatų peržiūra.

**Neįeina:**
- Rezultatų palyginimas su kitais mokiniais.
