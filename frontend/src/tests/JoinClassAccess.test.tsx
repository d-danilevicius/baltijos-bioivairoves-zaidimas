import { render, screen } from '@testing-library/react'
import { MemoryRouter } from 'react-router-dom'
import { getCurrentUser } from '../api/authApi'
import { accessDenied } from '../helpers/stateMessage'
import { markSessionStarted } from '../helpers/sessionMarker'
import App from '../app'
import type { CurrentUser } from '../types/auth'

vi.mock('../api/authApi', () => ({
  getCurrentUser: vi.fn(),
  loginUser: vi.fn(),
}))

function renderJoinPage(user: CurrentUser) {
  markSessionStarted()
  vi.mocked(getCurrentUser).mockResolvedValue({ status: 200, body: user })
  return render(
    <MemoryRouter initialEntries={['/join']}>
      <App />
    </MemoryRouter>,
  )
}

describe('Join class route', () => {
  afterEach(() => {
    vi.restoreAllMocks()
    vi.clearAllMocks()
    sessionStorage.clear()
  })

  it('shows the access denied page when a teacher opens the join page', async () => {
    renderJoinPage({ id: '1', userName: 'Jonas', email: 'jonas@example.com', role: 'Teacher' })

    expect(await screen.findByRole('heading', { name: accessDenied })).toBeInTheDocument()
    expect(screen.queryByLabelText('Klasės kodas')).not.toBeInTheDocument()
  })

  it('shows the join form when a student opens the join page', async () => {
    renderJoinPage({ id: '2', userName: 'Ona', email: 'ona@example.com', role: 'Student' })

    expect(await screen.findByLabelText('Klasės kodas')).toBeInTheDocument()
  })
})
