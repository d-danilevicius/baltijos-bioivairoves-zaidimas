import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { MemoryRouter, Route, Routes, useLocation } from 'react-router-dom'
import { joinClass } from '../api/classMembershipApi'
import {
  alreadyInClass,
  changeClassQuestion,
  classCodeInvalid,
  classCodeNotFound,
  classCodeRequired,
  joinedClass,
} from '../helpers/joinClassMessage'
import { hadSession, markSessionStarted } from '../helpers/sessionMarker'
import { serverError, sessionExpired } from '../helpers/stateMessage'
import JoinClassPage from '../pages/JoinClassPage'

vi.mock('../api/classMembershipApi', () => ({ joinClass: vi.fn() }))

function ProfileStub() {
  const location = useLocation()
  const message = (location.state as { message?: string } | null)?.message
  return <div>Profile stub {message}</div>
}

function LoginStub() {
  const location = useLocation()
  const message = (location.state as { message?: string } | null)?.message
  return <div>Login stub {message}</div>
}

function renderJoinClass() {
  return render(
    <MemoryRouter initialEntries={['/join']}>
      <Routes>
        <Route path="/join" element={<JoinClassPage />} />
        <Route path="/profile" element={<ProfileStub />} />
        <Route path="/login" element={<LoginStub />} />
      </Routes>
    </MemoryRouter>,
  )
}

async function submitCode(code: string) {
  const field = screen.getByLabelText('Klasės kodas')
  await userEvent.clear(field)
  if (code !== '') await userEvent.type(field, code)
  await userEvent.click(screen.getByRole('button', { name: 'Prisijungti' }))
}

describe('JoinClassPage', () => {
  afterEach(() => {
    vi.restoreAllMocks()
    vi.clearAllMocks()
  })

  it.each(['', '   '])('shows the required message and does not call the server for the code "%s"', async (code) => {
    renderJoinClass()

    await submitCode(code)

    expect(await screen.findByText(classCodeRequired)).toBeInTheDocument()
    expect(joinClass).not.toHaveBeenCalled()
  })

  it('shows the invalid message and does not call the server for a code of 5 characters', async () => {
    renderJoinClass()

    await submitCode('ABC12')

    expect(await screen.findByText(classCodeInvalid)).toBeInTheDocument()
    expect(joinClass).not.toHaveBeenCalled()
  })

  it('sends the normalized code and opens the profile with the joined message', async () => {
    vi.mocked(joinClass).mockResolvedValue({ status: 200, body: { className: '7a', alreadyMember: false } })
    renderJoinClass()

    await submitCode(' ab 12 cd ')

    expect(joinClass).toHaveBeenCalledWith('AB12CD', false)
    expect(await screen.findByText(new RegExp(`Profile stub ${joinedClass('7a')}`))).toBeInTheDocument()
  })

  it('opens the profile with the already in class message when the student is already in the class', async () => {
    vi.mocked(joinClass).mockResolvedValue({ status: 200, body: { className: '7a', alreadyMember: true } })
    renderJoinClass()

    await submitCode('AB12CD')

    expect(await screen.findByText(new RegExp(`Profile stub ${alreadyInClass}`))).toBeInTheDocument()
  })

  it('shows the not found message next to the field when the server does not find the class', async () => {
    vi.mocked(joinClass).mockResolvedValue({ status: 400, body: { errors: { code: ['NotFound'] } } })
    renderJoinClass()

    await submitCode('AB12CD')

    expect(await screen.findByText(classCodeNotFound)).toBeInTheDocument()
    expect(screen.getByLabelText('Klasės kodas')).toHaveValue('AB12CD')
  })

  it('asks for the confirmation with both class names when the server returns 409', async () => {
    vi.mocked(joinClass).mockResolvedValue({ status: 409, body: { currentClassName: '7b', newClassName: '7a' } })
    renderJoinClass()

    await submitCode('AB12CD')

    expect(await screen.findByText(changeClassQuestion('7b', '7a'))).toBeInTheDocument()
    expect(joinClass).toHaveBeenCalledTimes(1)
  })

  it('sends the code again with the confirmation and opens the profile when the student presses Pakeisti', async () => {
    vi.mocked(joinClass)
      .mockResolvedValueOnce({ status: 409, body: { currentClassName: '7b', newClassName: '7a' } })
      .mockResolvedValueOnce({ status: 200, body: { className: '7a', alreadyMember: false } })
    renderJoinClass()

    await submitCode('AB12CD')
    await userEvent.click(await screen.findByRole('button', { name: 'Pakeisti' }))

    expect(joinClass).toHaveBeenLastCalledWith('AB12CD', true)
    expect(await screen.findByText(new RegExp(`Profile stub ${joinedClass('7a')}`))).toBeInTheDocument()
  })

  it('does not send anything and stays on the page when the student presses Atšaukti', async () => {
    vi.mocked(joinClass).mockResolvedValue({ status: 409, body: { currentClassName: '7b', newClassName: '7a' } })
    renderJoinClass()

    await submitCode('AB12CD')
    await userEvent.click(await screen.findByRole('button', { name: 'Atšaukti' }))

    expect(joinClass).toHaveBeenCalledTimes(1)
    expect(screen.queryByText(changeClassQuestion('7b', '7a'))).not.toBeInTheDocument()
    expect(screen.getByLabelText('Klasės kodas')).toBeInTheDocument()
  })

  it('shows the server error and stays on the page when the request fails', async () => {
    vi.mocked(joinClass).mockResolvedValue({ status: 'server', message: 'x' })
    renderJoinClass()

    await submitCode('AB12CD')

    expect(await screen.findByRole('alert')).toHaveTextContent(serverError)
    expect(screen.getByLabelText('Klasės kodas')).toBeInTheDocument()
  })

  it('opens the login page with the session expired message when the server returns 401', async () => {
    markSessionStarted()
    vi.mocked(joinClass).mockResolvedValue({ status: 401 })
    renderJoinClass()

    await submitCode('AB12CD')

    expect(await screen.findByText(new RegExp(`Login stub ${sessionExpired}`))).toBeInTheDocument()
    expect(hadSession()).toBe(false)
  })

  it('disables the button while the request is being sent', async () => {
    vi.mocked(joinClass).mockReturnValue(new Promise(() => {}))
    renderJoinClass()

    await submitCode('AB12CD')

    expect(screen.getByRole('button', { name: 'Prisijungti' })).toBeDisabled()
  })
})
