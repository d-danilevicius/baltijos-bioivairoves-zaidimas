import {
  CurrentUser,
  LoggedInUser,
  LoginRequest,
  ProblemDetailsWithErrors,
  RegisterRequest,
  RegisterResponse201,
} from '../types/auth'
import { HttpStatus } from '../helpers/httpStatus'
import { apiBase } from './apiBase'

export type RegisterResult =
  | { status: typeof HttpStatus.Created; body: RegisterResponse201 }
  | { status: typeof HttpStatus.BadRequest; body: ProblemDetailsWithErrors }
  | { status: typeof HttpStatus.Conflict; body: ProblemDetailsWithErrors }
  | { status: 'network' | 'server'; message: string }

export type LoginResult =
  | { status: typeof HttpStatus.Ok; body: LoggedInUser }
  | { status: typeof HttpStatus.BadRequest; body: ProblemDetailsWithErrors }
  | { status: typeof HttpStatus.Unauthorized; body: ProblemDetailsWithErrors }
  | { status: typeof HttpStatus.Forbidden; body: ProblemDetailsWithErrors }
  | { status: 'network' | 'server'; message: string }

export type LogoutResult =
  | { status: typeof HttpStatus.NoContent }
  | { status: typeof HttpStatus.Unauthorized }
  | { status: 'network' | 'server'; message: string }

export type CurrentUserResult =
  | { status: typeof HttpStatus.Ok; body: CurrentUser }
  | { status: typeof HttpStatus.Unauthorized }
  | { status: 'network' | 'server'; message: string }

export async function registerUser(req: RegisterRequest): Promise<RegisterResult> {
  try {
    const res = await fetch(apiBase + '/api/auth/register', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(req),
    })

    if (res.status === HttpStatus.Created) {
      const body = (await res.json()) as RegisterResponse201
      return { status: HttpStatus.Created, body }
    }

    if (res.status === HttpStatus.BadRequest || res.status === HttpStatus.Conflict) {
      const body = (await res.json()) as ProblemDetailsWithErrors
      return { status: res.status, body }
    }

    return { status: 'server', message: 'Server error' }
  } catch (e) {
    const err = e as Error | undefined
    return { status: 'network', message: err?.message ?? 'Network error' }
  }
}

export async function fetchCsrfToken(): Promise<string> {
  const res = await fetch(apiBase + '/api/auth/csrf', {
    method: 'GET',
    credentials: 'include',
  })

  if (!res.ok) {
    throw new Error('Server error')
  }

  const body = (await res.json()) as { token?: string }
  if (!body.token) {
    throw new Error('Server error')
  }

  return body.token
}

export async function loginUser(req: LoginRequest): Promise<LoginResult> {
  try {
    const res = await fetch(apiBase + '/api/auth/login', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      credentials: 'include',
      body: JSON.stringify(req),
    })

    if (res.status === HttpStatus.Ok) {
      const body = (await res.json()) as LoggedInUser
      return { status: HttpStatus.Ok, body }
    }

    if (
      res.status === HttpStatus.BadRequest ||
      res.status === HttpStatus.Unauthorized ||
      res.status === HttpStatus.Forbidden
    ) {
      const body = (await res.json()) as ProblemDetailsWithErrors
      return { status: res.status, body }
    }

    return { status: 'server', message: 'Server error' }
  } catch (e) {
    const err = e as Error | undefined
    return { status: 'network', message: err?.message ?? 'Network error' }
  }
}

export async function getCurrentUser(): Promise<CurrentUserResult> {
  try {
    const res = await fetch(apiBase + '/api/auth/me', {
      method: 'GET',
      credentials: 'include',
    })

    if (res.status === HttpStatus.Ok) {
      const body = (await res.json()) as CurrentUser
      return { status: HttpStatus.Ok, body }
    }

    if (res.status === HttpStatus.Unauthorized) {
      return { status: HttpStatus.Unauthorized }
    }

    return { status: 'server', message: 'Server error' }
  } catch (e) {
    const err = e as Error | undefined
    return { status: 'network', message: err?.message ?? 'Network error' }
  }
}

export async function logoutUser(): Promise<LogoutResult> {
  try {
    const csrfToken = await fetchCsrfToken()
    const res = await fetch(apiBase + '/api/auth/logout', {
      method: 'POST',
      credentials: 'include',
      headers: { 'X-CSRF-TOKEN': csrfToken },
    })

    if (res.status === HttpStatus.NoContent) {
      return { status: HttpStatus.NoContent }
    }

    if (res.status === HttpStatus.Unauthorized) {
      return { status: HttpStatus.Unauthorized }
    }

    return { status: 'server', message: 'Server error' }
  } catch (e) {
    const err = e as Error | undefined
    const message = err?.message ?? 'Network error'
    const status = message === 'Server error' ? 'server' : 'network'
    return { status, message }
  }
}
