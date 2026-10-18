const sessionKey = 'sessionStarted'

export function markSessionStarted(): void {
  try {
    sessionStorage.setItem(sessionKey, '1')
  } catch {
    // storage unavailable: the session marker is optional
  }
}

export function clearSessionMarker(): void {
  try {
    sessionStorage.removeItem(sessionKey)
  } catch {
    // storage unavailable: nothing to clear
  }
}

export function hadSession(): boolean {
  try {
    return sessionStorage.getItem(sessionKey) === '1'
  } catch {
    return false
  }
}
