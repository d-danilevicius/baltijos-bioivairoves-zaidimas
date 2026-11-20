import { HttpStatus } from '../helpers/httpStatus'
import type { ProblemDetailsWithErrors } from '../types/auth'
import type { Submission, SubmissionRequest } from '../types/submission'
import { toFailure, type Failure } from './apiFailure'
import { apiBase } from './apiBase'
import { fetchCsrfToken } from './authApi'

export type SubmitAnswerResult =
  | { status: typeof HttpStatus.Created; body: Submission }
  | { status: typeof HttpStatus.BadRequest; body: ProblemDetailsWithErrors }
  | { status: typeof HttpStatus.Unauthorized }
  | { status: typeof HttpStatus.Forbidden }
  | { status: typeof HttpStatus.NotFound }
  | { status: typeof HttpStatus.Conflict }
  | Failure

export async function submitAnswer(taskId: string, request: SubmissionRequest): Promise<SubmitAnswerResult> {
  try {
    const res = await fetch(apiBase + `/api/my/tasks/${encodeURIComponent(taskId)}/submissions`, {
      method: 'POST',
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', 'X-CSRF-TOKEN': await fetchCsrfToken() },
      body: JSON.stringify(request),
    })
    if (res.status === HttpStatus.Created) {
      return { status: HttpStatus.Created, body: (await res.json()) as Submission }
    }
    if (res.status === HttpStatus.BadRequest) {
      return { status: HttpStatus.BadRequest, body: (await res.json()) as ProblemDetailsWithErrors }
    }
    if (
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
