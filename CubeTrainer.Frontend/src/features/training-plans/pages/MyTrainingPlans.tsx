import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useAxiosWithAuth } from '../../../lib/axios'
import { TRAINING_PLANS_QUERY_KEY } from '../lib/keys'
import Loader from '../../../components/Loader'
import Error from '../../../components/Error'
import TrainingPlansTable from '../components/TrainingPlansTable'
import { toast } from 'react-toastify'
import { Link } from 'react-router'

interface TrainingPlan {
  id: string
  name: string
}

interface TrainingPlansResponse {
  items: TrainingPlan[]
}

const sortTrainingPlans = (plans?: TrainingPlan[]) =>
  plans?.sort((a, b) => a.name.localeCompare(b.name)) ?? []

const MyTrainingPlans = () => {
  const axios = useAxiosWithAuth()
  const queryClient = useQueryClient()
  const { data, isLoading, isError } = useQuery({
    queryKey: [TRAINING_PLANS_QUERY_KEY],
    queryFn: () => axios.get<TrainingPlansResponse>('/training-plans'),
  })

  const deleteTrainingPlanMutation = useMutation({
    mutationFn: (planId: string) => axios.delete(`/training-plans/${planId}`),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [TRAINING_PLANS_QUERY_KEY],
      })
      toast('Training plan deleted', {
        type: 'success',
        theme: 'colored',
      })
    },
    onError: () => {
      toast('Failed to delete the training plan', {
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

  return (
    <div className="flex h-full w-full flex-col items-center gap-4">
      <h1 className="text-2xl font-bold">My Training Plans</h1>
      <p>Here you can see a list of all your training plans</p>
      <TrainingPlansTable
        trainingPlans={sortTrainingPlans(data?.data.items)}
        onTrainingPlanDelete={deleteTrainingPlanMutation.mutate}
      />
      <Link
        className="cursor-pointer rounded-sm bg-gray-800 px-4 py-2 text-white"
        to="new"
      >
        Create a New Training Plan
      </Link>
    </div>
  )
}

export default MyTrainingPlans
