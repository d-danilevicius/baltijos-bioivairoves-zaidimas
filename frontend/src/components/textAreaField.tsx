type Props = {
  id: string
  label: string
  value: string
  onChange: (v: string) => void
  error?: string | undefined
  hint?: string | undefined
}

export default function TextAreaField({ id, label, value, onChange, error, hint }: Props) {
  const describedBy =
    [hint ? `${id}-hint` : undefined, error ? `${id}-error` : undefined].filter(Boolean).join(' ') || undefined
  return (
    <div className="mb-4">
      <label htmlFor={id} className="mb-1 block font-semibold">
        {label}
      </label>
      <textarea
        id={id}
        rows={4}
        value={value}
        aria-invalid={!!error}
        aria-describedby={describedBy}
        onChange={(e) => onChange(e.target.value)}
        className="block w-full rounded-md border border-gray-500 bg-white px-3 py-2 text-base focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-brand aria-invalid:border-error"
      />
      {hint && (
        <p id={`${id}-hint`} className="mt-1 text-right text-sm">
          {hint}
        </p>
      )}
      {error && (
        <div id={`${id}-error`} role="alert" className="mt-1 text-error">
          {error}
        </div>
      )}
    </div>
  )
}
