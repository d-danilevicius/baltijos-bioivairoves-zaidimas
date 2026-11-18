export type ButtonVariant = 'primary' | 'secondary'

const base =
  'inline-flex items-center justify-center rounded-full border-2 px-6 py-2.5 font-semibold no-underline shadow-sm focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-ink disabled:cursor-not-allowed disabled:opacity-60'

const variants: Record<ButtonVariant, string> = {
  primary: 'border-brand bg-brand text-white hover:border-brand-dark hover:bg-brand-dark',
  secondary: 'border-brand bg-white text-brand hover:bg-page',
}

export function buttonClass(variant: ButtonVariant, fullWidth = false): string {
  return `${base} ${variants[variant]}${fullWidth ? ' w-full' : ''}`
}
