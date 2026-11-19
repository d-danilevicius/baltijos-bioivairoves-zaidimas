import { FormEvent, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { createTeacherTask, type CreateTeacherTaskResult } from '../api/classTaskApi'
import Button from '../components/button'
import Card from '../components/card'
import DifficultyChoice from '../components/difficultyChoice'
import PageLayout from '../components/pageLayout'
import SessionExpiredRedirect from '../components/sessionExpiredRedirect'
import TextAreaField from '../components/textAreaField'
import TextField from '../components/textField'
import { HttpStatus } from '../helpers/httpStatus'
import { teacherLinks } from '../helpers/navigationLink'
import {
  answerLabel,
  answerRequired,
  answerTooLong,
  confirmationLabel,
  confirmationRequired,
  createTeacherTaskTitle,
  groupRequired,
  questionLabel,
  questionRequired,
  questionTooLong,
  saveButton,
  saveFailed,
  taskSaved,
  titleLabel,
  titleRequired,
  titleTooLong,
} from '../helpers/teacherTaskMessage'
import { validateTeacherTask, type TeacherTaskFormErrors } from '../helpers/validateTeacherTask'
import AccessDeniedPage from './AccessDeniedPage'
import NotFoundPage from './NotFoundPage'

type Blocked = 'expired' | 'forbidden' | 'notFound'

function pick(codes: string[] | undefined, required: string, tooLong?: string): string | undefined {
  if (!codes || codes.length === 0) return undefined
  return tooLong && codes.includes('TooLong') ? tooLong : required
}

function getServerErrors(errors: Record<string, string[]> | undefined): TeacherTaskFormErrors {
  const result: TeacherTaskFormErrors = {}
  const title = pick(errors?.title, titleRequired, titleTooLong)
  const question = pick(errors?.question, questionRequired, questionTooLong)
  const answer = pick(errors?.correctAnswer, answerRequired, answerTooLong)
  const group = pick(errors?.difficultyGroup, groupRequired)
  const confirmation = pick(errors?.noOrganismDisturbanceConfirmed, confirmationRequired)
  if (title) result.title = title
  if (question) result.question = question
  if (answer) result.correctAnswer = answer
  if (group) result.difficultyGroup = group
  if (confirmation) result.confirmation = confirmation
  return result
}

export default function CreateTeacherTaskPage() {
  const { id = '' } = useParams()
  const navigate = useNavigate()
  const [title, setTitle] = useState('')
  const [question, setQuestion] = useState('')
  const [correctAnswer, setCorrectAnswer] = useState('')
  const [difficultyGroup, setDifficultyGroup] = useState('')
  const [confirmed, setConfirmed] = useState(false)
  const [errors, setErrors] = useState<TeacherTaskFormErrors>({})
  const [topError, setTopError] = useState<string | undefined>(undefined)
  const [sending, setSending] = useState(false)
  const [blocked, setBlocked] = useState<Blocked | undefined>(undefined)

  function showResult(res: CreateTeacherTaskResult) {
    if (res.status === HttpStatus.Created) navigate(`/classes/${id}/tasks`, { state: { message: taskSaved } })
    else if (res.status === HttpStatus.Unauthorized) setBlocked('expired')
    else if (res.status === HttpStatus.Forbidden) setBlocked('forbidden')
    else if (res.status === HttpStatus.NotFound) setBlocked('notFound')
    else if (res.status === HttpStatus.BadRequest) {
      const serverErrors = getServerErrors(res.body.errors)
      setErrors(serverErrors)
      if (Object.keys(serverErrors).length === 0) setTopError(saveFailed)
    } else setTopError(saveFailed)
  }

  async function submit() {
    setTopError(undefined)

    const clientErrors = validateTeacherTask({ title, question, correctAnswer, difficultyGroup, confirmed })
    setErrors(clientErrors)
    if (Object.keys(clientErrors).length > 0) return

    setSending(true)
    const res = await createTeacherTask(id, {
      title: title.trim(),
      question: question.trim(),
      correctAnswer: correctAnswer.trim(),
      difficultyGroup: Number(difficultyGroup),
      noOrganismDisturbanceConfirmed: confirmed,
    })
    setSending(false)
    showResult(res)
  }

  function onSubmit(e: FormEvent) {
    e.preventDefault()
    void submit()
  }

  if (blocked === 'expired') return <SessionExpiredRedirect />
  if (blocked === 'forbidden') return <AccessDeniedPage />
  if (blocked === 'notFound') return <NotFoundPage />

  return (
    <PageLayout links={teacherLinks}>
      <Card title={createTeacherTaskTitle} size="wide">
        <form onSubmit={onSubmit} noValidate>
          {topError && (
            <div role="alert" className="mb-4 text-error">
              {topError}
            </div>
          )}
          <TextField id="title" label={titleLabel} value={title} onChange={setTitle} error={errors.title} />
          <TextAreaField
            id="question"
            label={questionLabel}
            value={question}
            onChange={setQuestion}
            error={errors.question}
          />
          <TextField
            id="correctAnswer"
            label={answerLabel}
            value={correctAnswer}
            onChange={setCorrectAnswer}
            error={errors.correctAnswer}
          />
          <DifficultyChoice value={difficultyGroup} onChange={setDifficultyGroup} error={errors.difficultyGroup} />
          <div className="mb-4">
            <label className="flex items-start gap-2">
              <input
                type="checkbox"
                checked={confirmed}
                aria-describedby={errors.confirmation ? 'confirmation-error' : undefined}
                onChange={(e) => setConfirmed(e.target.checked)}
              />
              {confirmationLabel}
            </label>
            {errors.confirmation && (
              <div id="confirmation-error" role="alert" className="mt-1 text-error">
                {errors.confirmation}
              </div>
            )}
          </div>
          <Button type="submit" disabled={sending} fullWidth>
            {saveButton}
          </Button>
        </form>
      </Card>
    </PageLayout>
  )
}
