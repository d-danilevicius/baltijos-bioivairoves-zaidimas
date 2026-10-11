# Funkcijos specifikacija: Lygio baigimas ir kito lygio atrakinimas

**Funkcijos aplankas:** `specs/007-lygio-baigimas`  
**Būsena:** juodraštis  
**Susiję projekto reikalavimai:** FR-013, FR-014; VT-009, VT-014, VT-015

**Kodėl ši funkcija reikalinga:** Mokinys turi pamatyti rezultatą ir gauti galimybę žaisti toliau.

## Naudotojo istorijos ir priėmimo scenarijai

### 1 istorija – Mokinys baigia lygį (prioritetas: P1)

**Naudotojo istorija:** Kaip mokinys, noriu baigti lygį ir pamatyti rezultatą, kad žinočiau, kaip sekėsi, ir galėčiau žaisti toliau.  
**Kodėl šis prioritetas:** Be lygio pabaigos nėra pažangos.  
**Nepriklausomas patikrinimas:** Atsakyti į visas lygio situacijas ir patikrinti rezultatą bei kito lygio būseną.

**Priėmimo scenarijai:**

1. **Duota** mokinys atsakė į visas lygio situacijas, **kai** paskutinis atsakymas patvirtinamas, **tada** lygis baigiamas ir parodomas galutinis rezultatas.
2. **Duota** mokinys baigė lygį, **kai** jis grįžta į lygių sąrašą, **tada** kitas lygis yra atrakintas.
3. **Duota** mokinys paliko lygį neužbaigęs, **kai** jis vėl atidaro tą lygį, **tada** gali pradėti jį iš naujo, o nebaigto lygio rezultatas neįskaitomas.
4. **Duota** mokinys atsakė ne į visas lygio situacijas, **kai** jis bando pamatyti galutinį rezultatą, **tada** galutinis rezultatas nerodomas, kol neatsakyta į visas situacijas.

## Ribiniai atvejai

- Mokinys baigia paskutinį esamą lygį (nėra ką atrakinti).
- Mokinys pakartoja jau baigtą lygį.

## Reikalavimai

### Funkciniai reikalavimai

- **FR-001:** Sistema PRIVALO baigti lygį, kai mokinys atsako į visas to lygio situacijas, ir parodyti galutinį rezultatą.
- **FR-002:** Sistema PRIVALO laikyti lygio rezultatu galutinę ekosistemos būklės vertę lygio pabaigoje.
- **FR-003:** Sistema PRIVALO atrakinti kitą lygį, kai mokinys baigia dabartinį.
- **FR-004:** Sistema NEPRIVALO įskaityti nebaigto lygio rezultato.

### Pagrindinės esybės

- Lygio bandymas.
- Lygio rezultatas.

## Sėkmės kriterijai

- **SC-001:** 100 % baigtų lygių parodo galutinį rezultatą.
- **SC-002:** Kitas lygis atrakinamas iš karto po lygio baigimo.

## Paaiškinimai

**Sesija 2026-10-11**

- Neaiškumų nerasta.

Neišspręstų paaiškinimo žymų nėra.

## Prielaidos

- Trumpas paaiškinimas po lygio ir pasiekimų ženkleliai priklauso vėlesnėms versijoms.

## Priklausomybės

- 006 Sprendimų priėmimas.

## Apimties ribos

**Įeina:**
- Lygio pabaiga, rezultatas, atrakinimas.

**Neįeina:**
- Rezultato išsaugojimas (008).
- Pasiekimų ženkleliai.
