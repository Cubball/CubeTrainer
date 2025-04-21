import { useEffect } from 'react'
import { useCountdownStore } from '../state/countdown'

export interface CountdownStripProps {
  duration: number
  onComplete: () => void
}

const CountdownStrip = ({ duration, onComplete }: CountdownStripProps) => {
  const isVisible = useCountdownStore((state) => state.isVisible)
  const isShrinking = useCountdownStore((state) => state.isShrinking)
  const start = useCountdownStore((state) => state.start)
  const hide = useCountdownStore((state) => state.hide)
  useEffect(() => {
    if (!isVisible) {
      return
    }

    const shrinkTimeoutId = setTimeout(start, 10)
    const completeTimeoutId = setTimeout(() => {
      onComplete()
      hide()
    }, duration)
    return () => {
      clearTimeout(shrinkTimeoutId)
      clearTimeout(completeTimeoutId)
    }
  }, [isVisible, duration, onComplete])

  if (!isVisible) {
    return null
  }

  // TODO: refactor styles
  return (
    <div className="h-4 w-full overflow-hidden">
      <div
        className={`mx-auto h-full bg-gray-800 transition-all ease-linear`}
        style={{
          width: isShrinking ? '0%' : '100%',
          transitionDuration: `${duration}ms`,
        }}
      />
    </div>
  )
}

export default CountdownStrip
