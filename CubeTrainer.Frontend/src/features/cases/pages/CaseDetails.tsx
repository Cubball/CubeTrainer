import { Link, useParams } from 'react-router'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useAxiosWithAuth } from '../../../lib/axios'
import { CASE_DETAILS_QUERY_KEY } from '../lib/keys'
import { STATUS, STATUS_OPTIONS } from '../lib/types'
import { ALGORITHM_DETAILS_QUERY_KEY } from '../../algorithms/lib/keys'
import Loader from '../../../components/Loader'
import Error from '../../../components/Error'
import ScrambleView from '../../../components/ScrambleView'
import StarRating from '../../../components/StarRating'
import TitleWithBackButton from '../../../components/TitleWithBackButton'
import { toast } from 'react-toastify'

interface Algorithm {
  id: string
  moves: string
  isPublic: boolean
  isMine: boolean
  isDeleted: boolean
  usersCount: number
  totalRating: number
  usersRatingsCount: number
  myRating: { rating: number } | null
  myStatistic: {
    totalTimeSolvingInSeconds: number
    timedSolvesCount: number
    bestTimeInSeconds: number
  }
}

interface CaseDetailsResponse {
  case: {
    id: string
    name: string
    status: string
    defaultScramble: string
    selectedAlgorithm: Algorithm | null
  }
}

const formatTime = (seconds: number) => {
  if (seconds < 60) {
    return seconds.toFixed(3)
  }

  const minutes = Math.floor(seconds / 60)
  const remainingSeconds = seconds % 60
  return `${minutes}:${remainingSeconds.toFixed(3)}`
}

const calculateAverageTime = (algorithm?: Algorithm | null) => {
  if (
    !algorithm ||
    !algorithm.myStatistic ||
    algorithm.myStatistic.timedSolvesCount === 0
  ) {
    return 'N/A'
  }

  const averageTime =
    algorithm.myStatistic.totalTimeSolvingInSeconds /
    algorithm.myStatistic.timedSolvesCount
  return formatTime(averageTime)
}

const CaseDetails = () => {
  const axios = useAxiosWithAuth()
  const { id } = useParams()
  const queryClient = useQueryClient()
  const { data, isLoading, isError } = useQuery({
    queryKey: [CASE_DETAILS_QUERY_KEY, id],
    queryFn: () => axios.get<CaseDetailsResponse>(`/cases/${id}`),
  })

  const updateRatingMutation = useMutation({
    mutationFn: (rating: number | null) =>
      axios.post(
        `/algorithms/${data?.data?.case?.selectedAlgorithm?.id}/rate`,
        { rating },
      ),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [
          ALGORITHM_DETAILS_QUERY_KEY,
          data?.data?.case?.selectedAlgorithm?.id,
        ],
      })
      queryClient.invalidateQueries({ queryKey: [CASE_DETAILS_QUERY_KEY, id] })
      toast('Rating updated', {
        type: 'success',
        theme: 'colored',
      })
    },
    onError: () => {
      toast('Failed to update the algorithm rating', {
        type: 'error',
        theme: 'colored',
      })
    },
  })

  const updateStatusMutation = useMutation({
    mutationFn: (status: string) => axios.put(`/cases/${id}`, { status }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: [CASE_DETAILS_QUERY_KEY, id] })
      toast('Case status updated', {
        type: 'success',
        theme: 'colored',
      })
    },
    onError: () => {
      toast('Failed to update the case status', {
        type: 'error',
        theme: 'colored',
      })
    },
  })

  if (isLoading) {
    return <Loader />
  }

  if (isError) {
    return <Error />
  }

  const caseData = data?.data?.case
  const algorithm = caseData?.selectedAlgorithm

  return (
    <div className="flex w-full flex-col items-center gap-4">
      <TitleWithBackButton title={caseData?.name} />
      <div className="grid w-full max-w-7xl grid-cols-1 gap-6 md:grid-cols-2">
        <div className="flex flex-col items-center justify-center gap-4">
          <div className="aspect-square max-w-60">
            <ScrambleView
              scramble={caseData?.defaultScramble ?? ''}
              forceAspectSquare
            />
          </div>
          <Link
            to="algorithms"
            className="w-1/2 max-w-60 cursor-pointer rounded-sm bg-gray-800 px-4 py-2 text-center text-white"
          >
            View Available Algorithms
          </Link>
          {algorithm &&
            (algorithm.isPublic ||
              (algorithm.isMine && !algorithm.isDeleted)) && (
              <Link
                to={`/algorithms/${algorithm.id}`}
                className="w-1/2 max-w-60 cursor-pointer rounded-sm bg-gray-800 px-4 py-2 text-center text-white"
              >
                View This Algorithm
              </Link>
            )}
          <Link
            to="algorithms/new"
            className="w-1/2 max-w-60 cursor-pointer rounded-sm bg-gray-800 px-4 py-2 text-center text-white"
          >
            Create a New Algorithm
          </Link>
        </div>
        <div className="flex flex-col gap-4 p-4">
          <div>
            <h2 className="mb-2 text-xl font-semibold">Algorithm</h2>
            {algorithm ? (
              <div className="font-mono text-lg">{algorithm.moves}</div>
            ) : (
              <div className="text-gray-500 italic">No algorithm selected</div>
            )}
          </div>
          <div>
            <h2 className="mb-2 text-xl font-semibold">Status</h2>
            <div>
              <select
                value={caseData?.status || STATUS.NOT_LEARNED}
                onChange={(e) => updateStatusMutation.mutate(e.target.value)}
                className="w-fit cursor-pointer rounded pr-4"
                disabled={updateStatusMutation.isPending}
              >
                {STATUS_OPTIONS.map((option) => (
                  <option key={option.value} value={option.value}>
                    {option.label}
                  </option>
                ))}
              </select>
            </div>
          </div>
          {algorithm && (
            <div>
              <h2 className="mb-2 text-xl font-semibold">
                My Solve Statistics
              </h2>
              <div className="grid grid-cols-2 gap-2">
                <div>Solves:</div>
                <div className="font-semibold">
                  {algorithm.myStatistic.timedSolvesCount}
                </div>
                <div>Average solve time:</div>
                <div className="font-semibold">
                  {calculateAverageTime(algorithm)}
                </div>
                <div>Best solve time:</div>
                <div className="font-semibold">
                  {algorithm.myStatistic.bestTimeInSeconds > 0
                    ? formatTime(algorithm.myStatistic.bestTimeInSeconds)
                    : 'N/A'}
                </div>
              </div>
            </div>
          )}
          {algorithm && (algorithm.isPublic || algorithm.isMine) && (
            <div>
              <h2 className="mb-2 text-xl font-semibold">
                Community Statistics
              </h2>
              <div className="grid grid-cols-2 gap-2">
                <div>People using it currently:</div>
                <div className="font-semibold">{algorithm.usersCount}</div>
                <div>Average rating:</div>
                <div>
                  {algorithm.usersRatingsCount > 0 ? (
                    <div className="flex h-6 items-center gap-2">
                      <StarRating
                        rating={
                          algorithm.totalRating / algorithm.usersRatingsCount
                        }
                      />
                      <span className="text-sm text-gray-500">
                        ({algorithm.usersRatingsCount}
                        {algorithm.usersRatingsCount > 1
                          ? ' ratings'
                          : ' rating'}
                        )
                      </span>
                    </div>
                  ) : (
                    'No ratings'
                  )}
                </div>
                {!algorithm.isMine && (
                  <>
                    <div>My rating:</div>
                    <div className="h-6">
                      <StarRating
                        interactive={true}
                        rating={algorithm?.myRating?.rating || 0}
                        onRatingChange={(r) => updateRatingMutation.mutate(r)}
                      />
                    </div>
                  </>
                )}
              </div>
            </div>
          )}
          {algorithm && !algorithm.isMine && !algorithm.isPublic && (
            <div>
              <h2 className="mb-2 text-xl font-semibold">
                Community Statistics
              </h2>
              <div className="text-gray-500 italic">
                This algorithm was made private or deleted by the author. You
                can only see your own statistics
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  )
}

export default CaseDetails
