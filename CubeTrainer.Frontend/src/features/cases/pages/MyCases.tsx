import CasesTable from '../components/CasesTable'
import Tabs from '../../../components/Tabs'
import { useQuery } from '@tanstack/react-query'
import { useAxiosWithAuth } from '../../../lib/axios'
import { OLL_CASES_QUERY_KEY, PLL_CASES_QUERY_KEY } from '../lib/keys'
import Loader from '../../../components/Loader'
import Error from '../../../components/Error'

interface CasesResponse {
  items: {
    id: string
    name: string
    status: string
    defaultScramble: string
    selectedAlgorithm: {
      moves: string
    }
  }[]
}

const mapResponseToCases = (response?: CasesResponse) =>
  response?.items
    .map((item) => ({
      id: item.id,
      name: item.name,
      status: item.status,
      scramble: item.defaultScramble,
      selectedAlgorithmMoves: item.selectedAlgorithm?.moves ?? '',
    }))
    .sort((a, b) => a.name.localeCompare(b.name)) ?? []

const MyCases = () => {
  const axios = useAxiosWithAuth()
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

  if (isOllLoading || isPllLoading) {
    return <Loader />
  }

  if (isOllError || isPllError) {
    return <Error />
  }

  return (
    <div className="flex h-full w-full flex-col items-center gap-4">
      <h1 className="text-2xl font-bold">My Cases</h1>
      <p>
        Here you can see a list of all your cases, their status and selected
        algorithm
      </p>
      <Tabs
        tabs={[
          {
            name: 'OLL',
            element: <CasesTable cases={mapResponseToCases(ollData?.data)} />,
          },
          {
            name: 'PLL',
            element: <CasesTable cases={mapResponseToCases(pllData?.data)} />,
          },
        ]}
      />
    </div>
  )
}

export default MyCases
