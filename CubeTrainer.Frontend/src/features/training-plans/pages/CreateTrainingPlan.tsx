import { useState } from 'react'
import { useNavigate } from 'react-router'
import { useMutation, useQueryClient } from '@tanstack/react-query'
import { toast } from 'react-toastify'
import { useAxiosWithAuth } from '../../../lib/axios'
import TitleWithBackButton from '../../../components/TitleWithBackButton'
import { TRAINING_PLANS_QUERY_KEY } from '../lib/keys'

// TODO: use form hook here and in the create algorithm page
const CreateTrainingPlan = () => {
  const axios = useAxiosWithAuth()
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  const [name, setName] = useState('')

  const createTrainingPlanMutation = useMutation({
    mutationFn: () =>
      axios.post('/training-plans', {
        name: name.trim(),
      }),
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

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    if (!name.trim()) {
      toast('Please enter the training plan name', {
        type: 'error',
        theme: 'colored',
      })
      return
    }

    createTrainingPlanMutation.mutate()
  }

  return (
    <div className="flex w-full flex-col items-center gap-4">
      <TitleWithBackButton title="Create a Training Plan" />
      <div className="flex w-full max-w-2xl flex-col items-center gap-4">
        <form onSubmit={handleSubmit} className="flex w-full flex-col gap-4">
          <div>
            <h2 className="mb-2 text-xl font-semibold">Name</h2>
            <input
              className="w-full rounded border p-2"
              value={name}
              onChange={(e) => setName(e.target.value)}
              placeholder="Enter training plan name"
              required
            />
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
