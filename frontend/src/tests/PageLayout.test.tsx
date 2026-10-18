import { render, screen } from '@testing-library/react'
import { MemoryRouter } from 'react-router-dom'
import { homeTitle } from '../helpers/homeMessages'
import LoginPage from '../pages/LoginPage'
import RegisterPage from '../pages/RegisterPage'

describe('PageLayout', () => {
  it('shows the shared header with the project title on the login screen', () => {
    render(
      <MemoryRouter>
        <LoginPage />
      </MemoryRouter>,
    )

    expect(screen.getByRole('banner')).toHaveTextContent(homeTitle)
  })

  it('shows the shared header with the project title on the registration screen', () => {
    render(
      <MemoryRouter>
        <RegisterPage />
      </MemoryRouter>,
    )

    expect(screen.getByRole('banner')).toHaveTextContent(homeTitle)
  })

  it('links the project title in the header to the home page', () => {
    render(
      <MemoryRouter>
        <LoginPage />
      </MemoryRouter>,
    )

    expect(screen.getByRole('link', { name: homeTitle })).toHaveAttribute('href', '/')
  })

  it('shows the menu with the home, login and registration links to a visitor', () => {
    render(
      <MemoryRouter>
        <RegisterPage />
      </MemoryRouter>,
    )

    const menu = screen.getByRole('navigation', { name: 'Pagrindinė navigacija' })
    expect(menu).toHaveTextContent('Pagrindinis')
    expect(screen.getByRole('link', { name: 'Pagrindinis' })).toHaveAttribute('href', '/')
    expect(screen.getByRole('link', { name: 'Prisijungimas' })).toHaveAttribute('href', '/login')
    expect(screen.getByRole('link', { name: 'Registracija' })).toHaveAttribute('href', '/register')
  })

  it('marks the link of the current screen as the current page', () => {
    render(
      <MemoryRouter initialEntries={['/login']}>
        <LoginPage />
      </MemoryRouter>,
    )

    expect(screen.getByRole('link', { name: 'Prisijungimas' })).toHaveAttribute('aria-current', 'page')
  })
})
