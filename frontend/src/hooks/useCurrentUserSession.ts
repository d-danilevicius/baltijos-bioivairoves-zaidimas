import { useEffect, useState } from 'react'
import { getCurrentUser } from '../api/authApi'
import { loginRequired } from '../helpers/loginMessage'
import { clearSessionMarker, hadSession } from '../helpers/sessionMarker'
import { sessionExpired } from '../helpers/stateMessage'
import type { CurrentUser } from '../types/auth'
import { HttpStatus } from '../helpers/httpStatus'

export type SessionState =
  | { status: 'loading' }
  | { status: 'ready'; user: CurrentUser }
  | { status: 'unauthenticated'; message: string }
  | { status: 'error' }

export function useCurrentUserSession(): { session: SessionState; retry: () => void } {
  const [session, setSession] = useState<SessionState>({ status: 'loading' })
  const [attempt, setAttempt] = useState(0)

  useEffect(() => {
    let cancelled = false

    async function load() {
      const res = await getCurrentUser()
      if (cancelled) return

      if (res.status === HttpStatus.Ok) {
        setSession({ status: 'ready', user: res.body })
        return
      }

      if (res.status === HttpStatus.Unauthorized) {
        const message = hadSession() ? sessionExpired : loginRequired
        clearSessionMarker()
        setSession({ status: 'unauthenticated', message })
        return
      }

      setSession({ status: 'error' })
    }

    void load()

    return () => {
      cancelled = true
    }
  }, [attempt])

  function retry() {
    setSession({ status: 'loading' })
    setAttempt((a) => a + 1)
  }

  return { session, retry }
}
