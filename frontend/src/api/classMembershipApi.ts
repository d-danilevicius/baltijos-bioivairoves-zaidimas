import type { ProblemDetailsWithErrors } from '../types/auth'
import type { ClassChangeConfirmation, JoinClassResponse } from '../types/classMembership'
import { fetchCsrfToken } from './authApi'
import { toFailure, type Failure } from './apiFailure'
import { HttpStatus } from '../helpers/httpStatus'
import { apiBase } from './apiBase'

export type JoinClassResult =
  | { status: typeof HttpStatus.Ok; body: JoinClassResponse }
  | { status: typeof HttpStatus.BadRequest; body: ProblemDetailsWithErrors }
  | { status: typeof HttpStatus.Unauthorized }
  | { status: typeof HttpStatus.Conflict; body: ClassChangeConfirmation }
  | Failure

export async function joinClass(code: string, confirmChange: boolean): Promise<JoinClassResult> {
  try {
    const res = await fetch(apiBase + '/api/classes/join', {
      method: 'POST',
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', 'X-CSRF-TOKEN': await fetchCsrfToken() },
      body: JSON.stringify({ code, confirmChange }),
    })
    if (res.status === HttpStatus.Ok) {
      return { status: HttpStatus.Ok, body: (await res.json()) as JoinClassResponse }
    }
    if (res.status === HttpStatus.BadRequest) {
      return { status: HttpStatus.BadRequest, body: (await res.json()) as ProblemDetailsWithErrors }
    }
    if (res.status === HttpStatus.Conflict) {
      return { status: HttpStatus.Conflict, body: (await res.json()) as ClassChangeConfirmation }
    }
    if (res.status === HttpStatus.Unauthorized) {
      return { status: HttpStatus.Unauthorized }
    }
    return { status: 'server', message: 'Server error' }
  } catch (e) {
    return toFailure(e)
  }
}
