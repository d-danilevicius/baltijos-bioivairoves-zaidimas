import { classNameRequired, classNameTooLong } from '../helpers/classMessages'
import { validateClassName } from '../helpers/validateClassName'

describe('validateClassName', () => {
  it.each(['', '   '])('returns the required message for the name "%s"', (name) => {
    expect(validateClassName(name)).toBe(classNameRequired)
  })

  it('returns the too long message for a name of 51 characters', () => {
    expect(validateClassName('a'.repeat(51))).toBe(classNameTooLong)
  })

  it('returns nothing for a name of 50 characters', () => {
    expect(validateClassName('a'.repeat(50))).toBeUndefined()
  })

  it('does not count the spaces at the beginning and the end', () => {
    expect(validateClassName(`  ${'a'.repeat(50)}  `)).toBeUndefined()
  })
})
