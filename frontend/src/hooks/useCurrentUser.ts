import { useOutletContext } from 'react-router-dom'
import type { CurrentUser } from '../types/auth'

export function useCurrentUser(): CurrentUser {
  return useOutletContext<{ user: CurrentUser }>().user
}
