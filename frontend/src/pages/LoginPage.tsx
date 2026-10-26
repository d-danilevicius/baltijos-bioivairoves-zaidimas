import { FormEvent, useState } from 'react'
import { Link, useLocation, useNavigate } from 'react-router-dom'
import Button from '../components/button'
import Card from '../components/card'
import PageLayout from '../components/pageLayout'
import { visitorLinks } from '../helpers/navigationLinks'
import TextField from '../components/textField'
import { loginUser } from '../api/authApi'
import {
  accountBlocked,
  emailRequired,
  invalidCredentials,
  loginFailedLater,
  passwordRequired,
} from '../helpers/loginMessages'
import { type LoginFieldErrors, validateLogin } from '../helpers/validateLogin'
import { markSessionStarted } from '../helpers/sessionMarker'
import { getStartingScreen } from '../helpers/startingScreen'

export default function LoginPage() {
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [fieldErrors, setFieldErrors] = useState<LoginFieldErrors>({})
  const [topError, setTopError] = useState<string | undefined>(undefined)
  const [loading, setLoading] = useState(false)
  const navigate = useNavigate()
  const loc = useLocation()
  const state = (loc.state as { message?: string } | null) ?? {}

  async function onSubmit(e: FormEvent) {
    e.preventDefault()
    setTopError(undefined)

    const clientErrors = validateLogin(email, password)
    if (Object.keys(clientErrors).length > 0) {
      setFieldErrors(clientErrors)
      setPassword('')
      return
    }

    setFieldErrors({})
    setLoading(true)
    const res = await loginUser({ email, password })
    setLoading(false)

    if (res.status === 200) {
      markSessionStarted()
      navigate(getStartingScreen(res.body.role))
      return
    }

    setPassword('')

    if (res.status === 401) {
      setTopError(invalidCredentials)
      return
    }

    if (res.status === 403) {
      setTopError(accountBlocked)
      return
    }

    if (res.status === 400) {
      const errors = res.body.errors ?? {}
      const mapped: Record<string, string> = {}
      if (errors.email?.includes('Required')) mapped.email = emailRequired
      if (errors.password?.includes('Required')) mapped.password = passwordRequired
      setFieldErrors(mapped)
      return
    }

    setTopError(loginFailedLater)
  }

  return (
    <PageLayout links={visitorLinks}>
      <Card title="Prisijungimas">
        <form onSubmit={onSubmit}>
          {state.message && (
            <div role="status" className="mb-4 text-brand">
              {state.message}
            </div>
          )}
          {topError && (
            <div role="alert" className="mb-4 text-error">
              {topError}
            </div>
          )}

          <TextField
            id="email"
            label="El. paštas"
            value={email}
            onChange={setEmail}
            error={fieldErrors.email}
            autoComplete="email"
          />
          <TextField
            id="password"
            label="Slaptažodis"
            type="password"
            value={password}
            onChange={setPassword}
            error={fieldErrors.password}
            autoComplete="current-password"
          />

          <Button type="submit" disabled={loading} fullWidth>
            Prisijungti
          </Button>
        </form>
        <p className="mt-4 text-center">
          Dar neturite paskyros?{' '}
          <Link to="/register" className="font-semibold text-brand underline">
            Registruotis
          </Link>
        </p>
      </Card>
    </PageLayout>
  )
}
