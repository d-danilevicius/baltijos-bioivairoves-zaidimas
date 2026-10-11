# Funkcijos specifikacija: Rezultatų išsaugojimas ir žaidimo tęsimas

**Funkcijos aplankas:** `specs/008-rezultatu-issaugojimas`  
**Būsena:** juodraštis  
**Susiję projekto reikalavimai:** FR-015, FR-016; VT-014, VT-016; NFR-015

**Kodėl ši funkcija reikalinga:** Mokinys neturi prarasti pažangos ir turi galėti tęsti vėliau.

## Naudotojo istorijos ir priėmimo scenarijai

### 1 istorija – Mokinys tęsia žaidimą po pertraukos (prioritetas: P1)

**Naudotojo istorija:** Kaip mokinys, noriu, kad mano rezultatai būtų išsaugoti, kad galėčiau tęsti žaidimą vėliau.  
**Kodėl šis prioritetas:** Pamokos trumpos, mokinys gali žaisti keliomis dalimis.  
**Nepriklausomas patikrinimas:** Baigti lygį, atsijungti, prisijungti iš naujo ir patikrinti būseną.

**Priėmimo scenarijai:**

1. **Duota** mokinys baigė lygį, **kai** jis atsijungia ir vėl prisijungia, **tada** baigto lygio rezultatas išlikęs.
2. **Duota** mokinys nebaigė lygio, **kai** jis vėl prisijungia, **tada** gali tęsti nuo paskutinio neužbaigto lygio.
3. **Duota** mokinys pakartoja baigtą lygį, **kai** jis pasiekia kitokį rezultatą, **tada** lygio rezultatu laikomas geriausias pasiektas rezultatas.
4. **Duota** mokinys nebaigė lygio, **kai** jis atsijungia, **tada** nebaigto lygio rezultatas neišsaugomas kaip baigto.

## Ribiniai atvejai

- Ryšys nutrūksta iškart po lygio pabaigos.
- Pakartojus lygį gaunamas prastesnis rezultatas.

## Reikalavimai

### Funkciniai reikalavimai

- **FR-001:** Sistema PRIVALO išsaugoti mokinio rezultatą po kiekvieno baigto lygio.
- **FR-002:** Sistema PRIVALO išsaugoti rezultatą ir tada, kai ryšys nutrūksta iškart po lygio pabaigos.
- **FR-003:** Sistema PRIVALO leisti mokiniui tęsti žaidimą nuo paskutinio neužbaigto lygio po atsijungimo ir naujo prisijungimo.
- **FR-004:** Sistema PRIVALO kaip lygio rezultatą laikyti geriausią pasiektą rezultatą.

### Pagrindinės esybės

- Lygio rezultatas.
- Lygio bandymas.

## Sėkmės kriterijai

- **SC-001:** 100 % baigtų lygių rezultatų išlieka po atsijungimo ir naujo prisijungimo.
- **SC-002:** Pakartojus lygį rezultatas niekada nepablogėja.

## Paaiškinimai

**Sesija 2026-10-11**

- Neaiškumų nerasta.

Neišspręstų paaiškinimo žymų nėra.

## Prielaidos

- Nebaigto lygio tarpinė būsena išsaugoma tik tiek, kad mokinys galėtų pradėti tą lygį iš naujo.

## Priklausomybės

- 007 Lygio baigimas.

## Apimties ribos

**Įeina:**
- Rezultatų išsaugojimas, tęsimas, geriausias rezultatas.

**Neįeina:**
- Rezultatų peržiūra (009).
