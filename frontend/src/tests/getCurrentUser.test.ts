import { getCurrentUser } from '../api/authApi'

describe('getCurrentUser', () => {
  afterEach(() => {
    vi.restoreAllMocks()
  })

  it('returns the user when the server responds with 200', async () => {
    const user = { id: 'u1', userName: 'jonas', role: 'Teacher' }
    vi.spyOn(globalThis, 'fetch').mockResolvedValue(new Response(JSON.stringify(user), { status: 200 }))

    const result = await getCurrentUser()

    expect(result).toEqual({ status: 200, body: user })
  })

  it('returns status 401 when the server responds with 401', async () => {
    vi.spyOn(globalThis, 'fetch').mockResolvedValue(new Response(null, { status: 401 }))

    const result = await getCurrentUser()

    expect(result).toEqual({ status: 401 })
  })

  it('returns status network when the request fails', async () => {
    vi.spyOn(globalThis, 'fetch').mockRejectedValue(new TypeError('Failed to fetch'))

    const result = await getCurrentUser()

    expect(result.status).toBe('network')
  })

  it('returns status server when the server responds with another status', async () => {
    vi.spyOn(globalThis, 'fetch').mockResolvedValue(new Response(null, { status: 500 }))

    const result = await getCurrentUser()

    expect(result.status).toBe('server')
  })

  it('calls /api/auth/me with credentials included', async () => {
    const fetchSpy = vi.spyOn(globalThis, 'fetch').mockResolvedValue(new Response(null, { status: 401 }))

    await getCurrentUser()

    expect(fetchSpy).toHaveBeenCalledWith(
      expect.stringContaining('/api/auth/me'),
      expect.objectContaining({ credentials: 'include' })
    )
  })
})
