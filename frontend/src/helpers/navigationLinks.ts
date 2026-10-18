export type NavigationLink = { to: string; label: string }

export const visitorLinks: NavigationLink[] = [
  { to: '/', label: 'Pagrindinis' },
  { to: '/login', label: 'Prisijungimas' },
  { to: '/register', label: 'Registracija' },
]

export const userLinks: NavigationLink[] = [{ to: '/profile', label: 'Profilis' }]
