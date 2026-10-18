import { loading } from '../helpers/stateMessages'

export default function LoadingState() {
  return <div role="status" className="rounded-2xl bg-white px-6 py-4 shadow-md">{loading}</div>
}
