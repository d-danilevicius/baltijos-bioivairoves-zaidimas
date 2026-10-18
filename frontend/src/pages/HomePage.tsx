import { Link, Navigate } from 'react-router-dom'
import ErrorState from '../components/errorState'
import LoadingState from '../components/loadingState'
import PageLayout from '../components/pageLayout'
import { homeDescription, homeTitle, loginLink, registerLink } from '../helpers/homeMessages'
import { useCurrentUserSession } from '../hooks/useCurrentUserSession'

const startingScreen = '/profile'

export default function HomePage() {
  const { session, retry } = useCurrentUserSession()

  if (session.status === 'ready') {
    return <Navigate to={startingScreen} replace />
  }

  return (
    <PageLayout>
      {session.status === 'loading' && <LoadingState />}
      {session.status === 'error' && <ErrorState onRetry={retry} />}
      {session.status === 'unauthenticated' && (
        <section className="home">
          <h1>{homeTitle}</h1>
          <p>{homeDescription}</p>
          <p className="home-links">
            <Link className="button button-primary" to="/login">
              {loginLink}
            </Link>
            <Link className="button button-secondary" to="/register">
              {registerLink}
            </Link>
          </p>
        </section>
      )}
    </PageLayout>
  )
}
