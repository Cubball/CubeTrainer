import { useEffect, useRef, useState } from 'react'

enum StopwatchState {
  Idle = 'IDLE',
  Held = 'HELD',
  Running = 'RUNNING',
}

const MS_TO_HOLD_SPACE = 1000

const formatTime = (ms: number) => {
  const seconds = Math.max(Math.floor(ms / 1000), 0)
  const hundredths = Math.max(Math.floor(ms / 10), 0) - seconds * 100
  if (seconds < 60) {
    return `${seconds}.${hundredths.toString().padStart(2, '0')}`
  }

  const minutes = Math.floor(seconds / 60)
  const secondsRemainder = seconds - minutes * 60
  return `${minutes}:${secondsRemainder.toString().padStart(2, '0')}.${hundredths.toString().padStart(2, '0')}`
}

export interface StopwatchProps {
  onStop: (msElapsed: number) => void
}

const Stopwatch = ({ onStop }: StopwatchProps) => {
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
      onStop(Math.max(endTimeMsRef.current - startTimeMsRef.current, 0))
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

  return <div>{formatTime(endTimeMs - startTimeMs)}</div>
}

export default Stopwatch
