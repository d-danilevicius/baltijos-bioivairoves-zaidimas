import MessagePage from '../components/messagePage'
import { pageNotFound, toHomePage } from '../helpers/stateMessages'

export default function NotFoundPage() {
  return <MessagePage title={pageNotFound} actionLabel={toHomePage} />
}
