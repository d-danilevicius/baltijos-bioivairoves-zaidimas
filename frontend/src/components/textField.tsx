type Props = {
  id: string
  label: string
  type?: string
  value: string
  onChange: (v: string) => void
  error?: string | undefined
  autoComplete?: string
}

export default function TextField({ id, label, type = 'text', value, onChange, error, autoComplete }: Props) {
  const describedBy = error ? `${id}-error` : undefined
  return (
    <div className="mb-4">
      <label htmlFor={id} className="mb-1 block font-semibold">
        {label}
      </label>
      <input
        id={id}
        type={type}
        value={value}
        autoComplete={autoComplete}
        aria-invalid={!!error}
        aria-describedby={describedBy}
        onChange={(e) => onChange(e.target.value)}
        className="block w-full rounded-md border border-gray-500 bg-white px-3 py-2 text-base focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-brand aria-invalid:border-error"
      />
      {error && (
        <div id={`${id}-error`} role="alert" className="mt-1 text-error">
          {error}
        </div>
      )}
    </div>
  )
}
