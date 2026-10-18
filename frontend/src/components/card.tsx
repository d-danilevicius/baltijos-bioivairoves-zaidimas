import type { ReactNode } from 'react'

type CardProps = { title: string; size?: 'narrow' | 'wide'; children: ReactNode }

const sizes = { narrow: 'max-w-md', wide: 'max-w-xl' }

export default function Card({ title, size = 'narrow', children }: CardProps) {
  return (
    <section className={`mx-auto w-full rounded-2xl bg-white p-8 shadow-md ${sizes[size]}`}>
      <h1 className="mb-4 text-center text-2xl font-bold">{title}</h1>
      {children}
    </section>
  )
}
