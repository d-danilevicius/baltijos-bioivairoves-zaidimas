import { FieldErrors } from './validateRegistration'

export const messages = {
  field: {
    email: {
      Required: 'Įveskite el. pašto adresą.',
      InvalidFormat: 'Įveskite teisingą el. pašto adresą.',
      TooLong: 'El. paštas negali būti ilgesnis nei 254 simboliai.',
    },
    userName: {
      Required: 'Įveskite naudotojo vardą.',
      InvalidFormat: 'Naudotojo vardą turi sudaryti 3–30 raidžių, skaičių ar simbolių "_".',
    },
    password: {
      Required: 'Įveskite slaptažodį.',
      InvalidLength: 'Slaptažodį turi sudaryti nuo 8 iki 128 simbolių.',
    },
    role: {
      Required: 'Pasirinkite rolę.',
      NotAllowed: 'Pasirinkite rolę.',
    },
  },

  server: {
    emailRegistered: 'Šis el. pašto adresas jau užregistruotas.',
    userNameTaken: 'Toks naudotojo vardas jau naudojamas.',
    failure: 'Nepavyko užregistruoti. Bandykite vėliau.',
    success: 'Paskyra sukurta. Prisijunkite.',
  },

  formNotSubmitted: 'Forma neišsiųsta. Ištaisykite pažymėtus laukus.',
}

export function messageFor(field: keyof FieldErrors | undefined, code: string | undefined): string | undefined {
  if (!field || !code) return undefined
  const f = field as keyof typeof messages.field
  const mapping = (messages.field as Record<string, Record<string, string>>)[f as string]
  return mapping ? mapping[code] : undefined
}
