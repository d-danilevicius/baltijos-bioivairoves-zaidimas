# Independent Work 1 – Project Vision, Scope and Requirements Specification

## Objective

Define what application will be developed, why it is needed, who will use it, and what the system must do. At this stage, focus primarily on the problem and required behaviour, not implementation details. The main output is the initial project requirements documentation.

## 1. Define the Project

Create a clear project title and a short project description of approximately 100–200 words.

The description must explain **what the application is, what problem it solves, who will use it, and what main value it provides**. Do not use the technology stack as the project purpose.

**Example:** Instead of “This is an application made using .NET and React,” write “The application helps university students organize assignment deadlines, track progress, and identify overdue work.”

**Project title:** Baltijos biologinės įvairovės iššūkių žaidimas (Baltic Biodiversity Challenge Game)

**Project description:**

This is an educational field game for schools, nature centres and regional parks. A teacher selects a location in Lithuania and a season, and the application retrieves biodiversity observations near that location from the open GBIF database. From these observations the application creates tasks: recognise a species group, find features of a common local species, compare seasonal observations, or answer a question from an observation card. The teacher can also create their own task, for example about the school surroundings. Students submit their own evidence, earn points and unlock harder tasks. In this way they learn to recognise the nature around them from real observations, not only from a textbook. The game has three roles: student, teacher and administrator. The application clearly separates confirmed external observations from student-submitted data and does not encourage collecting or disturbing protected species.

**Problem solved:**

In nature lessons, biodiversity topics are often presented theoretically, so students find it hard to connect them to the nature near their school. The game solves this problem: tasks are based on real observations from the place chosen by the teacher, and students learn to recognise species groups, features of local species and seasonal changes.

**Who will use it:**

Groups of students in schools, nature centres and regional parks will use the game independently, during lessons or field activities, and teachers will use it as an additional teaching tool.

**Main value:**

Students learn from real, verifiable observations, and the teacher does not have to search for and prepare the material. The game encourages observing nature without disturbing it.

## 2. Define the Problem Statement

Describe the problem that currently exists before proposing the implementation. Explain what users have difficulty doing, why the problem matters, how the problem is currently handled, and which limitations exist in the current approach.

The problem statement should be concrete enough that another person can understand why the application is worth building.

**Example:** Students currently track deadlines across messages, learning platforms, and personal notes. Because the information is fragmented, tasks can be overlooked and progress is difficult to see in one place.

**Problem statement:**

In schools and nature centres, biodiversity topics are often taught from textbooks and slides. Students rarely see which species people actually observe near their school or park and how the observations change with the season. As a result, knowledge stays detached from local nature, and field activities lack a clear task structure.

Real observation data exists in open databases such as GBIF, but it is hard for a teacher to use: the teacher has to search for observations by place and time, select suitable ones and turn them into tasks. In addition, field tasks may encourage collecting or disturbing protected species, and student-submitted data may be confused with confirmed observations. A tool is needed that creates tasks from real observations, protects nature and clearly separates confirmed data from student-submitted data.

## 3. Identify Target Users and User Roles

Identify the people who will use the application. For every user type describe the role name, main goal, actions the user needs to perform, data the role may access, and actions the role must not be allowed to perform.

Do not create separate roles if they have exactly the same permissions and responsibilities.

- Possible roles include Visitor, Registered User, Administrator, Manager, Teacher, or Student.

**Example:** Registered User: can create and manage only their own tasks. Administrator: can manage user accounts but does not automatically own users’ personal tasks.

**Visitor**

- **Main goal:** learn about the game and register or log in. A visitor is a person who is not logged in, not an account role.

- **Actions:** view the home page, register, log in.

- **Accessible data:** only the public home page.

- **Forbidden:** open tasks, classes or results.

**Student**

- **Main goal:** complete tasks based on real observations, submit evidence, earn points and track own progress.

- **Actions:** join a class with a code, open own class tasks, complete them, submit answers and evidence (text and photos), view own points and results.

- **Accessible data:** own profile, own class tasks, own answers, points and results.

- **Forbidden:** view or change other students’ data, change GBIF observations, tasks or points.

**Teacher**

- **Main goal:** use the game as a field activity and teaching tool and follow the progress of own class.

- **Actions:** create and manage own classes, choose a location in Lithuania and a season for a class, create own tasks for a class, view the points, answers, evidence and results of own class students, remove a student from a class.

- **Accessible data:** own profile, tasks of own classes, and only the points, answers, evidence and results of students of own classes.

- **Forbidden:** view data of other teachers’ classes, change GBIF observations or student points, manage accounts. Moderation of student evidence is not part of the first version.

**Administrator**

- **Main goal:** manage accounts and ensure that the system is used appropriately.

- **Actions:** view, block, unblock and delete accounts.

- **Accessible data:** account information.

- **Forbidden:** view or change student points, answers, evidence and results, play on behalf of other users. The administrator manages accounts but is not automatically the owner of their learning data.

## 4. Define Project Goals

Create 3–7 project goals. Goals must describe user or business outcomes, not implementation technologies.

A project goal should help later decide whether a proposed feature belongs in the project.

**Example:** Good: “Allow users to manage their personal tasks.” Bad: “Use PostgreSQL” or “Create five controllers.”

1.  **Teach students to recognise local species.** After the tasks, a student can recognise species groups and name features of a common local species.

2.  **Let the teacher prepare field tasks for a place and a season.** The teacher chooses a location in Lithuania and a season and gets tasks created from real observations of that place, or creates a task themselves, for example about the school surroundings.

3.  **Encourage students to support answers with evidence.** A student submits evidence, earns points and unlocks harder tasks.

4.  **Help the teacher follow class progress.** The teacher sees the points of each student and the tasks with the lowest class average.

5.  **Protect nature and keep the data reliable.** The application does not encourage collecting or disturbing protected species and clearly separates confirmed external observations from student-submitted data.

6.  **Ensure that every user sees only the data meant for them.** A student sees only own results, a teacher only those of own classes, and only the administrator manages accounts.

## 5. Define Project Scope

Create two clearly labelled sections: **In Scope** and **Out of Scope**. In Scope lists functionality that will be implemented during the project. Out of Scope explicitly lists functionality that will not be implemented in the current version.

Use the scope to prevent uncontrolled growth of the project during implementation.

**Example:** In Scope: registration, authentication, task creation, task status management. Out of Scope: mobile application, payment processing, real-time chat, external calendar synchronization.

**In Scope**

- Registration and login.

- Class creation by a teacher and joining a class with a code by students.

- Selection of a location in Lithuania and a season for a class by the teacher.

- Retrieval of observations from the GBIF Occurrence API.

- Tasks created by the teacher for a class (for example, about the school surroundings).

- Creation of four task types from observations: species group recognition, features of a common local species, comparison of seasonal observations, a question from an observation card.

- Submission of student answers and evidence (text and photos), points and unlocking of harder tasks.

- Clear separation of confirmed GBIF observations from student-submitted data.

- Not encouraging collecting or disturbing protected species: warnings and hiding of exact coordinates.

- Viewing of student and class results.

- Account management by the administrator.

**Out of Scope**

- Moderation of student evidence by the teacher (a possible thesis extension).

- Offline field mode (a possible thesis extension).

- Submission of citizen-science data to external systems (a possible thesis extension).

- Assigning separate tasks to individual students.

- Mobile application.

- Multiplayer mode, chat, social features.

- Payments and a shop.

- Integration with school registers or other platforms.

- Multi-language interface (Lithuanian only).

## 6. Define the MVP

Define the **Minimum Viable Product (MVP)** and categorize planned functionality as Must Have, Should Have, or Could Have.

All Must Have functionality must be completed before significant time is spent on optional functionality.

**Example:** Must Have: login and personal task management. Should Have: filtering by status. Could Have: theme customization.

**Definition:** the teacher chooses a location and a season for a class, the application creates tasks from GBIF observations (the teacher can also create own tasks), a student joins the class, completes tasks, submits evidence and earns points, the teacher sees the class results, and the administrator manages accounts.

**Must Have**

- Registration and login.

- Class creation and joining with a code.

- Selection of a location and a season for a class.

- Retrieval of observations from the GBIF Occurrence API.

- Four task types created from observations.

- Tasks created by the teacher.

- Submission of answers and evidence (text and photos), points, unlocking of harder tasks.

- Separation of confirmed GBIF observations from student-submitted data.

- Protection of protected species: warnings and hiding of exact coordinates.

- Viewing of student and class results.

- Account management by the administrator.

- Access separation by role: a student sees only own data, a teacher only that of own classes.

**Should Have**

- A short explanation after the answer with a link to the observation card.

- Ability to repeat a task.

**Could Have**

- Achievement badges.

- Sound effects and animations.

## 7. Write Functional Requirements

Create uniquely identified functional requirements using IDs such as FR-001, FR-002, and FR-003.

Each requirement must describe one system behaviour, be understandable without reading source code, be testable, and avoid vague words such as easy, fast, good, or modern unless the term is made measurable.

Use **MUST** for mandatory requirements. The initial project should normally contain approximately 10–30 functional requirements, depending on project size.

**Example:** FR-002: The system MUST prevent registration when the email address is already associated with another account.

User interface texts are in Lithuanian (NFR-011). Where a requirement quotes an interface text, the Lithuanian text is given in quotes and its English meaning in parentheses.

**Accounts and access**

- **FR-001:** The system MUST allow a visitor to register as a student or a teacher by providing an email address, a username, a password and a role.

- **FR-002:** The system MUST prevent registration when the email address is already associated with another account and show an error message.

- **FR-003:** The system MUST prevent registration when the username is already taken and show an error message.

- **FR-004:** The system MUST reject registration data that does not meet the limits of VT-003 and VT-030 and indicate which field is invalid.

- **FR-005:** The system MUST allow a registered user to log in by providing a correct email address and password.

- **FR-006:** The system MUST reject a login with incorrect credentials and show a general error message that does not indicate which field is incorrect.

- **FR-007:** The system MUST prevent a blocked user from logging in and show a message about the block.

- **FR-008:** The system MUST allow a logged-in user to log out.

- **FR-009:** The system MUST prevent a user who is not logged in from opening task or result screens.

- **FR-010:** The system MUST show a visitor who is not logged in a home page with the project description and the links “Prisijungti” (Log in) and “Registruotis” (Register).

- **FR-011:** The system MUST redirect a logged-in user who opens the home page to the user’s starting screen according to the role.

- **FR-012:** The system MUST allow a logged-in user to view own profile data: username, email, role and (for a student) class.

**Class and teacher functions**

- **FR-013:** The system MUST allow a teacher to create a class by providing a class name.

- **FR-014:** The system MUST allow a teacher to view the list of own classes.

- **FR-015:** The system MUST generate a unique 6-character join code for each class and show it to the teacher.

- **FR-016:** The system MUST allow a student to join a class by entering a valid class code.

- **FR-017:** The system MUST reject a non-existent class code and show an error message; the code is checked according to VT-030.

- **FR-018:** The system MUST allow a teacher to view the list of students of own class.

- **FR-019:** The system MUST allow a teacher to view the points and completed tasks of each student of own class.

- **FR-020:** The system MUST allow a teacher to view the answers and evidence submitted by students of own class.

- **FR-021:** The system MUST show a teacher the class average score of each task and the tasks ordered from the lowest average.

- **FR-022:** The system MUST allow a teacher to remove a student from own class.

- **FR-023:** The system MUST allow a teacher to delete own class.

- **FR-024:** The system MUST prevent a teacher from viewing other teachers’ classes and the results of their students.

**Observations and tasks**

- **FR-025:** The system MUST allow a teacher to choose a location in Lithuania (for example, the school surroundings) and a season for own class.

- **FR-026:** The system MUST retrieve from the GBIF Occurrence API biodiversity observations near the chosen location in the chosen season according to VT-011.

- **FR-027:** The system MUST create the task “Atpažink rūšių grupę” (Recognise the species group) from a retrieved observation.

- **FR-028:** The system MUST create the task “Rask dažnos vietinės rūšies požymių” (Find features of a common local species) from the retrieved observations of that location.

- **FR-029:** The system MUST create the task “Palygink sezoninius stebėjimus” (Compare seasonal observations) from observations of the same location in the chosen season and another season.

- **FR-030:** The system MUST create the task “Atsakyk į klausimą” (Answer a question) from an observation card.

- **FR-031:** The system MUST allow a teacher to create own task for own class by providing a title, a question, a correct answer and a difficulty group, for example about the school surroundings (VT-018).

- **FR-032:** The system MUST, before saving a teacher’s task, ask the teacher to confirm that the task does not require collecting, touching or disturbing organisms.

- **FR-033:** The system MUST label tasks created by a teacher “Mokytojo užduotis” (Teacher task) and never show them as GBIF observations.

- **FR-034:** The system MUST allow a teacher to delete a task the teacher created.

- **FR-035:** The system MUST allow a teacher to view the task list of own class, showing the type, difficulty group and label of each task.

- **FR-036:** The system MUST NOT create tasks and MUST show the teacher a message if GBIF returns fewer observations than VT-011 requires.

- **FR-037:** The system MUST show a student the tasks of own class arranged by difficulty groups (VT-013).

- **FR-038:** The system MUST allow a student to open an unlocked task.

- **FR-039:** The system MUST allow a student to submit an answer and text evidence in a task.

- **FR-040:** The system MUST allow a student to attach photos to the evidence according to VT-030.

- **FR-041:** The system MUST automatically award points for a submitted answer according to VT-016.

- **FR-042:** The system MUST unlock the next difficulty group when the student reaches the share of points set by VT-014.

- **FR-043:** The system MUST show on each observation card the label “Patvirtintas GBIF stebėjimas” (Confirmed GBIF observation), the dataset name, the licence and a link to the GBIF record.

- **FR-044:** The system MUST label data submitted by students “Mokinio pateikti duomenys” (Student-submitted data) and never show them as confirmed observations.

- **FR-045:** The system MUST show on each task a warning to observe nature from a distance, not to collect or disturb organisms and not to photograph people.

- **FR-046:** The system MUST NOT show a student the exact coordinates of an observation of a protected species (VT-020).

- **FR-047:** The system MUST NOT create tasks that require collecting, touching or disturbing organisms (VT-021).

**Progress and results**

- **FR-048:** The system MUST save the student’s points after each submitted task.

- **FR-049:** The system MUST allow a student, after logging out and logging in again, to continue from the first unfinished unlocked task.

- **FR-050:** The system MUST allow a student to view own points and the results of completed tasks.

- **FR-051:** The system MUST allow a student to repeat a completed task.

- **FR-052:** The system MUST prevent a student from viewing or changing the results of another student.

- **FR-053:** The system MUST ask the user for confirmation before the actions listed in VT-029.

**Administration**

- **FR-054:** The system MUST allow an administrator to view the list of accounts.

- **FR-055:** The system MUST allow an administrator to block an account.

- **FR-056:** The system MUST allow an administrator to unblock an account.

- **FR-057:** The system MUST allow an administrator to delete an account together with its results.

- **FR-058:** The system MUST prevent an administrator from viewing or changing student points, answers and evidence.

- **FR-059:** The system MUST prevent a student or a teacher from accessing administrator functions.

## 8. Write Non-Functional Requirements

Create non-functional requirements using IDs such as NFR-001, NFR-002, and NFR-003.

Consider security, usability, performance, maintainability, accessibility, reliability, compatibility, logging, and privacy. Whenever possible, state a measurable or verifiable expectation.

**Example:** NFR-002: The application MUST provide a usable layout on desktop and mobile screen sizes without requiring horizontal scrolling for normal page content.

**Security**

- **NFR-001:** The system MUST store passwords only as slow, password-specific hash values with a unique salt (using ASP.NET Core Identity), and MUST NOT store them in plain text or as reversibly encrypted values.

- **NFR-002:** The system MUST transfer data between the browser, the server and GBIF only over a secure HTTPS connection.

- **NFR-003:** The system MUST check the user’s role on the server before every protected action and, if the user has no permission, return the error “Prieiga uždrausta” (Access denied).

- **NFR-004:** The system MUST automatically log a user out after 30 minutes of inactivity.

**Privacy**

- **NFR-005:** The system MUST store only the following personal data: email address, username, role, class membership, student answers, evidence and photos, points and activity log entries; no other personal data.

- **NFR-006:** The system MUST remove location and other metadata (EXIF) from photos uploaded by students before saving them.

- **NFR-007:** The system MUST, when an account is deleted, remove the data related to it according to VT-008 and VT-009.

**Usability and accessibility**

- **NFR-008:** The system MUST show a suitable layout on desktop and phone screens (from 360 px width) without horizontal scrolling.

- **NFR-009:** The system MUST allow a new student who already belongs to a class to start the first task in no more than 3 clicks after logging in.

- **NFR-010:** The system MUST use a text-to-background contrast of at least 4.5:1.

- **NFR-011:** The system MUST show all texts and messages in Lithuanian.

- **NFR-012:** Every error message MUST state what failed and what the user can do next.

- **NFR-013:** The system MUST ensure that all interface controls are reachable by keyboard (Tab, Enter, Space) and that errors are indicated by text, not only by colour.

**Performance**

- **NFR-014:** The system MUST open any page that does not require a request to GBIF within no more than 3 seconds on a normal internet connection (at least 10 Mbit/s).

- **NFR-015:** The system MUST show the earned points within no more than 1 second after an answer is submitted.

- **NFR-016:** The system MUST create the class tasks within no more than 30 seconds after the location and season are confirmed.

- **NFR-017:** The system MUST withstand a load of 50 simultaneous users so that 95 % of requests are answered within no more than 3 seconds.

**Reliability**

- **NFR-018:** The system MUST save the student’s points even when the connection is lost immediately after the answer is submitted.

- **NFR-019:** The system MUST create a daily backup of the data and keep it for at least 7 days.

- **NFR-020:** If GBIF does not respond within 10 seconds or returns an error, the system MUST show the teacher an error message with a button “Bandyti dar kartą” (Try again) and MUST NOT create incomplete tasks.

- **NFR-021:** The system MUST store a copy of the GBIF observations used for tasks, so that the tasks do not change when GBIF data change.

**Compatibility**

- **NFR-022:** The system MUST work in the latest versions of the Chrome, Firefox, Edge and Safari browsers without additional plug-ins.

**Maintainability, quality and logs**

- **NFR-023:** The system MUST record in a log logins, failed logins and administrator actions (date, time, user, action).

- **NFR-024:** The system MUST NOT write passwords to logs.

- **NFR-025:** The system MUST have at least one automated test for each functional requirement.

- **NFR-026:** The system MUST automatically run tests and code checks for every pull request into the main branch.

**User interface behaviour**

- **NFR-027:** When a screen does not exist, the system MUST show the message “Puslapis nerastas” (Page not found) and a button “Į pradžią” (To the home page).

- **NFR-028:** Every screen that loads data from the server MUST show a loading state until the data are received, and an error message with a button “Bandyti dar kartą” (Try again) if the data could not be received.

- **NFR-029:** Every list or table that may contain no records MUST show an empty-state message.

## 9. Define Main Business Rules

Document rules that determine how the system behaves independently of a specific user interface. Business rules are especially important because they later become sources for validation and automated tests.

Use clear statements that identify who or what is affected by the rule.

**Example:** “Only the creator of a task may edit or delete that task.” Another example: “A booking end date must be later than its start date.”

**Accounts**

- **VT-001:** One email address may be assigned to only one account.

- **VT-002:** A username must be unique among all accounts.

- **VT-003:** A password must consist of 8–128 characters.

- **VT-004:** When registering, a user may choose only the student or the teacher role. The administrator role may be granted only by an administrator.

- **VT-005:** Every user has exactly one role: student, teacher or administrator.

- **VT-006:** A blocked user cannot log in until an administrator unblocks the account.

- **VT-007:** An administrator cannot block or delete their own account, and at least one administrator must always remain in the system.

- **VT-008:** When an account is deleted, all of its results, answers and photos are deleted with it.

- **VT-009:** When a teacher account is deleted, the teacher’s classes and their tasks are deleted, but student results remain. When a student account is deleted, the student is removed from the class and class averages are recalculated.

**Observations, tasks and game flow**

- **VT-010:** The GBIF tasks of one class are created for one location within the territory of Lithuania and one season, both chosen by the teacher of the class.

- **VT-011:** Only GBIF observations that have coordinates and a species name and are no farther than 10 km from the chosen location are used. Tasks are created only if at least 10 such observations are found.

- **VT-012:** Seasons are defined by months: spring – March to May, summer – June to August, autumn – September to November, winter – December to February.

- **VT-013:** There are four task types and three difficulty groups: group 1 – species group recognition and a question from an observation card; group 2 – features of a common local species; group 3 – comparison of seasonal observations.

- **VT-014:** The first difficulty group is unlocked for every student of the class. Each next group is unlocked when the student earns at least 60 % of the maximum possible points of the previous group.

- **VT-015:** A task is considered completed when the student has submitted an answer and evidence. A submitted answer cannot be changed.

- **VT-016:** A correct answer earns 10 points in a group 1 task, 20 in a group 2 task and 30 in a group 3 task; an incorrect answer earns 0 points. Correctness is determined from the observation card data, and in a teacher’s task from the correct answer set by the teacher.

- **VT-017:** When a task is repeated, the result of the task is the highest number of points achieved.

- **VT-018:** A teacher’s task consists of a title (1–100 characters), a question (up to 500 characters), a correct answer (up to 100 characters) and a difficulty group chosen by the teacher. An answer is considered correct if it matches the correct answer, ignoring letter case and spaces at the beginning and end. A teacher may create and delete only tasks of own classes. When a task is deleted, the points already earned by students remain.

**Data separation and nature protection**

- **VT-019:** A confirmed external observation is only an observation provided by the GBIF Occurrence API. Student-submitted data are always labelled as student-submitted and never become confirmed observations. Tasks created by a teacher are not GBIF observations and are labelled as teacher tasks.

- **VT-020:** A species is considered protected if GBIF gives it an IUCN Red List category of VU, EN or CR. The exact coordinates of such a species are not shown to a student; only the place name is shown.

- **VT-021:** No task, including one created by a teacher, may require collecting, catching, touching or disturbing organisms; only observation from a distance is allowed.

**Results**

- **VT-022:** A student may view only own results, and a teacher only the results of students of own classes. An administrator does not see student results. Nobody can change saved results.

**Classes**

- **VT-023:** A class join code is unique among all classes.

- **VT-024:** Only a teacher can create a class, and the owner of a class is the teacher who created it.

- **VT-025:** A student can belong to only one class at a time. When joining a new class, the previous membership ends.

- **VT-026:** A student without a class can log in and view the profile but does not see tasks until joining a class.

- **VT-027:** When a student is removed from a class or a class is deleted, the student’s results are not deleted, and the teacher no longer sees them.

- **VT-028:** The average result of a class task is calculated only from the best results of the current class members for that task. If no member has completed the task, the average is not shown.

**Confirmations and input limits**

- **VT-029:** The user is asked for confirmation before these actions: deleting a class, deleting a teacher’s task, removing a student from a class, changing a student’s class, blocking an account, deleting an account, leaving an unfinished task.

- **VT-030:** Email – up to 254 characters. Username – 3–30 characters (letters, digits, “\_”). Class name – 1–50 characters. Class code – 6 uppercase letters (A–Z) or digits; when entered, lowercase letters are converted to uppercase and spaces are removed. Evidence text – up to 500 characters. Photo – JPEG or PNG format, up to 5 MB, at most 3 per answer.

## 10. Identify Main Data Entities

Create an initial list of important data concepts. For each entity give its name and briefly describe its purpose.

Do not design the complete database schema yet; focus on identifying the main business concepts.

**Example:** Task – represents a piece of work created by a user. Category – groups tasks for filtering. User – represents an authenticated account.

- **User** – an account that logs in to the system and has one role: student, teacher or administrator.

- **Class** – a group of students created by a teacher, with a name and a unique join code.

- **Class membership** – the relationship between a student and the class the student joined with a code.

- **Task set** – the tasks created for a class, linked to one location in Lithuania and one season.

- **GBIF observation** – a confirmed external observation retrieved from GBIF and stored as an observation card with source data.

- **Task** – a problem of one of four types created from GBIF observations, or a problem created by a teacher, with a difficulty group and points.

- **Student answer** – the answer and evidence (text and photos) submitted by a student for a specific task; labelled as student-submitted data.

- **Task result** – the highest number of points a student achieved in a task; used to show progress and to unlock harder tasks.

- **Activity log entry** – a recorded login, failed login or administrator action.

## 11. Define Assumptions, Constraints and Dependencies

Document known project assumptions, mandatory constraints, and external dependencies. Separate assumptions from confirmed requirements.

Typical items include a required backend platform, internet dependency, API usage limits, supported languages, hosting restrictions, and required third-party services.

**Example:** Constraint: the backend must use ASP.NET Core. Dependency: the application uses an external email provider. Assumption: only one interface language will be supported in the first version.

**Assumptions**

- Users have an internet connection and a modern browser.

- The first version will be in Lithuanian only.

- The teacher chooses the location on a map or by entering coordinates; the location must be in Lithuania.

- A search in the GBIF Occurrence API is available without a separate login.

- The administrator account will be created by the system developer.

- Teachers register themselves, and their identity is not verified in the first version.

**Constraints**

- The system is a web application used through a browser.

- The mandatory external API is the GBIF Occurrence API (https://techdocs.gbif.org/en/openapi/v1/occurrence).

- The backend is developed using the latest stable .NET and ASP.NET Core version.

- The project must be finished within the semester deadline of the course.

- Only the minimum necessary personal data are collected (GDPR).

- GBIF data are used in accordance with their licences.

**Dependencies**

- Internet connection.

- Availability of the GBIF Occurrence API.

- A map service whose map the teacher uses to choose a location.

- A server or hosting service where the application runs.

- File storage for student photos.

- A database for users, classes, copies of GBIF observations and results.

## 12. Create the Initial README

Create or update README.md. At this stage it must contain the project name, short description, problem statement, target users, major functionality, project status, backend requirement, selected or planned frontend, and links to project documentation.

The README will be continuously improved in later assignments; it is not a one-time document.

**Example:** The README can include a “Project status: Requirements and design phase” line and links to docs/requirements.md and the specs/ directory.

The README is the file `README.md` in the repository root.

## Required Result

Another person must be able to understand what the project is supposed to achieve without opening the source code.
