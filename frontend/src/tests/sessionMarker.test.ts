import { hadSession, markSessionStarted, clearSessionMarker } from '../helpers/sessionMarker'

describe('sessionMarker', () => {
  afterEach(() => {
    vi.restoreAllMocks()
    sessionStorage.clear()
  })

  it('hadSession returns false before a session was started', () => {
    expect(hadSession()).toBe(false)
  })

  it('hadSession returns true after markSessionStarted', () => {
    markSessionStarted()

    expect(hadSession()).toBe(true)
  })

  it('hadSession returns false after clearSessionMarker', () => {
    markSessionStarted()

    clearSessionMarker()

    expect(hadSession()).toBe(false)
  })

  it('works without errors when sessionStorage is unavailable', () => {
    vi.spyOn(Storage.prototype, 'setItem').mockImplementation(() => {
      throw new Error('unavailable')
    })
    vi.spyOn(Storage.prototype, 'getItem').mockImplementation(() => {
      throw new Error('unavailable')
    })
    vi.spyOn(Storage.prototype, 'removeItem').mockImplementation(() => {
      throw new Error('unavailable')
    })

    expect(() => markSessionStarted()).not.toThrow()
    expect(hadSession()).toBe(false)
    expect(() => clearSessionMarker()).not.toThrow()
  })
})
