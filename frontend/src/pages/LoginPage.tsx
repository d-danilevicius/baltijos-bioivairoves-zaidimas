import { FormEvent, useState } from 'react'
import { useLocation, useNavigate } from 'react-router-dom'
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
      navigate('/profile', { state: { user: res.body } })
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
    <form onSubmit={onSubmit} style={{ maxWidth: 480, margin: '0 auto' }}>
      <h1>Prisijungimas</h1>
      {state.message && <div role="status" style={{ marginTop: '1rem', color: 'green' }}>{state.message}</div>}
      {topError && <div role="alert" style={{ color: 'red', marginBottom: '1rem' }}>{topError}</div>}

      <TextField id="email" label="El. paštas" value={email} onChange={setEmail} error={fieldErrors.email} autoComplete="email" />
      <TextField
        id="password"
        label="Slaptažodis"
        type="password"
        value={password}
        onChange={setPassword}
        error={fieldErrors.password}
        autoComplete="current-password"
      />

      <button type="submit" disabled={loading} style={{ padding: '0.5rem 1rem', fontSize: '1rem' }}>
        Prisijungti
      </button>
    </form>
  )
}
