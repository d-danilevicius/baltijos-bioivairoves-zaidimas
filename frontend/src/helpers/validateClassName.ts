import { classNameRequired, classNameTooLong } from './classMessage'

const maxLength = 50

export function validateClassName(name: string): string | undefined {
  const trimmed = name.trim()

  if (trimmed === '') return classNameRequired
  if (trimmed.length > maxLength) return classNameTooLong

  return undefined
}
