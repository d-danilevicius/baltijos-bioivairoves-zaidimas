import { useLocation } from 'react-router-dom'

export default function LoginPage() {
  const loc = useLocation()
  const state = (loc.state as unknown as { message?: string }) ?? {}
  return (
    <div style={{ maxWidth: 480, margin: '0 auto' }}>
      <h1>Prisijungimas</h1>
      {state.message && <div role="status" style={{ marginTop: '1rem', color: 'green' }}>{state.message}</div>}
    </div>
  )
}
