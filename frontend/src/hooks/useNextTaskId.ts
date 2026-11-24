import { useEffect, useState } from 'react'
import { getMyTasks } from '../api/studentTaskApi'
import { HttpStatus } from '../helpers/httpStatus'

// The task to open after the result (E07): the same task as "Tęsti" in the list (FR-049).
// Null when there is no other task or the list could not be loaded.
export function useNextTaskId(): string | null {
  const [nextTaskId, setNextTaskId] = useState<string | null>(null)

  useEffect(() => {
    let cancelled = false
    getMyTasks().then((result) => {
      if (!cancelled && result.status === HttpStatus.Ok) {
        setNextTaskId(result.body.continueTaskId)
      }
    })
    return () => {
      cancelled = true
    }
  }, [])

  return nextTaskId
}
