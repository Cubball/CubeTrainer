import { useQuery } from '@tanstack/react-query'
import { OLL_ALGORITHMS_QUERY_KEY, PLL_ALGORITHMS_QUERY_KEY } from '../lib/keys'
import { useAxiosWithAuth } from '../../../lib/axios'
import Tabs from '../../../components/Tabs'
import Loader from '../../../components/Loader'
import Error from '../../../components/Error'
import AlgorithmsTable from '../components/AlgorithmsTable'

interface AlgorithmsResponse {
  items: {
    id: string
    case: {
      id: string
      name: string
      defaultScramble: string
    }
    moves: string
    isPublic: boolean
    usersCount: number
    totalRating: number
    usersRatingsCount: number
  }[]
}

const mapResponseToAlgorithms = (response?: AlgorithmsResponse) =>
  response?.items
    .map((item) => ({
      id: item.id,
      caseName: item.case.name,
      scramble: item.case.defaultScramble,
      moves: item.moves,
      isPublic: item.isPublic,
      usersCount: item.usersCount,
      totalRating: item.totalRating,
      usersRatingsCount: item.usersRatingsCount,
    }))
    .sort((a, b) => a.caseName.localeCompare(b.caseName)) ?? []

const MyAlgorithms = () => {
  const axios = useAxiosWithAuth()
  const {
    data: ollData,
    isLoading: isOllLoading,
    isError: isOllError,
  } = useQuery({
    queryKey: [OLL_ALGORITHMS_QUERY_KEY],
    queryFn: () => axios.get<AlgorithmsResponse>('/algorithms/OLL/my'),
  })

  const {
    data: pllData,
    isLoading: isPllLoading,
    isError: isPllError,
  } = useQuery({
    queryKey: [PLL_ALGORITHMS_QUERY_KEY],
    queryFn: () => axios.get<AlgorithmsResponse>('/algorithms/PLL/my'),
  })

  if (isOllLoading || isPllLoading) {
    return <Loader />
  }

  if (isOllError || isPllError) {
    return <Error />
  }

  return (
    <div className="flex h-full w-full flex-col items-center gap-4">
      <h1 className="text-2xl font-bold">My Algorithms</h1>
      <p>
        Here you can see a list of all your created algorithms, their usage
        statistics and visibility
      </p>
      <Tabs
        tabs={[
          {
            name: 'OLL',
            element: (
              <AlgorithmsTable
                algorithms={mapResponseToAlgorithms(ollData?.data)}
              />
            ),
          },
          {
            name: 'PLL',
            element: (
              <AlgorithmsTable
                algorithms={mapResponseToAlgorithms(pllData?.data)}
              />
            ),
          },
        ]}
      />
    </div>
  )
}

export default MyAlgorithms
