import { getUserLinks, teacherLinks } from '../helpers/navigationLinks'

describe('getUserLinks', () => {
  it('returns the class list and profile links for a teacher', () => {
    expect(getUserLinks('Teacher')).toEqual([
      { to: '/classes', label: 'Klasės' },
      { to: '/profile', label: 'Profilis' },
    ])
    expect(teacherLinks).toEqual(getUserLinks('Teacher'))
  })

  it('returns only the profile link for a student', () => {
    expect(getUserLinks('Student')).toEqual([{ to: '/profile', label: 'Profilis' }])
  })
})
