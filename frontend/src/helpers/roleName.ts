import type { LoggedInUser } from '../types/auth'

const roleNames: Record<LoggedInUser['role'], string> = {
  Student: 'Mokinys',
  Teacher: 'Mokytojas',
  Administrator: 'Administratorius',
}

export function getRoleName(role: LoggedInUser['role']): string {
  return roleNames[role]
}
