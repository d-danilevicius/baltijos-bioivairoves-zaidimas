import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { logoutUser } from '../api/authApi'
import { useCurrentUser } from '../hooks/useCurrentUser'
import { logoutFailed } from '../helpers/loginMessages'
import { getRoleName } from '../helpers/roleNames'
import { clearSessionMarker } from '../helpers/sessionMarker'

export default function ProfilePage() {
  const navigate = useNavigate()
  const user = useCurrentUser()
  const [loading, setLoading] = useState(false)
  const [topError, setTopError] = useState<string | undefined>(undefined)

  async function onLogout() {
    setTopError(undefined)
    setLoading(true)
    const res = await logoutUser()
    setLoading(false)

    if (res.status === 204 || res.status === 401) {
      clearSessionMarker()
      navigate('/login', { replace: true })
      return
    }

    setTopError(logoutFailed)
  }

  return (
    <div style={{ maxWidth: 480, margin: '0 auto' }}>
      <h1>Profilis</h1>
      {topError && <div role="alert" className="error-text" style={{ marginBottom: '1rem' }}>{topError}</div>}

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
