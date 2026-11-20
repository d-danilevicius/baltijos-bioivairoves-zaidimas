import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { submitAnswer } from '../api/submissionApi'
import AnswerForm from '../components/answerForm'
import { HttpStatus } from '../helpers/httpStatus'
import {
  answerInvalid,
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
import type { Submission } from '../types/submission'

vi.mock('../api/submissionApi', () => ({ submitAnswer: vi.fn() }))

const options = ['Žvirblis', 'Zylė', 'Karvelis']

const saved: Submission = {
  id: 's1',
  taskId: 't1',
  answer: 'Žvirblis',
  evidence: 'Mačiau ant stogo',
  label: 'Mokinio pateikti duomenys',
  submittedAtUtc: '2026-11-19T10:00:00Z',
}

function renderForm(choices: string[] = []) {
  const onSubmitted = vi.fn()
  const onRejected = vi.fn()
  const onDirtyChange = vi.fn()
  render(
    <AnswerForm
      taskId="t1"
      options={choices}
      onSubmitted={onSubmitted}
      onRejected={onRejected}
      onDirtyChange={onDirtyChange}
    />,
  )
  return { onSubmitted, onRejected, onDirtyChange }
}

describe('AnswerForm', () => {
  beforeEach(() => {
    vi.mocked(submitAnswer).mockReset()
  })

  it('shows radio options and no text field when the task has options', () => {
    renderForm(options)
    for (const option of options) {
      expect(screen.getByRole('radio', { name: option })).not.toBeChecked()
    }
    expect(screen.queryByRole('textbox', { name: answerLabel })).not.toBeInTheDocument()
  })

  it('shows a text field and no radio options when the task has no options', () => {
    renderForm()
    expect(screen.getByRole('textbox', { name: answerLabel })).toBeInTheDocument()
    expect(screen.queryByRole('radio')).not.toBeInTheDocument()
  })

  it('shows the evidence counter, the note and the submit button at the start', () => {
    renderForm()
    expect(screen.getByLabelText(evidenceLabel)).toBeInTheDocument()
    expect(screen.getByText('0 / 500')).toBeInTheDocument()
    expect(screen.getByText(submitNote)).toBeInTheDocument()
    expect(screen.getByRole('button', { name: submitButton })).toBeInTheDocument()
  })

  it('updates the counter while the student types evidence', async () => {
    renderForm()
    await userEvent.type(screen.getByLabelText(evidenceLabel), 'Mačiau')
    expect(screen.getByText('6 / 500')).toBeInTheDocument()
  })

  it('shows an error for answer and evidence and sends nothing when the form is empty', async () => {
    renderForm(options)
    await userEvent.click(screen.getByRole('button', { name: submitButton }))
    expect(screen.getByText(answerRequired)).toBeInTheDocument()
    expect(screen.getByText(evidenceRequired)).toBeInTheDocument()
    expect(submitAnswer).not.toHaveBeenCalled()
  })

  it('shows an error when the typed answer is longer than 100 characters', async () => {
    renderForm()
    await userEvent.type(screen.getByRole('textbox', { name: answerLabel }), 'a'.repeat(101))
    await userEvent.type(screen.getByLabelText(evidenceLabel), 'Mačiau')
    await userEvent.click(screen.getByRole('button', { name: submitButton }))
    expect(screen.getByText(answerTooLong)).toBeInTheDocument()
    expect(submitAnswer).not.toHaveBeenCalled()
  })

  it('shows an error when the evidence is longer than 500 characters', async () => {
    renderForm(options)
    await userEvent.click(screen.getByRole('radio', { name: 'Žvirblis' }))
    await userEvent.click(screen.getByLabelText(evidenceLabel))
    await userEvent.paste('a'.repeat(501))
    await userEvent.click(screen.getByRole('button', { name: submitButton }))
    expect(screen.getByText(evidenceTooLong)).toBeInTheDocument()
    expect(submitAnswer).not.toHaveBeenCalled()
  })

  it('sends the chosen option and trimmed evidence and reports the saved submission', async () => {
    vi.mocked(submitAnswer).mockResolvedValue({ status: HttpStatus.Created, body: saved })
    const { onSubmitted } = renderForm(options)
    await userEvent.click(screen.getByRole('radio', { name: 'Žvirblis' }))
    await userEvent.type(screen.getByLabelText(evidenceLabel), '  Mačiau ant stogo  ')
    await userEvent.click(screen.getByRole('button', { name: submitButton }))
    expect(submitAnswer).toHaveBeenCalledWith('t1', { answer: 'Žvirblis', evidence: 'Mačiau ant stogo' })
    expect(onSubmitted).toHaveBeenCalledWith(saved)
  })

  it('sends the typed answer when the task has no options', async () => {
    vi.mocked(submitAnswer).mockResolvedValue({ status: HttpStatus.Created, body: saved })
    renderForm()
    await userEvent.type(screen.getByRole('textbox', { name: answerLabel }), '5')
    await userEvent.type(screen.getByLabelText(evidenceLabel), 'Mačiau')
    await userEvent.click(screen.getByRole('button', { name: submitButton }))
    expect(submitAnswer).toHaveBeenCalledWith('t1', { answer: '5', evidence: 'Mačiau' })
  })

  it('shows server field errors from a 400 response', async () => {
    vi.mocked(submitAnswer).mockResolvedValue({
      status: HttpStatus.BadRequest,
      body: { errors: { answer: ['Invalid'], evidence: ['TooLong'] } },
    })
    renderForm(options)
    await userEvent.click(screen.getByRole('radio', { name: 'Zylė' }))
    await userEvent.type(screen.getByLabelText(evidenceLabel), 'Mačiau')
    await userEvent.click(screen.getByRole('button', { name: submitButton }))
    expect(await screen.findByText(answerInvalid)).toBeInTheDocument()
    expect(screen.queryByText(answerRequired)).not.toBeInTheDocument()
    expect(screen.getByText(evidenceTooLong)).toBeInTheDocument()
  })

  it.each([HttpStatus.Unauthorized, HttpStatus.Forbidden, HttpStatus.NotFound, HttpStatus.Conflict])(
    'reports status %i to the page',
    async (status) => {
      vi.mocked(submitAnswer).mockResolvedValue({ status })
      const { onRejected } = renderForm()
      await userEvent.type(screen.getByRole('textbox', { name: answerLabel }), '5')
      await userEvent.type(screen.getByLabelText(evidenceLabel), 'Mačiau')
      await userEvent.click(screen.getByRole('button', { name: submitButton }))
      expect(onRejected).toHaveBeenCalledWith(status)
    },
  )

  it('keeps the entered data on a saving failure and sends again after "Bandyti dar kartą"', async () => {
    vi.mocked(submitAnswer).mockResolvedValueOnce({ status: 'server', message: 'x' })
    vi.mocked(submitAnswer).mockResolvedValueOnce({ status: HttpStatus.Created, body: saved })
    const { onSubmitted } = renderForm()
    await userEvent.type(screen.getByRole('textbox', { name: answerLabel }), '5')
    await userEvent.type(screen.getByLabelText(evidenceLabel), 'Mačiau')
    await userEvent.click(screen.getByRole('button', { name: submitButton }))

    expect(await screen.findByText(saveFailed)).toBeInTheDocument()
    expect(screen.getByRole('textbox', { name: answerLabel })).toHaveValue('5')
    expect(screen.getByLabelText(evidenceLabel)).toHaveValue('Mačiau')

    await userEvent.click(screen.getByRole('button', { name: retryButton }))
    expect(submitAnswer).toHaveBeenCalledTimes(2)
    expect(onSubmitted).toHaveBeenCalledWith(saved)
  })

  it('tells the page when the form has entered data and when it is empty again', async () => {
    const { onDirtyChange } = renderForm()
    await userEvent.type(screen.getByLabelText(evidenceLabel), 'a')
    expect(onDirtyChange).toHaveBeenLastCalledWith(true)
    await userEvent.clear(screen.getByLabelText(evidenceLabel))
    expect(onDirtyChange).toHaveBeenLastCalledWith(false)
  })
})
