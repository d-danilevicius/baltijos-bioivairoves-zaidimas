import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { MemoryRouter, Route, Routes } from 'react-router-dom'
import { logoutUser } from '../api/authApi'
import { loginRequired, logoutFailed } from '../helpers/loginMessages'
import LoginPage from '../pages/LoginPage'
import ProfilePage from '../pages/ProfilePage'

vi.mock('../api/authApi', () => ({
  logoutUser: vi.fn(),
}))

function renderWithRoutes(initialEntry: string | { pathname: string; state?: unknown }) {
  return render(
    <MemoryRouter initialEntries={[initialEntry]}>
      <Routes>
        <Route path="/profile" element={<ProfilePage />} />
        <Route path="/login" element={<LoginPage />} />
      </Routes>
    </MemoryRouter>
  )
}

describe('ProfilePage', () => {
  afterEach(() => {
    vi.restoreAllMocks()
    vi.clearAllMocks()
  })

  it('shows user name and Lithuanian role name', () => {
    renderWithRoutes({
      pathname: '/profile',
      state: { user: { id: '1', userName: 'Jonas', role: 'Teacher' } },
    })

    expect(screen.getByRole('heading', { name: 'Profilis' })).toBeInTheDocument()
    expect(screen.getByText(/Naudotojo vardas:/)).toBeInTheDocument()
    expect(screen.getByText(/Jonas/)).toBeInTheDocument()
    expect(screen.getByText(/Rolė:/)).toBeInTheDocument()
    expect(screen.getByText(/Mokytojas/)).toBeInTheDocument()
  })

  it('redirects to login with loginRequired message when user state is missing', async () => {
    renderWithRoutes('/profile')

    await waitFor(() => expect(screen.getByRole('heading', { name: 'Prisijungimas' })).toBeInTheDocument())
    expect(screen.getByRole('status')).toHaveTextContent(loginRequired)
  })

  it('navigates to login when logout returns 204', async () => {
    const mockLogoutUser = vi.mocked(logoutUser)
    mockLogoutUser.mockResolvedValue({ status: 204 })
    renderWithRoutes({
      pathname: '/profile',
      state: { user: { id: '1', userName: 'Jonas', role: 'Student' } },
    })

    await userEvent.click(screen.getByRole('button', { name: /Atsijungti/i }))

    await waitFor(() => expect(screen.getByRole('heading', { name: 'Prisijungimas' })).toBeInTheDocument())
  })

  it('navigates to login when logout returns 401', async () => {
    const mockLogoutUser = vi.mocked(logoutUser)
    mockLogoutUser.mockResolvedValue({ status: 401 })
    renderWithRoutes({
      pathname: '/profile',
      state: { user: { id: '1', userName: 'Jonas', role: 'Student' } },
    })

    await userEvent.click(screen.getByRole('button', { name: /Atsijungti/i }))

    await waitFor(() => expect(screen.getByRole('heading', { name: 'Prisijungimas' })).toBeInTheDocument())
  })

  it('shows logoutFailed and stays on profile when logout fails', async () => {
    const mockLogoutUser = vi.mocked(logoutUser)
    mockLogoutUser.mockResolvedValue({ status: 'server', message: 'Server error' })
    renderWithRoutes({
      pathname: '/profile',
      state: { user: { id: '1', userName: 'Jonas', role: 'Administrator' } },
    })

    await userEvent.click(screen.getByRole('button', { name: /Atsijungti/i }))

    expect(await screen.findByText(logoutFailed)).toBeInTheDocument()
    expect(screen.getByRole('heading', { name: 'Profilis' })).toBeInTheDocument()
  })

  it('disables logout button while request is pending', async () => {
    let resolve: (value: unknown) => void = () => {}
    const pending = new Promise((res) => {
      resolve = res
    })
    const mockLogoutUser = vi.mocked(logoutUser)
    mockLogoutUser.mockReturnValue(pending as ReturnType<typeof logoutUser>)
    renderWithRoutes({
      pathname: '/profile',
      state: { user: { id: '1', userName: 'Jonas', role: 'Student' } },
    })

    await userEvent.click(screen.getByRole('button', { name: /Atsijungti/i }))

    expect(screen.getByRole('button', { name: /Atsijungti/i })).toBeDisabled()

    resolve({ status: 204 })
    await waitFor(() => expect(screen.getByRole('heading', { name: 'Prisijungimas' })).toBeInTheDocument())
  })
})
