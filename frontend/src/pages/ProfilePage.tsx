import { useEffect, useState } from 'react'
import { useLocation, useNavigate } from 'react-router-dom'
import { logoutUser } from '../api/authApi'
import { loginRequired, logoutFailed } from '../helpers/loginMessages'
import { getRoleName } from '../helpers/roleNames'
import type { LoggedInUser } from '../types/auth'

export default function ProfilePage() {
  const navigate = useNavigate()
  const location = useLocation()
  const [loading, setLoading] = useState(false)
  const [topError, setTopError] = useState<string | undefined>(undefined)
  const state = (location.state as { user?: LoggedInUser } | null) ?? {}
  const user = state.user

  useEffect(() => {
    if (!user) {
      navigate('/login', { state: { message: loginRequired }, replace: true })
    }
  }, [navigate, user])

  if (!user) {
    return null
  }

  async function onLogout() {
    setTopError(undefined)
    setLoading(true)
    const res = await logoutUser()
    setLoading(false)

    if (res.status === 204 || res.status === 401) {
      navigate('/login', { replace: true })
      return
    }

    setTopError(logoutFailed)
  }

  return (
    <div style={{ maxWidth: 480, margin: '0 auto' }}>
      <h1>Profilis</h1>
      {topError && <div role="alert" style={{ color: 'red', marginBottom: '1rem' }}>{topError}</div>}

      <div style={{ marginBottom: '0.5rem' }}>
        <strong>Naudotojo vardas:</strong> {user.userName}
      </div>
      <div style={{ marginBottom: '1rem' }}>
        <strong>Rolė:</strong> {getRoleName(user.role)}
      </div>

      <button type="button" onClick={onLogout} disabled={loading} style={{ padding: '0.5rem 1rem', fontSize: '1rem' }}>
        Atsijungti
      </button>
    </div>
  )
}
