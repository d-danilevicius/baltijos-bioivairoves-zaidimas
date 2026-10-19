import { useState } from 'react'
import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import ConfirmDialog from '../components/confirmDialog'

const question = 'Ištrinti klasę?'

function renderDialog(overrides: Partial<Parameters<typeof ConfirmDialog>[0]> = {}) {
  const onConfirm = vi.fn()
  const onCancel = vi.fn()
  render(<ConfirmDialog open message={question} onConfirm={onConfirm} onCancel={onCancel} {...overrides} />)
  return { onConfirm, onCancel }
}

function DialogWithTrigger() {
  const [open, setOpen] = useState(false)
  return (
    <>
      <button onClick={() => setOpen(true)}>Atidaryti</button>
      <ConfirmDialog open={open} message={question} onConfirm={() => setOpen(false)} onCancel={() => setOpen(false)} />
    </>
  )
}

describe('ConfirmDialog', () => {
  it('shows the question and the default button texts', () => {
    renderDialog()

    expect(screen.getByText(question)).toBeInTheDocument()
    expect(screen.getByRole('button', { name: 'Patvirtinti' })).toBeInTheDocument()
    expect(screen.getByRole('button', { name: 'Atšaukti' })).toBeInTheDocument()
  })

  it('shows the button texts set by the screen', () => {
    renderDialog({ confirmLabel: 'Pakeisti', cancelLabel: 'Likti' })

    expect(screen.getByRole('button', { name: 'Pakeisti' })).toBeInTheDocument()
    expect(screen.getByRole('button', { name: 'Likti' })).toBeInTheDocument()
  })

  it('calls onConfirm once and does not call onCancel when the confirm button is pressed', async () => {
    const { onConfirm, onCancel } = renderDialog()

    await userEvent.click(screen.getByRole('button', { name: 'Patvirtinti' }))

    expect(onConfirm).toHaveBeenCalledTimes(1)
    expect(onCancel).not.toHaveBeenCalled()
  })

  it('calls onCancel and does not call onConfirm when the cancel button is pressed', async () => {
    const { onConfirm, onCancel } = renderDialog()

    await userEvent.click(screen.getByRole('button', { name: 'Atšaukti' }))

    expect(onCancel).toHaveBeenCalledTimes(1)
    expect(onConfirm).not.toHaveBeenCalled()
  })

  it('calls onCancel and does not call onConfirm when the Esc key is pressed', async () => {
    const { onConfirm, onCancel } = renderDialog()

    await userEvent.keyboard('{Escape}')

    expect(onCancel).toHaveBeenCalledTimes(1)
    expect(onConfirm).not.toHaveBeenCalled()
  })

  it('keeps the focus inside the dialog when Shift+Tab is pressed on the first button', async () => {
    render(<DialogWithTrigger />)
    await userEvent.click(screen.getByRole('button', { name: 'Atidaryti' }))

    await userEvent.tab({ shift: true })

    expect(screen.getByRole('button', { name: 'Patvirtinti' })).toHaveFocus()
  })

  it('keeps the focus inside the dialog when Tab is pressed on the last button', async () => {
    render(<DialogWithTrigger />)
    await userEvent.click(screen.getByRole('button', { name: 'Atidaryti' }))
    await userEvent.tab()
    expect(screen.getByRole('button', { name: 'Patvirtinti' })).toHaveFocus()

    await userEvent.tab()

    expect(screen.getByRole('button', { name: 'Atšaukti' })).toHaveFocus()
  })

  it('shows nothing when the dialog is closed', () => {
    renderDialog({ open: false })

    expect(screen.queryByRole('dialog')).not.toBeInTheDocument()
    expect(screen.queryByText(question)).not.toBeInTheDocument()
  })

  it('has the dialog role and is labelled by the question', () => {
    renderDialog()

    const dialog = screen.getByRole('dialog', { name: question })
    expect(dialog).toHaveAttribute('aria-modal', 'true')
  })

  it('moves the focus to the cancel button when it opens and back to the previous element when it closes', async () => {
    render(<DialogWithTrigger />)
    const trigger = screen.getByRole('button', { name: 'Atidaryti' })

    await userEvent.click(trigger)
    expect(screen.getByRole('button', { name: 'Atšaukti' })).toHaveFocus()

    await userEvent.click(screen.getByRole('button', { name: 'Atšaukti' }))
    expect(screen.queryByRole('dialog')).not.toBeInTheDocument()
    expect(trigger).toHaveFocus()
  })
})
