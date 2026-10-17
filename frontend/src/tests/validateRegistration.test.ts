import { validateRegistration } from '../helpers/validateRegistration'
import type { RegisterRequest } from '../types/auth'

describe('validateRegistration', () => {
  it('accepts valid values', () => {
    const v = { email: 'a@b.com', userName: 'user_12', password: 'Password123', role: 'Student' as const }
    const errors = validateRegistration(v)
    expect(errors).toEqual({})
  })

  it('rejects empty fields', () => {
    const errors = validateRegistration({})
    expect(errors.email).toBe('Required')
    expect(errors.userName).toBe('Required')
    expect(errors.password).toBe('Required')
    expect(errors.role).toBe('Required')
  })

  it('rejects invalid email format and too long', () => {
    const tooLong = 'a'.repeat(255) + '@x.com'
    expect(validateRegistration({ email: tooLong }).email).toBe('TooLong')
    expect(validateRegistration({ email: 'bad' }).email).toBe('InvalidFormat')
  })

  it('rejects invalid username', () => {
    expect(validateRegistration({ userName: 'ab' }).userName).toBe('InvalidFormat')
    expect(validateRegistration({ userName: 'bad$name' }).userName).toBe('InvalidFormat')
  })

  it('rejects invalid password length', () => {
    expect(validateRegistration({ password: 'short' }).password).toBe('InvalidLength')
    expect(validateRegistration({ password: 'p'.repeat(129) }).password).toBe('InvalidLength')
  })

  it('rejects invalid role', () => {
    expect(validateRegistration({ role: 'Admin' as unknown as RegisterRequest['role'] }).role).toBe('NotAllowed')
  })
})
