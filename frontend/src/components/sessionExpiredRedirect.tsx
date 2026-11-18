import { useEffect } from 'react'
import { Navigate } from 'react-router-dom'
import { loginRequired } from '../helpers/loginMessage'
import { clearSessionMarker, hadSession } from '../helpers/sessionMarker'
import { sessionExpired } from '../helpers/stateMessage'

export default function SessionExpiredRedirect() {
  const message = hadSession() ? sessionExpired : loginRequired

  useEffect(() => {
    clearSessionMarker()
  }, [])

  return <Navigate to="/login" replace state={{ message }} />
}
