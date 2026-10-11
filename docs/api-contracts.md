# API sutartys

Šiame dokumente aprašyta kiekviena serverio užklausa: metodas, adresas, kas gali naudoti, siunčiami ir grąžinami duomenys, būsenos kodai ir klaidos. Elgsena atitinka `specs/` aplanko specifikacijas.

## Bendros taisyklės

**Prisijungimas.** Naudojamas slapukas, gaunamas po `POST /api/auth/login`. Visos užklausos, išskyrus registraciją ir prisijungimą, reikalauja prisijungimo.

**Rolės.** `Mokinys`, `Mokytojas`, `Administratorius`. Rolė tikrinama serveryje.

**Apsauga nuo užklausų klastojimo (CSRF).** Kiekviena keičianti užklausa (POST, PUT, DELETE) siunčia antraštę `X-XSRF-TOKEN`.

**Duomenų formatas.** JSON, laukų pavadinimai camelCase.

**Klaidų atsakymas.** Visos klaidos grąžinamos kaip `ProblemDetails` (RFC 9457): `title`, `status`, `detail`. Validacijos klaidoms (400) pridedamas `errors`: laukas → klaidų sąrašas. Klaidoje niekada nerodomi technikos duomenys.

**Būsenos kodai.**
- 200 – pavyko, grąžinami duomenys
- 201 – sukurta
- 204 – pavyko, duomenų negrąžinama
- 400 – neteisingi įvesties duomenys (validacija)
- 401 – neprisijungęs
- 403 – nepakanka teisių (kita rolė ar svetimi duomenys)
- 404 – nerasta
- 409 – konfliktas (pasikartojimas arba netinkama būsena)
- 500 – netikėta serverio klaida (be technikos detalių)

---

## Paskyros (001, 002, 004)

### POST /api/auth/register
- **Kas gali:** visi (neprisijungę).
- **Prašymas:** `{ "email": "...", "userName": "...", "password": "...", "role": "Mokinys" | "Mokytojas" }`
- **Atsakymas 201:** `{ "id": "...", "userName": "...", "role": "..." }`
- **400:** neteisingas el. pašto formatas; naudotojo vardas ne 3–30 simbolių arba su kitais simboliais nei raidės, skaičiai, „_“; slaptažodis trumpesnis nei 8 simboliai; rolė ne `Mokinys` ar `Mokytojas` (administratoriaus rolė draudžiama).
- **409:** el. paštas jau naudojamas; naudotojo vardas jau naudojamas.

### POST /api/auth/login
- **Kas gali:** visi (neprisijungę).
- **Prašymas:** `{ "email": "...", "password": "..." }`
- **Atsakymas 200:** `{ "userName": "...", "role": "..." }`, nustato prisijungimo slapuką.
- **400:** tuščias el. paštas ar slaptažodis.
- **401:** neteisingi duomenys. Pranešimas bendras, nenurodo, kuris laukas klaidingas.
- **403:** paskyra užblokuota.

### POST /api/auth/logout
- **Kas gali:** prisijungę.
- **Prašymas:** be duomenų.
- **Atsakymas 204.**
- **401:** neprisijungęs.

### GET /api/profile
- **Kas gali:** prisijungę. Grąžina tik savo duomenis.
- **Atsakymas 200:** `{ "userName": "...", "email": "...", "role": "...", "className": "..." | null }`
- **401:** neprisijungęs.

---

## Žaidimas (005–009)

### GET /api/levels
- **Kas gali:** `Mokinys`.
- **Atsakymas 200:** `[ { "id": 1, "orderNo": 1, "title": "...", "isUnlocked": true, "bestScore": 70 | null } ]`. Tik aktyvūs lygiai.
- **401, 403.**

### GET /api/levels/{id}
- **Kas gali:** `Mokinys`.
- **Atsakymas 200:** `{ "id": 1, "title": "...", "description": "...", "startState": 60, "startSpeciesCount": 40, "situationCount": 5, "isUnlocked": true }`
- **404:** lygio nėra arba jis neaktyvus.

### POST /api/levels/{id}/attempts
- **Kas gali:** `Mokinys`.
- **Prašymas:** be duomenų.
- **Atsakymas 201:** `{ "attemptId": "...", "currentState": 60, "speciesCount": 40 }`
- **403:** lygis užrakintas.
- **404:** lygio nėra arba jis neaktyvus.

### GET /api/attempts/{id}/situation
- **Kas gali:** `Mokinys`, tik savo bandymas.
- **Atsakymas 200:** `{ "situationId": "...", "orderNo": 1, "total": 5, "text": "...", "options": [ { "id": "...", "text": "..." } ] }`
- **403:** svetimas bandymas.
- **404:** bandymo nėra.
- **409:** bandymas jau baigtas.

### POST /api/attempts/{id}/choices
- **Kas gali:** `Mokinys`, tik savo bandymas.
- **Prašymas:** `{ "situationId": "...", "optionId": "..." }`
- **Atsakymas 200:** `{ "currentState": 55, "speciesCount": 38, "stateChange": -5, "speciesChange": -2, "isFinished": false }`
- **400:** variantas nepriklauso šiai situacijai.
- **403:** svetimas bandymas.
- **404:** bandymo nėra.
- **409:** šioje situacijoje jau pasirinkta; bandymas jau baigtas.

### GET /api/attempts/{id}/result
- **Kas gali:** `Mokinys`, tik savo bandymas.
- **Atsakymas 200:** `{ "levelId": 1, "finalScore": 55, "speciesCount": 38, "nextLevelUnlocked": true }`
- **403:** svetimas bandymas.
- **404:** bandymo nėra.
- **409:** bandymas dar nebaigtas.

### GET /api/attempts/current
- **Kas gali:** `Mokinys`.
- **Atsakymas 200:** `{ "attemptId": "...", "levelId": 1 }`.
- **204:** nebaigto bandymo nėra.

### GET /api/results
- **Kas gali:** `Mokinys`. Grąžina tik savo rezultatus, įskaitant ištrintų lygių.
- **Atsakymas 200:** `[ { "levelId": 1, "levelTitle": "...", "bestScore": 70, "updatedAt": "..." } ]`. Tuščias sąrašas, jei lygių nebaigta.

---

## Klasės (010–013)

### POST /api/classes
- **Kas gali:** `Mokytojas`.
- **Prašymas:** `{ "name": "..." }`
- **Atsakymas 201:** `{ "id": "...", "name": "...", "code": "AB12CD" }`
- **400:** pavadinimas tuščias arba ilgesnis nei 50 simbolių.
- **403:** ne mokytojas.

### GET /api/classes
- **Kas gali:** `Mokytojas`. Tik savo klasės.
- **Atsakymas 200:** `[ { "id": "...", "name": "...", "code": "...", "studentCount": 12 } ]`

### DELETE /api/classes/{id}
- **Kas gali:** `Mokytojas`, tik savo klasė.
- **Atsakymas 204.** Mokinių rezultatai neištrinami.
- **403:** svetima klasė.
- **404:** klasės nėra.

### POST /api/classes/join
- **Kas gali:** `Mokinys`.
- **Prašymas:** `{ "code": "AB12CD", "confirmChange": false }`
- **Atsakymas 200:** `{ "classId": "...", "className": "..." }`
- **404:** kodas neegzistuoja arba kitokio formato nei 6 didžiosios raidės ar skaičiai.
- **409:** mokinys jau priklauso kitai klasei, o `confirmChange` yra `false`. Atsakyme grąžinamas dabartinės klasės pavadinimas.

### GET /api/classes/{id}/students
- **Kas gali:** `Mokytojas`, tik savo klasė.
- **Atsakymas 200:** `[ { "studentId": "...", "userName": "...", "completedLevels": 3 } ]`
- **403:** svetima klasė.
- **404:** klasės nėra.

### GET /api/classes/{id}/students/{studentId}/results
- **Kas gali:** `Mokytojas`, tik savo klasės mokinys.
- **Atsakymas 200:** `[ { "levelId": 1, "levelTitle": "...", "bestScore": 70 } ]`
- **403:** svetima klasė.
- **404:** klasės nėra arba mokinys nėra šios klasės narys.

### GET /api/classes/{id}/summary
- **Kas gali:** `Mokytojas`, tik savo klasė.
- **Atsakymas 200:** `[ { "levelId": 1, "levelTitle": "...", "averageScore": 62.5 | null, "completedCount": 8 } ]`. Išdėstyta nuo mažiausio vidurkio. `averageScore` yra `null`, jei lygio nebaigė nė vienas narys.
- **403:** svetima klasė.
- **404:** klasės nėra.

### DELETE /api/classes/{id}/students/{studentId}
- **Kas gali:** `Mokytojas`, tik savo klasė.
- **Atsakymas 204.** Mokinio rezultatai neištrinami.
- **403:** svetima klasė.
- **404:** klasės nėra arba mokinys nėra šios klasės narys.

---

## Administravimas (014, 015)

### GET /api/admin/levels
- **Kas gali:** `Administratorius`.
- **Atsakymas 200:** lygių sąrašas su situacijomis ir variantais: `[ { "id": 1, "orderNo": 1, "title": "...", "description": "...", "startState": 60, "startSpeciesCount": 40, "situations": [ { "id": "...", "text": "...", "options": [ { "id": "...", "text": "...", "stateEffect": -10, "speciesEffect": -2 } ] } ] } ]`

### POST /api/admin/levels
- **Kas gali:** `Administratorius`.
- **Prašymas:** tokia pat struktūra kaip aukščiau, be `id`.
- **Atsakymas 201:** `{ "id": 1 }`
- **400:** eilės numeris ne sveikas skaičius nuo 1; nėra nė vienos situacijos; situacijoje ne 2–4 variantai; pradinė būklė ne 0–100; varianto poveikis būklei ne −50…50; poveikis rūšių skaičiui ne −10…10; tuščias pavadinimas.
- **409:** toks eilės numeris jau naudojamas.

### PUT /api/admin/levels/{id}
- **Kas gali:** `Administratorius`.
- **Prašymas:** kaip `POST`.
- **Atsakymas 200:** `{ "id": 1 }`. Išsaugoti rezultatai nekeičiami.
- **400, 409:** kaip `POST`.
- **404:** lygio nėra.

### DELETE /api/admin/levels/{id}
- **Kas gali:** `Administratorius`.
- **Atsakymas 204.** Lygis tampa neaktyvus, rezultatai išlieka, pradėtus bandymus galima užbaigti.
- **404:** lygio nėra.

### GET /api/admin/users
- **Kas gali:** `Administratorius`.
- **Atsakymas 200:** `[ { "id": "...", "userName": "...", "email": "...", "role": "...", "isBlocked": false } ]`

### POST /api/admin/users/{id}/block
- **Kas gali:** `Administratorius`.
- **Atsakymas 204.**
- **404:** naudotojo nėra.
- **409:** bandoma užblokuoti save.

### POST /api/admin/users/{id}/unblock
- **Kas gali:** `Administratorius`.
- **Atsakymas 204.**
- **404:** naudotojo nėra.

### DELETE /api/admin/users/{id}
- **Kas gali:** `Administratorius`.
- **Atsakymas 204.** Ištrinami to naudotojo rezultatai ir bandymai. Mokytojo atveju – ir jo klasės (mokinių rezultatai išlieka). Mokinys pašalinamas iš klasės.
- **404:** naudotojo nėra.
- **409:** bandoma ištrinti save arba paskutinį administratorių.

---

## Visoms apsaugotoms užklausoms

- **401** – jei naudotojas neprisijungęs.
- **403** – jei rolė neatitinka, pranešimas „Prieiga uždrausta“.
- Visi administratoriaus veiksmai įrašomi į veiksmų žurnalą.
