import { render, screen } from '@testing-library/react'
import { MemoryRouter, Route, Routes } from 'react-router-dom'
import { getClass } from '../api/classApi'
import { listClassTasks } from '../api/classTaskApi'
import { classSeasonInfo, createFromGbif, noTasksYet, taskTypeLabels } from '../helpers/gbifTaskMessage'
import { accessDenied, pageNotFound } from '../helpers/stateMessage'
import ClassTasksPage from '../pages/ClassTasksPage'

vi.mock('../api/classApi', () => ({ getClass: vi.fn() }))
vi.mock('../api/classTaskApi', () => ({ listClassTasks: vi.fn() }))

const schoolClass = { id: 'abc', name: '7a klasė', code: 'AB12CD', createdAtUtc: '2026-10-19T10:00:00Z' }
const task = { id: 't1', type: 'CommonSpecies' as const, difficultyGroup: 2, label: 'GBIF' }

function renderPage() {
  return render(
    <MemoryRouter initialEntries={['/classes/abc/tasks']}>
      <Routes>
        <Route path="/classes/:id/tasks" element={<ClassTasksPage />} />
      </Routes>
    </MemoryRouter>,
  )
}

describe('ClassTasksPage', () => {
  beforeEach(() => {
    vi.mocked(getClass).mockReset()
    vi.mocked(listClassTasks).mockReset()
  })

  it('shows the empty state and the GBIF button when the class has no tasks', async () => {
    vi.mocked(getClass).mockResolvedValue({ status: 200, body: schoolClass })
    vi.mocked(listClassTasks).mockResolvedValue({ status: 200, body: [] })
    renderPage()
    expect(await screen.findByText(noTasksYet)).toBeInTheDocument()
    expect(screen.getByRole('link', { name: createFromGbif })).toHaveAttribute('href', '/classes/abc/tasks/gbif')
  })

  it('lists the tasks and hides the GBIF button when the season is set', async () => {
    vi.mocked(getClass).mockResolvedValue({ status: 200, body: { ...schoolClass, season: 'Autumn' } })
    vi.mocked(listClassTasks).mockResolvedValue({ status: 200, body: [task] })
    renderPage()
    expect(await screen.findByText(taskTypeLabels.CommonSpecies)).toBeInTheDocument()
    expect(screen.getByText(classSeasonInfo('Autumn'))).toBeInTheDocument()
    expect(screen.queryByRole('link', { name: createFromGbif })).not.toBeInTheDocument()
  })

  it('shows access denied for status 403', async () => {
    vi.mocked(getClass).mockResolvedValue({ status: 403 })
    vi.mocked(listClassTasks).mockResolvedValue({ status: 403 })
    renderPage()
    expect(await screen.findByText(accessDenied)).toBeInTheDocument()
  })

  it('shows page not found for status 404', async () => {
    vi.mocked(getClass).mockResolvedValue({ status: 404 })
    vi.mocked(listClassTasks).mockResolvedValue({ status: 404 })
    renderPage()
    expect(await screen.findByText(pageNotFound)).toBeInTheDocument()
  })

  it('shows a retry button after a server error', async () => {
    vi.mocked(getClass).mockResolvedValue({ status: 'server', message: 'x' })
    vi.mocked(listClassTasks).mockResolvedValue({ status: 200, body: [] })
    renderPage()
    expect(await screen.findByRole('button', { name: 'Bandyti dar kartą' })).toBeInTheDocument()
  })
})
