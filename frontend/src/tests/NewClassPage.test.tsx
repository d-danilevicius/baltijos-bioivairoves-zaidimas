import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { MemoryRouter, Route, Routes, useLocation } from 'react-router-dom'
import { createClass } from '../api/classesApi'
import { classCreated, classNameRequired, classNameTooLong } from '../helpers/classMessages'
import { hadSession, markSessionStarted } from '../helpers/sessionMarker'
import { serverError, sessionExpired } from '../helpers/stateMessages'
import NewClassPage from '../pages/NewClassPage'

vi.mock('../api/classesApi', () => ({ createClass: vi.fn() }))

function DetailsStub() {
  const location = useLocation()
  const message = (location.state as { message?: string } | null)?.message
  return (
    <div>
      Class details stub {location.pathname} {message}
    </div>
  )
}

function LoginStub() {
  const location = useLocation()
  const message = (location.state as { message?: string } | null)?.message
  return <div>Login stub {message}</div>
}

function renderNewClass() {
  return render(
    <MemoryRouter initialEntries={['/classes/new']}>
      <Routes>
        <Route path="/classes/new" element={<NewClassPage />} />
        <Route path="/login" element={<LoginStub />} />
        <Route path="/classes/:id" element={<DetailsStub />} />
      </Routes>
    </MemoryRouter>,
  )
}

async function submitName(name: string) {
  const field = screen.getByLabelText('Klasės pavadinimas')
  await userEvent.clear(field)
  if (name !== '') await userEvent.type(field, name)
  await userEvent.click(screen.getByRole('button', { name: 'Sukurti' }))
}

describe('NewClassPage', () => {
  afterEach(() => {
    vi.restoreAllMocks()
    vi.clearAllMocks()
  })

  it.each(['', '   '])('shows the required message and does not call the server for the name "%s"', async (name) => {
    renderNewClass()

    await submitName(name)

    expect(await screen.findByText(classNameRequired)).toBeInTheDocument()
    expect(createClass).not.toHaveBeenCalled()
  })

  it('shows the too long message and does not call the server for a name of 51 characters', async () => {
    renderNewClass()

    await submitName('a'.repeat(51))

    expect(await screen.findByText(classNameTooLong)).toBeInTheDocument()
    expect(createClass).not.toHaveBeenCalled()
  })

  it('creates the class with the trimmed name and opens its information with the created message', async () => {
    vi.mocked(createClass).mockResolvedValue({
      status: 201,
      body: { id: 'abc', name: '7a klasė', code: 'AB12CD', createdAtUtc: '2026-10-19T10:00:00Z' },
    })
    renderNewClass()

    await submitName('  7a klasė  ')

    expect(createClass).toHaveBeenCalledWith('7a klasė')
    expect(await screen.findByText(new RegExp(`/classes/abc ${classCreated}`))).toBeInTheDocument()
  })

  it('shows the error of the server next to the field when the server returns 400', async () => {
    vi.mocked(createClass).mockResolvedValue({ status: 400, body: { errors: { name: ['TooLong'] } } })
    renderNewClass()

    await submitName('7a klasė')

    expect(await screen.findByText(classNameTooLong)).toBeInTheDocument()
  })

  it('shows the server error and stays on the page when the request fails', async () => {
    vi.mocked(createClass).mockResolvedValue({ status: 'server', message: 'x' })
    renderNewClass()

    await submitName('7a klasė')

    expect(await screen.findByRole('alert')).toHaveTextContent(serverError)
    expect(screen.getByLabelText('Klasės pavadinimas')).toBeInTheDocument()
  })

  it('opens the login page with the session expired message when the server returns 401', async () => {
    markSessionStarted()
    vi.mocked(createClass).mockResolvedValue({ status: 401 })
    renderNewClass()

    await submitName('7a klasė')

    expect(await screen.findByText(new RegExp(`Login stub ${sessionExpired}`))).toBeInTheDocument()
    expect(hadSession()).toBe(false)
  })

  it('disables the button while the class is being created', async () => {
    vi.mocked(createClass).mockReturnValue(new Promise(() => {}))
    renderNewClass()

    await submitName('7a klasė')

    expect(screen.getByRole('button', { name: 'Sukurti' })).toBeDisabled()
  })
})
