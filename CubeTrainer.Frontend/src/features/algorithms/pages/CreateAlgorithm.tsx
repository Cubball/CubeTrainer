import { useNavigate, useParams } from 'react-router'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { toast } from 'react-toastify'
import { useForm } from 'react-hook-form'
import { z } from 'zod'
import { zodResolver } from '@hookform/resolvers/zod'
import { useAxiosWithAuth } from '../../../lib/axios'
import {
  CASE_ALGORITHMS_QUERY_KEY,
  CASE_DETAILS_QUERY_KEY,
} from '../../cases/lib/keys'
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

const schema = z.object({
  moves: z
    .string()
    .min(2, 'Algorithm moves are too short')
    .max(500, 'Algorithm moves are too long'),
})

type FormData = z.infer<typeof schema>

const CreateAlgorithm = () => {
  const axios = useAxiosWithAuth()
  const { id } = useParams()
  const navigate = useNavigate()
  const queryClient = useQueryClient()

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<FormData>({
    resolver: zodResolver(schema),
  })

  const { data, isLoading, isError } = useQuery({
    queryKey: [CASE_DETAILS_QUERY_KEY, id],
    queryFn: () => axios.get<CaseDetailsResponse>(`/cases/${id}`),
  })

  const createAlgorithmMutation = useMutation({
    mutationFn: (data: FormData) =>
      axios.post('/algorithms', {
        caseId: id,
        moves: data.moves.trim(),
      }),
    onSuccess: (data) => {
      queryClient.invalidateQueries({
        queryKey: [CASE_ALGORITHMS_QUERY_KEY, id],
      })
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
        <form
          onSubmit={handleSubmit((data) =>
            createAlgorithmMutation.mutate(data),
          )}
          className="flex w-full flex-col gap-4"
        >
          <div>
            <h2 className="mb-2 text-xl font-semibold">Algorithm Moves</h2>
            <input
              className="w-full rounded border p-2 font-mono"
              {...register('moves')}
              placeholder="Enter algorithm moves (e.g., R U R' U')"
            />
            {errors.moves && (
              <p className="text-red-500">{errors.moves.message}</p>
            )}
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
