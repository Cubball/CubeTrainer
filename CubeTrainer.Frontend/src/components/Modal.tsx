import { ReactNode, useEffect, useRef } from 'react'

interface ModalProps {
  isOpen: boolean
  onClose: () => void
  title: string
  children: ReactNode
  actions?: ReactNode
  panelClassName?: string
  bodyClassName?: string
}

const Modal = ({
  isOpen,
  onClose,
  title,
  children,
  actions,
  panelClassName,
  bodyClassName,
}: ModalProps) => {
  const modalRef = useRef<HTMLDivElement>(null)

  useEffect(() => {
    const handleEscape = (e: KeyboardEvent) => {
      if (e.key === 'Escape' && isOpen) {
        onClose()
      }
    }

    const handleClickOutside = (e: MouseEvent) => {
      if (
        modalRef.current &&
        !modalRef.current.contains(e.target as Node) &&
        isOpen
      ) {
        onClose()
      }
    }

    document.addEventListener('keydown', handleEscape)
    document.addEventListener('mousedown', handleClickOutside)

    return () => {
      document.removeEventListener('keydown', handleEscape)
      document.removeEventListener('mousedown', handleClickOutside)
    }
  }, [isOpen, onClose])

  if (!isOpen) {
    return null
  }

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50">
      <div
        ref={modalRef}
        className={`w-full rounded-lg bg-white p-6 ${panelClassName ?? 'max-w-md'}`}
      >
        <div className="flex items-center justify-between">
          <h3 className="text-xl font-semibold">{title}</h3>
          <button onClick={onClose} className="cursor-pointer text-gray-500">
            ✕
          </button>
        </div>

        <div className={bodyClassName ?? 'py-4'}>{children}</div>

        {actions && <div className="flex justify-end gap-2">{actions}</div>}
      </div>
    </div>
  )
}

export default Modal
