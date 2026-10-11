# Funkcijos specifikacija: Naudotojo profilis

**Funkcijos aplankas:** `specs/004-profilis`  
**Būsena:** juodraštis  
**Susiję projekto reikalavimai:** FR-036; NFR-003

**Kodėl ši funkcija reikalinga:** Naudotojas gali pamatyti, kokie jo duomenys saugomi sistemoje.

## Naudotojo istorijos ir priėmimo scenarijai

### 1 istorija – Naudotojas peržiūri savo profilį (prioritetas: P2)

**Naudotojo istorija:** Kaip prisijungęs naudotojas, noriu peržiūrėti savo profilį, kad žinočiau, kokie mano duomenys saugomi.  
**Kodėl šis prioritetas:** Padeda patikrinti paskyros duomenis ir klasę.  
**Nepriklausomas patikrinimas:** Prisijungti ir atidaryti profilį.

**Priėmimo scenarijai:**

1. **Duota** prisijungęs naudotojas, **kai** jis atidaro profilį, **tada** jis mato savo naudotojo vardą, el. paštą ir rolę.
2. **Duota** prisijungęs mokinys, priklausantis klasei, **kai** jis atidaro profilį, **tada** papildomai mato savo klasę.
3. **Duota** naudotojas bando atidaryti kito naudotojo profilį, **kai** jis įveda to profilio adresą, **tada** prieiga atmetama.

## Ribiniai atvejai

- Mokinys nepriklauso jokiai klasei.
- Naudotojas bando atidaryti kito naudotojo profilį.

## Reikalavimai

### Funkciniai reikalavimai

- **FR-001:** Sistema PRIVALO leisti prisijungusiam naudotojui peržiūrėti savo profilio duomenis: naudotojo vardą, el. paštą, rolę ir (mokiniui) klasę.
- **FR-002:** Sistema NEPRIVALO rodyti kito naudotojo profilio.

### Pagrindinės esybės

- Naudotojas.
- Klasės narystė.

## Sėkmės kriterijai

- **SC-001:** Profilis atsidaro per ne daugiau kaip 3 sekundes.
- **SC-002:** 100 % bandymų pamatyti kito naudotojo profilį atmetama.

## Paaiškinimai

**Sesija 2026-10-11**

- Neaiškumų nerasta.

Neišspręstų paaiškinimo žymų nėra.

## Prielaidos

- Profilio duomenų keitimas pirmoje versijoje nenumatytas.

## Priklausomybės

- 002 Prisijungimas.

## Apimties ribos

**Įeina:**
- Profilio peržiūra.

**Neįeina:**
- Profilio duomenų redagavimas.
