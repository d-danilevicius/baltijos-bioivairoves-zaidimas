export type Failure = { status: 'network' | 'server'; message: string }

export function toFailure(e: unknown): Failure {
  const message = (e as Error | undefined)?.message ?? 'Network error'
  return { status: message === 'Server error' ? 'server' : 'network', message }
}
