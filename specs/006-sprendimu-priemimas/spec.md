# Funkcijos specifikacija: Sprendimų priėmimas lygyje

**Funkcijos aplankas:** `specs/006-sprendimu-priemimas`  
**Būsena:** juodraštis  
**Susiję projekto reikalavimai:** FR-010, FR-011, FR-012; VT-010–VT-012; NFR-013

**Kodėl ši funkcija reikalinga:** Pagrindinė žaidimo idėja: mokinys priima sprendimą ir iš karto mato pasekmes ekosistemai.

## Naudotojo istorijos ir priėmimo scenarijai

### 1 istorija – Mokinys pasirenka sprendimą situacijoje (prioritetas: P1)

**Naudotojo istorija:** Kaip mokinys, noriu pasirinkti sprendimą situacijoje, kad iš karto matyčiau jo poveikį jūros ekosistemai.  
**Kodėl šis prioritetas:** Tai pagrindinė mokymosi veikla.  
**Nepriklausomas patikrinimas:** Pradėti lygį, pasirinkti sprendimo variantą ir patikrinti rodomą pokytį.

**Priėmimo scenarijai:**

1. **Duota** mokinys žaidžia lygį, **kai** jam parodoma situacija, **tada** jis mato bent 2 sprendimo variantus.
2. **Duota** mokinys pasirenka vieną variantą, **kai** jis jį patvirtina, **tada** iš karto parodomas rūšių skaičiaus ir ekosistemos būklės pokytis.
3. **Duota** mokinys jau patvirtino pasirinkimą, **kai** jis bando jį pakeisti, **tada** pakeisti neleidžiama.

## Ribiniai atvejai

- Sprendimas išeitų už būklės ribų 0–100 (vertė prilyginama artimiausiai ribai).
- Mokinys bando pasirinkti kelis variantus vienoje situacijoje.
- Ryšys nutrūksta pasirinkimo metu.

## Reikalavimai

### Funkciniai reikalavimai

- **FR-001:** Sistema PRIVALO kiekvienoje situacijoje pateikti bent 2 sprendimo variantus.
- **FR-002:** Sistema PRIVALO leisti mokiniui pasirinkti vieną variantą kiekvienoje situacijoje.
- **FR-003:** Sistema NEPRIVALO leisti pakeisti patvirtinto pasirinkimo.
- **FR-004:** Sistema PRIVALO po kiekvieno pasirinkimo parodyti rūšių skaičiaus ir ekosistemos būklės (0–100) pokytį.
- **FR-005:** Sistema PRIVALO laikyti ekosistemos būklę ribose nuo 0 iki 100.

### Pagrindinės esybės

- Situacija.
- Sprendimo variantas.
- Mokinio pasirinkimas.
- Lygio bandymas.

## Sėkmės kriterijai

- **SC-001:** Pokytis parodomas per ne daugiau kaip 1 sekundę po pasirinkimo.
- **SC-002:** 100 % patvirtintų pasirinkimų yra užfiksuoti ir negali būti pakeisti.

## Paaiškinimai

**Sesija 2026-10-11**

- Neaiškumų nerasta.

Neišspręstų paaiškinimo žymų nėra.

## Prielaidos

- Kiekvieno varianto poveikis nustatytas iš anksto.

## Priklausomybės

- 005 Lygių sąrašas ir lygio pradžia.

## Apimties ribos

**Įeina:**
- Situacijos rodymas, pasirinkimas, pokyčio rodymas.

**Neįeina:**
- Lygio baigimas (007).
