import { render, screen } from '@testing-library/react'
import HomePage from '../pages/HomePage'

describe('HomePage', () => {
  it('shows the project title as the main heading', () => {
    render(<HomePage />)

    expect(
      screen.getByRole('heading', {
        level: 1,
        name: 'Baltijos biologinės įvairovės iššūkių žaidimas',
      }),
    ).toBeInTheDocument()
  })
})
