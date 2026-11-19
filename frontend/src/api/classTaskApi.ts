import type { ProblemDetailsWithErrors } from '../types/auth'
import type { ClassTask, GbifTasksRequest, GbifTasksResponse, TeacherTaskRequest } from '../types/classTask'
import { fetchCsrfToken } from './authApi'
import { toFailure, type Failure } from './apiFailure'
import { HttpStatus } from '../helpers/httpStatus'
import { apiBase } from './apiBase'

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

export type CreateTeacherTaskResult =
  | { status: typeof HttpStatus.Created; body: ClassTask }
  | { status: typeof HttpStatus.BadRequest; body: ProblemDetailsWithErrors }
  | { status: typeof HttpStatus.Unauthorized }
  | { status: typeof HttpStatus.Forbidden }
  | { status: typeof HttpStatus.NotFound }
  | Failure

export type DeleteClassTaskResult =
  | { status: typeof HttpStatus.NoContent }
  | { status: typeof HttpStatus.Unauthorized }
  | { status: typeof HttpStatus.Forbidden }
  | { status: typeof HttpStatus.NotFound }
  | { status: typeof HttpStatus.Conflict }
  | Failure

export async function listClassTasks(classId: string): Promise<ListClassTasksResult> {
  try {
    const res = await fetch(apiBase + `/api/classes/${encodeURIComponent(classId)}/tasks`, { credentials: 'include' })
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
    const res = await fetch(apiBase + `/api/classes/${encodeURIComponent(classId)}/gbif-tasks`, {
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

export async function createTeacherTask(
  classId: string,
  request: TeacherTaskRequest,
): Promise<CreateTeacherTaskResult> {
  try {
    const res = await fetch(apiBase + `/api/classes/${encodeURIComponent(classId)}/tasks`, {
      method: 'POST',
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', 'X-CSRF-TOKEN': await fetchCsrfToken() },
      body: JSON.stringify(request),
    })
    if (res.status === HttpStatus.Created) {
      return { status: HttpStatus.Created, body: (await res.json()) as ClassTask }
    }
    if (res.status === HttpStatus.BadRequest) {
      return { status: HttpStatus.BadRequest, body: (await res.json()) as ProblemDetailsWithErrors }
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

export async function deleteClassTask(classId: string, taskId: string): Promise<DeleteClassTaskResult> {
  try {
    const res = await fetch(
      apiBase + `/api/classes/${encodeURIComponent(classId)}/tasks/${encodeURIComponent(taskId)}`,
      {
        method: 'DELETE',
        credentials: 'include',
        headers: { 'X-CSRF-TOKEN': await fetchCsrfToken() },
      },
    )
    if (
      res.status === HttpStatus.NoContent ||
      res.status === HttpStatus.Unauthorized ||
      res.status === HttpStatus.Forbidden ||
      res.status === HttpStatus.NotFound ||
      res.status === HttpStatus.Conflict
    ) {
      return { status: res.status }
    }
    return { status: 'server', message: 'Server error' }
  } catch (e) {
    return toFailure(e)
  }
}
