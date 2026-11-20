# Tasks: Task completion

**Specification:** `specs/014-task-completion/spec.md`
**Plan:** `specs/014-task-completion/plan.md`
**Depends on features:** 003-access-control, 006-confirmation-dialog, 012-student-task-list, 013-observation-card, 024-page-styling
**Tags:** `[US1]` = User Story 1 (task completion)

## Data

- [x] T001 [US1] Create the entity `Submission` and add the value `Done` to `StudentTaskStatus` – `backend/src/Project.Api/Entities/Submission.cs`, `backend/src/Project.Api/Entities/StudentTaskStatus.cs`
- [x] T002 [US1] Create `SubmissionConfiguration` (limits, the unique index on the task and the student, `SetNull` for the task, cascade for the student) and add `DbSet<Submission>` – `backend/src/Project.Api/Data/Configurations/SubmissionConfiguration.cs`, `backend/src/Project.Api/Data/ApplicationDbContext.cs`
- [x] T003 [US1] Create the EF Core migration `CreateSubmissionsTable` and check that it only creates the table, the index and the two foreign keys – `backend/src/Project.Api/Migrations/`

## Tests (written first)

- [x] T004 [US1] Unit tests of `CreateSubmissionRequest`: an empty answer, an empty evidence (also only spaces), an answer over 100 and an evidence over 500 characters are invalid; spaces at the beginning and the end are removed before the check; a request at the limits is valid – `backend/tests/Project.UnitTests/Submissions/CreateSubmissionRequestValidationTests.cs`
- [x] T005 [US1] Unit tests of `SubmissionService`: a valid submission is saved and returned; an answer that is not an option is rejected; a task without options accepts a text answer; a second submission gives `AlreadySubmitted` and the first answer stays; a locked task, a task of another class, a missing task, a student without a class and a student whose membership ended give the right outcome and save nothing; a student who changed the class cannot submit a task of the old class – `backend/tests/Project.UnitTests/Submissions/SubmissionServiceTests.cs`
- [x] T006 [US1] Unit tests of `StudentTaskService`: a submitted task has the status `Done`; “Tęsti” skips done tasks; when all tasks of unlocked groups are done there is nothing to continue – `backend/tests/Project.UnitTests/StudentTasks/StudentTaskServiceTests.cs`
- [x] T007 [US1] Unit tests of `StudentTaskDetailsService`: the response has the student’s submission after submitting and none before; the submission of another student is not returned – `backend/tests/Project.UnitTests/ObservationCards/StudentTaskDetailsServiceTests.cs`
- [x] T008 [US1] Integration tests of `POST /api/my/tasks/{taskId}/submissions`: 201 and the submission in the database; the label in the JSON; a second submission 409 and the saved answer unchanged (also two requests at the same time); 400 with errors per field; a locked task 403 and nothing saved; another class’s task and a missing task 404; a task deleted by the teacher 404; the submission of a deleted task stays with an empty task; a teacher 403; an administrator 403; a visitor 401; a request without the CSRF token is rejected; `GET /api/my/tasks` shows `Done` – `backend/tests/Project.IntegrationTests/Submissions/SubmissionEndpointTests.cs`, `backend/tests/Project.IntegrationTests/Classes/ClassTestClient.cs`
- [x] T009 [US1] Frontend tests of `validateSubmission`: the answer and the evidence limits, the messages – `frontend/src/tests/validateSubmission.test.ts`
- [x] T010 [US1] Frontend tests of `AnswerForm`: the options or the text field by the task data; the validation messages; the counter “0 / 500”; the note; the saving failure keeps the entered data and “Bandyti dar kartą” sends again – `frontend/src/tests/AnswerForm.test.tsx`
- [x] T011 [US1] Frontend tests of the task screen: submitting shows the result with the label “Mokinio pateikti duomenys”; 400 errors next to the fields; 404 “Puslapis nerastas”; 403 “Prieiga uždrausta”; the expired session; a submitted task shows the saved answer instead of the form; the leave dialog is shown for entered data (“Išeiti” leaves, “Likti” stays) and not for an empty form or after submitting – `frontend/src/tests/StudentTaskSubmission.test.tsx`
- [x] T012 [US1] Update the existing tests for the new property `submission`, the status `Done` and the data router: the task list shows “Padaryta” – `frontend/src/tests/StudentTaskPage.test.tsx`, `frontend/src/tests/StudentTasksPage.test.tsx`

## Implementation (depends on: Tests)

- [x] T013 [US1] Create the request and response DTOs `CreateSubmissionRequest` and `SubmissionResponse` and add `Submission` to `StudentTaskDetailsResponse` – `backend/src/Project.Api/DTOs/Requests/CreateSubmissionRequest.cs`, `backend/src/Project.Api/DTOs/Responses/SubmissionResponse.cs`, `backend/src/Project.Api/DTOs/Responses/StudentTaskDetailsResponse.cs`
- [x] T014 [US1] Create `StudentTaskAccess` and use it in `StudentTaskDetailsService`, which also loads the student’s submission – `backend/src/Project.Api/Services/StudentTaskAccess.cs`, `backend/src/Project.Api/Services/StudentTaskDetailsService.cs`
- [x] T015 [US1] Create `SubmissionExtensions`, `SubmissionOutcome`, `SubmissionResult`, `ISubmissionService` and `SubmissionService` and register the service – `backend/src/Project.Api/Services/`
- [x] T016 [US1] Set the status `Done` and the first task to continue from in `StudentTaskService` – `backend/src/Project.Api/Services/StudentTaskService.cs`
- [x] T017 [US1] Create the controller with `POST api/my/tasks/{taskId:guid}/submissions` – `backend/src/Project.Api/Controllers/StudentSubmissionController.cs`
- [x] T018 [US1] Add the types and the API call `submitAnswer` and the status `Done` – `frontend/src/types/submission.ts`, `frontend/src/types/studentTaskDetails.ts`, `frontend/src/types/studentTask.ts`, `frontend/src/api/submissionApi.ts`
- [x] T019 [US1] Add the texts and the validation – `frontend/src/helpers/submissionMessage.ts`, `frontend/src/helpers/validateSubmission.ts`, `frontend/src/helpers/studentTaskMessage.ts`
- [x] T020 [US1] Build the components `OptionChoice`, `AnswerForm` and `SubmissionResult` – `frontend/src/components/optionChoice.tsx`, `frontend/src/components/answerForm.tsx`, `frontend/src/components/submissionResult.tsx`
- [x] T021 [US1] Create `useLeaveConfirmation` and change the router to a data router – `frontend/src/hooks/useLeaveConfirmation.ts`, `frontend/src/main.tsx`
- [x] T022 [US1] Change `StudentTaskPage` to show the form, the result and the leave dialog – `frontend/src/pages/StudentTaskPage.tsx`

## Verification

- [x] T023 Run `dotnet build backend/Project.slnx`, `dotnet test backend/Project.slnx`, `dotnet format` check, `npm run lint`, `npm run format:check` (only on the changed files), `npm test` and `npm run build`
- [x] T024 Open E06 and E07 in the browser (a task with options, a task without options, the validation errors, the result, the saved answer after a second visit, the leave dialog, the task list with “Padaryta”) and take the screenshots
- [x] T025 Update README (the endpoint, the answer form and the migration) and `docs/architecture.md` (the endpoint) and check the OpenAPI document
- [ ] T026 Review `git diff` and open the Pull Request with screenshots and the separate AI review evidence
