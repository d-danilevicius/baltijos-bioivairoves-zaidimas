import Card from './card'
import LinkButton from './linkButton'
import PageLayout from './pageLayout'

type MessagePageProps = {
  title: string
  actionLabel: string
}

export default function MessagePage({ title, actionLabel }: MessagePageProps) {
  return (
    <PageLayout>
      <Card title={title}>
        <LinkButton to="/">{actionLabel}</LinkButton>
      </Card>
    </PageLayout>
  )
}
