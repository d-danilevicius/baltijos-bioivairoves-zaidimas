import { Navigate } from 'react-router-dom'
import Card from '../components/card'
import ErrorState from '../components/errorState'
import LinkButton from '../components/linkButton'
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
        <Card title={homeTitle} size="wide">
          <p className="mb-6 text-center">{homeDescription}</p>
          <div className="flex flex-wrap justify-center gap-3">
            <LinkButton to="/login">{loginLink}</LinkButton>
            <LinkButton to="/register" variant="secondary">
              {registerLink}
            </LinkButton>
          </div>
        </Card>
      )}
    </PageLayout>
  )
}
