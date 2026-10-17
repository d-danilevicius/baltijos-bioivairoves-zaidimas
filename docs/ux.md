# Independent Work 2 – User Experience, User Flows and Wireframes

## Objective

Transform requirements into a clear description of how users interact with the application. Important user flows must be understandable before implementation begins.

User interface texts are in Lithuanian (NFR-011). In this document each interface text is quoted in Lithuanian, and its English meaning is given in parentheses where needed. The wireframe images show the Lithuanian interface texts.

## 1. Create a Screen Inventory

Create a list of all major application screens or pages. For each screen state its purpose and which user roles may access it.

Use the inventory to verify that required workflows have an interface and that restricted screens are identified early.

**Example:** Task Details – shows one task, its status, due date, and allowed actions. Accessible to the task owner; administrative access must be explicitly justified.

**Visitor (not logged in)**

- **E01 Home page.** Shows what the game is and has links to registration and login.

- **E02 Registration.** Creates a student or teacher account.

- **E03 Login.** Login with email and password.

**Student**

- **E04 Task list.** Shows the class tasks by difficulty groups, their status, the best result and the button “Tęsti” (Continue). If the student has no class, no tasks are shown and joining a class is suggested.

- **E05 Join a class.** The student enters a class code.

- **E06 Task.** Shows a warning, an observation card (or the question of a teacher’s task) and an answer and evidence form. Available only for an unlocked task of the student’s own class.

- **E07 Task result.** Shows the points, the correct answer and the data submitted by the student, and allows repeating the task or moving on.

- **E08 My results.** Shows the points and the results of completed tasks. A student sees only own results.

**Teacher**

- **E09 Class list.** Shows the teacher’s classes and their codes.

- **E10 New class.** Class creation.

- **E11 Class information.** Shows the class code, location and season and the students, and allows removing a student or deleting the class. Reachable only by the teacher who created the class.

- **E12 Class tasks.** Shows the class task list (type, group, label), allows starting to create tasks (E13, E14) and deleting a task the teacher created. Reachable only by the teacher who created the class.

- **E13 Task creation from GBIF.** The teacher chooses a location and a season, the system retrieves GBIF observations and creates tasks.

- **E14 Teacher task form.** The teacher creates own task, for example about the school surroundings.

- **E15 Class results.** Shows the task averages and the students’ points. Reachable only by the teacher who created the class.

- **E16 Student results.** Shows the points, answers and evidence of one student, view only. Reachable only by the teacher whose class includes that student.

**Administrator**

- **E17 Account management.** Account list, blocking, unblocking, deletion. Shows only account data, not student results.

**All logged-in users**

- **E18 Profile.** Shows the username, email, role and (for a student) class; logging out is possible here. Everyone sees only own profile.

- **E19 Access denied.** Shown when the user has no permission for a screen.

**All users**

- **E20 Page not found.** Shown when a screen does not exist (also shown to a visitor).

- **E21 Confirmation dialog.** Not a separate page but a dialog shown before actions that require confirmation (VT-029).

**Screens with restricted access**

- **E04–E08** are for students only. The administrator does not have them, because the administrator can neither view nor change student results.

- **E09–E16** are for teachers only. For E11–E16 the server checks that the class (and the student) belongs to that teacher; otherwise E19 is shown.

- **E17** is for the administrator only.

- **E06–E07:** the server checks that the task belongs to the student’s class and is unlocked for that student.

- **E13:** the button “Sukurti iš GBIF” (Create from GBIF) is not shown when the class already has GBIF tasks (VT-010).

- **Flow check.** Registration, login, joining a class, task creation from GBIF, creating and deleting a teacher’s task, completing a task, results, account management and error cases all have screens.

## 2. Define Main User Journeys

Create at least three major user journeys. A journey shows how a user achieves a meaningful goal across multiple steps.

Each journey must identify the starting point, user goal, individual steps, and expected final state.

**Example:** Visitor → Registration → Login → Dashboard → Create Task → Task appears in Task List.

**Journey 1. A new student completes the first task**

- **Start:** Home page (E01), visitor.

- **Goal:** join a class and complete the first task.

- **Steps:** Visitor → Registration (E02), chooses the role “Mokinys” (Student) → Login (E03) → Task list (E04), no class, joining is suggested → Join a class (E05), enters the code → Task list (E04), sees the class tasks → Task (E06), reads the warning and the observation card, submits the answer and evidence → Task result (E07).

- **Final state:** the points are saved, the task list shows “Atlikta” (Done) and the earned points next to the task; when the share of points set by VT-014 is reached, the next difficulty group is unlocked.

**Journey 2. A teacher prepares a class and tasks about the school surroundings**

- **Start:** Home page (E01), visitor.

- **Goal:** create a class, choose the school surroundings and a season, get tasks from GBIF and add an own task.

- **Steps:** Visitor → Registration (E02), chooses the role “Mokytojas” (Teacher) → Login (E03) → Class list (E09) → New class (E10), enters a name → Class information (E11), sees the code and gives it to the students → Class tasks (E12) → Task creation from GBIF (E13), chooses the school location on the map and a season → Class tasks (E12) → Teacher task form (E14), enters own task → Class tasks (E12).

- **Final state:** the class has tasks from GBIF observations and an own task labelled “Mokytojo užduotis” (Teacher task); students can join with the code.

**Journey 3. A teacher views class results**

- **Start:** Login (E03), teacher.

- **Goal:** find out which tasks are hardest for the students and see the answers of one student.

- **Steps:** Login (E03) → Class list (E09) → Class information (E11) → Class results (E15), sees the tasks from the lowest average and the students’ points → Student results (E16), sees the answers and evidence.

- **Final state:** the teacher sees the task averages and the answers of the chosen student, labelled “Mokinio pateikti duomenys” (Student-submitted data).

**Journey 4. An administrator blocks an account**

- **Start:** Login (E03), administrator.

- **Goal:** block an account.

- **Steps:** Login (E03) → Account management (E17), chooses an account and “Užblokuoti” (Block) → Confirmation dialog (E21), confirms → Account management (E17).

- **Final state:** the account status is “Užblokuota” (Blocked), the user can no longer log in (FR-007), and the action is recorded in the log (NFR-023).

Explanation of numbers. E01, E02 and the others denote screens from section 1 “Screen inventory”; for example, E01 is the Home page.

## 3. Create User Flow Diagrams

Create diagrams for the main workflows. Show screens, user actions, important decisions, validation failures, success paths, and navigation destinations.

Complex features should have their own flow rather than being hidden inside one unreadable diagram.

**Example:** For login, include both “credentials valid → dashboard” and “credentials invalid → error message → remain on login page.”

<img src="ux/media/image35.png" style="width:6.6in;height:7.49691in" />

<img src="ux/media/image36.png" style="width:6.6in;height:5.27244in" />

<img src="ux/media/image37.png" style="width:6.6in;height:6.91452in" />

<img src="ux/media/image38.png" style="width:6.58593in;height:8.3in" />

<img src="ux/media/image39.png" style="width:6.44336in;height:8.3in" />

<img src="ux/media/image40.png" style="width:6.45128in;height:8.3in" />

<img src="ux/media/image41.png" style="width:5.68657in;height:8.3in" />

<img src="ux/media/image42.png" style="width:6.6in;height:4.76722in" />

<img src="ux/media/image43.png" style="width:6.6in;height:5.47648in" />

## 4. Create Low-Fidelity Wireframes

Create low-fidelity wireframes for all major screens using Figma, Penpot, diagrams, or another suitable tool.

Wireframes must show navigation, page hierarchy, forms, buttons, lists, important data, and actions available to users. Visual branding is not the goal at this stage.

**Example:** A Task List wireframe should show the page title, create button, filter controls, list/table of tasks, and the actions available for each task.

The wireframes are shown for a desktop screen. Phone wireframes are shown in section 8.

<img src="ux/media/image44.png" style="width:6.6in;height:2.31in" />

<img src="ux/media/image45.png" style="width:6.6in;height:4.43143in" />

<img src="ux/media/image46.png" style="width:6.6in;height:2.95114in" />

<img src="ux/media/image47.png" style="width:6.6in;height:6.05314in" />

<img src="ux/media/image48.png" style="width:6.6in;height:2.69657in" />

<img src="ux/media/image49.png" style="width:6.6in;height:6.93943in" />

<img src="ux/media/image50.png" style="width:6.6in;height:3.696in" />

<img src="ux/media/image51.png" style="width:6.6in;height:3.50743in" />

<img src="ux/media/image52.png" style="width:6.6in;height:2.67771in" />

<img src="ux/media/image53.png" style="width:6.6in;height:2.54571in" />

<img src="ux/media/image54.png" style="width:6.6in;height:3.67714in" />

<img src="ux/media/image55.png" style="width:6.6in;height:4.01657in" />

<img src="ux/media/image56.png" style="width:6.6in;height:4.41257in" />

<img src="ux/media/image57.png" style="width:6.6in;height:5.04429in" />

<img src="ux/media/image58.png" style="width:6.6in;height:4.17686in" />

<img src="ux/media/image59.png" style="width:6.6in;height:4.026in" />

<img src="ux/media/image60.png" style="width:6.6in;height:3.102in" />

<img src="ux/media/image61.png" style="width:6.6in;height:3.48857in" />

<img src="ux/media/image62.png" style="width:6.6in;height:2.31in" />

<img src="ux/media/image63.png" style="width:6.6in;height:2.00829in" />

<img src="ux/media/image64.png" style="width:6.6in;height:2.15914in" />

## 5. Define Form Behaviour

For every important form specify fields, required and optional fields, input restrictions, validation messages, submit action, successful result, and unsuccessful result.

Do not define only what a form looks like; define how it behaves.

**Example:** Email: required, valid email format, maximum length defined, duplicate address rejected during registration, and the user receives a clear validation message.

**5.1. Registration**

**What the user enters**

- **Email.** A valid email address, at most 254 characters (VT-030). The address must not repeat the address of another user (VT-001).

  - If empty: „Įveskite el. pašto adresą.“ (Enter an email address.)

  - If invalid: „Įveskite teisingą el. pašto adresą.“ (Enter a valid email address.)

  - If too long: „El. paštas negali būti ilgesnis nei 254 simboliai.“ (The email must not be longer than 254 characters.)

  - If already registered: „Šis el. pašto adresas jau užregistruotas.“ (This email address is already registered.)

- **Username.** 3–30 characters: letters, digits and “\_” (VT-030). The username must not repeat the username of another user (VT-002).

  - If empty: „Įveskite naudotojo vardą.“ (Enter a username.)

  - If invalid: „Naudotojo vardą turi sudaryti 3–30 raidžių, skaičių ar simbolių „\_“.“ (The username must consist of 3–30 letters, digits or “\_” characters.)

  - If already used: „Toks naudotojo vardas jau naudojamas.“ (This username is already in use.)

- **Password.** 8–128 characters (VT-003). Dots are shown while typing. The password is never displayed and never written to logs (NFR-024).

  - If empty: „Įveskite slaptažodį.“ (Enter a password.)

  - If shorter than 8 or longer than 128 characters: „Slaptažodį turi sudaryti nuo 8 iki 128 simbolių.“ (The password must consist of 8 to 128 characters.)

- **Role.** “Mokinys” (Student) or “Mokytojas” (Teacher) must be chosen (VT-004). Initially none is selected.

  - If not chosen: „Pasirinkite rolę.“ (Choose a role.)

**When “Registruotis” (Register) is clicked**

- **On success:** the account is created and the login page opens with the message „Paskyra sukurta. Prisijunkite.“ (Account created. Log in.)

- **On failure:** the user stays on the registration page. At the top the message „Forma neišsiųsta. Ištaisykite pažymėtus laukus.“ (The form was not submitted. Correct the marked fields.) is shown, and next to each invalid field its error. The entered data remain, except the password. If the server fails: „Nepavyko užregistruoti. Bandykite vėliau.“ (Registration failed. Try again later.)

**5.2. Login**

**What the user enters**

- **Email.** If empty: „Įveskite el. pašto adresą.“ (Enter an email address.)

- **Password.** If empty: „Įveskite slaptažodį.“ (Enter a password.)

**When “Prisijungti” (Log in) is clicked**

- **On success:** the user’s starting screen opens according to the role: the task list for a student (E04), the class list for a teacher (E09), account management for an administrator (E17).

- **On failure:** the user stays on the login page, and the password field is cleared.

- **Incorrect credentials:** „Neteisingas el. paštas arba slaptažodis.“ (Incorrect email or password.) The message does not say which of them is incorrect (FR-006).

- **Account blocked:** „Paskyra užblokuota. Kreipkitės į administratorių.“ (The account is blocked. Contact the administrator.) (FR-007)

- **User was inactive for 30 minutes:** „Sesija baigėsi dėl neveiklumo. Prisijunkite iš naujo.“ (The session ended due to inactivity. Log in again.) (NFR-004)

**5.3. Joining a class**

**What the student enters**

- **Class code.** 6 characters: uppercase letters A–Z or digits (VT-030). Before checking, lowercase letters are converted to uppercase and spaces are removed.

  - If empty: „Įveskite klasės kodą.“ (Enter the class code.)

  - If not 6 letters or digits: „Kodą sudaro 6 raidės ar skaičiai.“ (The code consists of 6 letters or digits.)

  - If there is no such class: „Klasė su tokiu kodu nerasta.“ (No class with this code was found.)

**When “Prisijungti” (Join) is clicked**

- **On success:** the student is assigned to the class and the task list (E04) opens with the message „Prisijungėte prie klasės „7a“.“ (You joined class “7a”.) If the student is already in another class, a confirmation dialog (E21) is shown first: „Jau priklausote klasei „7b“. Pakeisti į „7a“?“ (You already belong to class “7b”. Change to “7a”?) with the buttons “Pakeisti” (Change) and “Atšaukti” (Cancel). After the change, the previous membership ends (VT-025).

- **On failure:** the student stays on the same page, and the error is shown next to the field.

**5.4. New class (teacher)**

**What the teacher enters**

- **Class name.** 1–50 characters (VT-030). Spaces at the beginning and end are removed. Name uniqueness is not checked, because a class is identified by its unique code (VT-023).

  - If empty: „Įveskite klasės pavadinimą.“ (Enter the class name.)

  - If too long: „Pavadinimas negali būti ilgesnis nei 50 simbolių.“ (The name must not be longer than 50 characters.)

**When “Sukurti” (Create) is clicked**

- **On success:** the class is created, the system generates its code itself (FR-015), and the class information (E11) opens with the message „Klasė sukurta.“ (Class created.)

- **On failure:** the teacher stays on the same page, and the error is shown next to the field.

**5.5. Task creation from GBIF (teacher)**

**What the teacher does**

- **Location.** A point is chosen on the map, or latitude and longitude are entered. The location must be within the territory of Lithuania (VT-010).

  - If not chosen: „Pasirinkite vietovę.“ (Choose a location.)

  - If the coordinates are not numbers: „Platuma ir ilguma turi būti skaičiai.“ (Latitude and longitude must be numbers.)

  - If not in Lithuania: „Pasirinkite vietovę Lietuvos teritorijoje.“ (Choose a location within Lithuania.)

- **Season.** One of four must be chosen: spring, summer, autumn, winter (VT-012).

  - If not chosen: „Pasirinkite metų laiką.“ (Choose a season.)

- **Note in the form:** „Naudojami stebėjimai ne toliau kaip 10 km nuo vietovės.“ (Observations no farther than 10 km from the location are used.) (VT-011)

**When “Gauti stebėjimus ir sukurti užduotis” (Get observations and create tasks) is clicked**

- **While the request is running:** „Kraunami stebėjimai iš GBIF…“ (Loading observations from GBIF…) is shown. The tasks are created within no more than 30 seconds (NFR-016).

- **On success:** the class tasks (E12) open with the message „Užduotys sukurtos.“ (Tasks created.) A copy of the observations used is stored (NFR-021).

- **If GBIF does not respond within 10 seconds or returns an error:** „Nepavyko gauti stebėjimų iš GBIF. Bandykite dar kartą.“ (Observations could not be retrieved from GBIF. Try again.) and the button “Bandyti dar kartą” (Try again). Incomplete tasks are not created (NFR-020).

- **If there are too few observations:** „Šioje vietovėje šiuo metų laiku rasta per mažai stebėjimų. Pasirinkite kitą vietovę ar metų laiką.“ (Too few observations were found at this location in this season. Choose another location or season.) Tasks are not created (FR-036, VT-011).

- **If the class already has GBIF tasks:** the button “Sukurti iš GBIF” (Create from GBIF) is not shown (VT-010).

**5.6. Teacher task form**

**What the teacher enters**

- **Title.** 1–100 characters (VT-018).

  - If empty: „Įveskite užduoties pavadinimą.“ (Enter the task title.)

  - If too long: „Pavadinimas negali būti ilgesnis nei 100 simbolių.“ (The title must not be longer than 100 characters.)

- **Question.** Up to 500 characters (VT-018).

  - If empty: „Įveskite klausimą.“ (Enter the question.)

  - If too long: „Klausimas negali būti ilgesnis nei 500 simbolių.“ (The question must not be longer than 500 characters.)

- **Correct answer.** Up to 100 characters (VT-018). A student’s answer is considered correct if it matches it, ignoring letter case and spaces at the beginning and end.

  - If empty: „Įveskite teisingą atsakymą.“ (Enter the correct answer.)

  - If too long: „Atsakymas negali būti ilgesnis nei 100 simbolių.“ (The answer must not be longer than 100 characters.)

- **Difficulty group.** One of three groups must be chosen (VT-013). Initially none is selected.

  - If not chosen: „Pasirinkite sudėtingumo grupę.“ (Choose a difficulty group.)

- **Confirmation.** The teacher must tick: „Patvirtinu, kad užduotis nereikalauja rinkti, liesti ar trikdyti organizmų.“ (I confirm that the task does not require collecting, touching or disturbing organisms.) (FR-032, VT-021)

  - If not ticked: „Pažymėkite, kad užduotis nereikalauja rinkti, liesti ar trikdyti organizmų.“ (Tick that the task does not require collecting, touching or disturbing organisms.)

**When “Išsaugoti” (Save) is clicked**

- **On success:** the task is saved with the label “Mokytojo užduotis” (Teacher task) and the class tasks (E12) open with the message „Užduotis išsaugota.“ (Task saved.)

- **On failure:** nothing is saved, the teacher stays on the same page, and the errors are shown next to the fields and at the top. If the server fails: „Nepavyko išsaugoti. Bandykite dar kartą.“ (Saving failed. Try again.)

- **Deletion:** the button “Ištrinti” (Delete) is shown only next to a task the teacher created (E12). Before deleting, a confirmation dialog (E21) is shown: „Ištrinti užduotį? Mokinių jau gauti taškai išliks.“ (Delete the task? Points already earned by students will remain.)

**5.7. Task answer (student)**

**What the student does**

- **Answer.** In a task from GBIF observations, one of the given options is chosen. In a teacher’s task the answer is entered as text of up to 100 characters (VT-018).

  - If not chosen or empty: „Pasirinkite arba įrašykite atsakymą.“ (Choose or enter an answer.)

  - If too long: „Atsakymas negali būti ilgesnis nei 100 simbolių.“ (The answer must not be longer than 100 characters.)

- **Evidence.** Text of up to 500 characters (VT-030). A counter is shown below the field, for example “0 / 500”.

  - If empty: „Įrašykite įrodymą.“ (Enter the evidence.)

  - If too long: „Įrodymas negali būti ilgesnis nei 500 simbolių.“ (The evidence must not be longer than 500 characters.)

- **Photos.** Optional. JPEG or PNG format, up to 5 MB, at most 3 per answer (VT-030). Location and other metadata are removed from the photos (NFR-006).

  - If not JPEG or PNG: „Nuotrauka turi būti JPEG arba PNG formato.“ (The photo must be in JPEG or PNG format.)

  - If larger than 5 MB: „Nuotrauka negali būti didesnė nei 5 MB.“ (The photo must not be larger than 5 MB.)

  - If more than 3: „Galima pridėti ne daugiau kaip 3 nuotraukas.“ (No more than 3 photos can be added.)

- **Warning.** A warning is always shown above the form: „Stebėkite gamtą iš atstumo. Nerinkite ir netrikdykite organizmų. Nefotografuokite žmonių.“ (Observe nature from a distance. Do not collect or disturb organisms. Do not photograph people.) (FR-045)

**When “Pateikti atsakymą” (Submit answer) is clicked**

- **Note next to the button:** „Pateikto atsakymo pakeisti nebus galima.“ (A submitted answer cannot be changed.) (VT-015)

- **On success:** within no more than 1 second the task result (E07) opens with the points earned (NFR-015). The points are saved even if the connection is lost immediately after submission (NFR-018).

- **On failure:** the student stays in the same task, and the entered data remain. If saving failed, „Nepavyko išsaugoti. Bandykite dar kartą.“ (Saving failed. Try again.) and the button “Bandyti dar kartą” (Try again) are shown.

- **If the task was deleted while the student had it open:** the page “Puslapis nerastas” (Page not found) (E20) is shown and no points are awarded.

- **When leaving without submitting an answer:** a confirmation dialog (E21) is shown: „Išeiti iš užduoties? Neišsiųstas atsakymas nebus išsaugotas.“ (Leave the task? The unsent answer will not be saved.) with the buttons “Išeiti” (Leave) and “Likti” (Stay).

- **Repeating:** the button “Pakartoti užduotį” (Repeat the task) (E07) opens the task again. The highest number of points achieved is the result of the task (VT-017).

## 6. Design Application States

For relevant screens define normal, loading, empty, validation error, server error, unauthorized, not-found, and success states.

A screen is not complete if only the successful state has been designed.

**Example:** Task List empty state: display “No tasks yet” and a visible action to create the first task instead of showing an unexplained blank area.

**All screens after login**

- **Loading:** grey placeholders and the text „Kraunama…“ (Loading…) are shown instead of the content (NFR-028).

- **Server error:** the message „Įvyko klaida. Bandykite vėliau.“ (An error occurred. Try again later.) and the button “Bandyti dar kartą” (Try again) are shown (NFR-028).

- **Not logged in or the session has expired:** the user is redirected to the login page with the message „Prisijunkite, kad galėtumėte tęsti.“ (Log in to continue.) (FR-009)

- **No permission:** the page “Prieiga uždrausta” (Access denied) is shown with the button “Į pradžią” (To the home page). This happens when a student opens an administrator or teacher page, a teacher opens another teacher’s class, or a student opens a locked task (NFR-003).

- **Not found:** if the page or record does not exist, the page “Puslapis nerastas” (Page not found) is shown with the button “Į pradžią” (To the home page) (NFR-027).

**Registration**

- **Normal:** an empty form.

- **Validation error:** an error next to each invalid field and the message at the top „Forma neišsiųsta. Ištaisykite pažymėtus laukus.“ (The form was not submitted. Correct the marked fields.)

- **Server error:** „Nepavyko užregistruoti. Bandykite vėliau.“ (Registration failed. Try again later.)

- **Success:** the login page opens with the message „Paskyra sukurta. Prisijunkite.“ (Account created. Log in.)

**Login**

- **Normal:** an empty form.

- **Validation error:** „Įveskite el. pašto adresą.“ (Enter an email address.), „Įveskite slaptažodį.“ (Enter a password.), „Neteisingas el. paštas arba slaptažodis.“ (Incorrect email or password.), „Paskyra užblokuota. Kreipkitės į administratorių.“ (The account is blocked. Contact the administrator.)

- **Server error:** „Nepavyko prisijungti. Bandykite vėliau.“ (Login failed. Try again later.)

- **Success:** the starting screen opens according to the role.

**Task list (student)**

- **Normal:** tasks by difficulty groups with the status (done, not done, locked), labels, the best result and the button “Tęsti” (Continue).

- **Empty, when the student has no class:** „Dar nepriklausote klasei. Įveskite klasės kodą.“ (You do not belong to a class yet. Enter a class code.) and the button “Prisijungti prie klasės” (Join a class) (VT-026).

- **Empty, when the class has no tasks:** „Mokytojas dar nesukūrė užduočių.“ (The teacher has not created tasks yet.)

- **Locked group:** „Atrakinama surinkus 60 % ankstesnės grupės taškų.“ (Unlocked after earning 60 % of the points of the previous group.) (VT-014)

- **Success:** after joining a class the message „Prisijungėte prie klasės“ (You joined the class) is shown.

**Join a class (student)**

- **Normal:** an empty form.

- **Validation error:** „Įveskite klasės kodą.“ (Enter the class code.), „Kodą sudaro 6 raidės ar skaičiai.“ (The code consists of 6 letters or digits.), „Klasė su tokiu kodu nerasta.“ (No class with this code was found.)

- **Success:** the task list opens with the message „Prisijungėte prie klasės“ (You joined the class).

**Task (student)**

- **Normal:** a warning, an observation card with the label “Patvirtintas GBIF stebėjimas” (Confirmed GBIF observation) (or the question of a teacher’s task with the label “Mokytojo užduotis” (Teacher task)) and an answer form.

- **Loading:** after submission „Saugomas atsakymas…“ (Saving the answer…).

- **Validation error:** errors next to the fields (section 5.7).

- **Server error:** „Nepavyko išsaugoti. Bandykite dar kartą.“ (Saving failed. Try again.) and the button “Bandyti dar kartą” (Try again).

- **Protected species:** instead of the exact coordinates only the place name and the note „Tikslios koordinatės paslėptos, nes rūšis saugoma.“ (The exact coordinates are hidden because the species is protected.) are shown (FR-046).

- **Success:** the task result (E07) opens.

**Task result (student)**

- **Normal:** the points earned, the correct answer, the data submitted by the student with the label “Mokinio pateikti duomenys” (Student-submitted data) and the buttons “Pakartoti užduotį” (Repeat the task), “Kita užduotis” (Next task), “Į užduočių sąrašą” (To the task list).

- **Success:** if a new group has been unlocked, „Atrakinta 2 sudėtingumo grupė.“ (Difficulty group 2 unlocked.) is shown (FR-042).

**My results (student)**

- **Normal:** a table of the results of completed tasks and the sum of points.

- **Empty:** „Dar neatlikote nė vienos užduoties.“ (You have not completed any task yet.) and a link to the task list.

**Class list (teacher)**

- **Normal:** a table of the teacher’s classes.

- **Empty:** „Dar nesukūrėte nė vienos klasės.“ (You have not created any class yet.) and the button “Sukurti pirmą klasę” (Create the first class).

- **Success:** a message after a class is created or deleted.

**New class (teacher)**

- **Normal:** an empty form.

- **Validation error:** „Įveskite klasės pavadinimą.“ (Enter the class name.), „Pavadinimas negali būti ilgesnis nei 50 simbolių.“ (The name must not be longer than 50 characters.)

- **Success:** the class information opens with the message „Klasė sukurta.“ (Class created.)

**Class information (teacher)**

- **Normal:** the class code, the location and season (if set) and the student list.

- **Empty:** „Į klasę dar niekas neprisijungė. Perduokite mokiniams kodą.“ (Nobody has joined the class yet. Give the code to the students.)

- **Success:** „Mokinys pašalintas iš klasės.“ (The student was removed from the class.)

**Class tasks (teacher)**

- **Normal:** a task table with the type, group and label.

- **Empty:** „Klasė dar neturi užduočių.“ (The class has no tasks yet.) and the buttons “Sukurti iš GBIF” (Create from GBIF) and “Nauja savo užduotis” (New own task).

- **Success:** „Užduotys sukurtos.“ (Tasks created.), „Užduotis išsaugota.“ (Task saved.) or „Užduotis ištrinta.“ (Task deleted.)

**Task creation from GBIF (teacher)**

- **Normal:** location selection, season and the button “Gauti stebėjimus ir sukurti užduotis” (Get observations and create tasks).

- **Loading:** „Kraunami stebėjimai iš GBIF…“ (Loading observations from GBIF…).

- **Validation error:** „Pasirinkite vietovę.“ (Choose a location.), „Pasirinkite vietovę Lietuvos teritorijoje.“ (Choose a location within Lithuania.), „Pasirinkite metų laiką.“ (Choose a season.)

- **Server error:** „Nepavyko gauti stebėjimų iš GBIF. Bandykite dar kartą.“ (Observations could not be retrieved from GBIF. Try again.) and the button “Bandyti dar kartą” (Try again).

- **Too few observations:** „Šioje vietovėje šiuo metų laiku rasta per mažai stebėjimų. Pasirinkite kitą vietovę ar metų laiką.“ (Too few observations were found at this location in this season. Choose another location or season.)

- **Success:** the class tasks open with the message „Užduotys sukurtos.“ (Tasks created.)

**Teacher task form (teacher)**

- **Normal:** an empty form.

- **Validation error:** an error next to each invalid field and a message at the top (section 5.6).

- **Server error:** „Nepavyko išsaugoti. Bandykite dar kartą.“ (Saving failed. Try again.)

- **Success:** the class tasks open with the message „Užduotis išsaugota.“ (Task saved.)

**Class results (teacher)**

- **Normal:** the task averages from the lowest and a table of the students’ points.

- **Empty:** „Mokiniai dar neatliko nė vienos užduoties.“ (Students have not completed any task yet.) The average of a task that no student has completed is not shown (VT-028).

**Student results (teacher)**

- **Normal:** the student’s points, answers and evidence, view only.

- **Empty:** „Mokinys dar neatliko nė vienos užduoties.“ (The student has not completed any task yet.)

**Account management (administrator)**

- **Normal:** a table of accounts with the status (active, blocked). Next to the administrator’s own account „Jūsų paskyra“ (Your account) is written and there are no actions (VT-007).

- **Empty:** does not occur, because there is always at least one administrator in the system.

- **Success:** „Paskyra užblokuota.“ (Account blocked.), „Paskyra atblokuota.“ (Account unblocked.) or „Paskyra ištrinta.“ (Account deleted.)

**Confirmation dialog**

- **Normal:** a question and the buttons “Patvirtinti” (Confirm) and “Atšaukti” (Cancel). Shown before the actions from the VT-029 list.

- **Cancelled:** the action is not performed, and the user stays on the same screen.

## 7. Design Navigation

Define primary navigation, any necessary secondary navigation, behaviour for authenticated and unauthenticated users, and role-dependent navigation.

A user should be able to understand where they are, where they can go, and how to return to a previous meaningful location.

**Example:** Unauthenticated users see Home, Login, and Register. Authenticated users see Dashboard, Tasks, Profile, and Logout. Admin appears only for authorized administrators.

**Primary navigation (menu at the top)**

- **Visitor who is not logged in:** “Pradžia” (Home), “Prisijungti” (Log in), “Registruotis” (Register).

- **Student:** “Užduotys” (Tasks), “Mano rezultatai” (My results), “Profilis” (Profile), “Atsijungti” (Log out).

- **Teacher:** “Klasės” (Classes), “Profilis” (Profile), “Atsijungti” (Log out).

- **Administrator:** “Paskyros” (Accounts), “Profilis” (Profile), “Atsijungti” (Log out).

**Secondary navigation**

- **Breadcrumbs** are shown on deeper pages so that the user can see where they are. Examples: „Užduotys > Atpažink rūšių grupę“ (Tasks > Recognise the species group), „Klasės > 7a > Rezultatai > jonas7“ (Classes > 7a > Results > jonas7), „Klasės > 7a > Užduotys > Nauja užduotis“ (Classes > 7a > Tasks > New task).

- **The button “Atgal” (Back)** returns to the previous meaningful page, for example from student results to class results, from class results to class information, from class information to the class list.

- **The title “Baltijos biologinės įvairovės iššūkių žaidimas”** at the top always opens the user’s starting screen.

**Behaviour for a user who is not logged in**

- **Sees only the home, registration and login pages.**

- **When trying to open another page**, the user is redirected to login with the message „Prisijunkite, kad galėtumėte tęsti.“ (Log in to continue.)

**Behaviour for a logged-in user**

- **After login** the starting screen opens according to the role: the task list for a student (E04), the class list for a teacher (E09), account management for an administrator (E17).

- **If a logged-in user** opens the home, registration or login page, the user is redirected to the user’s starting screen (FR-011).

- **“Atsijungti” (Log out)** logs the user out and opens the login page (FR-008).

**Navigation by role**

- **Everyone sees only the menu items of their own role.** The student and teacher menus have no administrator links, and the administrator menu has no links to student results.

- **If a user enters the address of a page of another role,** the page “Prieiga uždrausta” (Access denied) is shown with the button “Į pradžią” (To the home page).

**How the user understands where they are**

- **The active menu item** is marked with a dark background and bold text.

- **At the top of every page** there is its title.

- **In the task list** the progress is shown next to each group, for example „1 grupė · surinkta 30 iš 50“ (Group 1 · earned 30 of 50).

- **When leaving an unfinished task** the question „Išeiti iš užduoties? Neišsiųstas atsakymas nebus išsaugotas.“ (Leave the task? The unsent answer will not be saved.) is shown with the buttons “Išeiti” (Leave) and “Likti” (Stay).

## 8. Consider Responsive Design

Show how important screens behave on at least desktop and narrow/mobile screen widths.

Normal content must remain usable without horizontal scrolling unless horizontal scrolling is an intentional requirement for a specific data view.

**Example:** On mobile, a wide task table can become stacked task cards with the most important fields visible first instead of shrinking text until it is unreadable.

- **Width.** All screens display correctly from 360 px width without horizontal scrolling (NFR-008).

- **Menu.** On a phone the menu collapses into the button “☰ Meniu” (Menu).

- **Tables.** On a phone tables become cards, and the most important fields (title, label, status) are shown first.

- **Forms.** On a phone fields and buttons take the full width, and the text is not reduced.

<img src="ux/media/image65.png" style="width:3.1in;height:2.74982in" /><img src="ux/media/image66.png" style="width:3.1in;height:4.805in" />

<img src="ux/media/image67.png" style="width:3.1in;height:5.28148in" /><img src="ux/media/image68.png" style="width:3.1in;height:3.52482in" />

<img src="ux/media/image69.png" style="width:3.1in;height:4.23093in" /><img src="ux/media/image70.png" style="width:3.1in;height:3.30667in" />

## 9. Include Basic Accessibility Requirements

The design must consider readable labels, keyboard-accessible controls, meaningful button names, clear content hierarchy, semantic structure, and validation that is not communicated only by colour.

Accessibility decisions made here should later be reflected in the implementation.

**Example:** Use a visible “Delete task” button label and an associated accessible name instead of an unlabeled trash-can icon that can only be understood visually.

**Accessibility requirements**

1.  **Clear field labels.** Every field has a visible label above it, for example „El. paštas“ (Email). Placeholder text in the field does not replace the label.

2.  **Keyboard control.** The application can be used without a mouse: the Tab key moves to the next field or button, Enter and Space press a button. The selected element is marked with an outline (NFR-013).

3.  **Clear button names.** Buttons have text that is understandable without a picture, for example „Ištrinti klasę“ (Delete the class), „Pašalinti mokinį“ (Remove the student). Buttons with only an icon are not used.

4.  **Clear content order.** At the top of every page there is a large title, and below it smaller sections by importance.

5.  **Proper page structure.** The menu, the main content, forms and tables are marked with elements intended for them, so that a screen reader can also understand them.

6.  **Errors not by colour only.** An error is written as text next to the field and marked with the sign “⚠”, so that a person who cannot distinguish colours can see it too (NFR-013).

7.  **Information not by colour only.** The task status is written in words („Atlikta“ (Done), „Neatlikta“ (Not done), „Užrakinta“ (Locked)), points are shown as numbers, for example „30 iš 50“ (30 of 50), and the labels „Patvirtintas GBIF stebėjimas“ (Confirmed GBIF observation), „Mokytojo užduotis“ (Teacher task) and „Mokinio pateikti duomenys“ (Student-submitted data) are written as text.

8.  **Sufficient contrast.** The text clearly differs from the background (contrast of at least 4.5:1, NFR-010).

9.  **Location selection.** The location can also be given by entering coordinates, so the map does not need to be operated with a mouse.

These decisions form requirements for building the interface: while the application is built, every screen will be checked against this list.

## 10. Trace Wireframes to Requirements

Connect major screens and flows to requirements from Independent Work 1. If a wireframe introduces new behaviour, update the requirements before implementation.

Do not silently add product functionality only through a design mock-up.

**Example:** If the wireframe adds “Archive task” but no functional requirement exists for archiving, create a new FR requirement and update the scope before development.

**Screens and requirements**

- **Home page (E01):** FR-010, FR-011

- **Registration (E02):** FR-001–FR-004, VT-001–VT-005, VT-030

- **Login (E03):** FR-005–FR-007, FR-009, FR-011, NFR-004

- **Task list (E04):** FR-033, FR-037, FR-042, FR-049, VT-013, VT-014, VT-026

- **Join a class (E05):** FR-016, FR-017, FR-053, VT-025, VT-029, VT-030

- **Task (E06):** FR-038–FR-040, FR-043–FR-047, FR-053, VT-015, VT-019–VT-021, VT-030

- **Task result (E07):** FR-041, FR-042, FR-044, FR-048, FR-051, VT-014–VT-017, VT-019, NFR-015, NFR-018

- **My results (E08):** FR-050, FR-052, VT-022

- **Class list (E09):** FR-014

- **New class (E10):** FR-013, FR-015, VT-023, VT-024, VT-030

- **Class information (E11):** FR-015, FR-018, FR-022, FR-023, FR-053, VT-027

- **Class tasks (E12):** FR-033–FR-035, FR-053, VT-010, VT-018

- **Task creation from GBIF (E13):** FR-025–FR-030, FR-036, VT-010–VT-013, NFR-016, NFR-020, NFR-021

- **Teacher task form (E14):** FR-031–FR-033, VT-018, VT-021

- **Class results (E15):** FR-019, FR-021, FR-024, VT-022, VT-028

- **Student results (E16):** FR-019, FR-020, FR-024, FR-044, VT-019, VT-022

- **Account management (E17):** FR-053–FR-059, VT-006–VT-009, NFR-023

- **Profile (E18):** FR-008, FR-012

- **Access denied (E19):** FR-024, FR-052, FR-058, FR-059, NFR-003

- **Page not found (E20):** NFR-027

- **Confirmation dialog (E21):** FR-053, VT-029

- **All screens:** NFR-008, NFR-009, NFR-010, NFR-011, NFR-012, NFR-013, NFR-014, NFR-028, NFR-029

**Flows and requirements**

- **Registration:** FR-001–FR-004, VT-001–VT-005

- **Login:** FR-005–FR-007, NFR-004

- **Student joins a class:** FR-016, FR-017, FR-053, VT-025, VT-029, VT-030

- **Completing a task:** FR-038–FR-042, FR-048, FR-051, VT-014–VT-017, NFR-015

- **Task creation from GBIF:** FR-025–FR-030, FR-036, VT-010–VT-013, NFR-016, NFR-020, NFR-021

- **Creating and deleting a teacher’s task:** FR-031–FR-035, VT-018

- **A teacher creates a class and views results:** FR-013–FR-015, FR-018–FR-021, FR-024

- **An administrator manages accounts:** FR-054–FR-059, VT-007, VT-008

- **Access checking:** FR-009, FR-024, FR-052, FR-058, FR-059, NFR-003, NFR-004, NFR-027

**New behaviour introduced by the wireframes and the requirements update**

The wireframes did not add new features. The requirements of Work 1 were supplemented with two requirements that did not exist although the screens show them:

- **FR-014:** the teacher’s class list (E09).

- **FR-035:** the class task list for the teacher (E12).

These rules follow from existing requirements and do not require new requirements:

- **E12:** the button “Sukurti iš GBIF” (Create from GBIF) is not shown when the class already has GBIF tasks (VT-010).

- **E04:** a student without a class does not see tasks and sees a suggestion to join a class (VT-026).

- **E06:** if the task was deleted while the student had it open, “Puslapis nerastas” (Page not found) is shown (NFR-027).

- **E15, E16, E17:** the administrator has no result screens and does not see student results (FR-058, VT-022).

## Required Result

The student must be able to demonstrate how a user completes the application’s most important activities before those activities are programmed.
