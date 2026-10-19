import type { CurrentUser } from '../types/auth'

export function getStartingScreen(role: CurrentUser['role']): string {
  return role === 'Teacher' ? '/classes' : '/profile'
}
