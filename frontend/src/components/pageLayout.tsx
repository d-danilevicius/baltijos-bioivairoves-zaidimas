import type { ReactNode } from 'react'
import { Link, NavLink } from 'react-router-dom'
import { homeTitle } from '../helpers/homeMessages'
import type { NavigationLink } from '../helpers/navigationLinks'

type PageLayoutProps = { children: ReactNode; links?: NavigationLink[] }

const linkBase =
  'rounded-full border-2 border-white px-4 py-1 font-semibold no-underline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-white'

export default function PageLayout({ children, links = [] }: PageLayoutProps) {
  return (
    <div className="flex min-h-screen flex-col">
      <header className="border-b-4 border-accent bg-brand px-4 py-4 text-center text-white">
        <Link to="/" className="text-lg font-bold text-white no-underline hover:underline">
          {homeTitle}
        </Link>
        {links.length > 0 && (
          <nav aria-label="Pagrindinė navigacija" className="mt-3 flex flex-wrap justify-center gap-2">
            {links.map((link) => (
              <NavLink
                key={link.to}
                to={link.to}
                end
                className={({ isActive }) =>
                  `${linkBase} ${isActive ? 'bg-white text-brand' : 'text-white hover:bg-brand-dark'}`
                }
              >
                {link.label}
              </NavLink>
            ))}
          </nav>
        )}
      </header>
      <main className="flex flex-1 items-center justify-center px-4 py-8">{children}</main>
    </div>
  )
}
