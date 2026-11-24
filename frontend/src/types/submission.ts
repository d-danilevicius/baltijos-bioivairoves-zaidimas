export interface Submission {
  id: string
  taskId: string
  answer: string
  evidence: string
  label: string
  submittedAtUtc: string
  points: number
  correctAnswer: string
  // The number of the group that this submission unlocked; only in the answer to the submission itself
  unlockedGroup?: number | null
}

export interface SubmissionRequest {
  answer: string
  evidence: string
}
