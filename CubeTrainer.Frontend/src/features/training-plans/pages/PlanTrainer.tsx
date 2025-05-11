import { toast } from 'react-toastify'
import { Link, useParams } from 'react-router'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useState } from 'react'
import axios, { AxiosResponse } from 'axios'
import { useAxiosWithAuth } from '../../../lib/axios'
import { TRAINING_PLAN_RANDOM_SCRAMBLE_QUERY_KEY } from '../lib/keys'
import Stopwatch from '../../../components/Stopwatch'
import Error from '../../../components/Error'
import Loader from '../../../components/Loader'
import ScrambleSidebar from '../../../components/ScrambleSidebar'
import TitleWithBackButton from '../../../components/TitleWithBackButton'

interface TrainingPlanRandomScrambleResponse {
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
  difficultyRating?: string
}

const PlanTrainer = () => {
  const { id } = useParams()
  const [hintVisible, setHintVisible] = useState(false)
  const [lastSolveData, setLastSolveData] = useState<SolveData | null>(null)
  const [justFinished, setJustFinished] = useState(false)

  const axiosInstance = useAxiosWithAuth()
  const queryClient = useQueryClient()
  const { data, isLoading, isError, refetch, error } = useQuery({
    queryKey: [TRAINING_PLAN_RANDOM_SCRAMBLE_QUERY_KEY, id],
    queryFn: () =>
      axiosInstance.get<TrainingPlanRandomScrambleResponse>(
        `/scrambles/training-plans/${id}/random`,
      ),
    retry: false,
  })
  const { mutate } = useMutation({
    mutationFn: (data: SolveData) =>
      axiosInstance.post(`/solves/training-plans/${id}`, data),
    onError: () =>
      toast('Failed to save the solve', {
        type: 'error',
        theme: 'colored',
      }),
  })

  const onSolveFinished = (ms: number) => {
    const caseId =
      queryClient.getQueryData<
        AxiosResponse<TrainingPlanRandomScrambleResponse>
      >([TRAINING_PLAN_RANDOM_SCRAMBLE_QUERY_KEY, id])?.data.scramble.case.id ??
      ''
    setLastSolveData({
      caseId,
      time: ms / 1000,
    })
    setHintVisible(false)
    setJustFinished(true)
    refetch()
  }

  if (axios.isAxiosError(error) && error.response?.status === 404) {
    return (
      <div className="my-auto flex h-full w-full flex-col items-center justify-center gap-4">
        <div className="text-xl">
          There are no cases to learn in this training plan
        </div>
        <Link
          to={`/training-plans/${id}`}
          className="cursor-pointer rounded-sm bg-gray-800 px-4 py-2 text-white"
        >
          Go to the Training Plan
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
    <div className="relative flex w-full flex-col">
      <div className="mb-4 lg:absolute lg:mb-0">
        <TitleWithBackButton />
      </div>
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
            {justFinished ? (
              <div className="flex h-full w-full items-center justify-center">
                {lastSolveData?.time}
              </div>
            ) : (
              <Stopwatch onStop={onSolveFinished} />
            )}
          </div>
          {justFinished && (
            <div className="absolute bottom-0 left-0 w-full">
              <div className="mb-2 w-full text-center text-xl">
                How was the solve?
              </div>
              <div className="mb-2 flex justify-center gap-2">
                <button
                  className="cursor-pointer rounded-sm bg-gray-800 px-4 py-2 text-white"
                  onClick={() => {
                    setJustFinished(false)
                    mutate({
                      ...lastSolveData!,
                      difficultyRating: 'Easy',
                    })
                  }}
                >
                  Easy
                </button>
                <button
                  className="cursor-pointer rounded-sm bg-gray-800 px-4 py-2 text-white"
                  onClick={() => {
                    setJustFinished(false)
                    mutate({
                      ...lastSolveData!,
                      difficultyRating: 'Normal',
                    })
                  }}
                >
                  Normal
                </button>
                <button
                  className="cursor-pointer rounded-sm bg-gray-800 px-4 py-2 text-white"
                  onClick={() => {
                    setJustFinished(false)
                    mutate({
                      ...lastSolveData!,
                      difficultyRating: 'Hard',
                    })
                  }}
                >
                  Hard
                </button>
              </div>
              <div className="mb-2 flex justify-center">
                <button
                  className="cursor-pointer rounded-sm bg-red-700 px-4 py-2 text-white"
                  onClick={() => setJustFinished(false)}
                >
                  Delete This Solve
                </button>
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  )
}

export default PlanTrainer
