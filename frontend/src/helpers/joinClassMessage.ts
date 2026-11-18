export const classCodeRequired = 'Įveskite klasės kodą.'
export const classCodeInvalid = 'Kodą sudaro 6 raidės ar skaičiai.'
export const classCodeNotFound = 'Klasė su tokiu kodu nerasta.'
export const alreadyInClass = 'Jau priklausote šiai klasei.'
export const joinClassTitle = 'Prisijungti prie klasės'
export const changeClassConfirm = 'Pakeisti'

export function joinedClass(className: string): string {
  return `Prisijungėte prie klasės „${className}“.`
}

export function changeClassQuestion(currentClassName: string, newClassName: string): string {
  return `Jau priklausote klasei „${currentClassName}“. Pakeisti į „${newClassName}“?`
}
