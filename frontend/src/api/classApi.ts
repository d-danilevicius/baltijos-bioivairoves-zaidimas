import type { ProblemDetailsWithErrors } from '../types/auth'
import type { SchoolClass } from '../types/classes'
import { fetchCsrfToken } from './authApi'
import { HttpStatus } from '../helpers/httpStatus'

const base = (import.meta.env.VITE_API_URL ?? '') as string

type Failure = { status: 'network' | 'server'; message: string }

export type ListClassesResult =
  { status: typeof HttpStatus.Ok; body: SchoolClass[] } | { status: typeof HttpStatus.Unauthorized } | Failure

export type GetClassResult =
  | { status: typeof HttpStatus.Ok; body: SchoolClass }
  | { status: typeof HttpStatus.Unauthorized }
  | { status: typeof HttpStatus.Forbidden }
  | { status: typeof HttpStatus.NotFound }
  | Failure

export type CreateClassResult =
  | { status: typeof HttpStatus.Created; body: SchoolClass }
  | { status: typeof HttpStatus.BadRequest; body: ProblemDetailsWithErrors }
  | { status: typeof HttpStatus.Unauthorized }
  | Failure

export type DeleteClassResult =
  | { status: typeof HttpStatus.NoContent }
  | { status: typeof HttpStatus.Unauthorized }
  | { status: typeof HttpStatus.Forbidden }
  | { status: typeof HttpStatus.NotFound }
  | Failure

function toFailure(e: unknown): Failure {
  const message = (e as Error | undefined)?.message ?? 'Network error'
  return { status: message === 'Server error' ? 'server' : 'network', message }
}

export async function listClasses(): Promise<ListClassesResult> {
  try {
    const res = await fetch(base + '/api/classes', { credentials: 'include' })
    if (res.status === HttpStatus.Ok) {
      return { status: HttpStatus.Ok, body: (await res.json()) as SchoolClass[] }
    }
    if (res.status === HttpStatus.Unauthorized) {
      return { status: HttpStatus.Unauthorized }
    }
    return { status: 'server', message: 'Server error' }
  } catch (e) {
    return toFailure(e)
  }
}

export async function getClass(id: string): Promise<GetClassResult> {
  try {
    const res = await fetch(base + `/api/classes/${encodeURIComponent(id)}`, { credentials: 'include' })
    if (res.status === HttpStatus.Ok) {
      return { status: HttpStatus.Ok, body: (await res.json()) as SchoolClass }
    }
    if (
      res.status === HttpStatus.Unauthorized ||
      res.status === HttpStatus.Forbidden ||
      res.status === HttpStatus.NotFound
    ) {
      return { status: res.status }
    }
    return { status: 'server', message: 'Server error' }
  } catch (e) {
    return toFailure(e)
  }
}

export async function createClass(name: string): Promise<CreateClassResult> {
  try {
    const res = await fetch(base + '/api/classes', {
      method: 'POST',
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', 'X-CSRF-TOKEN': await fetchCsrfToken() },
      body: JSON.stringify({ name }),
    })
    if (res.status === HttpStatus.Created) {
      return { status: HttpStatus.Created, body: (await res.json()) as SchoolClass }
    }
    if (res.status === HttpStatus.BadRequest) {
      return { status: HttpStatus.BadRequest, body: (await res.json()) as ProblemDetailsWithErrors }
    }
    if (res.status === HttpStatus.Unauthorized) {
      return { status: HttpStatus.Unauthorized }
    }
    return { status: 'server', message: 'Server error' }
  } catch (e) {
    return toFailure(e)
  }
}

export async function deleteClass(id: string): Promise<DeleteClassResult> {
  try {
    const res = await fetch(base + `/api/classes/${encodeURIComponent(id)}`, {
      method: 'DELETE',
      credentials: 'include',
      headers: { 'X-CSRF-TOKEN': await fetchCsrfToken() },
    })
    if (res.status === HttpStatus.NoContent) {
      return { status: HttpStatus.NoContent }
    }
    if (
      res.status === HttpStatus.Unauthorized ||
      res.status === HttpStatus.Forbidden ||
      res.status === HttpStatus.NotFound
    ) {
      return { status: res.status }
    }
    return { status: 'server', message: 'Server error' }
  } catch (e) {
    return toFailure(e)
  }
}
