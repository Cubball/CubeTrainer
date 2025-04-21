import { useEffect } from 'react'
import { useCountdownStore } from '../state/countdown'

export interface CountdownStripProps {
  durationMs: number
  onComplete: () => void
}

const CountdownStrip = ({ durationMs, onComplete }: CountdownStripProps) => {
  const isVisible = useCountdownStore((state) => state.isVisible)
  const isShrinking = useCountdownStore((state) => state.isShrinking)
  const start = useCountdownStore((state) => state.start)
  const hide = useCountdownStore((state) => state.hide)
  useEffect(() => {
    if (!isVisible) {
      return
    }

    const startTimeoutId = setTimeout(start, 10)
    const completeTimeoutId = setTimeout(() => {
      onComplete()
      hide()
    }, durationMs)
    return () => {
      clearTimeout(startTimeoutId)
      clearTimeout(completeTimeoutId)
    }
  }, [isVisible, durationMs, onComplete])

  if (!isVisible) {
    return null
  }

  return (
    <div className="flex h-4 w-full justify-center">
      <div
        className={
          'h-full bg-gray-800 transition-all ease-linear ' +
          (isShrinking ? 'w-0' : 'w-full')
        }
        style={{
          transitionDuration: `${durationMs}ms`,
        }}
      />
    </div>
  )
}

export default CountdownStrip
