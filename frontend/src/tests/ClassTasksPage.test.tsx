import { render, screen, within } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { MemoryRouter, Route, Routes } from 'react-router-dom'
import { getClass } from '../api/classApi'
import { deleteClassTask, listClassTasks } from '../api/classTaskApi'
import { classSeasonInfo, createFromGbif, noTasksYet, taskTypeLabels } from '../helpers/gbifTaskMessage'
import { accessDenied, pageNotFound, serverError } from '../helpers/stateMessage'
import {
  createTeacherTaskLink,
  deleteTaskButton,
  deleteTaskQuestion,
  teacherTaskLabel,
} from '../helpers/teacherTaskMessage'
import ClassTasksPage from '../pages/ClassTasksPage'

vi.mock('../api/classApi', () => ({ getClass: vi.fn() }))
vi.mock('../api/classTaskApi', () => ({ listClassTasks: vi.fn(), deleteClassTask: vi.fn() }))

const schoolClass = { id: 'abc', name: '7a klasė', code: 'AB12CD', createdAtUtc: '2026-10-19T10:00:00Z' }
const task = { id: 't1', type: 'CommonSpecies' as const, difficultyGroup: 2, label: 'GBIF' }
const teacherTask = { id: 't2', type: 'TeacherTask' as const, difficultyGroup: 1, label: teacherTaskLabel }

function renderPage(state?: { message: string }) {
  return render(
    <MemoryRouter initialEntries={[{ pathname: '/classes/abc/tasks', state }]}>
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
    vi.mocked(deleteClassTask).mockReset()
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

  it('shows the teacher task label and the button to create an own task', async () => {
    vi.mocked(getClass).mockResolvedValue({ status: 200, body: { ...schoolClass, season: 'Autumn' } })
    vi.mocked(listClassTasks).mockResolvedValue({ status: 200, body: [task, teacherTask] })
    renderPage()
    expect(await screen.findAllByText(teacherTaskLabel)).not.toHaveLength(0)
    expect(screen.getByText('GBIF')).toBeInTheDocument()
    expect(screen.getByRole('link', { name: createTeacherTaskLink })).toHaveAttribute('href', '/classes/abc/tasks/new')
  })

  it('shows the delete button only next to a teacher task', async () => {
    vi.mocked(getClass).mockResolvedValue({ status: 200, body: schoolClass })
    vi.mocked(listClassTasks).mockResolvedValue({ status: 200, body: [task, teacherTask] })
    renderPage()
    await screen.findByText('GBIF')
    expect(screen.getAllByRole('button', { name: deleteTaskButton })).toHaveLength(1)
  })

  it('asks for confirmation before deleting a teacher task', async () => {
    vi.mocked(getClass).mockResolvedValue({ status: 200, body: schoolClass })
    vi.mocked(listClassTasks).mockResolvedValue({ status: 200, body: [teacherTask] })
    renderPage()
    await userEvent.click(await screen.findByRole('button', { name: deleteTaskButton }))
    expect(screen.getByRole('dialog')).toHaveTextContent(deleteTaskQuestion)
    expect(deleteClassTask).not.toHaveBeenCalled()
  })

  it('keeps the task when the teacher cancels the deletion', async () => {
    vi.mocked(getClass).mockResolvedValue({ status: 200, body: schoolClass })
    vi.mocked(listClassTasks).mockResolvedValue({ status: 200, body: [teacherTask] })
    renderPage()
    await userEvent.click(await screen.findByRole('button', { name: deleteTaskButton }))
    await userEvent.click(screen.getByRole('button', { name: 'Atšaukti' }))
    expect(screen.queryByRole('dialog')).not.toBeInTheDocument()
    expect(deleteClassTask).not.toHaveBeenCalled()
    expect(screen.getAllByText(teacherTaskLabel)).not.toHaveLength(0)
  })

  it('deletes the teacher task after the confirmation and removes it from the list', async () => {
    vi.mocked(getClass).mockResolvedValue({ status: 200, body: schoolClass })
    vi.mocked(listClassTasks)
      .mockResolvedValueOnce({ status: 200, body: [teacherTask] })
      .mockResolvedValue({ status: 200, body: [] })
    vi.mocked(deleteClassTask).mockResolvedValue({ status: 204 })
    renderPage()
    await userEvent.click(await screen.findByRole('button', { name: deleteTaskButton }))
    await userEvent.click(within(screen.getByRole('dialog')).getByRole('button', { name: deleteTaskButton }))
    expect(deleteClassTask).toHaveBeenCalledWith('abc', 't2')
    expect(await screen.findByText(noTasksYet)).toBeInTheDocument()
  })

  it('removes the saved message after a task is deleted', async () => {
    vi.mocked(getClass).mockResolvedValue({ status: 200, body: schoolClass })
    vi.mocked(listClassTasks).mockResolvedValue({ status: 200, body: [teacherTask] })
    vi.mocked(deleteClassTask).mockResolvedValue({ status: 204 })
    renderPage({ message: 'Užduotis išsaugota.' })
    expect(await screen.findByText('Užduotis išsaugota.')).toBeInTheDocument()
    await userEvent.click(screen.getByRole('button', { name: deleteTaskButton }))
    await userEvent.click(within(screen.getByRole('dialog')).getByRole('button', { name: deleteTaskButton }))
    expect(await screen.findByText(noTasksYet)).toBeInTheDocument()
    expect(screen.queryByText('Užduotis išsaugota.')).not.toBeInTheDocument()
  })

  it('shows the server error and keeps the task when deleting fails', async () => {
    vi.mocked(getClass).mockResolvedValue({ status: 200, body: schoolClass })
    vi.mocked(listClassTasks).mockResolvedValue({ status: 200, body: [teacherTask] })
    vi.mocked(deleteClassTask).mockResolvedValue({ status: 'server', message: 'x' })
    renderPage()
    await userEvent.click(await screen.findByRole('button', { name: deleteTaskButton }))
    await userEvent.click(within(screen.getByRole('dialog')).getByRole('button', { name: deleteTaskButton }))
    expect(await screen.findByRole('alert')).toHaveTextContent(serverError)
    expect(screen.getAllByText(teacherTaskLabel)).not.toHaveLength(0)
  })

  it('shows page not found when the task was already deleted (404)', async () => {
    vi.mocked(getClass).mockResolvedValue({ status: 200, body: schoolClass })
    vi.mocked(listClassTasks).mockResolvedValue({ status: 200, body: [teacherTask] })
    vi.mocked(deleteClassTask).mockResolvedValue({ status: 404 })
    renderPage()
    await userEvent.click(await screen.findByRole('button', { name: deleteTaskButton }))
    await userEvent.click(within(screen.getByRole('dialog')).getByRole('button', { name: deleteTaskButton }))
    expect(await screen.findByText(pageNotFound)).toBeInTheDocument()
  })
})
