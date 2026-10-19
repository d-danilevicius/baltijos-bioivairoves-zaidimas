import { getStartingScreen } from '../helpers/startingScreen'

describe('getStartingScreen', () => {
  it('returns the class list for a teacher', () => {
    expect(getStartingScreen('Teacher')).toBe('/classes')
  })

  it.each(['Student', 'Administrator'] as const)('returns the profile for the role %s', (role) => {
    expect(getStartingScreen(role)).toBe('/profile')
  })
})
