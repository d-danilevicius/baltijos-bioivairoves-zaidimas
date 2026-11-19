# Tasks: Student task list

**Specification:** `specs/012-student-task-list/spec.md`
**Plan:** `specs/012-student-task-list/plan.md`
**Depends on features:** 003-access-control, 007-class-create-and-delete, 008-join-class, 010-gbif-task-generation, 011-teacher-tasks, 024-page-styling
**Tags:** `[US1]` – belongs to user story 1; no tag – setup or verification task.

Do the tasks in order. Each phase depends on the previous one. Tests are written before the implementation.

## Data

- [ ] T001 Add the enum `StudentTaskStatus` (`NotDone`, `Locked`); there is no schema change and no migration – `backend/src/Project.Api/Entities/StudentTaskStatus.cs`

## Tests (written first) (depends on: Data)

- [ ] T002 [US1] Unit tests of `TaskGroupUnlocking`: group 1 is unlocked, groups 2 and 3 are locked while the previous group has tasks, a group after an empty group is unlocked at once, no tasks gives no groups – `backend/tests/Project.UnitTests/StudentTasks/TaskGroupUnlockingTests.cs`
- [ ] T003 [US1] Unit tests of `StudentTaskService`: the groups and the statuses, the labels, a student without a class, an ended membership, a class without tasks, a group without tasks is not returned, tasks of another class, the task to continue from, a changed class – `backend/tests/Project.UnitTests/StudentTasks/StudentTaskServiceTests.cs`
- [ ] T004 [US1] Integration tests of `GET /api/my/tasks`: a student gets 200 and the groups, no title, question, correct answer or options in the response, a student without a class gets `hasClass = false`, a teacher 403, an administrator 403, a visitor 401, a deleted task disappears – `backend/tests/Project.IntegrationTests/StudentTasks/StudentTaskEndpointTests.cs`
- [ ] T005 [US1] Frontend tests of E04: the groups, the statuses and the labels, the locked group, “Tęsti” and the two clicks to the task, no class, no tasks, the error state, the expired session, the message after joining a class, a teacher sees “Prieiga uždrausta” – `frontend/src/tests/StudentTasksPage.test.tsx`
- [ ] T006 [US1] Update the tests of the starting screen, the student menu and the screen after joining a class – `frontend/src/tests/startingScreen.test.ts`, `frontend/src/tests/navigationLink.test.ts`, `frontend/src/tests/JoinClassPage.test.tsx`, `frontend/src/tests/JoinClassAccess.test.tsx`

## Implementation (depends on: Tests)

- [ ] T007 [US1] Create the response DTOs `StudentTaskResponse`, `StudentTaskGroupResponse` and `StudentTaskListResponse` – `backend/src/Project.Api/DTOs/Responses/`
- [ ] T008 [US1] Split `ToLabel()` out of `ClassTaskExtensions.ToResponse()` – `backend/src/Project.Api/Services/ClassTaskExtensions.cs`
- [ ] T009 [US1] Implement `TaskGroupUnlocking` – `backend/src/Project.Api/Services/TaskGroupUnlocking.cs`
- [ ] T010 [US1] Implement `IStudentTaskService` and `StudentTaskService` and register them – `backend/src/Project.Api/Services/IStudentTaskService.cs`, `backend/src/Project.Api/Services/StudentTaskService.cs`, `backend/src/Project.Api/Services/ServiceCollectionExtensions.cs`
- [ ] T011 [US1] Add `StudentTaskController` with `GET api/my/tasks` – `backend/src/Project.Api/Controllers/StudentTaskController.cs`
- [ ] T012 [US1] Add the types and the API call `getMyTasks` – `frontend/src/types/studentTask.ts`, `frontend/src/api/studentTaskApi.ts`
- [ ] T013 [US1] Add the texts of E04, change the starting screen of a student and add the link “Užduotys” to the student menu – `frontend/src/helpers/studentTaskMessage.ts`, `frontend/src/helpers/startingScreen.ts`, `frontend/src/helpers/navigationLink.ts`
- [ ] T014 [US1] Build the component of a group – `frontend/src/components/studentTaskGroup.tsx`
- [ ] T015 [US1] Build `StudentTasksPage` (E04), open `/tasks` after joining a class and add the route `/tasks` – `frontend/src/pages/StudentTasksPage.tsx`, `frontend/src/pages/JoinClassPage.tsx`, `frontend/src/app.tsx`

## Verification (depends on: Implementation)

- [ ] T016 Run `dotnet build backend/Project.slnx`, `dotnet test backend/Project.slnx`, `npm run lint`, `npm run format:check`, `npm test` and `npm run build`
- [ ] T017 Update README (the endpoint, the student task list screen), `docs/architecture.md` (the endpoint and the starting screen) and check the OpenAPI document
- [ ] T018 Review `git diff` and open the Pull Request with screenshots and the separate AI review evidence
