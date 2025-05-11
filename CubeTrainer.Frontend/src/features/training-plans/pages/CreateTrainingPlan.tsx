import { useNavigate } from 'react-router'
import { useMutation, useQueryClient } from '@tanstack/react-query'
import { toast } from 'react-toastify'
import { z } from 'zod'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { useAxiosWithAuth } from '../../../lib/axios'
import { TRAINING_PLANS_QUERY_KEY } from '../lib/keys'
import TitleWithBackButton from '../../../components/TitleWithBackButton'

const schema = z.object({
  name: z
    .string()
    .min(1, 'Training plan name is required')
    .max(100, 'Training plan name is too long'),
})

type FormData = z.infer<typeof schema>

const CreateTrainingPlan = () => {
  const axios = useAxiosWithAuth()
  const navigate = useNavigate()
  const queryClient = useQueryClient()

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<FormData>({
    resolver: zodResolver(schema),
  })

  const createTrainingPlanMutation = useMutation({
    mutationFn: (data: FormData) => axios.post('/training-plans', data),
    onSuccess: (data) => {
      queryClient.invalidateQueries({ queryKey: [TRAINING_PLANS_QUERY_KEY] })
      toast('Training plan created successfully', {
        type: 'success',
        theme: 'colored',
      })
      navigate(`/training-plans/${data.data.id}`)
    },
    onError: () => {
      toast('Failed to create the training plan', {
        type: 'error',
        theme: 'colored',
      })
    },
  })

  return (
    <div className="flex w-full flex-col items-center gap-4">
      <TitleWithBackButton title="Create a Training Plan" />
      <div className="flex w-full max-w-2xl flex-col items-center gap-4">
        <form
          onSubmit={handleSubmit((data) =>
            createTrainingPlanMutation.mutate(data),
          )}
          className="flex w-full flex-col gap-4"
        >
          <div>
            <h2 className="mb-2 text-xl font-semibold">Name</h2>
            <input
              className="w-full rounded border p-2"
              {...register('name')}
              placeholder="Enter training plan name"
            />
            {errors.name && (
              <p className="mt-1 text-red-500">{errors.name.message}</p>
            )}
          </div>
          <button
            type="submit"
            className="mx-auto w-fit cursor-pointer rounded-sm bg-gray-800 px-4 py-2 text-center text-white disabled:cursor-not-allowed disabled:bg-gray-500"
            disabled={createTrainingPlanMutation.isPending}
          >
            {createTrainingPlanMutation.isPending
              ? 'Creating...'
              : 'Create Training Plan'}
          </button>
        </form>
      </div>
    </div>
  )
}

export default CreateTrainingPlan
