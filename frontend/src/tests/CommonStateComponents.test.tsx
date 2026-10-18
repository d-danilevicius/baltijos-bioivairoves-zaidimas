import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import LoadingState from '../components/loadingState'
import ErrorState from '../components/errorState'
import EmptyState from '../components/emptyState'
import { loading, retry, serverError } from '../helpers/stateMessages'

describe('Common state components', () => {
  it('LoadingState shows the loading text in a status element', () => {
    render(<LoadingState />)

    expect(screen.getByRole('status')).toHaveTextContent(loading)
  })

  it('ErrorState shows the server error text in an alert element', () => {
    render(<ErrorState onRetry={vi.fn()} />)

    expect(screen.getByRole('alert')).toHaveTextContent(serverError)
  })

  it('ErrorState calls onRetry once when the retry button is clicked', async () => {
    const onRetry = vi.fn()
    render(<ErrorState onRetry={onRetry} />)

    await userEvent.click(screen.getByRole('button', { name: retry }))

    expect(onRetry).toHaveBeenCalledTimes(1)
  })

  it('EmptyState shows the given message in a status element', () => {
    render(<EmptyState message="Nėra įrašų." />)

    expect(screen.getByRole('status')).toHaveTextContent('Nėra įrašų.')
  })

  it('EmptyState renders the given children', () => {
    render(
      <EmptyState message="Nėra įrašų.">
        <button type="button">Pridėti</button>
      </EmptyState>
    )

    expect(screen.getByRole('button', { name: 'Pridėti' })).toBeInTheDocument()
  })
})
