export interface Submission {
  id: string
  taskId: string
  answer: string
  evidence: string
  label: string
  submittedAtUtc: string
}

export interface SubmissionRequest {
  answer: string
  evidence: string
}
