import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useParams } from 'react-router'
import { TRAINING_PLAN_DETAILS_QUERY_KEY } from '../lib/keys'
import { useAxiosWithAuth } from '../../../lib/axios'
import Loader from '../../../components/Loader'
import Error from '../../../components/Error'
import TitleWithBackButton from '../../../components/TitleWithBackButton'
import { OLL_CASES_QUERY_KEY, PLL_CASES_QUERY_KEY } from '../../cases/lib/keys'
import TrainingPlanCasesTable from '../components/TrainingPlanCasesTable'
import { toast } from 'react-toastify'

interface TrainingPlanCase {
  solvesToLearnCount: number
  lastDifficultyRating: string
  lastSolved: string
  case: {
    id: string
    name: string
  }
}

interface TrainingPlanDetailsResponse {
  trainingPlan: {
    id: string
    name: string
    trainingPlanCases: TrainingPlanCase[]
  }
}

interface Case {
  id: string
  name: string
  defaultScramble: string
}

interface CasesResponse {
  items: Case[]
}

const sortCases = (oll: Case[], pll: Case[]) => {
  const ollSorted = oll.sort((a, b) => a.name.localeCompare(b.name))
  const pllSorted = pll.sort((a, b) => a.name.localeCompare(b.name))
  return [...ollSorted, ...pllSorted]
}

const getSelectedCases = (
  trainingPlanCases: TrainingPlanCase[],
  cases: Case[],
) =>
  cases
    .map((c) => ({
      ...c,
      trainingPlanCase: trainingPlanCases.find((tc) => tc.case.id === c.id),
    }))
    .filter((c) => c.trainingPlanCase)
    .map((c) => ({
      id: c.id,
      name: c.name,
      defaultScramble: c.defaultScramble,
      isInPlan: true,
      solvesToLearnCount: c.trainingPlanCase!.solvesToLearnCount,
      lastDifficultyRating: c.trainingPlanCase!.lastDifficultyRating,
      lastSolved: c.trainingPlanCase!.lastSolved,
    }))

const getNotSelectedCases = (
  trainingPlanCases: TrainingPlanCase[],
  cases: Case[],
) =>
  cases
    .filter((c) => !trainingPlanCases.some((tc) => tc.case.id === c.id))
    .map((c) => ({
      id: c.id,
      name: c.name,
      defaultScramble: c.defaultScramble,
      isInPlan: false,
    }))

const TrainingPlanDetails = () => {
  const axios = useAxiosWithAuth()
  const { id } = useParams()
  const queryClient = useQueryClient()

  const {
    data: planData,
    isLoading: isPlanLoading,
    isError: isPlanError,
  } = useQuery({
    queryKey: [TRAINING_PLAN_DETAILS_QUERY_KEY, id],
    queryFn: () =>
      axios.get<TrainingPlanDetailsResponse>(`/training-plans/${id}`),
  })

  const {
    data: ollData,
    isLoading: isOllLoading,
    isError: isOllError,
  } = useQuery({
    queryKey: [OLL_CASES_QUERY_KEY],
    queryFn: () => axios.get<CasesResponse>('/cases/OLL/my'),
  })

  const {
    data: pllData,
    isLoading: isPllLoading,
    isError: isPllError,
  } = useQuery({
    queryKey: [PLL_CASES_QUERY_KEY],
    queryFn: () => axios.get<CasesResponse>('/cases/PLL/my'),
  })

  const addMutation = useMutation({
    mutationFn: (caseId: string) =>
      axios.post(`/training-plans/${id}/cases`, { caseId }),
    onSuccess: () => {
      toast('Case added to the training plan', {
        type: 'success',
        theme: 'colored',
      })
      queryClient.invalidateQueries({
        queryKey: [TRAINING_PLAN_DETAILS_QUERY_KEY, id],
      })
    },
    onError: () => {
      toast('Failed to add the case to the training plan', {
        type: 'error',
        theme: 'colored',
      })
    },
  })

  const removeMutation = useMutation({
    mutationFn: (caseId: string) =>
      axios.delete(`/training-plans/${id}/cases/${caseId}`),
    onSuccess: () => {
      toast('Case removed from the training plan', {
        type: 'success',
        theme: 'colored',
      })
      queryClient.invalidateQueries({
        queryKey: [TRAINING_PLAN_DETAILS_QUERY_KEY, id],
      })
    },
    onError: () => {
      toast('Failed to remove the case from the training plan', {
        type: 'error',
        theme: 'colored',
      })
    },
  })

  if (isPlanLoading || isOllLoading || isPllLoading) {
    return <Loader />
  }

  if (isPlanError || isOllError || isPllError) {
    return <Error />
  }

  const cases = sortCases(ollData?.data.items ?? [], pllData?.data.items ?? [])
  const trainingPlanCases = planData?.data.trainingPlan.trainingPlanCases ?? []
  const selectedCases = getSelectedCases(trainingPlanCases, cases)
  const notSelectedCases = getNotSelectedCases(trainingPlanCases, cases)
  const allCases = [...selectedCases, ...notSelectedCases]

  return (
    <div className="flex w-full flex-col items-center gap-4">
      <TitleWithBackButton title={planData?.data?.trainingPlan.name} />
      <div className="w-3/4 max-w-2xl text-center">
        Here you can select the cases you want to train. You can also see when
        each case was last solved, how difficult was it and how many solves you
        have left to learn.
      </div>
      <TrainingPlanCasesTable
        cases={allCases}
        onCaseAction={(caseId, isInPlan) => {
          if (isInPlan) {
            removeMutation.mutate(caseId)
          } else {
            addMutation.mutate(caseId)
          }
        }}
      />
    </div>
  )
}

export default TrainingPlanDetails
