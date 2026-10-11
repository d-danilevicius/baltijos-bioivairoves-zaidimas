# Projektavimo artefaktų peržiūra

Peržiūrėti 15 funkcijų planai (`plan.md`). Atskirų `research.md`, `data-model.md` ir `quickstart.md` failų nėra: jų turinys įtrauktas į planus. Žemiau pateiktas bendras duomenų modelis ir radiniai. Visi sprendimai laikomi pasiūlymais, kol studentas juos patvirtins.

## Bendras duomenų modelis

- **Users** – Id, Email (unikalus), UserName (unikalus), PasswordHash, Role, IsBlocked.
- **Classes** – Id, TeacherId → Users, Name, Code (unikalus), CreatedAt.
- **ClassMemberships** – Id, StudentId → Users (unikalus), ClassId → Classes, JoinedAt.
- **Levels** – Id, OrderNo (unikalus), Title, Description, StartState, StartSpeciesCount, IsActive.
- **Situations** – Id, LevelId → Levels, OrderNo, Text.
- **DecisionOptions** – Id, SituationId → Situations, Text, StateEffect, SpeciesEffect.
- **LevelAttempts** – Id, StudentId → Users, LevelId → Levels, StartedAt, FinishedAt, Status, CurrentState, SpeciesCount, FinalScore.
- **Choices** – Id, AttemptId → LevelAttempts, SituationId → Situations, OptionId → DecisionOptions; unikalus (AttemptId, SituationId).
- **LevelResults** – Id, StudentId → Users, LevelId → Levels, BestScore, UpdatedAt; unikalus (StudentId, LevelId).
- **ActionLog** – Id, Time, UserId, Action, Result.

Ryšių „daug su daug“ nėra. Naudotojas ir klasė siejami per `ClassMemberships`, bet dėl unikalaus `StudentId` mokinys vienu metu priklauso tik vienai klasei.

## Radiniai

1. **Trūko apsaugos nuo užklausų klastojimo (CSRF).** *Ištaisyta:* 002 plane pridėtas apsaugos raktas kiekvienai keičiančiai užklausai.
2. **Du kito lygio atrakinimo šaltiniai.** *Ištaisyta:* 008 plane nurodyta, kad atrakinimas lieka pagal baigtus bandymus, o `LevelResults` skirta tik geriausiam rezultatui.
3. **Funkcijų eilė.** 005 reikia lygių, o jų kūrimo funkcija (014) paskutinė. *Ištaisyta:* pradiniai 3 lygiai įrašomi migracijoje.
4. **`ClassMemberships` galima supaprastinti** stulpeliu `ClassId` prie mokinio. *Palikta:* lentelė atitinka esybę „Klasės narystė“. Pasirinkimą patvirtina studentas.
5. **`LevelResults` galima apskaičiuoti iš `LevelAttempts`.** *Palikta:* lentelė atitinka esybę „Lygio rezultatas“. Pasirinkimą patvirtina studentas.
6. **`LevelAttempts.CurrentState` ir `SpeciesCount` galima perskaičiuoti iš pasirinkimų.** *Palikta:* dėl ribos 0–100 perskaičiuoti brangiau. Pasirinkimą patvirtina studentas.

## Būsena

Radiniai 1–3 ištaisyti planuose. Radiniai 4–6 laukia studento patvirtinimo.
