import { Link } from 'react-router'
import ScrambleView from '../../../components/ScrambleView'
import StarRating from '../../../components/StarRating'

export interface AlgorithmsTableProps {
  algorithms: {
    id: string
    caseName: string
    scramble: string
    moves: string
    isPublic: boolean
    usersCount: number
    totalRating: number
    usersRatingsCount: number
  }[]
}

const calculateAverageRating = (algorithm: {
  totalRating: number
  usersRatingsCount: number
}) => {
  if (algorithm.usersRatingsCount === 0) {
    return 0
  }

  return algorithm.totalRating / algorithm.usersRatingsCount
}

const AlgorithmsTable = ({ algorithms }: AlgorithmsTableProps) => {
  if (algorithms.length === 0) {
    return (
      <h2 className="text-center text-xl text-gray-500 italic">
        You haven't created any algorithms yet
      </h2>
    )
  }

  return (
    <table className="w-full border-2 border-gray-800">
      <thead>
        <tr className="bg-gray-800 text-white *:p-2">
          <th>Name</th>
          <th>Case</th>
          <th>Moves</th>
          <th>Users</th>
          <th>Rating</th>
          <th>Visibility</th>
        </tr>
      </thead>
      <tbody>
        {algorithms.map((a) => (
          <tr
            key={a.id}
            className="border-t-2 border-t-gray-800 text-center *:px-4 *:py-2 md:*:px-6 lg:*:px-8"
          >
            <td>
              <Link to={`/algorithms/${a.id}`}>{a.caseName}</Link>
            </td>
            <td className="max-w-24 md:max-w-36 lg:max-w-48">
              <Link to={`/algorithms/${a.id}`}>
                <ScrambleView scramble={a.scramble} forceAspectSquare />
              </Link>
            </td>
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
            <td>
              <Link to={`/algorithms/${a.id}`}>
                {a.isPublic ? 'Public' : 'Private'}
              </Link>
            </td>
          </tr>
        ))}
      </tbody>
    </table>
  )
}

export default AlgorithmsTable
