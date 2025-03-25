import { useQuery } from '@tanstack/react-query'
import { useAxiosWithAuth } from '../../../lib/axios'
import ScrambleView from '../components/ScrambleView'
import { useState } from 'react'

const RANDOM_SCRAMBLE_QUERY_KEY = 'random-scramble'

interface RandomScrambleResponse {
  scramble: {
    moves: string
    case: {
      id: string
      name: string
      imageUrl: string // TODO: obsolete?
      selectedAlgorithm?: {
        id: string
        moves: string
      }
    }
  }
}

const Timer = () => {
  const axios = useAxiosWithAuth()
  const [hintVisible, setHintVisible] = useState(false)
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
    <div className="flex-1">
      {/* TODO: width */}
      <div className="flex h-full w-1/4 flex-col items-center justify-center gap-4">
        <p className="w-full text-center text-xl font-bold">
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
        {/* TODO: it jumps when toggling */}
        <p className="invisible w-full text-center text-xl font-bold">
          {hintVisible &&
            'Hint: ' +
              (data?.data.scramble.case.selectedAlgorithm?.moves ??
                "You haven't seleceted an algorithm for this case!")}
        </p>
      </div>
    </div>
  )
}

export default Timer
