import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { logoutUser } from '../api/authApi'
import Button from '../components/button'
import Card from '../components/card'
import PageLayout from '../components/pageLayout'
import { getUserLinks } from '../helpers/navigationLinks'
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
    <PageLayout links={getUserLinks(user.role)}>
      <Card title="Profilis">
        {topError && (
          <div role="alert" className="mb-4 text-error">
            {topError}
          </div>
        )}

        <p className="mb-2 break-all">
          <strong>Naudotojo vardas:</strong> {user.userName}
        </p>
        <p className="mb-2 break-all">
          <strong>El. paštas:</strong> {user.email}
        </p>
        <p className="mb-4">
          <strong>Rolė:</strong> {getRoleName(user.role)}
        </p>

        <Button type="button" onClick={onLogout} disabled={loading}>
          Atsijungti
        </Button>
      </Card>
    </PageLayout>
  )
}
