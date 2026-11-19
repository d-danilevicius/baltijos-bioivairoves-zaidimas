import { getUserLinks, studentLinks, teacherLinks } from '../helpers/navigationLink'

describe('getUserLinks', () => {
  it('returns the class list and profile links for a teacher', () => {
    expect(getUserLinks('Teacher')).toEqual([
      { to: '/classes', label: 'Klasės' },
      { to: '/profile', label: 'Profilis' },
    ])
    expect(teacherLinks).toEqual(getUserLinks('Teacher'))
  })

  it('returns the task list, join class and profile links for a student', () => {
    expect(getUserLinks('Student')).toEqual([
      { to: '/tasks', label: 'Užduotys' },
      { to: '/join', label: 'Prisijungti prie klasės' },
      { to: '/profile', label: 'Profilis' },
    ])
    expect(studentLinks).toEqual(getUserLinks('Student'))
  })

  it('returns only the profile link for an administrator', () => {
    expect(getUserLinks('Administrator')).toEqual([{ to: '/profile', label: 'Profilis' }])
  })
})
