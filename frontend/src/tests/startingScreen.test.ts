import { getStartingScreen } from '../helpers/startingScreen'

describe('getStartingScreen', () => {
  it('returns the class list for a teacher', () => {
    expect(getStartingScreen('Teacher')).toBe('/classes')
  })

  it('returns the task list for a student', () => {
    expect(getStartingScreen('Student')).toBe('/tasks')
  })

  it('returns the profile for an administrator', () => {
    expect(getStartingScreen('Administrator')).toBe('/profile')
  })
})
