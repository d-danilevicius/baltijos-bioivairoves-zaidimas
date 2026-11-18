import type { ProblemDetailsWithErrors } from '../types/auth'
import type { ClassTask, GbifTasksRequest, GbifTasksResponse } from '../types/classTask'
import { fetchCsrfToken } from './authApi'
import { toFailure, type Failure } from './apiFailure'
import { HttpStatus } from '../helpers/httpStatus'

const base = (import.meta.env.VITE_API_URL ?? '') as string

export type ListClassTasksResult =
  | { status: typeof HttpStatus.Ok; body: ClassTask[] }
  | { status: typeof HttpStatus.Unauthorized }
  | { status: typeof HttpStatus.Forbidden }
  | { status: typeof HttpStatus.NotFound }
  | Failure

export type CreateGbifTasksResult =
  | { status: typeof HttpStatus.Created; body: GbifTasksResponse }
  | { status: typeof HttpStatus.BadRequest; body: ProblemDetailsWithErrors }
  | { status: typeof HttpStatus.Unauthorized }
  | { status: typeof HttpStatus.Forbidden }
  | { status: typeof HttpStatus.NotFound }
  | { status: typeof HttpStatus.Conflict }
  | { status: typeof HttpStatus.UnprocessableEntity }
  | { status: typeof HttpStatus.BadGateway }
  | Failure

export async function listClassTasks(classId: string): Promise<ListClassTasksResult> {
  try {
    const res = await fetch(base + `/api/classes/${encodeURIComponent(classId)}/tasks`, { credentials: 'include' })
    if (res.status === HttpStatus.Ok) {
      return { status: HttpStatus.Ok, body: (await res.json()) as ClassTask[] }
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

export async function createGbifTasks(classId: string, request: GbifTasksRequest): Promise<CreateGbifTasksResult> {
  try {
    const res = await fetch(base + `/api/classes/${encodeURIComponent(classId)}/gbif-tasks`, {
      method: 'POST',
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', 'X-CSRF-TOKEN': await fetchCsrfToken() },
      body: JSON.stringify(request),
    })
    if (res.status === HttpStatus.Created) {
      return { status: HttpStatus.Created, body: (await res.json()) as GbifTasksResponse }
    }
    if (res.status === HttpStatus.BadRequest) {
      return { status: HttpStatus.BadRequest, body: (await res.json()) as ProblemDetailsWithErrors }
    }
    if (
      res.status === HttpStatus.Unauthorized ||
      res.status === HttpStatus.Forbidden ||
      res.status === HttpStatus.NotFound ||
      res.status === HttpStatus.Conflict ||
      res.status === HttpStatus.UnprocessableEntity ||
      res.status === HttpStatus.BadGateway
    ) {
      return { status: res.status }
    }
    return { status: 'server', message: 'Server error' }
  } catch (e) {
    return toFailure(e)
  }
}
