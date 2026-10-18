import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { MemoryRouter, Routes, Route } from 'react-router-dom'
import RegisterPage from '../pages/RegisterPage'
import LoginPage from '../pages/LoginPage'
import * as api from '../api/authApi'

function renderWithRoutes() {
  return render(
    <MemoryRouter initialEntries={["/register"]}>
      <Routes>
        <Route path="/register" element={<RegisterPage />} />
        <Route path="/login" element={<LoginPage />} />
      </Routes>
    </MemoryRouter>
  )
}

describe('RegisterPage', () => {
  afterEach(() => { vi.restoreAllMocks() })

  it('empty form shows errors and does not call API', async () => {
    const mock = vi.spyOn(api, 'registerUser')
    renderWithRoutes()
    await userEvent.click(screen.getByRole('button', { name: /Registruotis/i }))
    expect(await screen.findByText(/Forma neišsiųsta/)).toBeInTheDocument()
    expect(screen.getByText(/Įveskite el. pašto adresą/)).toBeInTheDocument()
    expect(mock).not.toHaveBeenCalled()
  })

  it('short password shows password error and does not call API', async () => {
    const mock = vi.spyOn(api, 'registerUser')
    renderWithRoutes()
    await userEvent.type(screen.getByLabelText(/El. paštas/), 'a@b.com')
    await userEvent.type(screen.getByLabelText(/Naudotojo vardas/), 'user1')
    await userEvent.type(screen.getByLabelText(/Slaptažodis/), 'short')
    await userEvent.click(screen.getByLabelText(/Mokinys/))
    await userEvent.click(screen.getByRole('button', { name: /Registruotis/i }))
    expect(await screen.findByText(/Slaptažodį turi sudaryti/)).toBeInTheDocument()
    expect(mock).not.toHaveBeenCalled()
  })

  it('valid form 201 navigates to login with success message', async () => {
    const mock = vi.spyOn(api, 'registerUser')
        mock.mockResolvedValue({ status: 201, body: { id: '1', userName: 'u', role: 'Student' } })
    renderWithRoutes()
    await userEvent.type(screen.getByLabelText(/El. paštas/), 'a@b.com')
    await userEvent.type(screen.getByLabelText(/Naudotojo vardas/), 'user1')
    await userEvent.type(screen.getByLabelText(/Slaptažodis/), 'Password123')
    await userEvent.click(screen.getByLabelText(/Mokinys/))
    await userEvent.click(screen.getByRole('button', { name: /Registruotis/i }))
    await waitFor(() => expect(screen.getByRole('heading', { name: 'Prisijungimas' })).toBeInTheDocument())
    expect(screen.getByText(/Paskyra sukurta. Prisijunkite./)).toBeInTheDocument()
  })

  it('409 for email shows server emailRegistered message and keeps email but clears password', async () => {
    const mock = vi.spyOn(api, 'registerUser')
        mock.mockResolvedValue({ status: 409, body: { errors: { email: ['AlreadyRegistered'] } } })
    renderWithRoutes()
    await userEvent.type(screen.getByLabelText(/El. paštas/), 'a@b.com')
    await userEvent.type(screen.getByLabelText(/Naudotojo vardas/), 'user1')
    await userEvent.type(screen.getByLabelText(/Slaptažodis/), 'Password123')
    await userEvent.click(screen.getByLabelText(/Mokinys/))
    await userEvent.click(screen.getByRole('button', { name: /Registruotis/i }))
    expect(await screen.findByText(/Šis el. pašto adresas jau užregistruotas./)).toBeInTheDocument()
    expect((screen.getByLabelText(/El. paštas/) as HTMLInputElement).value).toBe('a@b.com')
    expect((screen.getByLabelText(/Slaptažodis/) as HTMLInputElement).value).toBe('')
  })

  it('409 for userName shows userNameTaken message', async () => {
    const mock = vi.spyOn(api, 'registerUser')
        mock.mockResolvedValue({ status: 409, body: { errors: { userName: ['AlreadyInUse'] } } })
    renderWithRoutes()
    await userEvent.type(screen.getByLabelText(/El. paštas/), 'a@b.com')
    await userEvent.type(screen.getByLabelText(/Naudotojo vardas/), 'user1')
    await userEvent.type(screen.getByLabelText(/Slaptažodis/), 'Password123')
    await userEvent.click(screen.getByLabelText(/Mokinys/))
    await userEvent.click(screen.getByRole('button', { name: /Registruotis/i }))
    expect(await screen.findByText(/Toks naudotojo vardas jau naudojamas./)).toBeInTheDocument()
  })

  it('server failure shows server failure message', async () => {
    const mock = vi.spyOn(api, 'registerUser')
        mock.mockResolvedValue({ status: 'server', message: 'Server error' })
    renderWithRoutes()
    await userEvent.type(screen.getByLabelText(/El. paštas/), 'a@b.com')
    await userEvent.type(screen.getByLabelText(/Naudotojo vardas/), 'user1')
    await userEvent.type(screen.getByLabelText(/Slaptažodis/), 'Password123')
    await userEvent.click(screen.getByLabelText(/Mokinys/))
    await userEvent.click(screen.getByRole('button', { name: /Registruotis/i }))
    expect(await screen.findByText(/Nepavyko užregistruoti. Bandykite vėliau./)).toBeInTheDocument()
  })

  it('button is disabled while request pending', async () => {
    let resolve: (v: unknown) => void = () => {}
    const p = new Promise<unknown>((r) => { resolve = r })
    const mock = vi.spyOn(api, 'registerUser')
    mock.mockReturnValue(p as unknown as Promise<import('../api/authApi').RegisterResult>)
    renderWithRoutes()
    await userEvent.type(screen.getByLabelText(/El. paštas/), 'a@b.com')
    await userEvent.type(screen.getByLabelText(/Naudotojo vardas/), 'user1')
    await userEvent.type(screen.getByLabelText(/Slaptažodis/), 'Password123')
    await userEvent.click(screen.getByLabelText(/Mokinys/))
    await userEvent.click(screen.getByRole('button', { name: /Registruotis/i }))
    expect(screen.getByRole('button', { name: /Registruotis/i })).toBeDisabled()
    resolve({ status: 201, body: { id: '1', userName: 'u', role: 'Student' } })
    await waitFor(() => expect(screen.getByRole('heading', { name: 'Prisijungimas' })).toBeInTheDocument())
  })
})
