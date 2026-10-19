import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { MemoryRouter, Route, Routes } from 'react-router-dom'
import { getCurrentUser } from '../api/authApi'
import { listClasses } from '../api/classesApi'
import ProtectedRoute from '../components/protectedRoute'
import { noClasses, createFirstClass, newClass } from '../helpers/classMessages'
import { accessDenied, loading, retry, serverError } from '../helpers/stateMessages'
import ClassListPage from '../pages/ClassListPage'
import type { CurrentUser } from '../types/auth'

vi.mock('../api/authApi', () => ({ getCurrentUser: vi.fn() }))
vi.mock('../api/classesApi', () => ({ listClasses: vi.fn() }))

const teacher: CurrentUser = { id: '1', userName: 'Jonas', email: 'jonas@example.com', role: 'Teacher' }

function renderList(state?: { message: string }) {
  return render(
    <MemoryRouter initialEntries={[{ pathname: '/classes', state }]}>
      <Routes>
        <Route element={<ProtectedRoute allowedRoles={['Teacher']} />}>
          <Route path="/classes" element={<ClassListPage />} />
          <Route path="/classes/new" element={<div>New class stub</div>} />
          <Route path="/classes/:id" element={<div>Class details stub</div>} />
        </Route>
      </Routes>
    </MemoryRouter>,
  )
}

describe('ClassListPage', () => {
  beforeEach(() => {
    vi.mocked(getCurrentUser).mockResolvedValue({ status: 200, body: teacher })
  })

  afterEach(() => {
    vi.restoreAllMocks()
    vi.clearAllMocks()
    sessionStorage.clear()
  })

  it('shows the names and codes of the classes of the teacher', async () => {
    vi.mocked(listClasses).mockResolvedValue({
      status: 200,
      body: [
        { id: 'a', name: '7a klasė', code: 'AB12CD', createdAtUtc: '2026-10-19T10:00:00Z' },
        { id: 'b', name: '8b klasė', code: 'ZX98YW', createdAtUtc: '2026-10-19T11:00:00Z' },
      ],
    })
    renderList()

    expect(await screen.findByRole('link', { name: /7a klasė/ })).toHaveAttribute('href', '/classes/a')
    expect(screen.getByText(/AB12CD/)).toBeInTheDocument()
    expect(screen.getByRole('link', { name: /8b klasė/ })).toHaveAttribute('href', '/classes/b')
    expect(screen.getByText(/ZX98YW/)).toBeInTheDocument()
  })

  it('shows the link to create a new class when the teacher has classes', async () => {
    vi.mocked(listClasses).mockResolvedValue({
      status: 200,
      body: [{ id: 'a', name: '7a klasė', code: 'AB12CD', createdAtUtc: '2026-10-19T10:00:00Z' }],
    })
    renderList()

    expect(await screen.findByRole('link', { name: newClass })).toHaveAttribute('href', '/classes/new')
  })

  it('shows the empty message and the button to create the first class when there are no classes', async () => {
    vi.mocked(listClasses).mockResolvedValue({ status: 200, body: [] })
    renderList()

    expect(await screen.findByText(noClasses)).toBeInTheDocument()
    await userEvent.click(screen.getByRole('link', { name: createFirstClass }))
    expect(await screen.findByText('New class stub')).toBeInTheDocument()
  })

  it('shows the loading state while the classes are loaded', async () => {
    vi.mocked(listClasses).mockReturnValue(new Promise(() => {}))
    renderList()

    expect(await screen.findByText(loading)).toBeInTheDocument()
  })

  it('shows the error state and loads the classes again when retry is pressed', async () => {
    vi.mocked(listClasses).mockResolvedValueOnce({ status: 'server', message: 'x' })
    renderList()

    expect(await screen.findByRole('alert')).toHaveTextContent(serverError)

    vi.mocked(listClasses).mockResolvedValueOnce({ status: 200, body: [] })
    await userEvent.click(screen.getByRole('button', { name: retry }))

    expect(await screen.findByText(noClasses)).toBeInTheDocument()
    expect(listClasses).toHaveBeenCalledTimes(2)
  })

  it('shows the message passed from the previous screen', async () => {
    vi.mocked(listClasses).mockResolvedValue({ status: 200, body: [] })
    renderList({ message: 'Klasė ištrinta.' })

    expect(await screen.findByText('Klasė ištrinta.')).toBeInTheDocument()
  })

  it('shows the teacher menu with the class list and profile links', async () => {
    vi.mocked(listClasses).mockResolvedValue({ status: 200, body: [] })
    renderList()

    await screen.findByText(noClasses)
    expect(screen.getByRole('link', { name: 'Klasės' })).toHaveAttribute('href', '/classes')
    expect(screen.getByRole('link', { name: 'Profilis' })).toHaveAttribute('href', '/profile')
  })

  it('shows access denied to a student and does not load the classes', async () => {
    vi.mocked(getCurrentUser).mockResolvedValue({
      status: 200,
      body: { ...teacher, role: 'Student' },
    })
    renderList()

    expect(await screen.findByRole('heading', { name: accessDenied })).toBeInTheDocument()
    expect(listClasses).not.toHaveBeenCalled()
  })
})
