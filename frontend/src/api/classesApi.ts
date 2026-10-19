import type { ProblemDetailsWithErrors } from '../types/auth'
import type { SchoolClass } from '../types/classes'
import { fetchCsrfToken } from './authApi'

const base = (import.meta.env.VITE_API_URL ?? '') as string

type Failure = { status: 'network' | 'server'; message: string }

export type ListClassesResult = { status: 200; body: SchoolClass[] } | { status: 401 } | Failure

export type GetClassResult =
  | { status: 200; body: SchoolClass }
  | { status: 401 }
  | { status: 403 }
  | { status: 404 }
  | Failure

export type CreateClassResult =
  | { status: 201; body: SchoolClass }
  | { status: 400; body: ProblemDetailsWithErrors }
  | { status: 401 }
  | Failure

export type DeleteClassResult = { status: 204 } | { status: 401 } | { status: 403 } | { status: 404 } | Failure

function toFailure(e: unknown): Failure {
  const message = (e as Error | undefined)?.message ?? 'Network error'
  return { status: message === 'Server error' ? 'server' : 'network', message }
}

export async function listClasses(): Promise<ListClassesResult> {
  try {
    const res = await fetch(base + '/api/classes', { credentials: 'include' })
    if (res.status === 200) {
      return { status: 200, body: (await res.json()) as SchoolClass[] }
    }
    if (res.status === 401) {
      return { status: 401 }
    }
    return { status: 'server', message: 'Server error' }
  } catch (e) {
    return toFailure(e)
  }
}

export async function getClass(id: string): Promise<GetClassResult> {
  try {
    const res = await fetch(base + `/api/classes/${encodeURIComponent(id)}`, { credentials: 'include' })
    if (res.status === 200) {
      return { status: 200, body: (await res.json()) as SchoolClass }
    }
    if (res.status === 401 || res.status === 403 || res.status === 404) {
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
    if (res.status === 201) {
      return { status: 201, body: (await res.json()) as SchoolClass }
    }
    if (res.status === 400) {
      return { status: 400, body: (await res.json()) as ProblemDetailsWithErrors }
    }
    if (res.status === 401) {
      return { status: 401 }
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
    if (res.status === 204) {
      return { status: 204 }
    }
    if (res.status === 401 || res.status === 403 || res.status === 404) {
      return { status: res.status }
    }
    return { status: 'server', message: 'Server error' }
  } catch (e) {
    return toFailure(e)
  }
}
