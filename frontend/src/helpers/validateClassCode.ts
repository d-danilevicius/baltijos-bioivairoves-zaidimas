import { classCodeInvalid, classCodeRequired } from './joinClassMessage'

const codePattern = /^[A-Z0-9]{6}$/

export function normalizeClassCode(code: string): string {
  return code.replace(/\s/g, '').toUpperCase()
}

export function validateClassCode(code: string): string | undefined {
  const normalized = normalizeClassCode(code)

  if (normalized === '') return classCodeRequired
  if (!codePattern.test(normalized)) return classCodeInvalid

  return undefined
}
