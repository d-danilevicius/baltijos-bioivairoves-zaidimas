import { useBlocker } from 'react-router-dom'

// Blocks leaving the screen while there is unsent data (E21); needs a data router
export function useLeaveConfirmation(hasUnsentData: boolean) {
  const blocker = useBlocker(hasUnsentData)

  return {
    open: blocker.state === 'blocked',
    leave: () => blocker.proceed?.(),
    stay: () => blocker.reset?.(),
  }
}
