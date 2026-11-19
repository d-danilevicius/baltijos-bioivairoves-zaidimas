import {
  answerRequired,
  answerTooLong,
  confirmationRequired,
  groupRequired,
  questionRequired,
  questionTooLong,
  titleRequired,
  titleTooLong,
} from '../helpers/teacherTaskMessage'
import { validateTeacherTask, type TeacherTaskForm } from '../helpers/validateTeacherTask'

const validForm: TeacherTaskForm = {
  title: 'Mokyklos kiemas',
  question: 'Kokį paukštį matote ant stogo?',
  correctAnswer: 'Žvirblis',
  difficultyGroup: '2',
  confirmed: true,
}

function validate(changes: Partial<TeacherTaskForm>) {
  return validateTeacherTask({ ...validForm, ...changes })
}

describe('validateTeacherTask', () => {
  it('returns no errors for a valid form', () => {
    expect(validate({})).toEqual({})
  })

  it('accepts the limit lengths', () => {
    const errors = validate({ title: 'a'.repeat(100), question: 'b'.repeat(500), correctAnswer: 'c'.repeat(100) })
    expect(errors).toEqual({})
  })

  it('requires the title', () => {
    expect(validate({ title: '' }).title).toBe(titleRequired)
  })

  it('treats a title of only spaces as empty', () => {
    expect(validate({ title: '   ' }).title).toBe(titleRequired)
  })

  it('rejects a title longer than 100 characters', () => {
    expect(validate({ title: 'a'.repeat(101) }).title).toBe(titleTooLong)
  })

  it('does not count spaces around the title', () => {
    expect(validate({ title: ' ' + 'a'.repeat(100) + ' ' }).title).toBeUndefined()
  })

  it('requires the question', () => {
    expect(validate({ question: '' }).question).toBe(questionRequired)
  })

  it('rejects a question longer than 500 characters', () => {
    expect(validate({ question: 'b'.repeat(501) }).question).toBe(questionTooLong)
  })

  it('requires the correct answer', () => {
    expect(validate({ correctAnswer: '  ' }).correctAnswer).toBe(answerRequired)
  })

  it('rejects a correct answer longer than 100 characters', () => {
    expect(validate({ correctAnswer: 'c'.repeat(101) }).correctAnswer).toBe(answerTooLong)
  })

  it('requires a difficulty group', () => {
    expect(validate({ difficultyGroup: '' }).difficultyGroup).toBe(groupRequired)
  })

  it('requires the confirmation', () => {
    expect(validate({ confirmed: false }).confirmation).toBe(confirmationRequired)
  })

  it('returns an error for each wrong field', () => {
    const errors = validate({ title: '', question: '', correctAnswer: '', difficultyGroup: '', confirmed: false })
    expect(Object.keys(errors).sort()).toEqual(['confirmation', 'correctAnswer', 'difficultyGroup', 'question', 'title'])
  })
})
