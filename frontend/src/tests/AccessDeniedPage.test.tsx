import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { MemoryRouter, Route, Routes } from 'react-router-dom'
import AccessDeniedPage from '../pages/AccessDeniedPage'
import { accessDenied, toHomePage } from '../helpers/stateMessages'

function renderAccessDeniedPage() {
  return render(
    <MemoryRouter initialEntries={['/access-denied']}>
      <Routes>
        <Route path="/" element={<div>Home stub</div>} />
        <Route path="/access-denied" element={<AccessDeniedPage />} />
      </Routes>
    </MemoryRouter>,
  )
}

describe('AccessDeniedPage', () => {
  it('shows the access denied message as the main heading', () => {
    renderAccessDeniedPage()

    expect(screen.getByRole('heading', { level: 1, name: accessDenied })).toBeInTheDocument()
  })

  it('shows a link to the home page pointing to "/"', () => {
    renderAccessDeniedPage()

    expect(screen.getByRole('link', { name: toHomePage })).toHaveAttribute('href', '/')
  })

  it('navigates to the home page when the link is clicked', async () => {
    renderAccessDeniedPage()

    await userEvent.click(screen.getByRole('link', { name: toHomePage }))

    expect(screen.getByText('Home stub')).toBeInTheDocument()
  })
})
