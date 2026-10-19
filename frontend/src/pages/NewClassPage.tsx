import { FormEvent, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { createClass } from '../api/classesApi'
import Button from '../components/button'
import Card from '../components/card'
import PageLayout from '../components/pageLayout'
import TextField from '../components/textField'
import { classCreated, classNameRequired, classNameTooLong } from '../helpers/classMessages'
import { teacherLinks } from '../helpers/navigationLinks'
import { serverError } from '../helpers/stateMessages'
import { validateClassName } from '../helpers/validateClassName'

export default function NewClassPage() {
  const [name, setName] = useState('')
  const [nameError, setNameError] = useState<string | undefined>(undefined)
  const [topError, setTopError] = useState<string | undefined>(undefined)
  const [loading, setLoading] = useState(false)
  const navigate = useNavigate()

  async function onSubmit(e: FormEvent) {
    e.preventDefault()
    setTopError(undefined)

    const clientError = validateClassName(name)
    setNameError(clientError)
    if (clientError) return

    setLoading(true)
    const res = await createClass(name.trim())
    setLoading(false)

    if (res.status === 201) {
      navigate(`/classes/${res.body.id}`, { state: { message: classCreated } })
      return
    }

    if (res.status === 400) {
      const codes = res.body.errors?.name ?? []
      if (codes.includes('Required')) setNameError(classNameRequired)
      else if (codes.includes('TooLong')) setNameError(classNameTooLong)
      else setTopError(serverError)
      return
    }

    setTopError(serverError)
  }

  return (
    <PageLayout links={teacherLinks}>
      <Card title="Nauja klasė">
        <form onSubmit={onSubmit}>
          {topError && (
            <div role="alert" className="mb-4 text-error">
              {topError}
            </div>
          )}
          <TextField id="name" label="Klasės pavadinimas" value={name} onChange={setName} error={nameError} />
          <Button type="submit" disabled={loading} fullWidth>
            Sukurti
          </Button>
        </form>
      </Card>
    </PageLayout>
  )
}
