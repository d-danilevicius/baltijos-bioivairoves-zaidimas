import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { MemoryRouter, Route, Routes, useLocation } from 'react-router-dom'
import { getCurrentUser } from '../api/authApi'
import { getMyTasks } from '../api/studentTaskApi'
import App from '../app'
import { taskTypeLabels } from '../helpers/gbifTaskMessage'
import { hadSession, markSessionStarted } from '../helpers/sessionMarker'
import { accessDenied, retry, serverError, sessionExpired } from '../helpers/stateMessage'
import {
  continueButton,
  joinClassButton,
  lockedGroupMessage,
  noClassText,
  noTasksForStudent,
  studentStatusLabels,
} from '../helpers/studentTaskMessage'
import { groupLabel } from '../helpers/groupLabel'
import StudentTasksPage from '../pages/StudentTasksPage'
import type { StudentTaskList } from '../types/studentTask'

vi.mock('../api/studentTaskApi', () => ({ getMyTasks: vi.fn() }))
vi.mock('../api/authApi', () => ({ getCurrentUser: vi.fn(), loginUser: vi.fn() }))

const firstTaskId = 'task-1'

const taskList: StudentTaskList = {
  hasClass: true,
  continueTaskId: firstTaskId,
  groups: [
    {
      group: 1,
      isUnlocked: true,
      tasks: [{ id: firstTaskId, type: 'RecogniseSpeciesGroup', label: 'GBIF', status: 'NotDone' }],
    },
    {
      group: 2,
      isUnlocked: false,
      tasks: [{ id: 'task-2', type: 'TeacherTask', label: 'Mokytojo užduotis', status: 'Locked' }],
    },
  ],
}

function TaskStub() {
  return <div>Task screen</div>
}

function LoginStub() {
  const message = (useLocation().state as { message?: string } | null)?.message
  return <div>Login stub {message}</div>
}

function renderPage(state?: { message: string }) {
  return render(
    <MemoryRouter initialEntries={[{ pathname: '/tasks', state }]}>
      <Routes>
        <Route path="/tasks" element={<StudentTasksPage />} />
        <Route path="/tasks/:id" element={<TaskStub />} />
        <Route path="/join" element={<div>Join screen</div>} />
        <Route path="/login" element={<LoginStub />} />
      </Routes>
    </MemoryRouter>,
  )
}

describe('StudentTasksPage', () => {
  beforeEach(() => {
    vi.mocked(getMyTasks).mockReset()
  })

  afterEach(() => {
    sessionStorage.clear()
  })

  it('shows the groups with the type, the label and the status of each task', async () => {
    vi.mocked(getMyTasks).mockResolvedValue({ status: 200, body: taskList })
    renderPage()

    expect(await screen.findByText(groupLabel(1))).toBeInTheDocument()
    expect(screen.getByText(groupLabel(2))).toBeInTheDocument()
    expect(screen.getByText(taskTypeLabels.RecogniseSpeciesGroup)).toBeInTheDocument()
    expect(screen.getByText('GBIF')).toBeInTheDocument()
    expect(screen.getByText(studentStatusLabels.NotDone)).toBeInTheDocument()
    expect(screen.getAllByText(taskTypeLabels.TeacherTask)).toHaveLength(2)
  })

  it('shows the done status of a submitted task', async () => {
    const doneList: StudentTaskList = {
      hasClass: true,
      continueTaskId: null,
      groups: [
        {
          group: 1,
          isUnlocked: true,
          tasks: [{ id: firstTaskId, type: 'RecogniseSpeciesGroup', label: 'GBIF', status: 'Done' }],
        },
      ],
    }
    vi.mocked(getMyTasks).mockResolvedValue({ status: 200, body: doneList })
    renderPage()

    expect(await screen.findByText(studentStatusLabels.Done)).toBeInTheDocument()
    expect(screen.getByText(studentStatusLabels.Done)).toHaveTextContent('Padaryta')
    expect(screen.queryByText(studentStatusLabels.NotDone)).not.toBeInTheDocument()
  })

  it('shows the lock message and the locked status in a locked group', async () => {
    vi.mocked(getMyTasks).mockResolvedValue({ status: 200, body: taskList })
    renderPage()

    expect(await screen.findByText(lockedGroupMessage)).toBeInTheDocument()
    expect(screen.getByText(studentStatusLabels.Locked)).toBeInTheDocument()
    expect(screen.getAllByRole('link', { name: continueButton })).toHaveLength(1)
  })

  it('opens the first unfinished task with one click on the continue button', async () => {
    vi.mocked(getMyTasks).mockResolvedValue({ status: 200, body: taskList })
    renderPage()

    const link = await screen.findByRole('link', { name: continueButton })
    expect(link).toHaveAttribute('href', `/tasks/${firstTaskId}`)
    await userEvent.click(link)
    expect(screen.getByText('Task screen')).toBeInTheDocument()
  })

  it('shows the text and the join button when the student has no class', async () => {
    vi.mocked(getMyTasks).mockResolvedValue({
      status: 200,
      body: { hasClass: false, groups: [], continueTaskId: null },
    })
    renderPage()

    expect(await screen.findByText(noClassText)).toBeInTheDocument()
    const joinLinks = screen.getAllByRole('link', { name: joinClassButton })
    expect(joinLinks.map((link) => link.getAttribute('href'))).toEqual(['/join', '/join'])
    expect(screen.queryByRole('link', { name: continueButton })).not.toBeInTheDocument()
  })

  it('shows the text when the class has no tasks', async () => {
    vi.mocked(getMyTasks).mockResolvedValue({ status: 200, body: { hasClass: true, groups: [], continueTaskId: null } })
    renderPage()

    expect(await screen.findByText(noTasksForStudent)).toBeInTheDocument()
    expect(screen.queryByRole('link', { name: continueButton })).not.toBeInTheDocument()
  })

  it('shows the message received after joining a class', async () => {
    vi.mocked(getMyTasks).mockResolvedValue({ status: 200, body: taskList })
    renderPage({ message: 'Prisijungėte prie klasės „7a“.' })

    expect(await screen.findByText('Prisijungėte prie klasės „7a“.')).toBeInTheDocument()
  })

  it('shows the error state and loads the tasks again when retry is pressed', async () => {
    vi.mocked(getMyTasks).mockResolvedValueOnce({ status: 'server', message: 'x' })
    vi.mocked(getMyTasks).mockResolvedValue({ status: 200, body: taskList })
    renderPage()

    expect(await screen.findByRole('alert')).toHaveTextContent(serverError)
    await userEvent.click(screen.getByRole('button', { name: retry }))
    expect(await screen.findByText(groupLabel(1))).toBeInTheDocument()
    expect(getMyTasks).toHaveBeenCalledTimes(2)
  })

  it('opens the login page with the session expired message when loading returns 401', async () => {
    markSessionStarted()
    vi.mocked(getMyTasks).mockResolvedValue({ status: 401 })
    renderPage()

    expect(await screen.findByText(new RegExp(`Login stub ${sessionExpired}`))).toBeInTheDocument()
    expect(hadSession()).toBe(false)
  })

  it('shows access denied when loading returns 403', async () => {
    vi.mocked(getMyTasks).mockResolvedValue({ status: 403 })
    renderPage()

    expect(await screen.findByRole('heading', { name: accessDenied })).toBeInTheDocument()
  })
})

describe('Student task list route', () => {
  afterEach(() => {
    vi.clearAllMocks()
    sessionStorage.clear()
  })

  function renderRoute(role: 'Student' | 'Teacher') {
    markSessionStarted()
    vi.mocked(getCurrentUser).mockResolvedValue({
      status: 200,
      body: { id: '1', userName: 'Ona', email: 'ona@example.com', role },
    })
    vi.mocked(getMyTasks).mockResolvedValue({ status: 200, body: taskList })
    return render(
      <MemoryRouter initialEntries={['/tasks']}>
        <App />
      </MemoryRouter>,
    )
  }

  it('shows the task list when a student opens the route', async () => {
    renderRoute('Student')

    expect(await screen.findByText(groupLabel(1))).toBeInTheDocument()
  })

  it('shows the access denied page when a teacher opens the route', async () => {
    renderRoute('Teacher')

    expect(await screen.findByRole('heading', { name: accessDenied })).toBeInTheDocument()
    expect(getMyTasks).not.toHaveBeenCalled()
  })
})
