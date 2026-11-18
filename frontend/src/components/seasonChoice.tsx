import { seasonLabels, seasonOrder, seasonTitle } from '../helpers/gbifTaskMessage'

type Props = {
  value: string
  onChange: (value: string) => void
  error?: string | undefined
}

export default function SeasonChoice({ value, onChange, error }: Props) {
  return (
    <fieldset className="mb-4" aria-describedby={error ? 'season-error' : undefined}>
      <legend className="mb-1 font-semibold">{seasonTitle}</legend>
      <div className="flex flex-wrap gap-4">
        {seasonOrder.map((season) => (
          <label key={season} className="flex items-center gap-2">
            <input
              type="radio"
              name="season"
              value={season}
              checked={value === season}
              onChange={() => onChange(season)}
            />
            {seasonLabels[season]}
          </label>
        ))}
      </div>
      {error && (
        <div id="season-error" role="alert" className="mt-1 text-error">
          {error}
        </div>
      )}
    </fieldset>
  )
}
