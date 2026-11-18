import { useEffect, useState } from 'react'
import { useLocation, useNavigate, useParams } from 'react-router-dom'
import { deleteClass, getClass } from '../api/classApi'
import Button from '../components/button'
import Card from '../components/card'
import ConfirmDialog from '../components/confirmDialog'
import ErrorState from '../components/errorState'
import LoadingState from '../components/loadingState'
import PageLayout from '../components/pageLayout'
import SessionExpiredRedirect from '../components/sessionExpiredRedirect'
import { classDeleted } from '../helpers/classMessages'
import { teacherLinks } from '../helpers/navigationLinks'
import { serverError } from '../helpers/stateMessages'
import type { SchoolClass } from '../types/classes'
import AccessDeniedPage from './AccessDeniedPage'
import NotFoundPage from './NotFoundPage'
import { HttpStatus } from '../helpers/httpStatus'

type DetailsState =
  | { status: 'loading' }
  | { status: 'ready'; schoolClass: SchoolClass }
  | { status: 'expired' }
  | { status: 'forbidden' }
  | { status: 'notFound' }
  | { status: 'error' }

export default function ClassDetailsPage() {
  const { id = '' } = useParams()
  const navigate = useNavigate()
  const message = (useLocation().state as { message?: string } | null)?.message
  const [state, setState] = useState<DetailsState>({ status: 'loading' })
  const [attempt, setAttempt] = useState(0)
  const [confirmOpen, setConfirmOpen] = useState(false)
  const [deleteError, setDeleteError] = useState(false)

  useEffect(() => {
    let cancelled = false

    async function load() {
      const res = await getClass(id)
      if (cancelled) return
      if (res.status === HttpStatus.Ok) setState({ status: 'ready', schoolClass: res.body })
      else if (res.status === HttpStatus.Unauthorized) setState({ status: 'expired' })
      else if (res.status === HttpStatus.Forbidden) setState({ status: 'forbidden' })
      else if (res.status === HttpStatus.NotFound) setState({ status: 'notFound' })
      else setState({ status: 'error' })
    }

    load()
    return () => {
      cancelled = true
    }
  }, [id, attempt])

  function retry() {
    setState({ status: 'loading' })
    setAttempt((value) => value + 1)
  }

  async function onConfirmDelete() {
    setConfirmOpen(false)
    const res = await deleteClass(id)
    if (res.status === HttpStatus.NoContent) {
      navigate('/classes', { state: { message: classDeleted } })
      return
    }
    if (res.status === HttpStatus.Unauthorized) setState({ status: 'expired' })
    else if (res.status === HttpStatus.Forbidden) setState({ status: 'forbidden' })
    else if (res.status === HttpStatus.NotFound) setState({ status: 'notFound' })
    else setDeleteError(true)
  }

  if (state.status === 'expired') return <SessionExpiredRedirect />
  if (state.status === 'forbidden') return <AccessDeniedPage />
  if (state.status === 'notFound') return <NotFoundPage />

  return (
    <PageLayout links={teacherLinks}>
      {state.status === 'loading' && <LoadingState />}
      {state.status === 'error' && <ErrorState onRetry={retry} />}
      {state.status === 'ready' && (
        <Card title={state.schoolClass.name}>
          {message && (
            <div role="status" className="mb-4 text-brand">
              {message}
            </div>
          )}
          {deleteError && (
            <div role="alert" className="mb-4 text-error">
              {serverError}
            </div>
          )}
          <p className="mb-6">
            <strong>Klasės kodas:</strong> <span className="font-mono text-lg">{state.schoolClass.code}</span>
          </p>
          <Button type="button" variant="secondary" onClick={() => setConfirmOpen(true)}>
            Ištrinti klasę
          </Button>
          <ConfirmDialog
            open={confirmOpen}
            message={`Ištrinti klasę „${state.schoolClass.name}“?`}
            confirmLabel="Ištrinti"
            onConfirm={onConfirmDelete}
            onCancel={() => setConfirmOpen(false)}
          />
        </Card>
      )}
    </PageLayout>
  )
}
