import { answerChoiceTitle } from '../helpers/submissionMessage'

type Props = {
  options: string[]
  value: string
  onChange: (value: string) => void
  error?: string | undefined
}

export default function OptionChoice({ options, value, onChange, error }: Props) {
  return (
    <fieldset className="mb-4" aria-describedby={error ? 'answer-error' : undefined}>
      <legend className="mb-1 font-semibold">{answerChoiceTitle}</legend>
      <div className="flex flex-col gap-2">
        {options.map((option) => (
          <label key={option} className="flex items-center gap-2">
            <input
              type="radio"
              name="answer"
              value={option}
              checked={value === option}
              onChange={() => onChange(option)}
            />
            {option}
          </label>
        ))}
      </div>
      {error && (
        <div id="answer-error" role="alert" className="mt-1 text-error">
          {error}
        </div>
      )}
    </fieldset>
  )
}
