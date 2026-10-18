import { retry, serverError } from '../helpers/stateMessages'
import Button from './button'

type Props = {
  onRetry: () => void
}

export default function ErrorState({ onRetry }: Props) {
  return (
    <div className="rounded-2xl bg-white p-6 shadow-md">
      <div role="alert" className="mb-2 text-error">
        {serverError}
      </div>
      <Button type="button" onClick={onRetry}>
        {retry}
      </Button>
    </div>
  )
}
