import { Navigate, Outlet } from 'react-router-dom'
import ErrorState from './errorState'
import LoadingState from './loadingState'
import PageLayout from './pageLayout'
import { useCurrentUserSession } from '../hooks/useCurrentUserSession'
import AccessDeniedPage from '../pages/AccessDeniedPage'
import type { CurrentUser } from '../types/auth'

type ProtectedRouteProps = {
  allowedRoles?: CurrentUser['role'][]
}

export default function ProtectedRoute({ allowedRoles }: ProtectedRouteProps) {
  const { session, retry } = useCurrentUserSession()

  if (session.status === 'loading') {
    return (
      <PageLayout>
        <LoadingState />
      </PageLayout>
    )
  }

  if (session.status === 'unauthenticated') {
    return <Navigate to="/login" replace state={{ message: session.message }} />
  }

  if (session.status === 'error') {
    return (
      <PageLayout>
        <ErrorState onRetry={retry} />
      </PageLayout>
    )
  }

  if (allowedRoles && !allowedRoles.includes(session.user.role)) {
    return <AccessDeniedPage />
  }

  return <Outlet context={{ user: session.user }} />
}
