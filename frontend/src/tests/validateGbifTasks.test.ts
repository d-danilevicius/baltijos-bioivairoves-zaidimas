import {
  coordinatesNotNumbers,
  locationOutsideLithuania,
  locationRequired,
  seasonRequired,
} from '../helpers/gbifTaskMessage'
import { parseCoordinate, validateGbifTasks } from '../helpers/validateGbifTasks'

const valid = { latitude: '55.7', longitude: '21.1', season: 'Summer' }

describe('validateGbifTasks', () => {
  it('returns no errors for a point in Lithuania and a season', () => {
    expect(validateGbifTasks(valid)).toEqual({})
  })

  it('returns the location required message when the location is not chosen', () => {
    expect(validateGbifTasks({ ...valid, latitude: '', longitude: '  ' }).location).toBe(locationRequired)
  })

  it.each([
    ['abc', '21.1'],
    ['55.7', 'x'],
    ['55.7', ''],
    ['', '21.1'],
  ])('returns the not-numbers message for the coordinates "%s" and "%s"', (latitude, longitude) => {
    expect(validateGbifTasks({ ...valid, latitude, longitude }).location).toBe(coordinatesNotNumbers)
  })

  it.each([
    ['53.8', '24'],
    ['56.6', '24'],
    ['55', '20.8'],
    ['55', '27'],
  ])('returns the outside-Lithuania message for the coordinates "%s" and "%s"', (latitude, longitude) => {
    expect(validateGbifTasks({ ...valid, latitude, longitude }).location).toBe(locationOutsideLithuania)
  })

  it.each([
    ['53.9', '20.9'],
    ['56.5', '26.9'],
  ])('accepts the point on the border "%s", "%s"', (latitude, longitude) => {
    expect(validateGbifTasks({ ...valid, latitude, longitude }).location).toBeUndefined()
  })

  it('accepts a comma as the decimal separator', () => {
    expect(validateGbifTasks({ ...valid, latitude: '55,7', longitude: '21,1' }).location).toBeUndefined()
  })

  it('returns the season required message when the season is not chosen', () => {
    expect(validateGbifTasks({ ...valid, season: '' }).season).toBe(seasonRequired)
  })

  it('returns both messages when nothing is chosen', () => {
    expect(validateGbifTasks({ latitude: '', longitude: '', season: '' })).toEqual({
      location: locationRequired,
      season: seasonRequired,
    })
  })
})

describe('parseCoordinate', () => {
  it.each([
    ['55.7', 55.7],
    [' 55,7 ', 55.7],
  ])('converts "%s" to %s', (text, expected) => {
    expect(parseCoordinate(text)).toBe(expected)
  })

  it('returns NaN for an empty text', () => {
    expect(parseCoordinate('')).toBeNaN()
  })
})
