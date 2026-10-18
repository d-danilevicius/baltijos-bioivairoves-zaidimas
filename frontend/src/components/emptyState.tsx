import type { ReactNode } from 'react'

type Props = {
  message: string
  children?: ReactNode
}

export default function EmptyState({ message, children }: Props) {
  return (
    <div>
      <div role="status">{message}</div>
      {children}
    </div>
  )
}
