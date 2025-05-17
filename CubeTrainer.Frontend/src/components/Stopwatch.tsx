import { useEffect, useRef, useState } from 'react'

enum StopwatchState {
  Idle = 'IDLE',
  Held = 'HELD',
  Running = 'RUNNING',
}

const MS_TO_HOLD_SPACE = 500

const formatTime = (ms: number) => {
  const seconds = Math.max(Math.floor(ms / 1000), 0)
  const millisecondsRemainder = Math.max(ms, 0) - seconds * 1000
  if (seconds < 60) {
    return `${seconds}.${millisecondsRemainder.toString().padStart(3, '0')}`
  }

  const minutes = Math.floor(seconds / 60)
  const secondsRemainder = seconds - minutes * 60
  return `${minutes}:${secondsRemainder.toString().padStart(2, '0')}.${millisecondsRemainder.toString().padStart(3, '0')}`
}

export interface StopwatchProps {
  onStart?: () => void
  onStop?: (msElapsed: number) => void
}

const Stopwatch = ({ onStart, onStop }: StopwatchProps) => {
  const [stopwatchState, setStopwatchState] = useState(StopwatchState.Idle)
  const [startTimeMs, setStartTimeMs] = useState(0)
  const [endTimeMs, setEndTimeMs] = useState(0)

  const stopwatchStateRef = useRef(StopwatchState.Idle)
  stopwatchStateRef.current = stopwatchState
  const startTimeMsRef = useRef(0)
  startTimeMsRef.current = startTimeMs
  const endTimeMsRef = useRef(0)
  endTimeMsRef.current = endTimeMs
  const keyDownTimeStampRef = useRef(0)

  const handleKeyDown = (e: KeyboardEvent) => {
    if (stopwatchStateRef.current === StopwatchState.Running) {
      setStopwatchState(StopwatchState.Idle)
      const now = Date.now()
      setEndTimeMs(now)
      onStop?.(Math.max(now - startTimeMsRef.current, 0))
      return
    }

    if (stopwatchStateRef.current === StopwatchState.Idle && e.key === ' ') {
      setStopwatchState(StopwatchState.Held)
      keyDownTimeStampRef.current = e.timeStamp
    }
  }
  const handleKeyUp = (e: KeyboardEvent) => {
    if (stopwatchStateRef.current === StopwatchState.Held) {
      if (e.timeStamp - keyDownTimeStampRef.current >= MS_TO_HOLD_SPACE) {
        setStopwatchState(StopwatchState.Running)
        setStartTimeMs(Date.now())
        onStart?.()
      } else {
        setStopwatchState(StopwatchState.Idle)
      }
    }
  }
  const handleTouchStart = (e: React.TouchEvent) => {
    if (stopwatchStateRef.current === StopwatchState.Running) {
      setStopwatchState(StopwatchState.Idle)
      const now = Date.now()
      setEndTimeMs(now)
      onStop?.(Math.max(now - startTimeMsRef.current, 0))
      return
    }

    if (stopwatchStateRef.current === StopwatchState.Idle) {
      setStopwatchState(StopwatchState.Held)
      keyDownTimeStampRef.current = e.timeStamp
    }
  }
  const handleTouchEnd = (e: React.TouchEvent) => {
    if (stopwatchStateRef.current === StopwatchState.Held) {
      if (e.timeStamp - keyDownTimeStampRef.current >= MS_TO_HOLD_SPACE) {
        setStopwatchState(StopwatchState.Running)
        setStartTimeMs(Date.now())
        onStart?.()
      } else {
        setStopwatchState(StopwatchState.Idle)
      }
    }
  }

  useEffect(() => {
    document.addEventListener('keydown', handleKeyDown)
    document.addEventListener('keyup', handleKeyUp)
    return () => {
      document.removeEventListener('keydown', handleKeyDown)
      document.removeEventListener('keyup', handleKeyUp)
    }
  }, [])

  useEffect(() => {
    const intervalId = setInterval(() => {
      if (stopwatchStateRef.current === StopwatchState.Running) {
        setEndTimeMs(Date.now())
      }
    }, 10)
    return () => clearInterval(intervalId)
  }, [])

  return (
    <div
      onTouchStart={handleTouchStart}
      onTouchEnd={handleTouchEnd}
      className="flex h-full w-full items-center justify-center"
    >
      <div>{formatTime(endTimeMs - startTimeMs)}</div>
    </div>
  )
}

export default Stopwatch
