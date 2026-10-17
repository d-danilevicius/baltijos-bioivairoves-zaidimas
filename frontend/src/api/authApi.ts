import { RegisterRequest, RegisterResponse201, ProblemDetailsWithErrors } from '../types/auth'

const base = (import.meta.env.VITE_API_URL ?? '') as string

export type RegisterResult =
  | { status: 201; body: RegisterResponse201 }
  | { status: 400; body: ProblemDetailsWithErrors }
  | { status: 409; body: ProblemDetailsWithErrors }
  | { status: 'network' | 'server'; message: string }

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
