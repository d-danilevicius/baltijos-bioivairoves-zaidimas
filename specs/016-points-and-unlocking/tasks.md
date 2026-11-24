# Tasks: Points and group unlocking

**Specification:** `specs/016-points-and-unlocking/spec.md`
**Plan:** `specs/016-points-and-unlocking/plan.md`
**Depends on features:** 003-access-control, 012-student-task-list, 013-observation-card, 014-task-completion
**Tags:** `[US1]` = User Story 1 (points and group unlocking)

## Data

- [x] T001 [US1] Add the property `Points` to the entity `Submission` – `backend/src/Project.Api/Entities/Submission.cs`
- [x] T002 [US1] Create the EF Core migration `AddSubmissionPoints` and check that it only adds the column `Points` (integer, not null, default 0) – `backend/src/Project.Api/Migrations/`

## Tests (written first)

- [x] T003 [US1] Unit tests of `ClassTask`: 10, 20 and 30 points for a correct answer in groups 1, 2 and 3; 0 points for an incorrect one; letter case and spaces at the beginning and the end are ignored; inner spaces are kept (a different answer); „7“ is correct for 7 and „07“ is not – `backend/tests/Project.UnitTests/Submissions/ClassTaskPointsTests.cs`
- [x] T004 [US1] Unit tests of `StudentProgress` (they replace `TaskGroupUnlockingTests`): group 1 is unlocked without points; exactly 60 % unlocks and just below 60 % does not; group 3 depends on group 2; a group after an empty group is unlocked; a deleted task does not count; `GroupUnlockedBy` reports the unlocked group only when the submission crosses the threshold, and nothing for the last group or an already unlocked group; the maximum changes when a task is added – `backend/tests/Project.UnitTests/StudentTasks/StudentProgressTests.cs`, `backend/tests/Project.UnitTests/StudentTasks/TaskGroupUnlockingTests.cs` (deleted)
- [x] T005 [US1] Unit tests of `SubmissionService`: the points 10/20/30 and 0 are saved with the answer; `UnlockedGroup` is set when the submission reaches 60 % and is empty otherwise; the response has the correct answer; the failed checks (locked, other class, second submission, wrong option) save nothing – `backend/tests/Project.UnitTests/Submissions/SubmissionServiceTests.cs`
- [x] T006 [US1] Unit tests of `StudentTaskService`: done tasks have points; a group has the sum and the maximum; a locked group becomes unlocked by points; an empty previous group does not lock the next one; the points of another student are not used; update the tests that used the old rule – `backend/tests/Project.UnitTests/StudentTasks/StudentTaskServiceTests.cs`
- [x] T007 [US1] Unit tests of `StudentTaskDetailsService`: the correct answer and the points are in the submission only; a task without a submission has none; update the locked-task tests to the new rule – `backend/tests/Project.UnitTests/ObservationCards/StudentTaskDetailsServiceTests.cs`
- [x] T008 [US1] Integration tests: a correct answer gives 201 with 10, 20 or 30 points and the correct answer; an incorrect answer gives 0; the points are in the database when the response is returned and the same after a new login; the time of the POST is below 1 second; after enough points `GET /api/my/tasks` shows the next group unlocked and the POST returned its number; below 60 % it stays locked; the correct answer is absent in `GET /api/my/tasks/{id}` before the submission and present after it; two students have separate points; a teacher and an administrator get 403 and a visitor 401 on the three student endpoints; update the tests that used the old rule – `backend/tests/Project.IntegrationTests/Submissions/SubmissionPointsEndpointTests.cs`, `backend/tests/Project.IntegrationTests/Submissions/SubmissionEndpointTests.cs`, `backend/tests/Project.IntegrationTests/StudentTasks/StudentTaskEndpointTests.cs`
- [x] T009 [US1] Frontend tests of `SubmissionResult` on the task screen: the points, the correct answer, the submitted data with the label; the unlock message only when the group number comes; “Kita užduotis” opens the next task and is not shown when there is none or the call fails; “Į užduočių sąrašą” opens the list; a reopened submitted task shows the saved points – `frontend/src/tests/SubmissionResult.test.tsx`, `frontend/src/tests/StudentTaskSubmission.test.tsx`
- [x] T010 [US1] Frontend tests of the task list: the points of done tasks and of groups as text („10 tšk.“, „30 iš 50“) and the locked message – `frontend/src/tests/StudentTasksPage.test.tsx`

## Implementation (depends on: Tests)

- [x] T011 [US1] Add `PointsOfGroup` and `PointsFor` to `ClassTask` – `backend/src/Project.Api/Entities/ClassTask.cs`
- [x] T012 [US1] Create `StudentProgress` and delete `TaskGroupUnlocking` – `backend/src/Project.Api/Services/StudentProgress.cs`, `backend/src/Project.Api/Services/TaskGroupUnlocking.cs` (deleted)
- [x] T013 [US1] Add the new properties to the responses (`SubmissionResponse`, `StudentTaskResponse`, `StudentTaskGroupResponse`) and update the comment of `StudentTaskDetailsResponse` – `backend/src/Project.Api/DTOs/Responses/`
- [x] T014 [US1] Use `StudentProgress` in `StudentTaskAccess` and `StudentTaskService` (points of tasks and groups) and return the correct answer inside the submission in `StudentTaskDetailsService` – `backend/src/Project.Api/Services/StudentTaskAccess.cs`, `backend/src/Project.Api/Services/StudentTaskService.cs`, `backend/src/Project.Api/Services/StudentTaskDetailsService.cs`
- [x] T015 [US1] Calculate and save the points and the unlocked group in `SubmissionService` and update `SubmissionExtensions` – `backend/src/Project.Api/Services/SubmissionService.cs`, `backend/src/Project.Api/Services/SubmissionExtensions.cs`
- [x] T016 [US1] Add the types and the texts of the points, the correct answer, the unlock message and the buttons – `frontend/src/types/submission.ts`, `frontend/src/types/studentTask.ts`, `frontend/src/helpers/submissionMessage.ts`, `frontend/src/helpers/studentTaskMessage.ts`
- [x] T017 [US1] Create the hook `useNextTaskId` – `frontend/src/hooks/useNextTaskId.ts`
- [x] T018 [US1] Change `SubmissionResult` (E07) and `StudentTaskGroup` (E04) – `frontend/src/components/submissionResult.tsx`, `frontend/src/components/studentTaskGroup.tsx`

## Verification

- [x] T019 Run `dotnet build backend/Project.slnx`, `dotnet test backend/Project.slnx`, `dotnet format` check, `npm run lint`, `npm run format:check` (only on the changed files), `npm test` and `npm run build`
- [ ] T020 Open E04 and E07 in the browser (a correct and an incorrect answer, the points, the correct answer, the unlock message, “Kita užduotis”, “Į užduočių sąrašą”, a locked group before and after 60 %, the task list with points) and take the screenshots
- [ ] T021 Update README (the points rule, the unlocking rule, the new response fields and the migration) and `docs/architecture.md` if the endpoint description changes, and check the OpenAPI document
- [ ] T022 Review `git diff` and open the Pull Request with screenshots and the separate AI review evidence
