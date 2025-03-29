import { useMutation, useQuery } from '@tanstack/react-query'
import { useAxiosWithAuth } from '../../../lib/axios'
import { useState } from 'react'
import Stopwatch from '../components/Stopwatch'
import Error from '../../../components/Error'
import Loader from '../../../components/Loader'
import ScrambleSidebar from '../components/ScrambleSidebar'

const RANDOM_SCRAMBLE_QUERY_KEY = 'random-scramble'

interface RandomScrambleResponse {
  scramble: {
    moves: string
    case: {
      id: string
      name: string
      selectedAlgorithm?: {
        id: string
        moves: string
      }
    }
  }
}

interface SolveData {
  caseId: string
  time: number
}

// TODO: make this more generic, since
// will probably use it in 2 places
const Trainer = () => {
  const [hintVisible, setHintVisible] = useState(false)
  const axios = useAxiosWithAuth()
  const { data, isLoading, isError, refetch } = useQuery({
    queryKey: [RANDOM_SCRAMBLE_QUERY_KEY],
    queryFn: () => axios.get<RandomScrambleResponse>('/scrambles/random'),
    retry: true,
    refetchOnWindowFocus: false,
  })
  const { mutate } = useMutation({
    mutationFn: (data: SolveData) => axios.post('/solves', data),
    onError: (e) => console.log('error ', e), // TODO:
  })

  const onSolveFinished = (ms: number) => {
    // TODO:
    if (!data?.data.scramble.case.id) {
      return
    }

    const solveData: SolveData = {
      caseId: data?.data.scramble.case.id,
      time: ms / 1000,
    }
    mutate(solveData)
    setHintVisible(false)
    refetch()
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
      <div className="flex-1 rounded-lg border-2 border-gray-800 p-4 text-5xl font-bold">
        <Stopwatch onStop={onSolveFinished} />
      </div>
    </div>
  )
}

export default Trainer
