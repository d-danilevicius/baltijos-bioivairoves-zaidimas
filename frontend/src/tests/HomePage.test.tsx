import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { MemoryRouter, Route, Routes } from 'react-router-dom'
import { getCurrentUser } from '../api/authApi'
import { loading, retry, serverError } from '../helpers/stateMessages'
import HomePage from '../pages/HomePage'

vi.mock('../api/authApi', () => ({
  getCurrentUser: vi.fn(),
}))

function renderHomePage() {
  return render(
    <MemoryRouter initialEntries={['/']}>
      <Routes>
        <Route path="/" element={<HomePage />} />
        <Route path="/profile" element={<div>Profile stub</div>} />
      </Routes>
    </MemoryRouter>,
  )
}

describe('HomePage', () => {
  afterEach(() => {
    vi.restoreAllMocks()
    vi.clearAllMocks()
    sessionStorage.clear()
  })

  it('shows the title, the description and the login and register links to a visitor', async () => {
    vi.mocked(getCurrentUser).mockResolvedValue({ status: 401 })
    renderHomePage()

    expect(
      await screen.findByRole('heading', {
        level: 1,
        name: 'Baltijos biologinės įvairovės iššūkių žaidimas',
      }),
    ).toBeInTheDocument()
    expect(screen.getByText(/Edukacinis lauko žaidimas/)).toBeInTheDocument()
    expect(screen.getByRole('link', { name: 'Prisijungti' })).toHaveAttribute('href', '/login')
    expect(screen.getByRole('link', { name: 'Registruotis' })).toHaveAttribute('href', '/register')
  })

  it('redirects a logged-in user to the profile page', async () => {
    vi.mocked(getCurrentUser).mockResolvedValue({
      status: 200,
      body: { id: '1', userName: 'Jonas', email: 'jonas@example.com', role: 'Teacher' },
    })
    renderHomePage()

    expect(await screen.findByText('Profile stub')).toBeInTheDocument()
    expect(screen.queryByRole('link', { name: 'Prisijungti' })).not.toBeInTheDocument()
  })

  it('shows the loading state while the session is being checked', () => {
    vi.mocked(getCurrentUser).mockReturnValue(new Promise(() => {}))
    renderHomePage()

    expect(screen.getByRole('status')).toHaveTextContent(loading)
  })

  it('shows the error state and checks the session again when retry is pressed', async () => {
    vi.mocked(getCurrentUser).mockResolvedValueOnce({ status: 'server', message: 'x' })
    renderHomePage()

    expect(await screen.findByRole('alert')).toHaveTextContent(serverError)

    vi.mocked(getCurrentUser).mockResolvedValueOnce({ status: 401 })
    await userEvent.click(screen.getByRole('button', { name: retry }))

    expect(await screen.findByRole('link', { name: 'Prisijungti' })).toBeInTheDocument()
    expect(getCurrentUser).toHaveBeenCalledTimes(2)
  })

  it('does not show the menu to a visitor on the home page', async () => {
    vi.mocked(getCurrentUser).mockResolvedValue({ status: 401 })
    renderHomePage()

    await screen.findByRole('link', { name: 'Prisijungti' })
    expect(screen.queryByRole('navigation')).not.toBeInTheDocument()
  })
})
