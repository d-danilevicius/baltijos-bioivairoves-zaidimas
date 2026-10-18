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
    <div style={{ marginBottom: '1rem' }}>
      <label htmlFor={id} style={{ display: 'block', fontWeight: 600 }}>
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
        style={{ display: 'block', width: '100%', padding: '0.5rem', fontSize: '1rem' }}
      />
      {error && (
        <div id={`${id}-error`} role="alert" className="error-text" style={{ marginTop: '0.25rem' }}>
          {error}
        </div>
      )}
    </div>
  )
}
