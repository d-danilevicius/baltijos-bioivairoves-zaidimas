import { useEffect, useId, useRef } from 'react'
import Button from './button'

type ConfirmDialogProps = {
  open: boolean
  message: string
  confirmLabel?: string
  cancelLabel?: string
  onConfirm: () => void
  onCancel: () => void
}

export default function ConfirmDialog({
  open,
  message,
  confirmLabel = 'Patvirtinti',
  cancelLabel = 'Atšaukti',
  onConfirm,
  onCancel,
}: ConfirmDialogProps) {
  const messageId = useId()
  const cancelButtonRef = useRef<HTMLButtonElement>(null)
  const onCancelRef = useRef(onCancel)
  onCancelRef.current = onCancel

  useEffect(() => {
    if (!open) return

    const previousElement = document.activeElement as HTMLElement | null
    cancelButtonRef.current?.focus()

    function cancelOnEscape(event: KeyboardEvent) {
      if (event.key === 'Escape') onCancelRef.current()
    }

    document.addEventListener('keydown', cancelOnEscape)
    return () => {
      document.removeEventListener('keydown', cancelOnEscape)
      previousElement?.focus()
    }
  }, [open])

  if (!open) return null

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-ink/60 px-4">
      <div
        role="dialog"
        aria-modal="true"
        aria-labelledby={messageId}
        className="w-full max-w-md rounded-2xl bg-white p-8 shadow-md"
      >
        <p id={messageId} className="mb-6 text-center text-lg font-semibold break-words">
          {message}
        </p>
        <div className="flex flex-wrap justify-center gap-3">
          <Button type="button" variant="secondary" ref={cancelButtonRef} onClick={onCancel}>
            {cancelLabel}
          </Button>
          <Button type="button" onClick={onConfirm}>
            {confirmLabel}
          </Button>
        </div>
      </div>
    </div>
  )
}
