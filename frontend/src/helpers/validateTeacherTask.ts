import {
  answerRequired,
  answerTooLong,
  confirmationRequired,
  groupRequired,
  questionRequired,
  questionTooLong,
  titleRequired,
  titleTooLong,
} from './teacherTaskMessage'

const maxTitleLength = 100
const maxQuestionLength = 500
const maxAnswerLength = 100

export interface TeacherTaskForm {
  title: string
  question: string
  correctAnswer: string
  difficultyGroup: string
  confirmed: boolean
}

export interface TeacherTaskFormErrors {
  title?: string
  question?: string
  correctAnswer?: string
  difficultyGroup?: string
  confirmation?: string
}

// The spaces at the beginning and the end are not counted, as on the server
function validateText(value: string, maxLength: number, required: string, tooLong: string): string | undefined {
  const trimmed = value.trim()
  if (trimmed === '') return required
  return trimmed.length > maxLength ? tooLong : undefined
}

export function validateTeacherTask(form: TeacherTaskForm): TeacherTaskFormErrors {
  const errors: TeacherTaskFormErrors = {}

  const title = validateText(form.title, maxTitleLength, titleRequired, titleTooLong)
  const question = validateText(form.question, maxQuestionLength, questionRequired, questionTooLong)
  const answer = validateText(form.correctAnswer, maxAnswerLength, answerRequired, answerTooLong)
  if (title) errors.title = title
  if (question) errors.question = question
  if (answer) errors.correctAnswer = answer
  if (form.difficultyGroup === '') errors.difficultyGroup = groupRequired
  if (!form.confirmed) errors.confirmation = confirmationRequired

  return errors
}
