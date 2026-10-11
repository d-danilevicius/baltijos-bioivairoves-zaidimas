# Funkcijos specifikacija: Mokinio pašalinimas iš klasės

**Funkcijos aplankas:** `specs/013-mokinio-salinimas`  
**Būsena:** juodraštis  
**Susiję projekto reikalavimai:** FR-033, FR-040; VT-024

**Kodėl ši funkcija reikalinga:** Mokytojas gali sutvarkyti klasės sąrašą, pavyzdžiui, pašalinti per klaidą prisijungusį mokinį.

## Naudotojo istorijos ir priėmimo scenarijai

### 1 istorija – Mokytojas pašalina mokinį (prioritetas: P2)

**Naudotojo istorija:** Kaip mokytojas, noriu pašalinti mokinį iš klasės, kad ištaisyčiau klaidingą narystę.  
**Kodėl šis prioritetas:** Leidžia ištaisyti klaidingą narystę.  
**Nepriklausomas patikrinimas:** Atidaryti klasės sąrašą ir pašalinti mokinį.

**Priėmimo scenarijai:**

1. **Duota** mokytojas mato klasės mokinių sąrašą, **kai** jis pasirenka pašalinti mokinį, **tada** prašoma patvirtinimo.
2. **Duota** mokytojas patvirtina, **kai** mokinys pašalinamas, **tada** mokinys nebėra klasės narys, o jo rezultatų mokytojas nebemato, nors jie neištrinami.
3. **Duota** mokytojas atsisako patvirtinimo, **kai** jis uždaro patvirtinimo langą, **tada** mokinys lieka klasėje.
4. **Duota** mokytojas bando pašalinti mokinį iš kito mokytojo klasės, **kai** jis siunčia veiksmą, **tada** veiksmas atmetamas.

## Ribiniai atvejai

- Mokytojas atsisako patvirtinimo (niekas nekeičiama).
- Mokytojas bando pašalinti mokinį iš kito mokytojo klasės.

## Reikalavimai

### Funkciniai reikalavimai

- **FR-001:** Sistema PRIVALO leisti mokytojui pašalinti mokinį iš savo klasės.
- **FR-002:** Sistema PRIVALO prieš pašalinimą paprašyti patvirtinimo.
- **FR-003:** Sistema NEPRIVALO ištrinti pašalinto mokinio rezultatų.
- **FR-004:** Sistema NEPRIVALO rodyti pašalinto mokinio rezultatų mokytojui.

### Pagrindinės esybės

- Klasės narystė.

## Sėkmės kriterijai

- **SC-001:** 100 % pašalintų mokinių išnyksta iš klasės sąrašo.
- **SC-002:** Pašalinto mokinio rezultatai išlieka pačiam mokiniui.

## Paaiškinimai

**Sesija 2026-10-11**

- Neaiškumų nerasta.

Neišspręstų paaiškinimo žymų nėra.

## Prielaidos

- Pašalintas mokinys gali vėl prisijungti kodu.

## Priklausomybės

- 012 Klasės rezultatų peržiūra.

## Apimties ribos

**Įeina:**
- Mokinio pašalinimas iš klasės.

**Neįeina:**
- Mokinio paskyros trynimas.
