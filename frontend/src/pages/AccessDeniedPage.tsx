import MessagePage from '../components/messagePage'
import { accessDenied, toHomePage } from '../helpers/stateMessage'

export default function AccessDeniedPage() {
  return <MessagePage title={accessDenied} actionLabel={toHomePage} />
}
