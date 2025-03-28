import { useQuery } from '@tanstack/react-query'
import { useAxiosWithAuth } from '../../../lib/axios'
import ScrambleView from '../components/ScrambleView'
import { useEffect, useRef, useState } from 'react'

const RANDOM_SCRAMBLE_QUERY_KEY = 'random-scramble'

enum TimerState {
  Idle = 'IDLE',
  Held = 'HELD',
  Running = 'RUNNING',
}

interface RandomScrambleResponse {
  scramble: {
    moves: string
    case: {
      id: string
      name: string
      imageUrl: string // TODO: obsolete?
      selectedAlgorithm?: {
        id: string
        moves: string
      }
    }
  }
}

const MS_TO_HOLD_SPACE = 1000

const Timer = () => {
  const [timerState, setTimerState] = useState(TimerState.Idle)
  const [hintVisible, setHintVisible] = useState(false)
  const [startTimeMs, setStartTimeMs] = useState(0)
  const [endTimeMs, setEndTimeMs] = useState(0)
  const keyDownTimeStampRef = useRef(0)
  const timerStateRef = useRef(timerState)
  timerStateRef.current = timerState
  const handleKeyDown = (e: KeyboardEvent) => {
    if (timerStateRef.current === TimerState.Running) {
      setTimerState(TimerState.Idle)
      return
    }

    if (timerStateRef.current === TimerState.Idle && e.key === ' ') {
      setTimerState(TimerState.Held)
      keyDownTimeStampRef.current = e.timeStamp
    }
  }
  const handleKeyUp = (e: KeyboardEvent) => {
    if (timerStateRef.current === TimerState.Held) {
      if (e.timeStamp - keyDownTimeStampRef.current >= MS_TO_HOLD_SPACE) {
        setTimerState(TimerState.Running)
        setStartTimeMs(Date.now())
      } else {
        setTimerState(TimerState.Idle)
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
      if (timerStateRef.current === TimerState.Running) {
        setEndTimeMs(Date.now())
      }
    }, 10)
    return () => clearInterval(intervalId)
  }, [])
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

  const axios = useAxiosWithAuth()
  const { data, isLoading, error, refetch } = useQuery({
    queryKey: [RANDOM_SCRAMBLE_QUERY_KEY],
    queryFn: () => axios.get<RandomScrambleResponse>('/scrambles/random'),
  })

  // TODO: loading + error
  if (isLoading) {
    return 'loading'
  }

  const scramble = data?.data.scramble.moves ?? ''
  return (
    <div className="flex flex-1 flex-col lg:flex-row">
      <div className="flex h-full flex-col items-center justify-center gap-4 lg:basis-1/3">
        <p className="w-full px-4 text-center text-xl font-bold">
          Scramble:
          <br />
          {scramble}
        </p>
        <ScrambleView scramble={scramble} />
        <button
          className="w-1/2 cursor-pointer rounded-sm bg-gray-800 px-4 py-2 text-white"
          onClick={() => refetch()}
        >
          Regenerate
        </button>
        <button
          className="w-1/2 cursor-pointer rounded-sm bg-gray-800 px-4 py-2 text-white"
          onClick={() => setHintVisible((visible) => !visible)}
        >
          {hintVisible ? 'Hide hint' : 'Show hint'}
        </button>
        <p
          className={
            'w-full px-4 text-center text-xl font-bold' +
            (hintVisible ? '' : ' opacity-0')
          }
        >
          Hint:
          <br />
          {data?.data.scramble.case.selectedAlgorithm?.moves ??
            "You haven't seleceted an algorithm for this case!"}
        </p>
      </div>
      <div className="flex flex-1 items-center justify-center rounded-lg border-2 border-gray-800 p-4">
        <p className="text-5xl font-bold">
          {formatTime(endTimeMs - startTimeMs)}
        </p>
      </div>
    </div>
  )
}

export default Timer
