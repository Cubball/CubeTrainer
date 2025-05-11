import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useAxiosWithAuth } from '../../../lib/axios'
import { useState } from 'react'
import { AxiosResponse } from 'axios'
import Stopwatch from '../../../components/Stopwatch'
import Error from '../../../components/Error'
import Loader from '../../../components/Loader'
import ScrambleSidebar from '../../../components/ScrambleSidebar'
import CountdownStrip from '../../../components/CountdownStrip'
import { useCountdownStore } from '../../../state/countdown'
import { toast } from 'react-toastify'
import { useParams } from 'react-router'
import { TRAINING_PLAN_RANDOM_SCRAMBLE_QUERY_KEY } from '../lib/keys'

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

const PlanTrainer = () => {
  const { id } = useParams()
  const [hintVisible, setHintVisible] = useState(false)
  const [lastSolveTimeMs, setLastSolveTimeMs] = useState(0)
  const [lastSolveCaseId, setLastSolveCaseId] = useState('')

  const isCountdownVisible = useCountdownStore((state) => state.isVisible)
  const startCountdown = useCountdownStore((state) => state.start)
  const stopCountdown = useCountdownStore((state) => state.stop)

  const axios = useAxiosWithAuth()
  const queryClient = useQueryClient()
  const { data, isLoading, isError, refetch } = useQuery({
    queryKey: [TRAINING_PLAN_RANDOM_SCRAMBLE_QUERY_KEY, id],
    queryFn: () =>
      axios.get<RandomScrambleResponse>(
        `/scrambles/training-plans/${id}/random`,
      ),
    refetchOnWindowFocus: false,
  })
  const { mutate } = useMutation({
    mutationFn: (data: SolveData) => axios.post('/solves', data),
    onError: () =>
      toast('Failed to save the solve', {
        type: 'error',
        theme: 'colored',
      }),
  })

  // TODO: move countdown and stuff into its feature, since it wont be used here???

  const onSolveFinished = (ms: number) => {
    const caseId =
      queryClient.getQueryData<AxiosResponse<RandomScrambleResponse>>([
        TRAINING_PLAN_RANDOM_SCRAMBLE_QUERY_KEY,
        id,
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
    // TODO: this should prevent double counting
    setLastSolveCaseId('')
    setLastSolveTimeMs(0)
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
              // TODO: check if it'll double count
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

export default PlanTrainer
