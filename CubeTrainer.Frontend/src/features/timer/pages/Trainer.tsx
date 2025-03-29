import { useQuery } from '@tanstack/react-query'
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
        <Stopwatch onStop={() => setHintVisible(false)} />
      </div>
    </div>
  )
}

export default Trainer
