import type { CurrentUser } from '../types/auth'

export function getStartingScreen(role: CurrentUser['role']): string {
  if (role === 'Teacher') return '/classes'
  if (role === 'Student') return '/tasks'
  return '/profile'
}
