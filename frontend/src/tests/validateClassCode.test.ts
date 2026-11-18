import { classCodeInvalid, classCodeRequired } from '../helpers/joinClassMessage'
import { normalizeClassCode, validateClassCode } from '../helpers/validateClassCode'

describe('normalizeClassCode', () => {
  it.each([
    ['ab12cd', 'AB12CD'],
    [' ab 12 cd ', 'AB12CD'],
    ['AB12CD', 'AB12CD'],
  ])('converts "%s" to "%s"', (code, expected) => {
    expect(normalizeClassCode(code)).toBe(expected)
  })
})

describe('validateClassCode', () => {
  it.each(['', '   '])('returns the required message for the code "%s"', (code) => {
    expect(validateClassCode(code)).toBe(classCodeRequired)
  })

  it.each(['ABC12', 'ABC1234', 'ABC-12', 'ABČ123'])('returns the invalid message for the code "%s"', (code) => {
    expect(validateClassCode(code)).toBe(classCodeInvalid)
  })

  it('returns nothing for a code with lowercase letters and spaces', () => {
    expect(validateClassCode(' ab 12 cd ')).toBeUndefined()
  })
})
