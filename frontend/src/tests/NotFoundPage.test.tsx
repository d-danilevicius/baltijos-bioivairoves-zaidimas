import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { MemoryRouter, Route, Routes } from 'react-router-dom'
import App from '../app'
import NotFoundPage from '../pages/NotFoundPage'
import { pageNotFound, toHomePage } from '../helpers/stateMessages'

function renderNotFoundPage() {
  return render(
    <MemoryRouter initialEntries={['/nezinomas-adresas']}>
      <Routes>
        <Route path="/" element={<div>Home stub</div>} />
        <Route path="*" element={<NotFoundPage />} />
      </Routes>
    </MemoryRouter>,
  )
}

describe('NotFoundPage', () => {
  it('shows the page not found message as the main heading', () => {
    renderNotFoundPage()

    expect(screen.getByRole('heading', { level: 1, name: pageNotFound })).toBeInTheDocument()
  })

  it('shows a link to the home page pointing to "/"', () => {
    renderNotFoundPage()

    expect(screen.getByRole('link', { name: toHomePage })).toHaveAttribute('href', '/')
  })

  it('navigates to the home page when the link is clicked', async () => {
    renderNotFoundPage()

    await userEvent.click(screen.getByRole('link', { name: toHomePage }))

    expect(screen.getByText('Home stub')).toBeInTheDocument()
  })

  it('App renders the page not found screen for an unknown URL', () => {
    render(
      <MemoryRouter initialEntries={['/nezinomas-adresas']}>
        <App />
      </MemoryRouter>,
    )

    expect(screen.getByRole('heading', { level: 1, name: pageNotFound })).toBeInTheDocument()
    expect(screen.getByRole('link', { name: toHomePage })).toHaveAttribute('href', '/')
  })
})
