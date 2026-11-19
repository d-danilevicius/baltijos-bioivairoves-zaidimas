import { useEffect, useState } from 'react'
import { useLocation, useParams } from 'react-router-dom'
import { getClass } from '../api/classApi'
import { deleteClassTask, listClassTasks } from '../api/classTaskApi'
import Card from '../components/card'
import ClassTaskTable from '../components/classTaskTable'
import ConfirmDialog from '../components/confirmDialog'
import EmptyState from '../components/emptyState'
import ErrorState from '../components/errorState'
import LinkButton from '../components/linkButton'
import LoadingState from '../components/loadingState'
import PageLayout from '../components/pageLayout'
import SessionExpiredRedirect from '../components/sessionExpiredRedirect'
import {
  backToClass,
  classSeasonInfo,
  classTasksTitle,
  createFromGbif,
  noTasksYet,
} from '../helpers/gbifTaskMessage'
import { HttpStatus } from '../helpers/httpStatus'
import { teacherLinks } from '../helpers/navigationLink'
import { serverError } from '../helpers/stateMessage'
import { createTeacherTaskLink, deleteTaskButton, deleteTaskQuestion } from '../helpers/teacherTaskMessage'
import type { ClassTask } from '../types/classTask'
import type { SchoolClass } from '../types/schoolClass'
import AccessDeniedPage from './AccessDeniedPage'
import NotFoundPage from './NotFoundPage'

type TasksState =
  | { status: 'loading' }
  | { status: 'ready'; schoolClass: SchoolClass; tasks: ClassTask[] }
  | { status: 'expired' }
  | { status: 'forbidden' }
  | { status: 'notFound' }
  | { status: 'error' }

function failureState(status: number | string): TasksState {
  if (status === HttpStatus.Unauthorized) return { status: 'expired' }
  if (status === HttpStatus.Forbidden) return { status: 'forbidden' }
  if (status === HttpStatus.NotFound) return { status: 'notFound' }
  return { status: 'error' }
}

export default function ClassTasksPage() {
  const { id = '' } = useParams()
  const savedMessage = (useLocation().state as { message?: string } | null)?.message
  const [message, setMessage] = useState(savedMessage)
  const [state, setState] = useState<TasksState>({ status: 'loading' })
  const [attempt, setAttempt] = useState(0)
  const [taskToDelete, setTaskToDelete] = useState<string | undefined>(undefined)
  const [deleteError, setDeleteError] = useState<string | undefined>(undefined)

  useEffect(() => {
    let cancelled = false

    async function load() {
      const [classRes, tasksRes] = await Promise.all([getClass(id), listClassTasks(id)])
      if (cancelled) return
      if (classRes.status !== HttpStatus.Ok) setState(failureState(classRes.status))
      else if (tasksRes.status !== HttpStatus.Ok) setState(failureState(tasksRes.status))
      else setState({ status: 'ready', schoolClass: classRes.body, tasks: tasksRes.body })
    }

    void load()
    return () => {
      cancelled = true
    }
  }, [id, attempt])

  function retry() {
    setState({ status: 'loading' })
    setAttempt((value) => value + 1)
  }

  async function confirmDelete() {
    const taskId = taskToDelete
    setTaskToDelete(undefined)
    if (taskId === undefined) return

    const res = await deleteClassTask(id, taskId)
    if (res.status === HttpStatus.NoContent) {
      setMessage(undefined)
      setDeleteError(undefined)
      setState((current) =>
        current.status === 'ready' ? { ...current, tasks: current.tasks.filter((t) => t.id !== taskId) } : current,
      )
    } else if (
      res.status === HttpStatus.Unauthorized ||
      res.status === HttpStatus.Forbidden ||
      res.status === HttpStatus.NotFound
    ) {
      setState(failureState(res.status))
    } else setDeleteError(serverError)
  }

  if (state.status === 'expired') return <SessionExpiredRedirect />
  if (state.status === 'forbidden') return <AccessDeniedPage />
  if (state.status === 'notFound') return <NotFoundPage />

  return (
    <PageLayout links={teacherLinks}>
      {state.status === 'loading' && <LoadingState />}
      {state.status === 'error' && <ErrorState onRetry={retry} />}
      {state.status === 'ready' && (
        <Card title={classTasksTitle} size="wide">
          {message && (
            <div role="status" className="mb-4 text-brand">
              {message}
            </div>
          )}
          {deleteError && (
            <div role="alert" className="mb-4 text-error">
              {deleteError}
            </div>
          )}
          <p className="mb-2 font-semibold">{state.schoolClass.name}</p>
          {state.schoolClass.season && <p className="mb-4">{classSeasonInfo(state.schoolClass.season)}</p>}
          {state.tasks.length === 0 ? (
            <EmptyState message={noTasksYet} />
          ) : (
            <ClassTaskTable tasks={state.tasks} onDelete={setTaskToDelete} />
          )}
          <div className="flex flex-wrap gap-3">
            {!state.schoolClass.season && <LinkButton to={`/classes/${id}/tasks/gbif`}>{createFromGbif}</LinkButton>}
            <LinkButton to={`/classes/${id}/tasks/new`}>{createTeacherTaskLink}</LinkButton>
            <LinkButton to={`/classes/${id}`} variant="secondary">
              {backToClass}
            </LinkButton>
          </div>
          <ConfirmDialog
            open={taskToDelete !== undefined}
            message={deleteTaskQuestion}
            confirmLabel={deleteTaskButton}
            onConfirm={() => void confirmDelete()}
            onCancel={() => setTaskToDelete(undefined)}
          />
        </Card>
      )}
    </PageLayout>
  )
}
