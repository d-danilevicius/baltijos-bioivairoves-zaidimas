import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { MemoryRouter, Route, Routes, useLocation } from 'react-router-dom'
import { createTeacherTask } from '../api/classTaskApi'
import { getCurrentUser } from '../api/authApi'
import { markSessionStarted } from '../helpers/sessionMarker'
import { accessDenied, pageNotFound } from '../helpers/stateMessage'
import {
  answerLabel,
  answerRequired,
  answerTooLong,
  confirmationLabel,
  confirmationRequired,
  groupLabel,
  groupRequired,
  questionLabel,
  questionRequired,
  questionTooLong,
  saveButton,
  saveFailed,
  taskSaved,
  titleLabel,
  titleRequired,
  titleTooLong,
} from '../helpers/teacherTaskMessage'
import App from '../app'
import CreateTeacherTaskPage from '../pages/CreateTeacherTaskPage'

vi.mock('../api/classTaskApi', () => ({ createTeacherTask: vi.fn() }))
vi.mock('../api/authApi', () => ({ getCurrentUser: vi.fn(), loginUser: vi.fn() }))

function TasksStub() {
  const message = (useLocation().state as { message?: string } | null)?.message
  return <div>Tasks stub {message}</div>
}

function renderPage() {
  return render(
    <MemoryRouter initialEntries={['/classes/abc/tasks/new']}>
      <Routes>
        <Route path="/classes/:id/tasks/new" element={<CreateTeacherTaskPage />} />
        <Route path="/classes/:id/tasks" element={<TasksStub />} />
        <Route path="/login" element={<div>Login stub</div>} />
      </Routes>
    </MemoryRouter>,
  )
}

async function fillValidForm() {
  await userEvent.type(screen.getByLabelText(titleLabel), 'Mokyklos kiemas')
  await userEvent.type(screen.getByLabelText(questionLabel), 'Kokį paukštį matote ant stogo?')
  await userEvent.type(screen.getByLabelText(answerLabel), 'Žvirblis')
  await userEvent.click(screen.getByLabelText(groupLabel(2)))
  await userEvent.click(screen.getByLabelText(confirmationLabel))
}

describe('CreateTeacherTaskPage', () => {
  beforeEach(() => {
    vi.mocked(createTeacherTask).mockReset()
  })

  it('has no difficulty group selected at the start', () => {
    renderPage()
    for (const group of [1, 2, 3]) {
      expect(screen.getByLabelText(groupLabel(group))).not.toBeChecked()
    }
    expect(screen.getByLabelText(confirmationLabel)).not.toBeChecked()
  })

  it('shows an error for every field and sends nothing when the form is empty', async () => {
    renderPage()
    await userEvent.click(screen.getByRole('button', { name: saveButton }))
    expect(screen.getByText(titleRequired)).toBeInTheDocument()
    expect(screen.getByText(questionRequired)).toBeInTheDocument()
    expect(screen.getByText(answerRequired)).toBeInTheDocument()
    expect(screen.getByText(groupRequired)).toBeInTheDocument()
    expect(screen.getByText(confirmationRequired)).toBeInTheDocument()
    expect(createTeacherTask).not.toHaveBeenCalled()
  })

  it('shows only the confirmation error when everything else is filled in', async () => {
    renderPage()
    await fillValidForm()
    await userEvent.click(screen.getByLabelText(confirmationLabel))
    await userEvent.click(screen.getByRole('button', { name: saveButton }))
    expect(screen.getByText(confirmationRequired)).toBeInTheDocument()
    expect(screen.queryByText(titleRequired)).not.toBeInTheDocument()
    expect(createTeacherTask).not.toHaveBeenCalled()
  })

  it('shows the too long errors for the title, the question and the answer', async () => {
    renderPage()
    await fillValidForm()
    await userEvent.type(screen.getByLabelText(titleLabel), 'a'.repeat(100))
    await userEvent.type(screen.getByLabelText(questionLabel), 'b'.repeat(500))
    await userEvent.type(screen.getByLabelText(answerLabel), 'c'.repeat(100))
    await userEvent.click(screen.getByRole('button', { name: saveButton }))
    expect(screen.getByText(titleTooLong)).toBeInTheDocument()
    expect(screen.getByText(questionTooLong)).toBeInTheDocument()
    expect(screen.getByText(answerTooLong)).toBeInTheDocument()
  })

  it('sends the entered task and opens the task list with the success message', async () => {
    vi.mocked(createTeacherTask).mockResolvedValue({
      status: 201,
      body: { id: 't1', type: 'TeacherTask', difficultyGroup: 2, label: 'Mokytojo užduotis' },
    })
    renderPage()
    await fillValidForm()
    await userEvent.click(screen.getByRole('button', { name: saveButton }))
    expect(createTeacherTask).toHaveBeenCalledWith('abc', {
      title: 'Mokyklos kiemas',
      question: 'Kokį paukštį matote ant stogo?',
      correctAnswer: 'Žvirblis',
      difficultyGroup: 2,
      noOrganismDisturbanceConfirmed: true,
    })
    expect(await screen.findByText(`Tasks stub ${taskSaved}`)).toBeInTheDocument()
  })

  it('disables the button while the task is being saved', async () => {
    vi.mocked(createTeacherTask).mockReturnValue(new Promise(() => {}))
    renderPage()
    await fillValidForm()
    await userEvent.click(screen.getByRole('button', { name: saveButton }))
    expect(await screen.findByRole('button', { name: saveButton })).toBeDisabled()
  })

  it('shows the server errors next to the fields', async () => {
    vi.mocked(createTeacherTask).mockResolvedValue({
      status: 400,
      body: { errors: { title: ['TooLong'], question: ['Required'], difficultyGroup: ['Invalid'] } },
    })
    renderPage()
    await fillValidForm()
    await userEvent.click(screen.getByRole('button', { name: saveButton }))
    expect(await screen.findByText(titleTooLong)).toBeInTheDocument()
    expect(screen.getByText(questionRequired)).toBeInTheDocument()
    expect(screen.getByText(groupRequired)).toBeInTheDocument()
  })

  it('shows the save failed message and stays on the form after a server error', async () => {
    vi.mocked(createTeacherTask).mockResolvedValue({ status: 'server', message: 'x' })
    renderPage()
    await fillValidForm()
    await userEvent.click(screen.getByRole('button', { name: saveButton }))
    expect(await screen.findByText(saveFailed)).toBeInTheDocument()
    expect(screen.getByLabelText(titleLabel)).toHaveValue('Mokyklos kiemas')
  })

  it('shows access denied for status 403', async () => {
    vi.mocked(createTeacherTask).mockResolvedValue({ status: 403 })
    renderPage()
    await fillValidForm()
    await userEvent.click(screen.getByRole('button', { name: saveButton }))
    expect(await screen.findByText(accessDenied)).toBeInTheDocument()
  })

  it('shows page not found for status 404', async () => {
    vi.mocked(createTeacherTask).mockResolvedValue({ status: 404 })
    renderPage()
    await fillValidForm()
    await userEvent.click(screen.getByRole('button', { name: saveButton }))
    expect(await screen.findByText(pageNotFound)).toBeInTheDocument()
  })

  it('opens the login page for status 401', async () => {
    vi.mocked(createTeacherTask).mockResolvedValue({ status: 401 })
    renderPage()
    await fillValidForm()
    await userEvent.click(screen.getByRole('button', { name: saveButton }))
    expect(await screen.findByText('Login stub')).toBeInTheDocument()
  })
})

describe('Teacher task route', () => {
  afterEach(() => {
    vi.clearAllMocks()
    sessionStorage.clear()
  })

  it('shows the access denied page when a student opens the teacher task form', async () => {
    markSessionStarted()
    vi.mocked(getCurrentUser).mockResolvedValue({
      status: 200,
      body: { id: '2', userName: 'Ona', email: 'ona@example.com', role: 'Student' },
    })
    render(
      <MemoryRouter initialEntries={['/classes/abc/tasks/new']}>
        <App />
      </MemoryRouter>,
    )

    expect(await screen.findByRole('heading', { name: accessDenied })).toBeInTheDocument()
    expect(screen.queryByLabelText(titleLabel)).not.toBeInTheDocument()
  })
})
