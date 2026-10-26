import {
  CurrentUser,
  LoggedInUser,
  LoginRequest,
  ProblemDetailsWithErrors,
  RegisterRequest,
  RegisterResponse201,
} from '../types/auth'

const base = (import.meta.env.VITE_API_URL ?? '') as string

export type RegisterResult =
  | { status: 201; body: RegisterResponse201 }
  | { status: 400; body: ProblemDetailsWithErrors }
  | { status: 409; body: ProblemDetailsWithErrors }
  | { status: 'network' | 'server'; message: string }

export type LoginResult =
  | { status: 200; body: LoggedInUser }
  | { status: 400; body: ProblemDetailsWithErrors }
  | { status: 401; body: ProblemDetailsWithErrors }
  | { status: 403; body: ProblemDetailsWithErrors }
  | { status: 'network' | 'server'; message: string }

export type LogoutResult = { status: 204 } | { status: 401 } | { status: 'network' | 'server'; message: string }

export type CurrentUserResult =
  { status: 200; body: CurrentUser } | { status: 401 } | { status: 'network' | 'server'; message: string }

export async function registerUser(req: RegisterRequest): Promise<RegisterResult> {
  try {
    const res = await fetch(base + '/api/auth/register', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(req),
    })

    if (res.status === 201) {
      const body = (await res.json()) as RegisterResponse201
      return { status: 201, body }
    }

    if (res.status === 400 || res.status === 409) {
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
  const res = await fetch(base + '/api/auth/csrf', {
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
    const res = await fetch(base + '/api/auth/login', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      credentials: 'include',
      body: JSON.stringify(req),
    })

    if (res.status === 200) {
      const body = (await res.json()) as LoggedInUser
      return { status: 200, body }
    }

    if (res.status === 400 || res.status === 401 || res.status === 403) {
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
    const res = await fetch(base + '/api/auth/me', {
      method: 'GET',
      credentials: 'include',
    })

    if (res.status === 200) {
      const body = (await res.json()) as CurrentUser
      return { status: 200, body }
    }

    if (res.status === 401) {
      return { status: 401 }
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
    const res = await fetch(base + '/api/auth/logout', {
      method: 'POST',
      credentials: 'include',
      headers: { 'X-CSRF-TOKEN': csrfToken },
    })

    if (res.status === 204) {
      return { status: 204 }
    }

    if (res.status === 401) {
      return { status: 401 }
    }

    return { status: 'server', message: 'Server error' }
  } catch (e) {
    const err = e as Error | undefined
    const message = err?.message ?? 'Network error'
    const status = message === 'Server error' ? 'server' : 'network'
    return { status, message }
  }
}
