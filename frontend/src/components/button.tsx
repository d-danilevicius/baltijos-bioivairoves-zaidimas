import type { ButtonHTMLAttributes } from 'react'
import { type ButtonVariant, buttonClass } from '../helpers/buttonStyles'

type ButtonProps = ButtonHTMLAttributes<HTMLButtonElement> & { variant?: ButtonVariant; fullWidth?: boolean }

export default function Button({ variant = 'primary', fullWidth = false, ...rest }: ButtonProps) {
  return <button className={buttonClass(variant, fullWidth)} {...rest} />
}
