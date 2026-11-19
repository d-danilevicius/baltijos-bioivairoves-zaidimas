import { useEffect, useState } from 'react'
import { useParams } from 'react-router-dom'
import { getMyTask } from '../api/studentTaskApi'
import AnswerForm, { type RejectedStatus } from '../components/answerForm'
import Card from '../components/card'
import ConfirmDialog from '../components/confirmDialog'
import ErrorState from '../components/errorState'
import LoadingState from '../components/loadingState'
import ObservationCard from '../components/observationCard'
import PageLayout from '../components/pageLayout'
import SessionExpiredRedirect from '../components/sessionExpiredRedirect'
import SubmissionResult from '../components/submissionResult'
import { taskTypeLabels } from '../helpers/gbifTaskMessage'
import { HttpStatus } from '../helpers/httpStatus'
import { studentLinks } from '../helpers/navigationLink'
import { useLeaveConfirmation } from '../hooks/useLeaveConfirmation'
import { leaveCancel, leaveConfirm, leaveMessage } from '../helpers/submissionMessage'
import { taskWarning } from '../helpers/taskScreenMessage'
import type { Submission } from '../types/submission'
import type { StudentTaskDetails } from '../types/studentTaskDetails'
import AccessDeniedPage from './AccessDeniedPage'
import NotFoundPage from './NotFoundPage'

type TaskState =
  | { status: 'loading' }
  | { status: 'ready'; task: StudentTaskDetails }
  | { status: 'expired' }
  | { status: 'forbidden' }
  | { status: 'notFound' }
  | { status: 'error' }

export default function StudentTaskPage() {
  const id = useParams().id as string
  const [state, setState] = useState<TaskState>({ status: 'loading' })
  const [attempt, setAttempt] = useState(0)

  useEffect(() => {
    let cancelled = false

    async function load() {
      const res = await getMyTask(id)
      if (cancelled) return
      if (res.status === HttpStatus.Ok) setState({ status: 'ready', task: res.body })
      else if (res.status === HttpStatus.Unauthorized) setState({ status: 'expired' })
      else if (res.status === HttpStatus.Forbidden) setState({ status: 'forbidden' })
      else if (res.status === HttpStatus.NotFound) setState({ status: 'notFound' })
      else setState({ status: 'error' })
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

  // 409: the task was already submitted, so it is loaded again to show the saved data
  function onRejected(status: RejectedStatus) {
    if (status === HttpStatus.Unauthorized) setState({ status: 'expired' })
    else if (status === HttpStatus.Forbidden) setState({ status: 'forbidden' })
    else if (status === HttpStatus.NotFound) setState({ status: 'notFound' })
    else if (status === HttpStatus.Conflict) retry()
  }

  if (state.status === 'expired') return <SessionExpiredRedirect />
  if (state.status === 'forbidden') return <AccessDeniedPage />
  if (state.status === 'notFound') return <NotFoundPage />

  return (
    <PageLayout links={studentLinks}>
      {state.status === 'loading' && <LoadingState />}
      {state.status === 'error' && <ErrorState onRetry={retry} />}
      {state.status === 'ready' && <TaskContent task={state.task} onRejected={onRejected} />}
    </PageLayout>
  )
}

function TaskContent({ task, onRejected }: { task: StudentTaskDetails; onRejected: (status: RejectedStatus) => void }) {
  const [submission, setSubmission] = useState<Submission | null>(task.submission)
  const [dirty, setDirty] = useState(false)
  const leave = useLeaveConfirmation(dirty && submission === null)

  return (
    <Card title={task.title ?? taskTypeLabels[task.type]} size="wide">
      <p className="mb-2 font-semibold">{task.label}</p>
      <p className="mb-4 rounded border-l-4 border-accent bg-white p-2">{taskWarning}</p>
      {task.observation && <ObservationCard card={task.observation} />}
      <p className="mb-2">{task.question}</p>
      {submission ? (
        <SubmissionResult submission={submission} />
      ) : (
        <AnswerForm
          taskId={task.id}
          options={task.options}
          onSubmitted={setSubmission}
          onRejected={onRejected}
          onDirtyChange={setDirty}
        />
      )}
      <ConfirmDialog
        open={leave.open}
        message={leaveMessage}
        confirmLabel={leaveConfirm}
        cancelLabel={leaveCancel}
        onConfirm={leave.leave}
        onCancel={leave.stay}
      />
    </Card>
  )
}
