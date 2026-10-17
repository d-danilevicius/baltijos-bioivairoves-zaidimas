import { RegisterRequest } from '../types/auth'

export type FieldErrors = Partial<Record<keyof RegisterRequest, string>>

const emailRegex = /^(?:[A-Za-z0-9.!#$%&'*+/=?^_`{|}~-]+)@(?:[A-Za-z0-9-]+\.)+[A-Za-z]{2,}$/
const userNameRegex = /^[A-Za-z0-9_]{3,30}$/

export function validateRegistration(values: Partial<RegisterRequest>): FieldErrors {
  const errors: FieldErrors = {}

  // email
  if (!values.email || values.email.trim() === '') {
    errors.email = 'Required'
  } else if (values.email.length > 254) {
    errors.email = 'TooLong'
  } else if (!emailRegex.test(values.email)) {
    errors.email = 'InvalidFormat'
  }

  // userName
  if (!values.userName || values.userName.trim() === '') {
    errors.userName = 'Required'
  } else if (!userNameRegex.test(values.userName)) {
    errors.userName = 'InvalidFormat'
  }

  // password
  if (!values.password || values.password.trim() === '') {
    errors.password = 'Required'
  } else if (values.password.length < 8 || values.password.length > 128) {
    errors.password = 'InvalidLength'
  }

  // role
  if (!values.role || values.role.trim() === '') {
    errors.role = 'Required'
  } else if (values.role !== 'Student' && values.role !== 'Teacher') {
    errors.role = 'NotAllowed'
  }

  return errors
}
