import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { createMemoryRouter, RouterProvider, useLocation } from 'react-router-dom'
import { getCurrentUser } from '../api/authApi'
import { getMyTask } from '../api/studentTaskApi'
import App from '../app'
import { taskTypeLabels } from '../helpers/gbifTaskMessage'
import { hadSession, markSessionStarted } from '../helpers/sessionMarker'
import { accessDenied, pageNotFound, retry, serverError, sessionExpired } from '../helpers/stateMessage'
import {
  coordinatesCaption,
  datasetCaption,
  gbifRecordCaption,
  hiddenCoordinatesNote,
  licenceCaption,
  placeCaption,
  taskWarning,
} from '../helpers/taskScreenMessage'
import StudentTaskPage from '../pages/StudentTaskPage'
import type { ObservationCard, StudentTaskDetails } from '../types/studentTaskDetails'

vi.mock('../api/studentTaskApi', () => ({ getMyTask: vi.fn(), getMyTasks: vi.fn() }))
vi.mock('../api/authApi', () => ({ getCurrentUser: vi.fn(), loginUser: vi.fn() }))

const card: ObservationCard = {
  label: 'Patvirtintas GBIF stebėjimas',
  datasetName: 'iNaturalist',
  license: 'CC_BY_4_0',
  gbifUrl: 'https://www.gbif.org/occurrence/4242',
  locality: 'Verkių regioninis parkas',
  latitude: 55.123456,
  longitude: 24.654321,
  coordinatesHidden: false,
}

const gbifTask: StudentTaskDetails = {
  id: 'task-1',
  type: 'CommonSpecies',
  difficultyGroup: 1,
  label: 'GBIF',
  title: null,
  question: 'Kuri rūšis tarp gautų stebėjimų pastebėta dažniausiai?',
  options: ['Pilkoji pelėda', 'Paprastoji lapė'],
  observation: card,
  submission: null,
}

const teacherTask: StudentTaskDetails = {
  id: 'task-2',
  type: 'TeacherTask',
  difficultyGroup: 1,
  label: 'Mokytojo užduotis',
  title: 'Mokyklos kiemas',
  question: 'Kokį paukštį matote ant stogo?',
  options: [],
  observation: null,
  submission: null,
}

function LoginStub() {
  const message = (useLocation().state as { message?: string } | null)?.message
  return <div>Login stub {message}</div>
}

function renderPage() {
  const router = createMemoryRouter(
    [
      { path: '/tasks/:id', element: <StudentTaskPage /> },
      { path: '/login', element: <LoginStub /> },
    ],
    { initialEntries: ['/tasks/task-1'] },
  )
  return render(<RouterProvider router={router} />)
}

describe('StudentTaskPage', () => {
  beforeEach(() => {
    vi.mocked(getMyTask).mockReset()
  })

  afterEach(() => {
    sessionStorage.clear()
  })

  it('asks the server for the task of the address', async () => {
    vi.mocked(getMyTask).mockResolvedValue({ status: 200, body: gbifTask })
    renderPage()

    await screen.findByText(gbifTask.question)
    expect(getMyTask).toHaveBeenCalledWith('task-1')
  })

  it('shows the warning, the question and the answer options of a GBIF task', async () => {
    vi.mocked(getMyTask).mockResolvedValue({ status: 200, body: gbifTask })
    renderPage()

    expect(await screen.findByText(gbifTask.question)).toBeInTheDocument()
    expect(screen.getByText(taskWarning)).toBeInTheDocument()
    expect(screen.getByRole('heading', { name: taskTypeLabels.CommonSpecies })).toBeInTheDocument()
    expect(screen.getByText('Pilkoji pelėda')).toBeInTheDocument()
    expect(screen.getByText('Paprastoji lapė')).toBeInTheDocument()
  })

  it('shows the observation card with the label, the dataset, the licence and the link', async () => {
    vi.mocked(getMyTask).mockResolvedValue({ status: 200, body: gbifTask })
    renderPage()

    expect(await screen.findByText('Patvirtintas GBIF stebėjimas')).toBeInTheDocument()
    expect(screen.getByText(datasetCaption)).toBeInTheDocument()
    expect(screen.getByText('iNaturalist')).toBeInTheDocument()
    expect(screen.getByText(licenceCaption)).toBeInTheDocument()
    expect(screen.getByText('CC_BY_4_0')).toBeInTheDocument()
    const link = screen.getByRole('link', { name: gbifRecordCaption })
    expect(link).toHaveAttribute('href', 'https://www.gbif.org/occurrence/4242')
    expect(link).toHaveAttribute('target', '_blank')
    expect(link).toHaveAttribute('rel', expect.stringContaining('noopener'))
  })

  it('shows the place name and the coordinates of a species that is not protected', async () => {
    vi.mocked(getMyTask).mockResolvedValue({ status: 200, body: gbifTask })
    renderPage()

    expect(await screen.findByText(placeCaption)).toBeInTheDocument()
    expect(screen.getByText('Verkių regioninis parkas')).toBeInTheDocument()
    expect(screen.getByText(coordinatesCaption)).toBeInTheDocument()
    expect(screen.getByText(/55\.123456/)).toBeInTheDocument()
    expect(screen.queryByText(hiddenCoordinatesNote)).not.toBeInTheDocument()
  })

  it('shows only the place name and the note for a protected species', async () => {
    const protectedCard = { ...card, latitude: undefined, longitude: undefined, coordinatesHidden: true }
    vi.mocked(getMyTask).mockResolvedValue({ status: 200, body: { ...gbifTask, observation: protectedCard } })
    renderPage()

    expect(await screen.findByText(hiddenCoordinatesNote)).toBeInTheDocument()
    expect(screen.getByText('Verkių regioninis parkas')).toBeInTheDocument()
    expect(screen.queryByText(coordinatesCaption)).not.toBeInTheDocument()
    expect(screen.queryByText(/55\.123456/)).not.toBeInTheDocument()
  })

  it('shows the title, the question and the label of a teacher task without the observation card', async () => {
    vi.mocked(getMyTask).mockResolvedValue({ status: 200, body: teacherTask })
    renderPage()

    expect(await screen.findByRole('heading', { name: 'Mokyklos kiemas' })).toBeInTheDocument()
    expect(screen.getByText('Mokytojo užduotis')).toBeInTheDocument()
    expect(screen.getByText(teacherTask.question)).toBeInTheDocument()
    expect(screen.getByText(taskWarning)).toBeInTheDocument()
    expect(screen.queryByText('Patvirtintas GBIF stebėjimas')).not.toBeInTheDocument()
    expect(screen.queryByRole('link', { name: gbifRecordCaption })).not.toBeInTheDocument()
  })

  it('shows the page not found when loading returns 404', async () => {
    vi.mocked(getMyTask).mockResolvedValue({ status: 404 })
    renderPage()

    expect(await screen.findByRole('heading', { name: pageNotFound })).toBeInTheDocument()
  })

  it('shows access denied when loading returns 403', async () => {
    vi.mocked(getMyTask).mockResolvedValue({ status: 403 })
    renderPage()

    expect(await screen.findByRole('heading', { name: accessDenied })).toBeInTheDocument()
  })

  it('opens the login page with the session expired message when loading returns 401', async () => {
    markSessionStarted()
    vi.mocked(getMyTask).mockResolvedValue({ status: 401 })
    renderPage()

    expect(await screen.findByText(new RegExp(`Login stub ${sessionExpired}`))).toBeInTheDocument()
    expect(hadSession()).toBe(false)
  })

  it('shows the error state and loads the task again when retry is pressed', async () => {
    vi.mocked(getMyTask).mockResolvedValueOnce({ status: 'server', message: 'x' })
    vi.mocked(getMyTask).mockResolvedValue({ status: 200, body: gbifTask })
    renderPage()

    expect(await screen.findByRole('alert')).toHaveTextContent(serverError)
    await userEvent.click(screen.getByRole('button', { name: retry }))
    expect(await screen.findByText(gbifTask.question)).toBeInTheDocument()
    expect(getMyTask).toHaveBeenCalledTimes(2)
  })
})

describe('Student task route', () => {
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
    vi.mocked(getMyTask).mockResolvedValue({ status: 200, body: gbifTask })
    const router = createMemoryRouter([{ path: '*', element: <App /> }], { initialEntries: ['/tasks/task-1'] })
    return render(<RouterProvider router={router} />)
  }

  it('shows the task when a student opens the route', async () => {
    renderRoute('Student')

    expect(await screen.findByText(gbifTask.question)).toBeInTheDocument()
  })

  it('shows the access denied page when a teacher opens the route', async () => {
    renderRoute('Teacher')

    expect(await screen.findByRole('heading', { name: accessDenied })).toBeInTheDocument()
    expect(getMyTask).not.toHaveBeenCalled()
  })
})
