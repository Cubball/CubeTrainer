import { useQuery } from '@tanstack/react-query'
import { useAxiosWithAuth } from '../../../lib/axios'
import ScrambleView from '../components/ScrambleView'
import { useState } from 'react'
import Stopwatch from '../components/Stopwatch'

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

const Timer = () => {
  const [hintVisible, setHintVisible] = useState(false)
  const axios = useAxiosWithAuth()
  const { data, isLoading, error, refetch } = useQuery({
    queryKey: [RANDOM_SCRAMBLE_QUERY_KEY],
    queryFn: () => axios.get<RandomScrambleResponse>('/scrambles/random'),
  })

  // TODO: loading + error
  if (isLoading) {
    return 'loading'
  }

  const scramble = data?.data.scramble.moves ?? ''
  return (
    <div className="flex flex-1 flex-col lg:flex-row">
      <div className="flex h-full flex-col items-center justify-center gap-4 lg:basis-1/3">
        <p className="w-full px-4 text-center text-xl font-bold">
          Scramble:
          <br />
          {scramble}
        </p>
        <ScrambleView scramble={scramble} />
        <button
          className="w-1/2 cursor-pointer rounded-sm bg-gray-800 px-4 py-2 text-white"
          onClick={() => refetch()}
        >
          Regenerate
        </button>
        <button
          className="w-1/2 cursor-pointer rounded-sm bg-gray-800 px-4 py-2 text-white"
          onClick={() => setHintVisible((visible) => !visible)}
        >
          {hintVisible ? 'Hide hint' : 'Show hint'}
        </button>
        <p
          className={
            'w-full px-4 text-center text-xl font-bold' +
            (hintVisible ? '' : ' opacity-0')
          }
        >
          Hint:
          <br />
          {data?.data.scramble.case.selectedAlgorithm?.moves ??
            "You haven't seleceted an algorithm for this case!"}
        </p>
      </div>
      <div className="flex flex-1 items-center justify-center rounded-lg border-2 border-gray-800 p-4 text-5xl font-bold">
        <Stopwatch onStop={(n) => console.log(n)} />
      </div>
    </div>
  )
}

export default Timer
