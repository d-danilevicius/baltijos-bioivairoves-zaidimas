type Props = {
  value?: 'Student' | 'Teacher'
  onChange: (v: 'Student' | 'Teacher') => void
  error?: string | undefined
}

export default function RoleChoice({ value, onChange, error }: Props) {
  return (
    <fieldset style={{ marginBottom: '1rem' }} aria-describedby={error ? 'role-error' : undefined}>
      <legend style={{ fontWeight: 600 }}>Rolė</legend>
      <label style={{ marginRight: '1rem' }}>
        <input
          type="radio"
          name="role"
          value="Student"
          checked={value === 'Student'}
          onChange={() => onChange('Student')}
        />{' '}
        Mokinys
      </label>
      <label>
        <input
          type="radio"
          name="role"
          value="Teacher"
          checked={value === 'Teacher'}
          onChange={() => onChange('Teacher')}
        />{' '}
        Mokytojas
      </label>
      {error && <div id="role-error" role="alert" className="error-text" style={{ marginTop: '0.25rem' }}>{error}</div>}
    </fieldset>
  )
}
