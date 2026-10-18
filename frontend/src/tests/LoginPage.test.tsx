import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { MemoryRouter, Route, Routes, useLocation } from 'react-router-dom'
import LoginPage from '../pages/LoginPage'
import { loginUser } from '../api/authApi'
import { accountBlocked, invalidCredentials, loginFailedLater } from '../helpers/loginMessages'

vi.mock('../api/authApi', () => ({
  loginUser: vi.fn(),
}))

function ProfilePageStub() {
  const location = useLocation()
  const state = location.state as { user?: { userName?: string } } | null
  return <div>Profile page {state?.user?.userName ?? ''}</div>
}

function renderLoginPage(initialEntry: string | { pathname: string; state?: { message?: string } } = '/login') {
  return render(
    <MemoryRouter initialEntries={[initialEntry]}>
      <Routes>
        <Route path="/login" element={<LoginPage />} />
        <Route path="/profile" element={<ProfilePageStub />} />
      </Routes>
    </MemoryRouter>
  )
}

describe('LoginPage', () => {
  afterEach(() => {
    vi.restoreAllMocks()
    vi.clearAllMocks()
  })

  it('shows required messages for empty form and does not call login API', async () => {
    const mockLoginUser = vi.mocked(loginUser)
    renderLoginPage()

    await userEvent.click(screen.getByRole('button', { name: /Prisijungti/i }))

    expect(await screen.findByText(/Įveskite el. pašto adresą./)).toBeInTheDocument()
    expect(screen.getByText(/Įveskite slaptažodį./)).toBeInTheDocument()
    expect(mockLoginUser).not.toHaveBeenCalled()
  })

  it('shows invalid credentials error and clears password on 401', async () => {
    const mockLoginUser = vi.mocked(loginUser)
    mockLoginUser.mockResolvedValue({ status: 401, body: { errors: { '': ['InvalidCredentials'] } } })
    renderLoginPage()

    await userEvent.type(screen.getByLabelText(/El. paštas/), 'user@example.com')
    await userEvent.type(screen.getByLabelText(/Slaptažodis/), 'secret123')
    await userEvent.click(screen.getByRole('button', { name: /Prisijungti/i }))

    expect(await screen.findByText(invalidCredentials)).toBeInTheDocument()
    expect((screen.getByLabelText(/Slaptažodis/) as HTMLInputElement).value).toBe('')
  })

  it('shows account blocked error on 403', async () => {
    const mockLoginUser = vi.mocked(loginUser)
    mockLoginUser.mockResolvedValue({ status: 403, body: { errors: { '': ['AccountBlocked'] } } })
    renderLoginPage()

    await userEvent.type(screen.getByLabelText(/El. paštas/), 'user@example.com')
    await userEvent.type(screen.getByLabelText(/Slaptažodis/), 'secret123')
    await userEvent.click(screen.getByRole('button', { name: /Prisijungti/i }))

    expect(await screen.findByText(accountBlocked)).toBeInTheDocument()
  })

  it('shows generic login failure message for server or network failure', async () => {
    const mockLoginUser = vi.mocked(loginUser)
    mockLoginUser.mockResolvedValue({ status: 'server', message: 'Server error' })
    renderLoginPage()

    await userEvent.type(screen.getByLabelText(/El. paštas/), 'user@example.com')
    await userEvent.type(screen.getByLabelText(/Slaptažodis/), 'secret123')
    await userEvent.click(screen.getByRole('button', { name: /Prisijungti/i }))

    expect(await screen.findByText(loginFailedLater)).toBeInTheDocument()
  })

  it('disables submit button while login request is pending', async () => {
    let resolve: (value: unknown) => void = () => {}
    const pending = new Promise((res) => {
      resolve = res
    })
    const mockLoginUser = vi.mocked(loginUser)
    mockLoginUser.mockReturnValue(pending as ReturnType<typeof loginUser>)
    renderLoginPage()

    await userEvent.type(screen.getByLabelText(/El. paštas/), 'user@example.com')
    await userEvent.type(screen.getByLabelText(/Slaptažodis/), 'secret123')
    await userEvent.click(screen.getByRole('button', { name: /Prisijungti/i }))

    expect(screen.getByRole('button', { name: /Prisijungti/i })).toBeDisabled()

    resolve({ status: 200, body: { id: '1', userName: 'Jonas', role: 'Student' } })
    await waitFor(() => expect(screen.getByText(/Profile page Jonas/)).toBeInTheDocument())
  })

  it('navigates to profile with user state on successful login', async () => {
    const mockLoginUser = vi.mocked(loginUser)
    mockLoginUser.mockResolvedValue({ status: 200, body: { id: '1', userName: 'Jonas', role: 'Teacher' } })
    renderLoginPage()

    await userEvent.type(screen.getByLabelText(/El. paštas/), 'user@example.com')
    await userEvent.type(screen.getByLabelText(/Slaptažodis/), 'secret123')
    await userEvent.click(screen.getByRole('button', { name: /Prisijungti/i }))

    expect(await screen.findByText(/Profile page Jonas/)).toBeInTheDocument()
  })

  it('shows registration success message from router state', () => {
    renderLoginPage({ pathname: '/login', state: { message: 'Paskyra sukurta. Prisijunkite.' } })

    expect(screen.getByRole('status')).toHaveTextContent('Paskyra sukurta. Prisijunkite.')
  })
})
