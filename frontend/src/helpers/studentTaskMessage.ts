import type { StudentTaskStatus } from '../types/studentTask'

export const studentTasksTitle = 'Užduotys'
export const noClassText = 'Dar nepriklausote klasei. Įveskite klasės kodą.'
export const joinClassButton = 'Prisijungti prie klasės'
export const noTasksForStudent = 'Mokytojas dar nesukūrė užduočių.'
export const lockedGroupMessage = 'Atrakinama surinkus 60 % ankstesnės grupės taškų.'
export const continueButton = 'Tęsti'

export const studentStatusLabels: Record<StudentTaskStatus, string> = {
  NotDone: 'Nepadaryta',
  Done: 'Padaryta',
  Locked: 'Užrakinta',
}
