type Props = {
  value?: 'Student' | 'Teacher'
  onChange: (v: 'Student' | 'Teacher') => void
  error?: string | undefined
}

export default function RoleChoice({ value, onChange, error }: Props) {
  return (
    <fieldset className="mb-4" aria-describedby={error ? 'role-error' : undefined}>
      <legend className="font-semibold">Rolė</legend>
      <label className="mr-4">
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
      {error && (
        <div id="role-error" role="alert" className="mt-1 text-error">
          {error}
        </div>
      )}
    </fieldset>
  )
}
