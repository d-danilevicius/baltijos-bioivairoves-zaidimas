import { messageFor, messages } from '../helpers/registrationMessages'

describe('registrationMessages', () => {
  it('returns field messages for codes', () => {
    expect(messageFor('email', 'Required')).toBe(messages.field.email.Required)
    expect(messageFor('email', 'InvalidFormat')).toBe(messages.field.email.InvalidFormat)
    expect(messageFor('email', 'TooLong')).toBe(messages.field.email.TooLong)
    expect(messageFor('userName', 'Required')).toBe(messages.field.userName.Required)
    expect(messageFor('userName', 'InvalidFormat')).toBe(messages.field.userName.InvalidFormat)
    expect(messageFor('password', 'Required')).toBe(messages.field.password.Required)
    expect(messageFor('password', 'InvalidLength')).toBe(messages.field.password.InvalidLength)
    expect(messageFor('role', 'Required')).toBe(messages.field.role.Required)
    expect(messageFor('role', 'NotAllowed')).toBe(messages.field.role.NotAllowed)
  })

  it('returns server messages and form message', () => {
    expect(messages.server.emailRegistered).toBe('Šis el. pašto adresas jau užregistruotas.')
    expect(messages.server.userNameTaken).toBe('Toks naudotojo vardas jau naudojamas.')
    expect(messages.server.failure).toBe('Nepavyko užregistruoti. Bandykite vėliau.')
    expect(messages.server.success).toBe('Paskyra sukurta. Prisijunkite.')
    expect(messages.formNotSubmitted).toBe('Forma neišsiųsta. Ištaisykite pažymėtus laukus.')
  })
})
