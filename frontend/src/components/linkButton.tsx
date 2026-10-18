import type { ReactNode } from 'react'
import { Link } from 'react-router-dom'
import { type ButtonVariant, buttonClass } from '../helpers/buttonStyles'

type LinkButtonProps = { to: string; variant?: ButtonVariant; children: ReactNode }

export default function LinkButton({ to, variant = 'primary', children }: LinkButtonProps) {
  return (
    <Link className={buttonClass(variant)} to={to}>
      {children}
    </Link>
  )
}
