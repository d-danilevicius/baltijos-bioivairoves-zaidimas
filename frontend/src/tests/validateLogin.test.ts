import { emailRequired, passwordRequired } from '../helpers/loginMessages'
import { validateLogin } from '../helpers/validateLogin'

describe('validateLogin', () => {
  it('returns no errors when email and password are provided', () => {
    const errors = validateLogin('user@example.com', 'secret123')
    expect(errors).toEqual({})
  })

  it('returns emailRequired when email is empty', () => {
    const errors = validateLogin('', 'secret123')
    expect(errors.email).toBe(emailRequired)
  })

  it('returns emailRequired when email contains only spaces', () => {
    const errors = validateLogin('   ', 'secret123')
    expect(errors.email).toBe(emailRequired)
  })

  it('returns passwordRequired when password is empty', () => {
    const errors = validateLogin('user@example.com', '')
    expect(errors.password).toBe(passwordRequired)
  })

  it('returns passwordRequired when password contains only spaces', () => {
    const errors = validateLogin('user@example.com', '   ')
    expect(errors.password).toBe(passwordRequired)
  })

  it('returns both required messages when both values are empty or spaces', () => {
    const errors = validateLogin('   ', '')
    expect(errors.email).toBe(emailRequired)
    expect(errors.password).toBe(passwordRequired)
  })
})
