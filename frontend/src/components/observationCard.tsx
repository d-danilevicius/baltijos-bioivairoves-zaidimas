import {
  coordinatesCaption,
  datasetCaption,
  gbifRecordCaption,
  hiddenCoordinatesNote,
  licenceCaption,
  placeCaption,
} from '../helpers/taskScreenMessage'
import type { ObservationCard as ObservationCardData } from '../types/studentTaskDetails'

type Props = {
  card: ObservationCardData
}

type RowProps = {
  caption: string
  value: string | null
}

function CardRow({ caption, value }: RowProps) {
  if (!value) return null

  return (
    <div className="flex flex-wrap gap-x-2">
      <dt className="font-semibold">{caption}</dt>
      <dd>{value}</dd>
    </div>
  )
}

export default function ObservationCard({ card }: Props) {
  const coordinates =
    !card.coordinatesHidden && card.latitude !== null && card.longitude !== null
      ? `${card.latitude}, ${card.longitude}`
      : null

  return (
    <section aria-labelledby="observation-card-title" className="mb-4 rounded-lg border border-brand p-4">
      <h2 id="observation-card-title" className="mb-2 text-lg font-semibold">
        {card.label}
      </h2>
      <dl>
        <CardRow caption={datasetCaption} value={card.datasetName} />
        <CardRow caption={licenceCaption} value={card.license} />
        <CardRow caption={placeCaption} value={card.locality} />
        <CardRow caption={coordinatesCaption} value={coordinates} />
      </dl>
      {card.coordinatesHidden && <p className="mt-1">{hiddenCoordinatesNote}</p>}
      <p className="mt-2">
        <a href={card.gbifUrl} target="_blank" rel="noopener noreferrer" className="text-brand underline">
          {gbifRecordCaption}
        </a>
      </p>
    </section>
  )
}
