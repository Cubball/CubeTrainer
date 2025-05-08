import { Link, useNavigate, useParams } from 'react-router'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useAxiosWithAuth } from '../../../lib/axios'
import { ALGORITHM_DETAILS_QUERY_KEY } from './lib/keys'
import { CASE_DETAILS_QUERY_KEY } from '../../cases/lib/keys'
import Loader from '../../../components/Loader'
import Error from '../../../components/Error'
import ScrambleView from '../../../components/ScrambleView'
import StarRating from '../../../components/StarRating'
import Modal from '../../../components/Modal'
import { useState } from 'react'
import TitleWithBackButton from '../../../components/TitleWithBackButton'

interface Algorithm {
  id: string
  moves: string
  isPublic: boolean
  isMine: boolean
  createdAt: string
  usersCount: number
  totalRating: number
  usersRatingsCount: number
  case: {
    id: string
    type: string
    name: string
    imageUrl: string
    defaultScramble: string
  }
  myRating: null | { rating: number }
  myStatistic: {
    totalTimeSolvingInSeconds: number
    timedSolvesCount: number
    untimedSolvesCount: number
    bestTimeInSeconds: number
  }
}

interface AlgorithmDetailsResponse {
  algorithm: Algorithm
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

const AlgorithmDetails = () => {
  const axios = useAxiosWithAuth()
  const { id } = useParams()
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  const [isDeleteModalOpen, setIsDeleteModalOpen] = useState(false)
  const { data, isLoading, isError } = useQuery({
    queryKey: [ALGORITHM_DETAILS_QUERY_KEY, id],
    queryFn: () => axios.get<AlgorithmDetailsResponse>(`/algorithms/${id}`),
  })

  const updateRatingMutation = useMutation({
    mutationFn: (rating: number | null) =>
      axios.post(`/algorithms/${id}/rate`, { rating }),
    onSuccess: () => {
      // I'm not too happy about invalidating the query from a different feature
      queryClient.invalidateQueries({
        queryKey: [CASE_DETAILS_QUERY_KEY, algorithm?.case.id],
      })
      queryClient.invalidateQueries({
        queryKey: [ALGORITHM_DETAILS_QUERY_KEY, id],
      })
    },
  })

  const publishAlgorithmMutation = useMutation({
    mutationFn: () => axios.post(`/algorithms/${id}/publish`, {}),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [ALGORITHM_DETAILS_QUERY_KEY, id],
      })
    },
  })

  const unpublishAlgorithmMutation = useMutation({
    mutationFn: () => axios.post(`/algorithms/${id}/unpublish`, {}),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [ALGORITHM_DETAILS_QUERY_KEY, id],
      })
    },
  })

  const selectAlgorithmMutation = useMutation({
    mutationFn: () => axios.post(`/algorithms/${id}/select`, { id }),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [CASE_DETAILS_QUERY_KEY, id],
      })
    },
  })

  const deleteAlgorithmMutation = useMutation({
    mutationFn: () => axios.delete(`/algorithms/${id}`),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [ALGORITHM_DETAILS_QUERY_KEY, id],
      })
      navigate(`/cases/${algorithmCase?.id}/algorithms`)
    },
  })

  const handleDeleteConfirm = () => {
    deleteAlgorithmMutation.mutate()
    setIsDeleteModalOpen(false)
  }

  if (isLoading) {
    return <Loader />
  }

  if (isError) {
    return <Error />
  }

  const algorithm = data?.data.algorithm
  const algorithmCase = algorithm?.case

  return (
    <div className="flex w-full flex-col items-center gap-4">
      <TitleWithBackButton title={algorithmCase?.name} />
      <div className="grid w-full max-w-7xl grid-cols-1 gap-6 md:grid-cols-2">
        <div className="flex flex-col items-center justify-center gap-4">
          <div className="aspect-square max-w-60">
            <ScrambleView
              scramble={algorithmCase?.defaultScramble ?? ''}
              forceAspectSquare
            />
          </div>
          <Link
            to={`/cases/${algorithmCase?.id}`}
            className="w-1/2 max-w-60 cursor-pointer rounded-sm bg-gray-800 px-4 py-2 text-center text-white"
          >
            View case
          </Link>
          <button
            className="w-1/2 max-w-60 cursor-pointer rounded-sm bg-gray-800 px-4 py-2 text-center text-white"
            onClick={() => selectAlgorithmMutation.mutate()}
          >
            Pick this algorithm
          </button>
          {algorithm?.isMine && (
            <>
              {algorithm?.isPublic ? (
                <button
                  className="w-1/2 max-w-60 cursor-pointer rounded-sm bg-gray-800 px-4 py-2 text-center text-white"
                  onClick={() => unpublishAlgorithmMutation.mutate()}
                >
                  Make private
                </button>
              ) : (
                <button
                  className="w-1/2 max-w-60 cursor-pointer rounded-sm bg-gray-800 px-4 py-2 text-center text-white"
                  onClick={() => publishAlgorithmMutation.mutate()}
                >
                  Make public
                </button>
              )}
              <button
                className="w-1/2 max-w-60 cursor-pointer rounded-sm bg-red-700 px-4 py-2 text-center text-white"
                onClick={() => setIsDeleteModalOpen(true)}
              >
                Delete
              </button>
            </>
          )}
        </div>
        <div className="flex flex-col gap-4 p-4">
          <div>
            <h2 className="mb-2 text-xl font-semibold">Moves</h2>
            <div className="font-mono text-lg">{algorithm?.moves}</div>
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

      <Modal
        isOpen={isDeleteModalOpen}
        onClose={() => setIsDeleteModalOpen(false)}
        title="Confirm Deletion"
        actions={
          <>
            <button
              onClick={() => setIsDeleteModalOpen(false)}
              className="cursor-pointer rounded-sm border px-4 py-2 text-center"
            >
              Cancel
            </button>
            <button
              onClick={handleDeleteConfirm}
              className="cursor-pointer rounded-sm bg-red-700 px-4 py-2 text-center text-white"
            >
              Delete
            </button>
          </>
        }
      >
        <p>
          Are you sure you want to delete this algorithm for{' '}
          {algorithmCase?.name}? This action cannot be undone.
        </p>
      </Modal>
    </div>
  )
}

export default AlgorithmDetails
