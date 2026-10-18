import { Link } from 'react-router-dom'
import PageLayout from './pageLayout'

type MessagePageProps = {
  title: string
  actionLabel: string
}

export default function MessagePage({ title, actionLabel }: MessagePageProps) {
  return (
    <PageLayout>
      <h1>{title}</h1>
      <p>
        <Link to="/">{actionLabel}</Link>
      </p>
    </PageLayout>
  )
}
