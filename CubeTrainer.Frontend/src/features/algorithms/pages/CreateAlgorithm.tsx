import { useState } from 'react'
import { useNavigate, useParams } from 'react-router'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { toast } from 'react-toastify'
import { useAxiosWithAuth } from '../../../lib/axios'
import { CASE_DETAILS_QUERY_KEY } from '../../cases/lib/keys'
import Loader from '../../../components/Loader'
import Error from '../../../components/Error'
import ScrambleView from '../../../components/ScrambleView'
import TitleWithBackButton from '../../../components/TitleWithBackButton'

interface CaseDetailsResponse {
  case: {
    id: string
    name: string
    defaultScramble: string
  }
}

const CreateAlgorithm = () => {
  const axios = useAxiosWithAuth()
  const { id } = useParams()
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  const [moves, setMoves] = useState('')

  const { data, isLoading, isError } = useQuery({
    queryKey: [CASE_DETAILS_QUERY_KEY, id],
    queryFn: () => axios.get<CaseDetailsResponse>(`/cases/${id}`),
  })

  const createAlgorithmMutation = useMutation({
    mutationFn: () =>
      axios.post('/algorithms', {
        caseId: id,
        moves: moves.trim(),
      }),
    onSuccess: (data) => {
      queryClient.invalidateQueries({ queryKey: [CASE_DETAILS_QUERY_KEY, id] })
      toast('Algorithm created successfully', {
        type: 'success',
        theme: 'colored',
      })
      navigate(`/algorithms/${data.data.id}`)
    },
    onError: () => {
      toast('Failed to create the algorithm', {
        type: 'error',
        theme: 'colored',
      })
    },
  })

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    if (!moves.trim()) {
      toast('Please enter the algorithm moves', {
        type: 'error',
        theme: 'colored',
      })
      return
    }

    createAlgorithmMutation.mutate()
  }

  if (isLoading) {
    return <Loader />
  }

  if (isError) {
    return <Error />
  }

  const caseData = data?.data?.case

  return (
    <div className="flex w-full flex-col items-center gap-4">
      <TitleWithBackButton
        title={`Create an Algorithm for ${caseData?.name}`}
      />
      <div className="flex w-full max-w-2xl flex-col items-center gap-4">
        <div className="aspect-square max-w-60">
          <ScrambleView
            scramble={caseData?.defaultScramble ?? ''}
            forceAspectSquare
          />
        </div>
        <form onSubmit={handleSubmit} className="flex w-full flex-col gap-4">
          <div>
            <h2 className="mb-2 text-xl font-semibold">Algorithm Moves</h2>
            <input
              className="w-full rounded border p-2 font-mono"
              value={moves}
              onChange={(e) => setMoves(e.target.value)}
              placeholder="Enter algorithm moves (e.g., R U R' U')"
              required
            />
            <p className="mt-2 text-sm text-gray-500">
              Enter the moves using standard cube notation
            </p>
          </div>
          <button
            type="submit"
            className="mx-auto w-fit cursor-pointer rounded-sm bg-gray-800 px-4 py-2 text-center text-white disabled:cursor-not-allowed disabled:bg-gray-500"
            disabled={createAlgorithmMutation.isPending}
          >
            {createAlgorithmMutation.isPending
              ? 'Creating...'
              : 'Create Algorithm'}
          </button>
        </form>
      </div>
    </div>
  )
}

export default CreateAlgorithm
