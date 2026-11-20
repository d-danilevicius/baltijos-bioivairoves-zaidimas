import { answerLabel, evidenceLabel } from '../helpers/submissionMessage'
import type { Submission } from '../types/submission'

type Props = { submission: Submission }

// Only the data the student submitted; the correct answer and the points belong to feature 016
export default function SubmissionResult({ submission }: Props) {
  return (
    <section className="mt-4 rounded border-l-4 border-brand bg-white p-3">
      <p className="mb-2 font-semibold">{submission.label}</p>
      <p className="font-semibold">{answerLabel}</p>
      <p className="mb-2">{submission.answer}</p>
      <p className="font-semibold">{evidenceLabel}</p>
      <p>{submission.evidence}</p>
    </section>
  )
}
