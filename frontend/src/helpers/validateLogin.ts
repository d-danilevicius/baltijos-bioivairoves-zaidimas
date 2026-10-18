import { emailRequired, passwordRequired } from './loginMessages'

export interface LoginFieldErrors {
  email?: string
  password?: string
}

export function validateLogin(email: string, password: string): LoginFieldErrors {
  const errors: LoginFieldErrors = {}

  if (email.trim() === '') {
    errors.email = emailRequired
  }

  if (password.trim() === '') {
    errors.password = passwordRequired
  }

  return errors
}
