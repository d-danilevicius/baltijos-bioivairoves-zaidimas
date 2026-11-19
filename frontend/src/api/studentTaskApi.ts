import { HttpStatus } from '../helpers/httpStatus'
import type { StudentTaskList } from '../types/studentTask'
import { toFailure, type Failure } from './apiFailure'
import { apiBase } from './apiBase'

export type GetMyTasksResult =
  | { status: typeof HttpStatus.Ok; body: StudentTaskList }
  | { status: typeof HttpStatus.Unauthorized }
  | { status: typeof HttpStatus.Forbidden }
  | Failure

export async function getMyTasks(): Promise<GetMyTasksResult> {
  try {
    const res = await fetch(apiBase + '/api/my/tasks', { credentials: 'include' })
    if (res.status === HttpStatus.Ok) {
      return { status: HttpStatus.Ok, body: (await res.json()) as StudentTaskList }
    }
    if (res.status === HttpStatus.Unauthorized || res.status === HttpStatus.Forbidden) {
      return { status: res.status }
    }
    return { status: 'server', message: 'Server error' }
  } catch (e) {
    return toFailure(e)
  }
}
