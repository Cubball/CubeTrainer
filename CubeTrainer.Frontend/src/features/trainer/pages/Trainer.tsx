import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useAxiosWithAuth } from '../../../lib/axios'
import { useState } from 'react'
import { AxiosResponse } from 'axios'
import Stopwatch from '../components/Stopwatch'
import Error from '../../../components/Error'
import Loader from '../../../components/Loader'
import ScrambleSidebar from '../components/ScrambleSidebar'
import CountdownStrip from '../components/CountdownStrip'
import { useCountdownStore } from '../state/countdown'

const RANDOM_SCRAMBLE_QUERY_KEY = 'random-scramble'

interface RandomScrambleResponse {
  scramble: {
    moves: string
    case: {
      id: string
      name: string
      selectedAlgorithm?: {
        id: string
        moves: string
      }
    }
  }
}

interface SolveData {
  caseId: string
  time: number
}

// TODO: make this more generic, since
// will probably use it in 2 places
// to do this, just extract the calls to react-query and such
const Trainer = () => {
  const [hintVisible, setHintVisible] = useState(false)
  const [lastSolveTimeMs, setLastSolveTimeMs] = useState(0)
  const [lastSolveCaseId, setLastSolveCaseId] = useState('')

  const isCountdownVisible = useCountdownStore((state) => state.isVisible)
  const startCountdown = useCountdownStore((state) => state.start)
  const stopCountdown = useCountdownStore((state) => state.stop)

  const axios = useAxiosWithAuth()
  const queryClient = useQueryClient()
  const { data, isLoading, isError, refetch } = useQuery({
    queryKey: [RANDOM_SCRAMBLE_QUERY_KEY],
    queryFn: () => axios.get<RandomScrambleResponse>('/scrambles/random'),
    retry: true,
    refetchOnWindowFocus: false,
  })
  const { mutate } = useMutation({
    mutationFn: (data: SolveData) => axios.post('/solves', data),
    onError: (e) => console.log('error ', e), // TODO: maybe add a toast
  })

  const onSolveFinished = (ms: number) => {
    const caseId =
      queryClient.getQueryData<AxiosResponse<RandomScrambleResponse>>([
        RANDOM_SCRAMBLE_QUERY_KEY,
      ])?.data.scramble.case.id ?? ''
    setLastSolveCaseId(caseId)
    setLastSolveTimeMs(ms)
    startCountdown()
    setHintVisible(false)
    refetch()
  }

  const submitSolve = () => {
    if (!lastSolveCaseId || !lastSolveTimeMs) {
      return
    }

    const solveData: SolveData = {
      caseId: lastSolveCaseId,
      time: lastSolveTimeMs / 1000,
    }
    mutate(solveData)
  }

  if (isError) {
    return <Error />
  }

  if (isLoading) {
    return <Loader />
  }

  return (
    <div className="flex flex-1 flex-col lg:flex-row">
      <div className="lg:w-1/3">
        <ScrambleSidebar
          hintVisible={hintVisible}
          setHintVisible={setHintVisible}
          scramble={data?.data.scramble.moves ?? ''}
          hint={data?.data.scramble.case.selectedAlgorithm?.moves}
          onRegenerateClick={refetch}
        />
      </div>
      <div className="relative flex flex-1 flex-col rounded-lg border-2 border-gray-800 p-4">
        <div className="min-h-60 grow-1 text-5xl font-bold">
          <Stopwatch
            onStart={() => {
              submitSolve()
              stopCountdown()
            }}
            onStop={onSolveFinished}
          />
        </div>
        <div className="absolute bottom-0 left-0 w-full">
          <div className="flex justify-center">
            <button
              className={`mb-2 cursor-pointer rounded-sm bg-red-700 px-4 py-2 text-white ${isCountdownVisible ? '' : 'hidden'}`}
              onClick={stopCountdown}
            >
              Delete this solve
            </button>
          </div>
          <CountdownStrip durationMs={5000} onComplete={submitSolve} />
        </div>
      </div>
    </div>
  )
}

export default Trainer
