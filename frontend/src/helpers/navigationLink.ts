import type { CurrentUser } from '../types/auth'

export type NavigationLink = { to: string; label: string }

export const visitorLinks: NavigationLink[] = [
  { to: '/', label: 'Pagrindinis' },
  { to: '/login', label: 'Prisijungimas' },
  { to: '/register', label: 'Registracija' },
]

export const userLinks: NavigationLink[] = [{ to: '/profile', label: 'Profilis' }]

export const studentLinks: NavigationLink[] = [
  { to: '/tasks', label: 'Užduotys' },
  { to: '/join', label: 'Prisijungti prie klasės' },
  { to: '/profile', label: 'Profilis' },
]

export const teacherLinks: NavigationLink[] = [
  { to: '/classes', label: 'Klasės' },
  { to: '/profile', label: 'Profilis' },
]

export function getUserLinks(role: CurrentUser['role']): NavigationLink[] {
  if (role === 'Teacher') return teacherLinks
  if (role === 'Student') return studentLinks
  return userLinks
}
