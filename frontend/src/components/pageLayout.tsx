import type { ReactNode } from 'react'

type PageLayoutProps = { children: ReactNode }

export default function PageLayout({ children }: PageLayoutProps) {
  return (
    <>
      <header>
        <strong>Baltijos biologinės įvairovės iššūkių žaidimas</strong>
      </header>
      <main>{children}</main>
    </>
  )
}
