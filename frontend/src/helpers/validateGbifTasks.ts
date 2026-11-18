import { lithuaniaBounds } from './lithuaniaBounds'
import { coordinatesNotNumbers, locationOutsideLithuania, locationRequired, seasonRequired } from './gbifTaskMessage'

export interface GbifTasksForm {
  latitude: string
  longitude: string
  season: string
}

export interface GbifTasksFormErrors {
  location?: string
  season?: string
}

// A comma is accepted as the decimal separator
export function parseCoordinate(value: string): number {
  const text = value.trim().replace(',', '.')
  return text === '' ? Number.NaN : Number(text)
}

function validateLocation(latitudeText: string, longitudeText: string): string | undefined {
  if (latitudeText.trim() === '' && longitudeText.trim() === '') return locationRequired

  const latitude = parseCoordinate(latitudeText)
  const longitude = parseCoordinate(longitudeText)
  if (Number.isNaN(latitude) || Number.isNaN(longitude)) return coordinatesNotNumbers

  const insideLatitude = latitude >= lithuaniaBounds.minLatitude && latitude <= lithuaniaBounds.maxLatitude
  const insideLongitude = longitude >= lithuaniaBounds.minLongitude && longitude <= lithuaniaBounds.maxLongitude
  return insideLatitude && insideLongitude ? undefined : locationOutsideLithuania
}

export function validateGbifTasks(form: GbifTasksForm): GbifTasksFormErrors {
  const errors: GbifTasksFormErrors = {}

  const location = validateLocation(form.latitude, form.longitude)
  if (location) errors.location = location
  if (form.season === '') errors.season = seasonRequired

  return errors
}
