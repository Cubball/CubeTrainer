import { useEffect } from 'react'
import { useCountdownStore } from '../state/countdown'

export interface CountdownStripProps {
  durationMs: number
  onComplete: () => void
}

const CountdownStrip = ({ durationMs, onComplete }: CountdownStripProps) => {
  const isVisible = useCountdownStore((state) => state.isVisible)
  const isShrinking = useCountdownStore((state) => state.isShrinking)
  const startShrinking = useCountdownStore((state) => state.startShrinking)
  const stop = useCountdownStore((state) => state.stop)
  useEffect(() => {
    if (!isVisible) {
      return
    }

    const startTimeoutId = setTimeout(startShrinking, 10)
    const completeTimeoutId = setTimeout(() => {
      onComplete()
      stop()
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
