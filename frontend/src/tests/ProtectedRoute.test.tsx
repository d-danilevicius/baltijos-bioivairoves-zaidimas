import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { MemoryRouter, Route, Routes } from 'react-router-dom'
import { getCurrentUser } from '../api/authApi'
import ProtectedRoute from '../components/protectedRoute'
import { loginRequired } from '../helpers/loginMessages'
import { markSessionStarted } from '../helpers/sessionMarker'
import { accessDenied, loading, retry, serverError, sessionExpired } from '../helpers/stateMessages'
import LoginPage from '../pages/LoginPage'
import { useCurrentUser } from '../hooks/useCurrentUser'
import type { CurrentUser } from '../types/auth'

vi.mock('../api/authApi', () => ({
  getCurrentUser: vi.fn(),
  loginUser: vi.fn(),
}))

function ChildStub() {
  const user = useCurrentUser()
  return <div>Child for {user.userName}</div>
}

function renderRoute(allowedRoles?: CurrentUser['role'][]) {
  return render(
    <MemoryRouter initialEntries={['/protected']}>
      <Routes>
        <Route element={<ProtectedRoute allowedRoles={allowedRoles} />}>
          <Route path="/protected" element={<ChildStub />} />
        </Route>
        <Route path="/login" element={<LoginPage />} />
      </Routes>
    </MemoryRouter>
  )
}

const teacher: CurrentUser = { id: '1', userName: 'Jonas', email: 'jonas@example.com', role: 'Teacher' }

describe('ProtectedRoute', () => {
  afterEach(() => {
    vi.restoreAllMocks()
    vi.clearAllMocks()
    sessionStorage.clear()
  })

  it('shows the loading state while the session is being checked', () => {
    vi.mocked(getCurrentUser).mockReturnValue(new Promise(() => {}))
    renderRoute()

    expect(screen.getByRole('status')).toHaveTextContent(loading)
  })

  it('redirects to login with loginRequired when 401 and no earlier login', async () => {
    vi.mocked(getCurrentUser).mockResolvedValue({ status: 401 })
    renderRoute()

    expect(await screen.findByRole('heading', { name: 'Prisijungimas' })).toBeInTheDocument()
    expect(screen.getByRole('status')).toHaveTextContent(loginRequired)
  })

  it('redirects to login with sessionExpired when 401 after an earlier login', async () => {
    markSessionStarted()
    vi.mocked(getCurrentUser).mockResolvedValue({ status: 401 })
    renderRoute()

    expect(await screen.findByRole('heading', { name: 'Prisijungimas' })).toBeInTheDocument()
    expect(screen.getByRole('status')).toHaveTextContent(sessionExpired)
  })

  it('shows the error state when the server fails', async () => {
    vi.mocked(getCurrentUser).mockResolvedValue({ status: 'server', message: 'Server error' })
    renderRoute()

    expect(await screen.findByRole('alert')).toHaveTextContent(serverError)
  })

  it('shows the error state when the network fails', async () => {
    vi.mocked(getCurrentUser).mockResolvedValue({ status: 'network', message: 'Network error' })
    renderRoute()

    expect(await screen.findByRole('alert')).toHaveTextContent(serverError)
  })

  it('calls getCurrentUser again and shows the child when retry succeeds', async () => {
    vi.mocked(getCurrentUser)
      .mockResolvedValueOnce({ status: 'server', message: 'Server error' })
      .mockResolvedValueOnce({ status: 200, body: teacher })
    renderRoute()

    await userEvent.click(await screen.findByRole('button', { name: retry }))

    expect(await screen.findByText('Child for Jonas')).toBeInTheDocument()
    expect(getCurrentUser).toHaveBeenCalledTimes(2)
  })

  it('shows access denied when the role is not allowed', async () => {
    vi.mocked(getCurrentUser).mockResolvedValue({ status: 200, body: teacher })
    renderRoute(['Administrator'])

    expect(await screen.findByRole('heading', { name: accessDenied })).toBeInTheDocument()
    expect(screen.queryByText('Child for Jonas')).not.toBeInTheDocument()
  })

  it('renders the child route when the role is allowed', async () => {
    vi.mocked(getCurrentUser).mockResolvedValue({ status: 200, body: teacher })
    renderRoute(['Teacher', 'Administrator'])

    expect(await screen.findByText('Child for Jonas')).toBeInTheDocument()
  })

  it('renders the child route when no allowed roles are set', async () => {
    vi.mocked(getCurrentUser).mockResolvedValue({ status: 200, body: teacher })
    renderRoute()

    expect(await screen.findByText('Child for Jonas')).toBeInTheDocument()
  })
})
