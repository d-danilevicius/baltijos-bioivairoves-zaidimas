import { groupLabel, groupTitle } from '../helpers/teacherTaskMessage'

const groups = [1, 2, 3]

type Props = {
  value: string
  onChange: (value: string) => void
  error?: string | undefined
}

export default function DifficultyChoice({ value, onChange, error }: Props) {
  return (
    <fieldset className="mb-4" aria-describedby={error ? 'difficulty-error' : undefined}>
      <legend className="mb-1 font-semibold">{groupTitle}</legend>
      <div className="flex flex-wrap gap-4">
        {groups.map((group) => (
          <label key={group} className="flex items-center gap-2">
            <input
              type="radio"
              name="difficultyGroup"
              value={group}
              checked={value === String(group)}
              onChange={() => onChange(String(group))}
            />
            {groupLabel(group)}
          </label>
        ))}
      </div>
      {error && (
        <div id="difficulty-error" role="alert" className="mt-1 text-error">
          {error}
        </div>
      )}
    </fieldset>
  )
}
