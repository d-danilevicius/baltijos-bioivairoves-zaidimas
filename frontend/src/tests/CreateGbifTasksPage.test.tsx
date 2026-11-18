import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { MemoryRouter, Route, Routes, useLocation } from 'react-router-dom'
import { createGbifTasks } from '../api/classTaskApi'
import {
  createGbifTasksRetry,
  createGbifTasksSubmit,
  gbifFailed,
  gbifTasksCreated,
  locationOutsideLithuania,
  locationRequired,
  seasonRequired,
  tooFewObservations,
} from '../helpers/gbifTaskMessage'
import { accessDenied, pageNotFound, serverError } from '../helpers/stateMessage'
import CreateGbifTasksPage from '../pages/CreateGbifTasksPage'

vi.mock('../api/classTaskApi', () => ({ createGbifTasks: vi.fn() }))
vi.mock('../components/locationMap', () => ({
  default: ({ onSelect }: { onSelect: (latitude: number, longitude: number) => void }) => (
    <button type="button" onClick={() => onSelect(54.68716, 25.27971)}>
      Žemėlapio stub
    </button>
  ),
}))

function TasksStub() {
  const message = (useLocation().state as { message?: string } | null)?.message
  return <div>Tasks stub {message}</div>
}

function LoginStub() {
  return <div>Login stub</div>
}

function renderPage() {
  return render(
    <MemoryRouter initialEntries={['/classes/abc/tasks/gbif']}>
      <Routes>
        <Route path="/classes/:id/tasks/gbif" element={<CreateGbifTasksPage />} />
        <Route path="/classes/:id/tasks" element={<TasksStub />} />
        <Route path="/login" element={<LoginStub />} />
      </Routes>
    </MemoryRouter>,
  )
}

async function fillAndSubmit() {
  await userEvent.click(screen.getByRole('button', { name: 'Žemėlapio stub' }))
  await userEvent.click(screen.getByLabelText('Ruduo'))
  await userEvent.click(screen.getByRole('button', { name: createGbifTasksSubmit }))
}

describe('CreateGbifTasksPage', () => {
  beforeEach(() => {
    vi.mocked(createGbifTasks).mockReset()
  })

  it('shows location and season errors when the form is empty', async () => {
    renderPage()
    await userEvent.click(screen.getByRole('button', { name: createGbifTasksSubmit }))
    expect(screen.getByText(locationRequired)).toBeInTheDocument()
    expect(screen.getByText(seasonRequired)).toBeInTheDocument()
    expect(createGbifTasks).not.toHaveBeenCalled()
  })

  it('fills the coordinates when the map is clicked', async () => {
    renderPage()
    await userEvent.click(screen.getByRole('button', { name: 'Žemėlapio stub' }))
    expect(screen.getByLabelText('Platuma')).toHaveValue('54.68716')
    expect(screen.getByLabelText('Ilguma')).toHaveValue('25.27971')
  })

  it('sends the chosen location and season and opens the task list', async () => {
    vi.mocked(createGbifTasks).mockResolvedValue({ status: 201, body: { createdCount: 5 } })
    renderPage()
    await fillAndSubmit()
    expect(createGbifTasks).toHaveBeenCalledWith('abc', { latitude: 54.68716, longitude: 25.27971, season: 'Autumn' })
    expect(await screen.findByText(`Tasks stub ${gbifTasksCreated}`)).toBeInTheDocument()
  })

  it('shows a loading message and disables the button while sending', async () => {
    vi.mocked(createGbifTasks).mockReturnValue(new Promise(() => {}))
    renderPage()
    await fillAndSubmit()
    expect(await screen.findByRole('status')).toBeInTheDocument()
    expect(screen.getByRole('button', { name: createGbifTasksSubmit })).toBeDisabled()
  })

  it('shows the Lithuania error when the server rejects the location', async () => {
    vi.mocked(createGbifTasks).mockResolvedValue({
      status: 400,
      body: { errors: { latitude: ['OutsideLithuania'] } },
    })
    renderPage()
    await fillAndSubmit()
    expect(await screen.findByText(locationOutsideLithuania)).toBeInTheDocument()
  })

  it('opens the task list when the tasks already exist', async () => {
    vi.mocked(createGbifTasks).mockResolvedValue({ status: 409 })
    renderPage()
    await fillAndSubmit()
    expect(await screen.findByText(/Tasks stub/)).toBeInTheDocument()
  })

  it('shows the too few observations message', async () => {
    vi.mocked(createGbifTasks).mockResolvedValue({ status: 422 })
    renderPage()
    await fillAndSubmit()
    expect(await screen.findByText(tooFewObservations)).toBeInTheDocument()
  })

  it('shows the GBIF error and retries on request', async () => {
    vi.mocked(createGbifTasks)
      .mockResolvedValueOnce({ status: 502 })
      .mockResolvedValueOnce({ status: 201, body: { createdCount: 3 } })
    renderPage()
    await fillAndSubmit()
    expect(await screen.findByText(gbifFailed)).toBeInTheDocument()
    await userEvent.click(screen.getByRole('button', { name: createGbifTasksRetry }))
    expect(await screen.findByText(/Tasks stub/)).toBeInTheDocument()
    expect(createGbifTasks).toHaveBeenCalledTimes(2)
  })

  it('shows the server error message', async () => {
    vi.mocked(createGbifTasks).mockResolvedValue({ status: 'server', message: 'x' })
    renderPage()
    await fillAndSubmit()
    expect(await screen.findByText(serverError)).toBeInTheDocument()
  })

  it('shows access denied for status 403', async () => {
    vi.mocked(createGbifTasks).mockResolvedValue({ status: 403 })
    renderPage()
    await fillAndSubmit()
    expect(await screen.findByText(accessDenied)).toBeInTheDocument()
  })

  it('shows page not found for status 404', async () => {
    vi.mocked(createGbifTasks).mockResolvedValue({ status: 404 })
    renderPage()
    await fillAndSubmit()
    expect(await screen.findByText(pageNotFound)).toBeInTheDocument()
  })
})
