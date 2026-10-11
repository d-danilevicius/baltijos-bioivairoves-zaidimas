# Specifikacijų, planų ir užduočių analizė

Patikrinta, ar konstitucija, 15 specifikacijų, planų ir užduočių sąrašų sutampa. Radiniai ištaisyti, kad realizavimą būtų galima pradėti.

## Rasti ir ištaisyti trūkumai

**Trūkstami testai (reikalavimas buvo, bet jo niekas netikrino)**
1. 002: atsijungimas, sesijos pabaiga po 30 min, prisijungimų įrašymas į žurnalą. Pridėti 3 testai.
2. 005: ne mažiau kaip 3 lygiai ir neaktyvių lygių neradimas; lygio aprašymo laukai. Pridėti 2 testai.
3. 008: bandymo baigimas ir rezultato išsaugojimas kartu (ryšio nutrūkimas). Pridėtas testas.
4. 010: kodo pasikartojimo atvejis. Pridėtas testas.
5. 011: mokinys be klasės gali žaisti. Pridėtas testas.
6. 012: narių sąrašas, mokinio rezultatai, klasės pasirinkimas. Pridėti 3 testai.
7. 014: sėkmingas lygio sukūrimas. Pridėtas testas.
8. 015: paskyrų sąrašas. Pridėtas testas.

**Reikalavimai be įgyvendinimo užduočių**
9. 005: baigto lygio kartojimas (FR-043). Pridėti užduotis ir testas.
10. 006: pradinės situacijos ir variantai 3 pradiniams lygiams. Pridėta užduotis.
11. 011 / 004: klasės pavadinimas profilyje. Pridėtos užduotis ir testas.
12. 001: .gitignore (konstitucija 5.5), automatinė patikra .github (konstitucija, Valdymas), HTTPS nukreipimas (NFR-002), atsarginė kopija (NFR-016). Pridėtos 4 užduotys.
13. 015: 50 vienu metu žaidžiančių naudotojų (NFR-014) ir naršyklių patikra (NFR-017). Pridėtos 2 galutinės užduotys.

**Neaiškios priklausomybės**
14. 005 priklausė nuo 014, o 014 nuo 005 esybių (ratas). Pradiniai lygiai įrašomi migracijoje, 005 priklauso tik nuo 002.
15. 014 nurodė tik 002, nors naudoja 005 ir 006 esybes. Ištaisyta: 002, 005, 006.
16. 015 nurodė tik 002, nors trina rezultatus ir klases. Ištaisyta: 002, 008, 011.
17. 004 rodo klasę, kuri atsiranda tik 011. Pažymėta planuose ir užduotyse.
18. Užduočių failuose nebuvo nurodytos priklausomybės tarp funkcijų. Pridėta eilutė „Priklauso nuo funkcijų“.

## Patikrinta ir problemų nerasta

- Prieštaringų reikalavimų tarp specifikacijų nėra.
- Architektūros pažeidimų nėra: verslo taisyklės visur Application sluoksnyje.
- Visos užduotys susietos su specifikacija (žyma [US1]) arba yra paruošimo / galutinės patikros užduotys.
- Reikalavimai FR-041–FR-049 yra `docs/requirements.md`.

## Išvada

Specifikacijos, planai ir užduotys dabar sutampa. Užduočių iš viso: 218. Realizavimą galima pradėti.
