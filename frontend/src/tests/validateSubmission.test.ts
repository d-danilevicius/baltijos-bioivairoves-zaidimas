import { answerRequired, answerTooLong, evidenceRequired, evidenceTooLong } from '../helpers/submissionMessage'
import { validateSubmission, type SubmissionForm } from '../helpers/validateSubmission'

const validForm: SubmissionForm = {
  answer: 'Žvirblis',
  evidence: 'Mačiau prie ežero.',
}

function validate(changes: Partial<SubmissionForm>) {
  return validateSubmission({ ...validForm, ...changes })
}

describe('validateSubmission', () => {
  it('returns no errors for a valid form', () => {
    expect(validate({})).toEqual({})
  })

  it('accepts the limit lengths', () => {
    expect(validate({ answer: 'a'.repeat(100), evidence: 'b'.repeat(500) })).toEqual({})
  })

  it('requires the answer', () => {
    expect(validate({ answer: '' }).answer).toBe(answerRequired)
  })

  it('treats an answer of only spaces as empty', () => {
    expect(validate({ answer: '   ' }).answer).toBe(answerRequired)
  })

  it('rejects an answer longer than 100 characters', () => {
    expect(validate({ answer: 'a'.repeat(101) }).answer).toBe(answerTooLong)
  })

  it('does not count the spaces around the answer', () => {
    expect(validate({ answer: '  ' + 'a'.repeat(100) + '  ' })).toEqual({})
  })

  it('requires the evidence', () => {
    expect(validate({ evidence: '' }).evidence).toBe(evidenceRequired)
  })

  it('treats an evidence of only spaces as empty', () => {
    expect(validate({ evidence: '   ' }).evidence).toBe(evidenceRequired)
  })

  it('rejects an evidence longer than 500 characters', () => {
    expect(validate({ evidence: 'b'.repeat(501) }).evidence).toBe(evidenceTooLong)
  })

  it('does not count the spaces around the evidence', () => {
    expect(validate({ evidence: '  ' + 'b'.repeat(500) + '  ' })).toEqual({})
  })

  it('returns the errors of both fields at once', () => {
    expect(validate({ answer: '', evidence: '' })).toEqual({ answer: answerRequired, evidence: evidenceRequired })
  })
})
