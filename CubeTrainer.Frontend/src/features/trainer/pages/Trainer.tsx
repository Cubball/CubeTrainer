import { useState } from 'react'
import axios, { AxiosResponse } from 'axios'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { toast } from 'react-toastify'
import { useCountdownStore } from '../../../state/countdown'
import { RANDOM_SCRAMBLE_QUERY_KEY } from '../lib/keys'
import { useAxiosWithAuth } from '../../../lib/axios'
import Stopwatch from '../../../components/Stopwatch'
import Error from '../../../components/Error'
import Loader from '../../../components/Loader'
import ScrambleSidebar from '../../../components/ScrambleSidebar'
import CountdownStrip from '../../../components/CountdownStrip'
import { Link } from 'react-router'

interface RandomScrambleResponse {
  scramble: {
    moves: string
    case: {
      id: string
      selectedAlgorithm?: {
        moves: string
      }
    }
  }
}

interface SolveData {
  caseId: string
  time: number
}

const Trainer = () => {
  const [hintVisible, setHintVisible] = useState(false)
  const [lastSolveData, setLastSolveData] = useState<SolveData | null>(null)

  const isCountdownVisible = useCountdownStore((state) => state.isVisible)
  const startCountdown = useCountdownStore((state) => state.start)
  const stopCountdown = useCountdownStore((state) => state.stop)

  const axiosInstance = useAxiosWithAuth()
  const queryClient = useQueryClient()
  const { data, isLoading, isError, refetch, error } = useQuery({
    queryKey: [RANDOM_SCRAMBLE_QUERY_KEY],
    queryFn: () =>
      axiosInstance.get<RandomScrambleResponse>('/scrambles/random'),
    retry: false,
  })
  const { mutate } = useMutation({
    mutationFn: (data: SolveData) => axiosInstance.post('/solves', data),
    onError: () =>
      toast('Failed to save the solve', {
        type: 'error',
        theme: 'colored',
      }),
  })

  const onSolveFinished = (ms: number) => {
    const caseId =
      queryClient.getQueryData<AxiosResponse<RandomScrambleResponse>>([
        RANDOM_SCRAMBLE_QUERY_KEY,
      ])?.data.scramble.case.id ?? ''
    setLastSolveData({
      caseId,
      time: ms / 1000,
    })
    startCountdown()
    setHintVisible(false)
    refetch()
  }

  const submitSolve = () => {
    if (lastSolveData) {
      mutate(lastSolveData)
      setLastSolveData(null)
    }
  }

  if (axios.isAxiosError(error) && error.response?.status === 404) {
    return (
      <div className="my-auto flex h-full w-full flex-col items-center justify-center gap-4">
        <div className="text-xl">There are no cases to learn</div>
        <Link
          to="/cases"
          className="cursor-pointer rounded-sm bg-gray-800 px-4 py-2 text-white"
        >
          Go to My Cases
        </Link>
      </div>
    )
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
        <div className="min-h-[480px] grow-1 text-5xl font-bold">
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
              Delete This Solve
            </button>
          </div>
          <CountdownStrip durationMs={5000} onComplete={submitSolve} />
        </div>
      </div>
    </div>
  )
}

export default Trainer
