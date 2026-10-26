import { useEffect, useState } from 'react'
import { Link, useLocation } from 'react-router-dom'
import { listClasses } from '../api/classesApi'
import Card from '../components/card'
import EmptyState from '../components/emptyState'
import ErrorState from '../components/errorState'
import LinkButton from '../components/linkButton'
import LoadingState from '../components/loadingState'
import PageLayout from '../components/pageLayout'
import SessionExpiredRedirect from '../components/sessionExpiredRedirect'
import { createFirstClass, newClass, noClasses } from '../helpers/classMessages'
import { teacherLinks } from '../helpers/navigationLinks'
import type { SchoolClass } from '../types/classes'

type ListState =
  { status: 'loading' } | { status: 'ready'; classes: SchoolClass[] } | { status: 'expired' } | { status: 'error' }

export default function ClassListPage() {
  const [state, setState] = useState<ListState>({ status: 'loading' })
  const [attempt, setAttempt] = useState(0)
  const message = (useLocation().state as { message?: string } | null)?.message

  useEffect(() => {
    let cancelled = false

    async function load() {
      const res = await listClasses()
      if (cancelled) return
      if (res.status === 200) setState({ status: 'ready', classes: res.body })
      else if (res.status === 401) setState({ status: 'expired' })
      else setState({ status: 'error' })
    }

    load()
    return () => {
      cancelled = true
    }
  }, [attempt])

  function retry() {
    setState({ status: 'loading' })
    setAttempt((value) => value + 1)
  }

  if (state.status === 'expired') return <SessionExpiredRedirect />

  return (
    <PageLayout links={teacherLinks}>
      {state.status === 'loading' && <LoadingState />}
      {state.status === 'error' && <ErrorState onRetry={retry} />}
      {state.status === 'ready' && (
        <Card title="Klasės" size="wide">
          {message && (
            <div role="status" className="mb-4 text-brand">
              {message}
            </div>
          )}
          {state.classes.length === 0 ? (
            <EmptyState message={noClasses}>
              <div className="mt-4">
                <LinkButton to="/classes/new">{createFirstClass}</LinkButton>
              </div>
            </EmptyState>
          ) : (
            <>
              <ul className="mb-6 divide-y divide-gray-200">
                {state.classes.map((schoolClass) => (
                  <li key={schoolClass.id} className="flex flex-wrap items-center justify-between gap-2 py-3">
                    <Link to={`/classes/${schoolClass.id}`} className="break-all font-semibold text-brand underline">
                      {schoolClass.name}
                    </Link>
                    <span>Kodas: {schoolClass.code}</span>
                  </li>
                ))}
              </ul>
              <LinkButton to="/classes/new">{newClass}</LinkButton>
            </>
          )}
        </Card>
      )}
    </PageLayout>
  )
}
