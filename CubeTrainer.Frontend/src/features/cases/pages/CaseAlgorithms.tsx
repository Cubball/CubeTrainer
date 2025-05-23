import { useSearchParams, useParams, Link } from 'react-router'
import { useQuery } from '@tanstack/react-query'
import { useAxiosWithAuth } from '../../../lib/axios'
import { CASE_ALGORITHMS_QUERY_KEY, CASE_DETAILS_QUERY_KEY } from '../lib/keys'
import Loader from '../../../components/Loader'
import Error from '../../../components/Error'
import ScrambleView from '../../../components/ScrambleView'
import StarRating from '../../../components/StarRating'
import Pagination from '../../../components/Pagination'
import TitleWithBackButton from '../../../components/TitleWithBackButton'

interface Algorithm {
  id: string
  moves: string
  createdAt: string
  usersCount: number
  totalRating: number
  usersRatingsCount: number
}

interface AlgorithmsResponse {
  algorithms: {
    totalPages: number
    items: Algorithm[]
  }
}

interface CaseDetailsResponse {
  case: {
    id: string
    name: string
    defaultScramble: string
  }
}

type SortBy = 'rating' | 'created' | 'users'

const DEFAULT_PAGE_SIZE = 20

const CaseAlgorithms = () => {
  const { id } = useParams()
  const [searchParams, setSearchParams] = useSearchParams()
  const axios = useAxiosWithAuth()

  const page = parseInt(searchParams.get('page') || '1')
  const sortBy = (searchParams.get('sortBy') || 'users') as SortBy
  const ascending = searchParams.get('ascending') === 'true'

  const {
    data: caseData,
    isLoading: isCaseLoading,
    isError: isCaseError,
  } = useQuery({
    queryKey: [CASE_DETAILS_QUERY_KEY, id],
    queryFn: () => axios.get<CaseDetailsResponse>(`/cases/${id}`),
  })

  const {
    data: algorithmsData,
    isLoading: isAlgorithmsLoading,
    isError: isAlgorithmsError,
  } = useQuery({
    queryKey: [CASE_ALGORITHMS_QUERY_KEY, id, page, sortBy, ascending],
    queryFn: () =>
      axios.get<AlgorithmsResponse>(`/algorithms/cases/${id}`, {
        params: {
          page,
          pageSize: DEFAULT_PAGE_SIZE,
          sortBy,
          ascending,
        },
      }),
  })

  const getSortIcon = (field: SortBy) => {
    if (sortBy !== field) {
      return null
    }

    return ascending ? ' ↑' : ' ↓'
  }

  const toggleSort = (field: SortBy) => {
    if (sortBy === field) {
      updateParams({ ascending: !ascending })
    } else {
      updateParams({ sortBy: field, ascending: false })
    }
  }

  const calculateAverageRating = (algorithm: Algorithm) => {
    if (algorithm.usersRatingsCount === 0) {
      return 0
    }

    return algorithm.totalRating / algorithm.usersRatingsCount
  }

  const updateParams = (newParams: {
    page?: number
    sortBy?: SortBy
    ascending?: boolean
  }) => {
    const params = new URLSearchParams(searchParams)
    if (newParams.page !== undefined) {
      params.set('page', newParams.page.toString())
    }

    if (newParams.sortBy !== undefined) {
      params.set('sortBy', newParams.sortBy)
    }

    if (newParams.ascending !== undefined) {
      params.set('ascending', newParams.ascending.toString())
    }

    setSearchParams(params)
  }

  if (isCaseLoading || isAlgorithmsLoading) {
    return <Loader />
  }

  if (isCaseError || isAlgorithmsError) {
    return <Error />
  }

  const algorithmData = algorithmsData?.data.algorithms
  const algorithms = algorithmData?.items || []
  const totalPages = algorithmData?.totalPages || 1

  return (
    <>
      <div className="flex w-full flex-col items-center gap-4">
        <TitleWithBackButton
          title={`Algorithms for ${caseData?.data.case.name}`}
        />
        <div className="flex justify-center">
          <div className="aspect-square max-w-60">
            <ScrambleView
              scramble={caseData?.data.case.defaultScramble || ''}
              forceAspectSquare
            />
          </div>
        </div>
        <div className="max-w-7xl min-w-md md:w-3/4">
          {algorithms.length === 0 && (
            <div className="flex w-full flex-col items-center gap-4">
              <h2 className="text-center text-xl text-gray-500 italic">
                There are no algorithms for this case
              </h2>
              <Link
                to={`/cases/${caseData?.data.case.id}/algorithms/new`}
                className="w-fit cursor-pointer rounded-sm bg-gray-800 px-4 py-2 text-center text-white"
              >
                Create a New Algorithm
              </Link>
            </div>
          )}
          {algorithms.length > 0 && (
            <table className="w-full border-2 border-gray-800">
              <thead>
                <tr className="bg-gray-800 text-white *:p-2">
                  <th>Algorithm</th>
                  <th
                    onClick={() => toggleSort('users')}
                    className="cursor-pointer hover:underline"
                  >
                    Users{getSortIcon('users')}
                  </th>
                  <th
                    onClick={() => toggleSort('rating')}
                    className="cursor-pointer hover:underline"
                  >
                    Rating{getSortIcon('rating')}
                  </th>
                  <th
                    onClick={() => toggleSort('created')}
                    className="cursor-pointer hover:underline"
                  >
                    Created{getSortIcon('created')}
                  </th>
                </tr>
              </thead>
              <tbody>
                {algorithms.map((a) => (
                  <tr
                    key={a.id}
                    className="border-t-2 border-t-gray-800 text-center *:px-4 *:py-2 md:*:px-6 lg:*:px-8"
                  >
                    <td className="text-center font-mono">
                      <Link to={`/algorithms/${a.id}`}>{a.moves}</Link>
                    </td>
                    <td className="text-center">
                      <Link to={`/algorithms/${a.id}`}>{a.usersCount}</Link>
                    </td>
                    <td>
                      <Link
                        to={`/algorithms/${a.id}`}
                        className="flex items-center justify-center gap-2"
                      >
                        <StarRating rating={calculateAverageRating(a)} />
                        <span className="text-sm text-gray-500">
                          ({a.usersRatingsCount})
                        </span>
                      </Link>
                    </td>
                    <td className="text-center">
                      <Link to={`/algorithms/${a.id}`}>
                        {new Date(a.createdAt).toLocaleDateString()}
                      </Link>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </div>
        {algorithms.length > 0 && (
          <Pagination
            currentPage={page}
            totalPages={totalPages}
            onPageChange={(p) => updateParams({ page: p })}
          />
        )}
      </div>
    </>
  )
}

export default CaseAlgorithms
