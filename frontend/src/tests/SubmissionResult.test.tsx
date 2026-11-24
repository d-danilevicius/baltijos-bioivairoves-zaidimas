import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { MemoryRouter, Route, Routes } from 'react-router-dom'
import { getMyTasks } from '../api/studentTaskApi'
import SubmissionResult from '../components/submissionResult'
import {
  answerLabel,
  correctAnswerLabel,
  evidenceLabel,
  nextTaskButton,
  submittedLabel,
  toTaskListButton,
} from '../helpers/submissionMessage'
import type { Submission } from '../types/submission'

vi.mock('../api/studentTaskApi', () => ({ getMyTasks: vi.fn() }))

const submission: Submission = {
  id: 's1',
  taskId: 'task-1',
  answer: 'Paprastoji lapė',
  evidence: 'Mačiau prie miško',
  label: submittedLabel,
  submittedAtUtc: '2026-11-24T10:00:00Z',
  points: 10,
  correctAnswer: 'Paprastoji lapė',
  unlockedGroup: null,
}

function renderResult(value: Submission = submission) {
  return render(
    <MemoryRouter initialEntries={['/tasks/task-1']}>
      <Routes>
        <Route path="/tasks/task-1" element={<SubmissionResult submission={value} />} />
        <Route path="/tasks/:id" element={<div>Next task screen</div>} />
        <Route path="/tasks" element={<div>Task list screen</div>} />
      </Routes>
    </MemoryRouter>,
  )
}

function mockNextTask(continueTaskId: string | null) {
  vi.mocked(getMyTasks).mockResolvedValue({
    status: 200,
    body: { hasClass: true, groups: [], continueTaskId },
  })
}

describe('SubmissionResult', () => {
  beforeEach(() => {
    vi.mocked(getMyTasks).mockReset()
    mockNextTask(null)
  })

  it('shows the points, the correct answer and the submitted data with the label', async () => {
    renderResult()

    expect(await screen.findByText('Gavote 10 taškų.')).toBeInTheDocument()
    expect(screen.getByText(correctAnswerLabel)).toBeInTheDocument()
    expect(screen.getByText(submittedLabel)).toBeInTheDocument()
    expect(screen.getByText(answerLabel)).toBeInTheDocument()
    expect(screen.getByText(evidenceLabel)).toBeInTheDocument()
    expect(screen.getByText('Mačiau prie miško')).toBeInTheDocument()
  })

  it('shows zero points and the correct answer for an incorrect answer', async () => {
    renderResult({ ...submission, answer: 'Pilkoji pelėda', points: 0 })

    expect(await screen.findByText('Gavote 0 taškų.')).toBeInTheDocument()
    expect(screen.getByText('Pilkoji pelėda')).toBeInTheDocument()
    expect(screen.getByText('Paprastoji lapė')).toBeInTheDocument()
  })

  it('shows the unlocked group message when the submission unlocked a group', async () => {
    renderResult({ ...submission, unlockedGroup: 2 })

    expect(await screen.findByText('Atrakinta 2 sudėtingumo grupė.')).toBeInTheDocument()
  })

  it('does not show the unlocked group message when no group was unlocked', async () => {
    renderResult()
    await screen.findByText('Gavote 10 taškų.')

    expect(screen.queryByText(/Atrakinta/)).not.toBeInTheDocument()
  })

  it('opens the next task when "Kita užduotis" is pressed', async () => {
    mockNextTask('task-2')
    renderResult()

    const link = await screen.findByRole('link', { name: nextTaskButton })
    expect(link).toHaveAttribute('href', '/tasks/task-2')
    await userEvent.click(link)
    expect(screen.getByText('Next task screen')).toBeInTheDocument()
  })

  it('does not show "Kita užduotis" when there is no other task', async () => {
    renderResult()
    await screen.findByRole('link', { name: toTaskListButton })

    expect(screen.queryByRole('link', { name: nextTaskButton })).not.toBeInTheDocument()
  })

  it('does not show "Kita užduotis" when loading the task list fails', async () => {
    vi.mocked(getMyTasks).mockResolvedValue({ status: 'server', message: 'x' })
    renderResult()
    await screen.findByRole('link', { name: toTaskListButton })

    expect(screen.queryByRole('link', { name: nextTaskButton })).not.toBeInTheDocument()
  })

  it('opens the task list when "Į užduočių sąrašą" is pressed', async () => {
    renderResult()

    await userEvent.click(await screen.findByRole('link', { name: toTaskListButton }))

    expect(screen.getByText('Task list screen')).toBeInTheDocument()
  })
})
