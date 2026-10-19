import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { MemoryRouter, Route, Routes, useLocation } from 'react-router-dom'
import { deleteClass, getClass } from '../api/classesApi'
import { classCreated, classDeleted } from '../helpers/classMessages'
import { accessDenied, loading, pageNotFound, retry, serverError } from '../helpers/stateMessages'
import ClassDetailsPage from '../pages/ClassDetailsPage'

vi.mock('../api/classesApi', () => ({ getClass: vi.fn(), deleteClass: vi.fn() }))

const schoolClass = { id: 'abc', name: '7a klasė', code: 'AB12CD', createdAtUtc: '2026-10-19T10:00:00Z' }

function ListStub() {
  const location = useLocation()
  const message = (location.state as { message?: string } | null)?.message
  return <div>Class list stub {message}</div>
}

function renderDetails(state?: { message: string }) {
  return render(
    <MemoryRouter initialEntries={[{ pathname: '/classes/abc', state }]}>
      <Routes>
        <Route path="/classes" element={<ListStub />} />
        <Route path="/classes/:id" element={<ClassDetailsPage />} />
      </Routes>
    </MemoryRouter>,
  )
}

describe('ClassDetailsPage', () => {
  afterEach(() => {
    vi.restoreAllMocks()
    vi.clearAllMocks()
  })

  it('shows the name and the code of the class', async () => {
    vi.mocked(getClass).mockResolvedValue({ status: 200, body: schoolClass })
    renderDetails()

    expect(await screen.findByRole('heading', { name: '7a klasė' })).toBeInTheDocument()
    expect(screen.getByText('AB12CD')).toBeInTheDocument()
    expect(getClass).toHaveBeenCalledWith('abc')
  })

  it('shows the created message passed from the new class screen', async () => {
    vi.mocked(getClass).mockResolvedValue({ status: 200, body: schoolClass })
    renderDetails({ message: classCreated })

    expect(await screen.findByText(classCreated)).toBeInTheDocument()
  })

  it('shows the loading state while the class is loaded', async () => {
    vi.mocked(getClass).mockReturnValue(new Promise(() => {}))
    renderDetails()

    expect(await screen.findByText(loading)).toBeInTheDocument()
  })

  it('shows access denied when the server returns 403', async () => {
    vi.mocked(getClass).mockResolvedValue({ status: 403 })
    renderDetails()

    expect(await screen.findByRole('heading', { name: accessDenied })).toBeInTheDocument()
  })

  it('shows page not found when the server returns 404', async () => {
    vi.mocked(getClass).mockResolvedValue({ status: 404 })
    renderDetails()

    expect(await screen.findByRole('heading', { name: pageNotFound })).toBeInTheDocument()
  })

  it('shows the error state and loads the class again when retry is pressed', async () => {
    vi.mocked(getClass).mockResolvedValueOnce({ status: 'server', message: 'x' })
    renderDetails()

    expect(await screen.findByRole('alert')).toHaveTextContent(serverError)

    vi.mocked(getClass).mockResolvedValueOnce({ status: 200, body: schoolClass })
    await userEvent.click(screen.getByRole('button', { name: retry }))

    expect(await screen.findByRole('heading', { name: '7a klasė' })).toBeInTheDocument()
  })

  it('asks for confirmation with the name of the class before deleting', async () => {
    vi.mocked(getClass).mockResolvedValue({ status: 200, body: schoolClass })
    renderDetails()

    await userEvent.click(await screen.findByRole('button', { name: 'Ištrinti klasę' }))

    expect(screen.getByRole('dialog')).toHaveTextContent('Ištrinti klasę „7a klasė“?')
    expect(deleteClass).not.toHaveBeenCalled()
  })

  it('does not delete the class when the teacher cancels', async () => {
    vi.mocked(getClass).mockResolvedValue({ status: 200, body: schoolClass })
    renderDetails()

    await userEvent.click(await screen.findByRole('button', { name: 'Ištrinti klasę' }))
    await userEvent.click(screen.getByRole('button', { name: 'Atšaukti' }))

    expect(screen.queryByRole('dialog')).not.toBeInTheDocument()
    expect(deleteClass).not.toHaveBeenCalled()
    expect(screen.getByRole('heading', { name: '7a klasė' })).toBeInTheDocument()
  })

  it('deletes the class after confirmation and opens the class list with the deleted message', async () => {
    vi.mocked(getClass).mockResolvedValue({ status: 200, body: schoolClass })
    vi.mocked(deleteClass).mockResolvedValue({ status: 204 })
    renderDetails()

    await userEvent.click(await screen.findByRole('button', { name: 'Ištrinti klasę' }))
    await userEvent.click(screen.getByRole('button', { name: 'Ištrinti' }))

    expect(deleteClass).toHaveBeenCalledWith('abc')
    expect(await screen.findByText(new RegExp(`Class list stub ${classDeleted}`))).toBeInTheDocument()
  })

  it('shows the server error and keeps the class when deleting fails', async () => {
    vi.mocked(getClass).mockResolvedValue({ status: 200, body: schoolClass })
    vi.mocked(deleteClass).mockResolvedValue({ status: 'server', message: 'x' })
    renderDetails()

    await userEvent.click(await screen.findByRole('button', { name: 'Ištrinti klasę' }))
    await userEvent.click(screen.getByRole('button', { name: 'Ištrinti' }))

    expect(await screen.findByRole('alert')).toHaveTextContent(serverError)
    expect(screen.getByRole('heading', { name: '7a klasė' })).toBeInTheDocument()
  })
})
