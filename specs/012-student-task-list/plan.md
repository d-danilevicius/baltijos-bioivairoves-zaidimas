# Implementation Plan: Student task list

**Feature:** `specs/012-student-task-list/spec.md`
**Branch:** `012-student-task-list`
**Status:** draft
**Depends on features:** 003-access-control, 007-class-create-and-delete, 008-join-class, 010-gbif-task-generation, 011-teacher-tasks, 024-page-styling

## Summary

A student opens the task list (E04) and sees the tasks of the current class in three difficulty groups with the status and the label of each task. Group 1 is unlocked, the next groups are locked until the student earns the points (VT-014); the points and the status “done” come with features 014–016, so until then the unlocked tasks are “not done”. The button “Tęsti” opens the first unfinished task of an unlocked group. A student without a class, or with a class without tasks, sees the text from `docs/ux.md`. E04 becomes the starting screen of a student and the screen after joining a class. The server returns only the identifier, the type, the label and the status of a task.

## Technical Context

- **Language and version:** C# with .NET 10 (server); TypeScript (frontend).
- **Libraries:** as selected in `docs/architecture.md` (section 1). No new library. The mapping from a task to the list item is trivial (four fields) and uses the label method of feature 011; AutoMapper is not added for it.
- **Frontend:** React with Vite, React Router and Tailwind CSS.
- **Storage:** PostgreSQL through Entity Framework Core; the tables `ClassMemberships` and `ClassTasks` are read; no schema change and no migration.
- **Testing:** xUnit (unit and integration, PostgreSQL in Testcontainers), Vitest and React Testing Library.
- **Target platform:** web application in current Chrome, Firefox, Edge and Safari; screens from 360 px width.
- **Constraints:** HTTPS only; the endpoint only reads; no secret is stored in the repository.

## Structure and Main Components

- Server:
  - `Entities/StudentTaskStatus.cs` – new enum: `NotDone`, `Locked` (the value `Done` is added by feature 014).
  - `DTOs/Responses/StudentTaskResponse.cs` – identifier, type, label, status (no title, question, answer or options, FR-003).
  - `DTOs/Responses/StudentTaskGroupResponse.cs` – the group number (1–3), whether it is unlocked and its tasks.
  - `DTOs/Responses/StudentTaskListResponse.cs` – whether the student has a class, the groups that have tasks and the identifier of the task to continue from (empty when there is none).
  - `Services/ClassTaskExtensions.cs` – the label rule of feature 011 is split into `ToLabel()`, used by `ToResponse()` and by the student list (DRY: one place for “GBIF” / “Mokytojo užduotis”).
  - `Services/TaskGroupUnlocking.cs` – the rule “which groups are unlocked” (VT-014): group 1 is unlocked; a next group is unlocked when the previous group has no tasks; otherwise it is locked while the student has no points. Feature 016 replaces the second part with the 60 % rule.
  - `Services/IStudentTaskService.cs`, `Services/StudentTaskService.cs` – finds the current class of the student (membership without an end date), loads its tasks, builds the groups, the statuses and the task to continue from; a student without a class gets a response without groups.
  - `Services/ServiceCollectionExtensions.cs` – registers `IStudentTaskService`.
  - `Controllers/StudentTaskController.cs` – `GET api/my/tasks`, policy `Student`; calls the service and returns 200.
- Frontend:
  - `types/studentTask.ts` – `StudentTaskStatus`, `StudentTask`, `StudentTaskGroup`, `StudentTaskList`.
  - `api/studentTaskApi.ts` – `getMyTasks()` with a result type per status (200, 401, 403, failure).
  - `helpers/studentTaskMessage.ts` – the texts of E04: no class, the button “Prisijungti prie klasės”, no tasks, the locked group, “Tęsti”, the group title and the status names.
  - `helpers/startingScreen.ts` – the starting screen of a student is `/tasks` (E04).
  - `helpers/navigationLink.ts` – the student menu gets the link “Užduotys” to `/tasks`.
  - `components/studentTaskGroup.tsx` – one group: the title, the lock message or the list of its tasks with the status and the label.
  - `pages/StudentTasksPage.tsx` – E04: loading, error with “Bandyti dar kartą”, session expiry, access denied, the empty states, the groups, the message after joining a class and the button “Tęsti” (a link to `/tasks/{id}`).
  - `pages/JoinClassPage.tsx` – after joining the page opens `/tasks` with the message (instead of `/profile`).
  - `app.tsx` – route `/tasks` inside `ProtectedRoute allowedRoles={['Student']}`.

## Server Request Changes

- `GET /api/my/tasks` – returns the task list of the current class of the student (200 with `hasClass`, `groups` and `continueTaskId`; 401; 403 for a teacher and an administrator). A student without a class gets 200 with `hasClass = false`.

## Data Model Changes

- None. No migration. `Submission` does not exist yet; it is created by feature 014 together with the status “done”.

## Testing Plan

- Unit tests (`TaskGroupUnlocking`): group 1 is always unlocked; groups 2 and 3 are locked while the previous group has tasks and the student has no points; a group after an empty group is unlocked at once; all groups empty gives no groups.
- Unit tests (`StudentTaskService`): tasks of the current class are returned in the groups 1–3 with the statuses `NotDone` and `Locked`; labels “GBIF” and “Mokytojo užduotis”; a student without a class and a student whose membership ended get `hasClass = false` and no groups; a class without tasks gives `hasClass = true` and no groups; a group without tasks is not returned; tasks of another class are not returned; the task to continue from is the first task of group 1; it is empty when there are no unlocked tasks; a student who changed the class sees only the tasks of the new class.
- Integration tests: a student of a class gets 200 and the three groups; the response has no title, question, correct answer or options; a student without a class gets 200 and `hasClass = false`; a teacher gets 403; an administrator gets 403; a visitor gets 401; a task the teacher deleted disappears from the list.
- Frontend tests: E04 shows the groups, the statuses and the labels; the locked group shows the lock message and no task can be opened; “Tęsti” links to the address of the first unfinished task and is reached in no more than two clicks from E04; a student without a class sees the text and the button “Prisijungti prie klasės”; a class without tasks shows the text; the error state shows “Bandyti dar kartą”; the expired session opens the login; the message after joining a class is shown; the starting screen of a student is `/tasks`; the student menu has the link “Užduotys”; a teacher opening `/tasks` sees “Prieiga uždrausta”; the existing tests of `JoinClassPage`, `startingScreen` and `navigationLink` are updated.

## Technical Notes

- The class is taken from the membership of the logged-in student (`EndedAtUtc` is empty); the request has no class identifier.
- The tasks are read without change tracking and ordered by the group, the type and the question as in the teacher list; the question is used only for the order and is not returned.
- Until feature 014 there is no `Submission`: no task is done and the student has no points. Group 1 tasks are `NotDone`, a locked group has `Locked`. Feature 014 adds the status `Done` and the next task to continue from; feature 016 adds the points and replaces the locking rule in `TaskGroupUnlocking`.
- The button “Tęsti” links to `/tasks/{id}`, which feature 013 creates; before that the address shows “Puslapis nerastas” (Clarifications of the specification).
- The names of the statuses in the list are “Nepadaryta” (not done) and “Užrakinta” (locked); `docs/ux.md` gives only the English words.
- The message after joining a class moves from the profile to E04; the profile page keeps showing a message it receives.
- The frontend keeps the status 401 in the API wrapper; the page opens the login page with the session message through `SessionExpiredRedirect`.

## Constitution Check

- **1. Code quality:** complies. Each class has one responsibility and stays below 200 lines; the label rule exists in one place; React file names follow the naming rules.
- **2. Architecture:** complies. The controller only calls the service; the rules are in `StudentTaskService` and `TaskGroupUnlocking`; `ApplicationDbContext` is used directly (no Repository); DTOs are used at the API boundary.
- **3. Testing:** complies. FR-033, FR-037, FR-049, FR-001–FR-004, NFR-009, VT-014 and VT-026 are covered by the tests in the testing plan, including the authorization tests.
- **4. Security:** complies. The role is checked on the server; the class comes from the session, not from the request; the response has no question or answer; no secret is stored in Git.
- **5. Version control:** complies. The work is done in branch `012-student-task-list`, created from an updated `main`, and enters through a Pull Request.
- **6. Documentation:** complies. The specification is written before the code; README and `docs/` are updated.
- **7. AI assistance:** complies. The code is reviewed and tested by the student; the Pull Request gets a separate AI review.
- **8. Simplicity:** complies. No new libraries and no schema change; one service, one small rule class; the locking rule has only what the current data allows (YAGNI).

**No exceptions. The plan complies with the constitution.**
