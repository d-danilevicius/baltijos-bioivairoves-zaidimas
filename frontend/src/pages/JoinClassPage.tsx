import { FormEvent, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { joinClass, type JoinClassResult } from '../api/classMembershipApi'
import Button from '../components/button'
import Card from '../components/card'
import ConfirmDialog from '../components/confirmDialog'
import PageLayout from '../components/pageLayout'
import SessionExpiredRedirect from '../components/sessionExpiredRedirect'
import TextField from '../components/textField'
import { HttpStatus } from '../helpers/httpStatus'
import {
  alreadyInClass,
  changeClassConfirm,
  changeClassQuestion,
  classCodeInvalid,
  classCodeNotFound,
  classCodeRequired,
  joinClassTitle,
  joinedClass,
} from '../helpers/joinClassMessage'
import { studentLinks } from '../helpers/navigationLink'
import { serverError } from '../helpers/stateMessage'
import { normalizeClassCode, validateClassCode } from '../helpers/validateClassCode'
import type { ClassChangeConfirmation } from '../types/classMembership'

function getCodeError(codes: string[]): string | undefined {
  if (codes.includes('Required')) return classCodeRequired
  if (codes.includes('InvalidFormat')) return classCodeInvalid
  if (codes.includes('NotFound')) return classCodeNotFound
  return undefined
}

export default function JoinClassPage() {
  const [code, setCode] = useState('')
  const [codeError, setCodeError] = useState<string | undefined>(undefined)
  const [topError, setTopError] = useState<string | undefined>(undefined)
  const [loading, setLoading] = useState(false)
  const [expired, setExpired] = useState(false)
  const [confirmation, setConfirmation] = useState<ClassChangeConfirmation | undefined>(undefined)
  const navigate = useNavigate()

  function showResult(res: JoinClassResult) {
    if (res.status === HttpStatus.Ok) {
      const message = res.body.alreadyMember ? alreadyInClass : joinedClass(res.body.className)
      navigate('/profile', { state: { message } })
    } else if (res.status === HttpStatus.Conflict) {
      setConfirmation(res.body)
    } else if (res.status === HttpStatus.Unauthorized) {
      setExpired(true)
    } else if (res.status === HttpStatus.BadRequest) {
      const error = getCodeError(res.body.errors?.code ?? [])
      if (error) setCodeError(error)
      else setTopError(serverError)
    } else {
      setTopError(serverError)
    }
  }

  async function send(confirmChange: boolean) {
    setLoading(true)
    const res = await joinClass(normalizeClassCode(code), confirmChange)
    setLoading(false)
    showResult(res)
  }

  async function onSubmit(e: FormEvent) {
    e.preventDefault()
    setTopError(undefined)

    const clientError = validateClassCode(code)
    setCodeError(clientError)
    if (clientError) return

    await send(false)
  }

  async function onConfirmChange() {
    setConfirmation(undefined)
    await send(true)
  }

  if (expired) return <SessionExpiredRedirect />

  return (
    <PageLayout links={studentLinks}>
      <Card title={joinClassTitle}>
        <form onSubmit={onSubmit}>
          {topError && (
            <div role="alert" className="mb-4 text-error">
              {topError}
            </div>
          )}
          <TextField id="code" label="Klasės kodas" value={code} onChange={setCode} error={codeError} />
          <Button type="submit" disabled={loading} fullWidth>
            Prisijungti
          </Button>
        </form>
      </Card>
      <ConfirmDialog
        open={confirmation !== undefined}
        message={confirmation ? changeClassQuestion(confirmation.currentClassName, confirmation.newClassName) : ''}
        confirmLabel={changeClassConfirm}
        onConfirm={onConfirmChange}
        onCancel={() => setConfirmation(undefined)}
      />
    </PageLayout>
  )
}
