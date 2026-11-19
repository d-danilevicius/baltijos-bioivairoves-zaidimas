import { FormEvent, useEffect, useState } from 'react'
import { submitAnswer, type SubmitAnswerResult } from '../api/submissionApi'
import { HttpStatus } from '../helpers/httpStatus'
import {
  answerLabel,
  answerRequired,
  answerTooLong,
  evidenceLabel,
  evidenceRequired,
  evidenceTooLong,
  retryButton,
  saveFailed,
  submitButton,
  submitNote,
} from '../helpers/submissionMessage'
import { maxEvidenceLength, validateSubmission, type SubmissionFormErrors } from '../helpers/validateSubmission'
import type { Submission } from '../types/submission'
import Button from './button'
import OptionChoice from './optionChoice'
import TextAreaField from './textAreaField'
import TextField from './textField'

export type RejectedStatus =
  typeof HttpStatus.Unauthorized | typeof HttpStatus.Forbidden | typeof HttpStatus.NotFound | typeof HttpStatus.Conflict

type Props = {
  taskId: string
  options: string[]
  onSubmitted: (submission: Submission) => void
  onRejected: (status: RejectedStatus) => void
  onDirtyChange: (dirty: boolean) => void
}

function pick(codes: string[] | undefined, required: string, tooLong: string): string | undefined {
  if (!codes || codes.length === 0) return undefined
  return codes.includes('TooLong') ? tooLong : required
}

function getServerErrors(errors: Record<string, string[]> | undefined): SubmissionFormErrors {
  const result: SubmissionFormErrors = {}
  const answer = pick(errors?.answer, answerRequired, answerTooLong)
  const evidence = pick(errors?.evidence, evidenceRequired, evidenceTooLong)
  if (answer) result.answer = answer
  if (evidence) result.evidence = evidence
  return result
}

export default function AnswerForm({ taskId, options, onSubmitted, onRejected, onDirtyChange }: Props) {
  const [answer, setAnswer] = useState('')
  const [evidence, setEvidence] = useState('')
  const [errors, setErrors] = useState<SubmissionFormErrors>({})
  const [saveError, setSaveError] = useState(false)
  const [sending, setSending] = useState(false)

  useEffect(() => {
    onDirtyChange(answer.trim() !== '' || evidence.trim() !== '')
  }, [answer, evidence, onDirtyChange])

  function showResult(res: SubmitAnswerResult) {
    if (res.status === HttpStatus.Created) {
      onSubmitted(res.body)
    } else if (res.status === HttpStatus.BadRequest) {
      const serverErrors = getServerErrors(res.body.errors)
      setErrors(serverErrors)
      setSaveError(Object.keys(serverErrors).length === 0)
    } else if (
      res.status === HttpStatus.Unauthorized ||
      res.status === HttpStatus.Forbidden ||
      res.status === HttpStatus.NotFound ||
      res.status === HttpStatus.Conflict
    ) {
      onRejected(res.status)
    } else {
      setSaveError(true)
    }
  }

  async function submit() {
    setSaveError(false)

    const clientErrors = validateSubmission({ answer, evidence })
    setErrors(clientErrors)
    if (Object.keys(clientErrors).length > 0) return

    setSending(true)
    const res = await submitAnswer(taskId, { answer: answer.trim(), evidence: evidence.trim() })
    setSending(false)
    showResult(res)
  }

  function onSubmit(e: FormEvent) {
    e.preventDefault()
    void submit()
  }

  return (
    <form onSubmit={onSubmit} noValidate className="mt-4">
      {options.length > 0 ? (
        <OptionChoice options={options} value={answer} onChange={setAnswer} error={errors.answer} />
      ) : (
        <TextField id="answer" label={answerLabel} value={answer} onChange={setAnswer} error={errors.answer} />
      )}
      <TextAreaField
        id="evidence"
        label={evidenceLabel}
        value={evidence}
        onChange={setEvidence}
        error={errors.evidence}
        hint={`${evidence.length} / ${maxEvidenceLength}`}
      />
      <p className="mb-4">{submitNote}</p>
      {saveError && (
        <div role="alert" className="mb-4 text-error">
          <p>{saveFailed}</p>
          <Button type="button" variant="secondary" onClick={() => void submit()} disabled={sending}>
            {retryButton}
          </Button>
        </div>
      )}
      <Button type="submit" disabled={sending}>
        {submitButton}
      </Button>
    </form>
  )
}
