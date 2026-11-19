export const teacherTaskLabel = 'Mokytojo užduotis'
export const createTeacherTaskTitle = 'Nauja mokytojo užduotis'
export const createTeacherTaskLink = 'Sukurti savo užduotį'
export const titleLabel = 'Pavadinimas'
export const questionLabel = 'Klausimas'
export const answerLabel = 'Teisingas atsakymas'
export const groupTitle = 'Sudėtingumo grupė'
export const confirmationLabel = 'Patvirtinu, kad užduotis nereikalauja rinkti, liesti ar trikdyti organizmų.'
export const saveButton = 'Išsaugoti'
export const taskSaved = 'Užduotis išsaugota.'
export const saveFailed = 'Nepavyko išsaugoti. Bandykite dar kartą.'
export const deleteTaskButton = 'Ištrinti'
export const actionsColumn = 'Veiksmai'
export const deleteTaskQuestion = 'Ištrinti užduotį? Mokinių jau gauti taškai išliks.'

export const titleRequired = 'Įveskite užduoties pavadinimą.'
export const titleTooLong = 'Pavadinimas negali būti ilgesnis nei 100 simbolių.'
export const questionRequired = 'Įveskite klausimą.'
export const questionTooLong = 'Klausimas negali būti ilgesnis nei 500 simbolių.'
export const answerRequired = 'Įveskite teisingą atsakymą.'
export const answerTooLong = 'Atsakymas negali būti ilgesnis nei 100 simbolių.'
export const groupRequired = 'Pasirinkite sudėtingumo grupę.'
export const confirmationRequired = 'Pažymėkite, kad užduotis nereikalauja rinkti, liesti ar trikdyti organizmų.'

// The accessible name of a delete button; the row number tells the buttons apart for screen readers
export function deleteTaskRowLabel(row: number): string {
  return `${deleteTaskButton}, ${row} eilutė`
}

export function groupLabel(group: number): string {
  return `${group} grupė`
}
