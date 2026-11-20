import { act, render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { createMemoryRouter, RouterProvider, useLocation } from 'react-router-dom'
import { getMyTask } from '../api/studentTaskApi'
import { submitAnswer } from '../api/submissionApi'
import { HttpStatus } from '../helpers/httpStatus'
import { markSessionStarted } from '../helpers/sessionMarker'
import { accessDenied, pageNotFound, sessionExpired } from '../helpers/stateMessage'
import {
  evidenceLabel,
  leaveCancel,
  leaveConfirm,
  leaveMessage,
  submitButton,
  submittedLabel,
} from '../helpers/submissionMessage'
import StudentTaskPage from '../pages/StudentTaskPage'
import type { StudentTaskDetails } from '../types/studentTaskDetails'
import type { Submission } from '../types/submission'

vi.mock('../api/studentTaskApi', () => ({ getMyTask: vi.fn(), getMyTasks: vi.fn() }))
vi.mock('../api/submissionApi', () => ({ submitAnswer: vi.fn() }))

const saved: Submission = {
  id: 's1',
  taskId: 'task-1',
  answer: 'Paprastoji lapė',
  evidence: 'Mačiau prie miško',
  label: submittedLabel,
  submittedAtUtc: '2026-11-19T10:00:00Z',
}

const openTask: StudentTaskDetails = {
  id: 'task-1',
  type: 'CommonSpecies',
  difficultyGroup: 1,
  label: 'GBIF',
  title: null,
  question: 'Kuri rūšis tarp gautų stebėjimų pastebėta dažniausiai?',
  options: ['Pilkoji pelėda', 'Paprastoji lapė'],
  observation: null,
  submission: null,
}

const doneTask: StudentTaskDetails = { ...openTask, submission: saved }

function LoginStub() {
  const message = (useLocation().state as { message?: string } | null)?.message
  return <div>Login stub {message}</div>
}

function renderPage() {
  const router = createMemoryRouter(
    [
      { path: '/tasks/:id', element: <StudentTaskPage /> },
      { path: '/tasks', element: <div>Tasks stub</div> },
      { path: '/login', element: <LoginStub /> },
    ],
    { initialEntries: ['/tasks/task-1'] },
  )
  render(<RouterProvider router={router} />)
  return router
}

async function fillAndSubmit() {
  await userEvent.click(await screen.findByRole('radio', { name: 'Paprastoji lapė' }))
  await userEvent.type(screen.getByLabelText(evidenceLabel), 'Mačiau prie miško')
  await userEvent.click(screen.getByRole('button', { name: submitButton }))
}

describe('StudentTaskPage submission', () => {
  beforeEach(() => {
    vi.mocked(getMyTask).mockReset()
    vi.mocked(submitAnswer).mockReset()
  })

  afterEach(() => {
    sessionStorage.clear()
  })

  it('shows the answer form for a task that has not been submitted', async () => {
    vi.mocked(getMyTask).mockResolvedValue({ status: 200, body: openTask })
    renderPage()

    expect(await screen.findByRole('button', { name: submitButton })).toBeInTheDocument()
    expect(screen.getAllByRole('radio')).toHaveLength(2)
  })

  it('shows the form of the other task when the address changes after a submission', async () => {
    vi.mocked(getMyTask).mockImplementation(async (id) => ({ status: 200, body: { ...openTask, id } }))
    vi.mocked(submitAnswer).mockResolvedValue({ status: HttpStatus.Created, body: saved })
    const router = renderPage()
    await fillAndSubmit()
    await screen.findByText(submittedLabel)

    await act(() => router.navigate('/tasks/task-2'))

    expect(await screen.findByRole('button', { name: submitButton })).toBeInTheDocument()
    expect(screen.queryByText(submittedLabel)).not.toBeInTheDocument()
  })

  it('hides the previous task and its form while the task of the new address is loading', async () => {
    let resolveSecond: (value: Awaited<ReturnType<typeof getMyTask>>) => void = () => {}
    vi.mocked(getMyTask).mockImplementation((id) =>
      id === 'task-1'
        ? Promise.resolve({ status: 200, body: openTask })
        : new Promise((resolve) => {
            resolveSecond = resolve
          }),
    )
    vi.mocked(submitAnswer).mockResolvedValue({ status: HttpStatus.Created, body: saved })
    const router = renderPage()
    await screen.findByRole('button', { name: submitButton })

    await act(() => router.navigate('/tasks/task-2'))

    expect(screen.getByRole('status')).toBeInTheDocument()
    expect(screen.queryByRole('button', { name: submitButton })).not.toBeInTheDocument()
    expect(screen.queryByText(openTask.question)).not.toBeInTheDocument()

    await act(async () => resolveSecond({ status: 200, body: { ...openTask, id: 'task-2' } }))
    await fillAndSubmit()

    expect(submitAnswer).toHaveBeenCalledTimes(1)
    expect(vi.mocked(submitAnswer).mock.calls[0][0]).toBe('task-2')
  })

  it('shows the saved answer with the label instead of the form when the task is already submitted', async () => {
    vi.mocked(getMyTask).mockResolvedValue({ status: 200, body: doneTask })
    renderPage()

    expect(await screen.findByText(submittedLabel)).toBeInTheDocument()
    expect(screen.getByText('Paprastoji lapė')).toBeInTheDocument()
    expect(screen.getByText('Mačiau prie miško')).toBeInTheDocument()
    expect(screen.queryByRole('button', { name: submitButton })).not.toBeInTheDocument()
    expect(screen.queryByRole('radio')).not.toBeInTheDocument()
  })

  it('shows the submitted data with the label and hides the form after a successful submit', async () => {
    vi.mocked(getMyTask).mockResolvedValue({ status: 200, body: openTask })
    vi.mocked(submitAnswer).mockResolvedValue({ status: HttpStatus.Created, body: saved })
    renderPage()
    await fillAndSubmit()

    expect(await screen.findByText(submittedLabel)).toBeInTheDocument()
    expect(submitAnswer).toHaveBeenCalledWith('task-1', { answer: 'Paprastoji lapė', evidence: 'Mačiau prie miško' })
    expect(screen.queryByRole('button', { name: submitButton })).not.toBeInTheDocument()
  })

  it('shows the submitted data when the server answers 409 because the task was already submitted', async () => {
    vi.mocked(getMyTask).mockResolvedValueOnce({ status: 200, body: openTask })
    vi.mocked(getMyTask).mockResolvedValue({ status: 200, body: doneTask })
    vi.mocked(submitAnswer).mockResolvedValue({ status: HttpStatus.Conflict })
    renderPage()
    await fillAndSubmit()

    expect(await screen.findByText(submittedLabel)).toBeInTheDocument()
    expect(screen.queryByRole('button', { name: submitButton })).not.toBeInTheDocument()
  })

  it('shows the page not found when submitting returns 404', async () => {
    vi.mocked(getMyTask).mockResolvedValue({ status: 200, body: openTask })
    vi.mocked(submitAnswer).mockResolvedValue({ status: HttpStatus.NotFound })
    renderPage()
    await fillAndSubmit()

    expect(await screen.findByRole('heading', { name: pageNotFound })).toBeInTheDocument()
  })

  it('shows access denied when submitting returns 403', async () => {
    vi.mocked(getMyTask).mockResolvedValue({ status: 200, body: openTask })
    vi.mocked(submitAnswer).mockResolvedValue({ status: HttpStatus.Forbidden })
    renderPage()
    await fillAndSubmit()

    expect(await screen.findByRole('heading', { name: accessDenied })).toBeInTheDocument()
  })

  it('opens the login page with the session expired message when submitting returns 401', async () => {
    markSessionStarted()
    vi.mocked(getMyTask).mockResolvedValue({ status: 200, body: openTask })
    vi.mocked(submitAnswer).mockResolvedValue({ status: HttpStatus.Unauthorized })
    renderPage()
    await fillAndSubmit()

    expect(await screen.findByText(new RegExp(`Login stub ${sessionExpired}`))).toBeInTheDocument()
  })
})

describe('StudentTaskPage leave dialog', () => {
  beforeEach(() => {
    vi.mocked(getMyTask).mockReset()
    vi.mocked(submitAnswer).mockReset()
  })

  async function leaveToTasks() {
    await userEvent.click(screen.getAllByRole('link', { name: 'Užduotys' })[0] as HTMLElement)
  }

  it('leaves without asking when nothing has been entered', async () => {
    vi.mocked(getMyTask).mockResolvedValue({ status: 200, body: openTask })
    renderPage()
    await screen.findByRole('button', { name: submitButton })
    await leaveToTasks()

    expect(await screen.findByText('Tasks stub')).toBeInTheDocument()
    expect(screen.queryByText(leaveMessage)).not.toBeInTheDocument()
  })

  it('asks before leaving when data has been entered and stays when "Likti" is pressed', async () => {
    vi.mocked(getMyTask).mockResolvedValue({ status: 200, body: openTask })
    renderPage()
    await userEvent.type(await screen.findByLabelText(evidenceLabel), 'Mačiau')
    await leaveToTasks()

    expect(await screen.findByText(leaveMessage)).toBeInTheDocument()
    await userEvent.click(screen.getByRole('button', { name: leaveCancel }))
    expect(screen.queryByText(leaveMessage)).not.toBeInTheDocument()
    expect(screen.getByLabelText(evidenceLabel)).toHaveValue('Mačiau')
  })

  it('leaves the task when "Išeiti" is pressed', async () => {
    vi.mocked(getMyTask).mockResolvedValue({ status: 200, body: openTask })
    renderPage()
    await userEvent.type(await screen.findByLabelText(evidenceLabel), 'Mačiau')
    await leaveToTasks()
    await userEvent.click(await screen.findByRole('button', { name: leaveConfirm }))

    expect(await screen.findByText('Tasks stub')).toBeInTheDocument()
  })

  it('leaves without asking after the answer has been submitted', async () => {
    vi.mocked(getMyTask).mockResolvedValue({ status: 200, body: openTask })
    vi.mocked(submitAnswer).mockResolvedValue({ status: HttpStatus.Created, body: saved })
    renderPage()
    await fillAndSubmit()
    await screen.findByText(submittedLabel)
    await leaveToTasks()

    expect(await screen.findByText('Tasks stub')).toBeInTheDocument()
    expect(screen.queryByText(leaveMessage)).not.toBeInTheDocument()
  })
})
