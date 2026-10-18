import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { MemoryRouter, Outlet, Route, Routes } from 'react-router-dom'
import { logoutUser, type LogoutResult } from '../api/authApi'
import { logoutFailed } from '../helpers/loginMessages'
import { markSessionStarted, hadSession } from '../helpers/sessionMarker'
import ProfilePage from '../pages/ProfilePage'
import type { CurrentUser } from '../types/auth'

vi.mock('../api/authApi', () => ({
  logoutUser: vi.fn(),
}))

const mockLogoutUser = vi.mocked(logoutUser)

function createUser(overrides: Partial<CurrentUser> = {}): CurrentUser {
  return { id: '1', userName: 'Jonas', role: 'Student', ...overrides } as CurrentUser
}

function renderProfile(user: CurrentUser) {
  return render(
    <MemoryRouter initialEntries={['/profile']}>
      <Routes>
        <Route element={<Outlet context={{ user }} />}>
          <Route path="/profile" element={<ProfilePage />} />
        </Route>
        <Route path="/login" element={<div>Login stub</div>} />
      </Routes>
    </MemoryRouter>
  )
}

function getLogoutButton() {
  return screen.getByRole('button', { name: /Atsijungti/i })
}

describe('ProfilePage', () => {
  beforeEach(() => {
    markSessionStarted()
  })

  afterEach(() => {
    vi.restoreAllMocks()
    vi.clearAllMocks()
    sessionStorage.clear()
  })

  it('shows user name and Lithuanian role name', () => {
    renderProfile(createUser({ role: 'Teacher' }))

    expect(screen.getByRole('heading', { name: 'Profilis' })).toBeInTheDocument()
    expect(screen.getByText(/Naudotojo vardas:/)).toBeInTheDocument()
    expect(screen.getByText(/Jonas/)).toBeInTheDocument()
    expect(screen.getByText(/Mokytojas/)).toBeInTheDocument()
  })

  it('navigates to login and clears session marker when logout returns 204', async () => {
    mockLogoutUser.mockResolvedValue({ status: 204 } satisfies LogoutResult)
    renderProfile(createUser())

    await userEvent.click(getLogoutButton())

    expect(await screen.findByText('Login stub')).toBeInTheDocument()
    expect(hadSession()).toBe(false)
  })

  it('navigates to login and clears session marker when logout returns 401', async () => {
    mockLogoutUser.mockResolvedValue({ status: 401 } satisfies LogoutResult)
    renderProfile(createUser())

    await userEvent.click(getLogoutButton())

    expect(await screen.findByText('Login stub')).toBeInTheDocument()
    expect(hadSession()).toBe(false)
  })

  it.each<LogoutResult>([
    { status: 'network', message: 'Network error' },
    { status: 'server', message: 'Server error' },
  ])('shows logoutFailed and stays on profile when logout fails with $status', async (result) => {
    mockLogoutUser.mockResolvedValue(result)
    renderProfile(createUser({ role: 'Administrator' }))

    await userEvent.click(getLogoutButton())

    expect(await screen.findByText(logoutFailed)).toBeInTheDocument()
    expect(screen.getByRole('heading', { name: 'Profilis' })).toBeInTheDocument()
    expect(screen.queryByText('Login stub')).not.toBeInTheDocument()
    expect(hadSession()).toBe(true)
  })

  it('disables logout button while request is pending', async () => {
    let resolveLogout: (value: LogoutResult) => void = () => {}
    mockLogoutUser.mockReturnValue(
      new Promise<LogoutResult>((resolve) => {
        resolveLogout = resolve
      })
    )
    renderProfile(createUser())

    await userEvent.click(getLogoutButton())

    expect(getLogoutButton()).toBeDisabled()

    resolveLogout({ status: 204 })
    await waitFor(() => expect(screen.getByText('Login stub')).toBeInTheDocument())
  })
})
