import { retry, serverError } from '../helpers/stateMessages'

type Props = {
  onRetry: () => void
}

export default function ErrorState({ onRetry }: Props) {
  return (
    <div>
      <div role="alert" className="error-text" style={{ marginBottom: '0.5rem' }}>
        {serverError}
      </div>
      <button type="button" onClick={onRetry}>
        {retry}
      </button>
    </div>
  )
}
