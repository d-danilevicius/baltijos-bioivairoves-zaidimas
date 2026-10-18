import React, { useState } from 'react'
import Button from '../components/button'
import Card from '../components/card'
import PageLayout from '../components/pageLayout'
import { visitorLinks } from '../helpers/navigationLinks'
import TextField from '../components/textField'
import RoleChoice from '../components/roleChoice'
import { validateRegistration } from '../helpers/validateRegistration'
import { messageFor, messages } from '../helpers/registrationMessages'
import { registerUser } from '../api/authApi'
import type { RegisterRequest } from '../types/auth'
import { useNavigate } from 'react-router-dom'

export default function RegisterPage() {
  const [email, setEmail] = useState('')
  const [userName, setUserName] = useState('')
  const [password, setPassword] = useState('')
  const [role, setRole] = useState<'Student' | 'Teacher' | undefined>(undefined)
  const [fieldErrors, setFieldErrors] = useState<Record<string, string>>({})
  const [topError, setTopError] = useState<string | undefined>(undefined)
  const [loading, setLoading] = useState(false)
  const navigate = useNavigate()

  async function onSubmit(e: React.FormEvent) {
    e.preventDefault()
    setTopError(undefined)

    const values: Partial<RegisterRequest> = { email, userName, password, role }
    const errs = validateRegistration(values)
    if (Object.keys(errs).length > 0) {
      const mapped: Record<string, string> = {}
      for (const k of Object.keys(errs)) {
        const key = k as keyof typeof errs
        const code = errs[key] as string | undefined
        mapped[k] = messageFor(key as keyof typeof errs, code) ?? ''
      }
      setFieldErrors(mapped)
      setTopError(messages.formNotSubmitted)
      // clear password when client-side validation fails
      setPassword('')
      return
    }

    setFieldErrors({})
    setLoading(true)
    const req: RegisterRequest = { email, userName, password, role: role as 'Student' | 'Teacher' }
    const res = await registerUser(req)
    setLoading(false)

    if (res.status === 201) {
      navigate('/login', { state: { message: messages.server.success } })
      return
    }

    // clear password on any failure
    setPassword('')

    // map server conflict
    const m: Record<string, string> = {}
    if (res.status === 409 && res.body && res.body.errors) {
      if (res.body.errors.email) m.email = messages.server.emailRegistered
      if (res.body.errors.userName) m.userName = messages.server.userNameTaken
    }
    if (res.status === 409) {
      setFieldErrors(m)
      setTopError(messages.formNotSubmitted)
      return
    }

    if (res.status === 400) {
      const m: Record<string, string> = {}
      for (const key of Object.keys(res.body.errors ?? {})) {
        const k = key as keyof RegisterRequest
        const codes = res.body.errors?.[key] as string[] | undefined
        m[key] = messageFor(k, codes?.[0]) ?? ''
      }
      setFieldErrors(m)
      setTopError(messages.formNotSubmitted)
      return
    }

    // network/server failure
    setTopError(messages.server.failure)
  }

  return (
    <PageLayout links={visitorLinks}>
      <Card title="Registracija">
        <form onSubmit={onSubmit}>
          {topError && (
            <div role="alert" className="mb-4 text-error">
              {topError}
            </div>
          )}

          <TextField id="email" label="El. paštas" value={email} onChange={setEmail} error={fieldErrors.email} autoComplete="email" />
          <TextField id="userName" label="Naudotojo vardas" value={userName} onChange={setUserName} error={fieldErrors.userName} autoComplete="username" />
          <TextField id="password" label="Slaptažodis" type="password" value={password} onChange={setPassword} error={fieldErrors.password} autoComplete="new-password" />
          <RoleChoice value={role} onChange={setRole} error={fieldErrors.role} />

          <Button type="submit" disabled={loading} fullWidth>
            Registruotis
          </Button>
        </form>
      </Card>
    </PageLayout>
  )
}
