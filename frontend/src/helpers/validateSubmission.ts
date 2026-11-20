import { answerRequired, answerTooLong, evidenceRequired, evidenceTooLong } from './submissionMessage'

const maxAnswerLength = 100
export const maxEvidenceLength = 500

export interface SubmissionForm {
  answer: string
  evidence: string
}

export interface SubmissionFormErrors {
  answer?: string
  evidence?: string
}

// The spaces at the beginning and the end are not counted, as on the server
function validateText(value: string, maxLength: number, required: string, tooLong: string): string | undefined {
  const trimmed = value.trim()
  if (trimmed === '') return required
  return trimmed.length > maxLength ? tooLong : undefined
}

export function validateSubmission(form: SubmissionForm): SubmissionFormErrors {
  const errors: SubmissionFormErrors = {}
  const answer = validateText(form.answer, maxAnswerLength, answerRequired, answerTooLong)
  const evidence = validateText(form.evidence, maxEvidenceLength, evidenceRequired, evidenceTooLong)
  if (answer) errors.answer = answer
  if (evidence) errors.evidence = evidence
  return errors
}
