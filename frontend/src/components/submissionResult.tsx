import { Link } from 'react-router-dom'
import { buttonClass } from '../helpers/buttonStyle'
import {
  answerLabel,
  correctAnswerLabel,
  evidenceLabel,
  nextTaskButton,
  pointsMessage,
  toTaskListButton,
  unlockedGroupMessage,
} from '../helpers/submissionMessage'
import { useNextTaskId } from '../hooks/useNextTaskId'
import type { Submission } from '../types/submission'

type Props = { submission: Submission }

// E07: the points, the correct answer and the data the student submitted
export default function SubmissionResult({ submission }: Props) {
  const nextTaskId = useNextTaskId()

  return (
    <section className="mt-4 rounded border-l-4 border-brand bg-white p-3">
      <p className="mb-2 text-lg font-semibold">{pointsMessage(submission.points)}</p>
      {submission.unlockedGroup && <p className="mb-2">{unlockedGroupMessage(submission.unlockedGroup)}</p>}
      <p className="font-semibold">{correctAnswerLabel}</p>
      <p className="mb-2">{submission.correctAnswer}</p>
      <p className="mb-2 font-semibold">{submission.label}</p>
      <p className="font-semibold">{answerLabel}</p>
      <p className="mb-2">{submission.answer}</p>
      <p className="font-semibold">{evidenceLabel}</p>
      <p className="mb-4">{submission.evidence}</p>
      <div className="flex flex-wrap gap-2">
        {nextTaskId && (
          <Link className={buttonClass('primary')} to={`/tasks/${nextTaskId}`}>
            {nextTaskButton}
          </Link>
        )}
        <Link className={buttonClass('secondary')} to="/tasks">
          {toTaskListButton}
        </Link>
      </div>
    </section>
  )
}
