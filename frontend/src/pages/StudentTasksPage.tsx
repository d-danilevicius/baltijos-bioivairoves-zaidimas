import { useEffect, useState } from 'react'
import { useLocation } from 'react-router-dom'
import { getMyTasks } from '../api/studentTaskApi'
import Card from '../components/card'
import EmptyState from '../components/emptyState'
import ErrorState from '../components/errorState'
import LinkButton from '../components/linkButton'
import LoadingState from '../components/loadingState'
import PageLayout from '../components/pageLayout'
import SessionExpiredRedirect from '../components/sessionExpiredRedirect'
import StudentTaskGroup from '../components/studentTaskGroup'
import { HttpStatus } from '../helpers/httpStatus'
import { studentLinks } from '../helpers/navigationLink'
import {
  continueButton,
  joinClassButton,
  noClassText,
  noTasksForStudent,
  studentTasksTitle,
} from '../helpers/studentTaskMessage'
import type { StudentTaskList } from '../types/studentTask'
import AccessDeniedPage from './AccessDeniedPage'

type TasksState =
  | { status: 'loading' }
  | { status: 'ready'; list: StudentTaskList }
  | { status: 'expired' }
  | { status: 'forbidden' }
  | { status: 'error' }

export default function StudentTasksPage() {
  const message = (useLocation().state as { message?: string } | null)?.message
  const [state, setState] = useState<TasksState>({ status: 'loading' })
  const [attempt, setAttempt] = useState(0)

  useEffect(() => {
    let cancelled = false

    async function load() {
      const res = await getMyTasks()
      if (cancelled) return
      if (res.status === HttpStatus.Ok) setState({ status: 'ready', list: res.body })
      else if (res.status === HttpStatus.Unauthorized) setState({ status: 'expired' })
      else if (res.status === HttpStatus.Forbidden) setState({ status: 'forbidden' })
      else setState({ status: 'error' })
    }

    void load()
    return () => {
      cancelled = true
    }
  }, [attempt])

  function retry() {
    setState({ status: 'loading' })
    setAttempt((value) => value + 1)
  }

  if (state.status === 'expired') return <SessionExpiredRedirect />
  if (state.status === 'forbidden') return <AccessDeniedPage />

  return (
    <PageLayout links={studentLinks}>
      {state.status === 'loading' && <LoadingState />}
      {state.status === 'error' && <ErrorState onRetry={retry} />}
      {state.status === 'ready' && (
        <Card title={studentTasksTitle} size="wide">
          {message && (
            <div role="status" className="mb-4 text-brand">
              {message}
            </div>
          )}
          <TaskListContent list={state.list} />
        </Card>
      )}
    </PageLayout>
  )
}

function TaskListContent({ list }: { list: StudentTaskList }) {
  if (!list.hasClass) {
    return (
      <>
        <EmptyState message={noClassText} />
        <LinkButton to="/join">{joinClassButton}</LinkButton>
      </>
    )
  }

  if (list.groups.length === 0) return <EmptyState message={noTasksForStudent} />

  return (
    <>
      {list.groups.map((group) => (
        <StudentTaskGroup key={group.group} group={group} />
      ))}
      {list.continueTaskId && <LinkButton to={`/tasks/${list.continueTaskId}`}>{continueButton}</LinkButton>}
    </>
  )
}
