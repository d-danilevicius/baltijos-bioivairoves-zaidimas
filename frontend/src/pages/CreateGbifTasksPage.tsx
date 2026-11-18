import { FormEvent, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { createGbifTasks, type CreateGbifTasksResult } from '../api/classTaskApi'
import Button from '../components/button'
import Card from '../components/card'
import LocationMap from '../components/locationMap'
import PageLayout from '../components/pageLayout'
import SeasonChoice from '../components/seasonChoice'
import SessionExpiredRedirect from '../components/sessionExpiredRedirect'
import TextField from '../components/textField'
import {
  createGbifTasksRetry,
  createGbifTasksSubmit,
  createGbifTasksTitle,
  gbifFailed,
  gbifLoading,
  gbifTasksCreated,
  latitudeLabel,
  locationOutsideLithuania,
  locationRequired,
  longitudeLabel,
  mapHint,
  radiusNote,
  seasonRequired,
  tooFewObservations,
} from '../helpers/gbifTaskMessage'
import { HttpStatus } from '../helpers/httpStatus'
import { teacherLinks } from '../helpers/navigationLink'
import { serverError } from '../helpers/stateMessage'
import { parseCoordinate, validateGbifTasks, type GbifTasksFormErrors } from '../helpers/validateGbifTasks'
import type { Season } from '../types/classTask'
import AccessDeniedPage from './AccessDeniedPage'
import NotFoundPage from './NotFoundPage'

type Blocked = 'expired' | 'forbidden' | 'notFound'

function getServerErrors(errors: Record<string, string[]> | undefined): GbifTasksFormErrors {
  const result: GbifTasksFormErrors = {}
  const location = [...(errors?.latitude ?? []), ...(errors?.longitude ?? [])]
  if (location.includes('OutsideLithuania')) result.location = locationOutsideLithuania
  else if (location.length > 0) result.location = locationRequired
  if (errors?.season) result.season = seasonRequired
  return result
}

export default function CreateGbifTasksPage() {
  const { id = '' } = useParams()
  const navigate = useNavigate()
  const [latitude, setLatitude] = useState('')
  const [longitude, setLongitude] = useState('')
  const [season, setSeason] = useState('')
  const [errors, setErrors] = useState<GbifTasksFormErrors>({})
  const [topError, setTopError] = useState<string | undefined>(undefined)
  const [canRetry, setCanRetry] = useState(false)
  const [sending, setSending] = useState(false)
  const [blocked, setBlocked] = useState<Blocked | undefined>(undefined)

  function goToTasks() {
    navigate(`/classes/${id}/tasks`, { state: { message: gbifTasksCreated } })
  }

  function showResult(res: CreateGbifTasksResult) {
    if (res.status === HttpStatus.Created) goToTasks()
    else if (res.status === HttpStatus.Conflict) navigate(`/classes/${id}/tasks`)
    else if (res.status === HttpStatus.Unauthorized) setBlocked('expired')
    else if (res.status === HttpStatus.Forbidden) setBlocked('forbidden')
    else if (res.status === HttpStatus.NotFound) setBlocked('notFound')
    else if (res.status === HttpStatus.UnprocessableEntity) setTopError(tooFewObservations)
    else if (res.status === HttpStatus.BadGateway) {
      setTopError(gbifFailed)
      setCanRetry(true)
    } else if (res.status === HttpStatus.BadRequest) {
      const serverErrors = getServerErrors(res.body.errors)
      setErrors(serverErrors)
      if (Object.keys(serverErrors).length === 0) setTopError(serverError)
    } else setTopError(serverError)
  }

  async function submit() {
    setTopError(undefined)
    setCanRetry(false)

    const clientErrors = validateGbifTasks({ latitude, longitude, season })
    setErrors(clientErrors)
    if (Object.keys(clientErrors).length > 0) return

    setSending(true)
    const res = await createGbifTasks(id, {
      latitude: parseCoordinate(latitude),
      longitude: parseCoordinate(longitude),
      season: season as Season,
    })
    setSending(false)
    showResult(res)
  }

  function onSubmit(e: FormEvent) {
    e.preventDefault()
    void submit()
  }

  function onMapSelect(selectedLatitude: number, selectedLongitude: number) {
    setLatitude(selectedLatitude.toFixed(5))
    setLongitude(selectedLongitude.toFixed(5))
  }

  if (blocked === 'expired') return <SessionExpiredRedirect />
  if (blocked === 'forbidden') return <AccessDeniedPage />
  if (blocked === 'notFound') return <NotFoundPage />

  const mapLatitude = parseCoordinate(latitude)
  const mapLongitude = parseCoordinate(longitude)

  return (
    <PageLayout links={teacherLinks}>
      <Card title={createGbifTasksTitle} size="wide">
        <form onSubmit={onSubmit} noValidate>
          {topError && (
            <div role="alert" className="mb-4 text-error">
              {topError}
            </div>
          )}
          <p className="mb-2">{mapHint}</p>
          <LocationMap
            latitude={Number.isNaN(mapLatitude) ? undefined : mapLatitude}
            longitude={Number.isNaN(mapLongitude) ? undefined : mapLongitude}
            onSelect={onMapSelect}
          />
          <div className="grid gap-x-4 sm:grid-cols-2">
            <TextField id="latitude" label={latitudeLabel} value={latitude} onChange={setLatitude} />
            <TextField id="longitude" label={longitudeLabel} value={longitude} onChange={setLongitude} />
          </div>
          {errors.location && (
            <div role="alert" className="mb-4 text-error">
              {errors.location}
            </div>
          )}
          <SeasonChoice value={season} onChange={setSeason} error={errors.season} />
          <p className="mb-4 text-sm">{radiusNote}</p>
          {sending && (
            <div role="status" className="mb-4">
              {gbifLoading}
            </div>
          )}
          <Button type="submit" disabled={sending} fullWidth>
            {createGbifTasksSubmit}
          </Button>
          {canRetry && (
            <div className="mt-3">
              <Button type="button" variant="secondary" disabled={sending} onClick={() => void submit()} fullWidth>
                {createGbifTasksRetry}
              </Button>
            </div>
          )}
        </form>
      </Card>
    </PageLayout>
  )
}
