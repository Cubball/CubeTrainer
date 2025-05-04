import { Link } from 'react-router'
import ScrambleView from '../../../components/ScrambleView'
import { getHumanReadableStatus } from '../lib/utils'

export interface CasesTableProps {
  cases: {
    id: string
    name: string
    status: string
    scramble: string
    selectedAlgorithmMoves: string
  }[]
}

const CasesTable = ({ cases }: CasesTableProps) => {
  return (
    <table className="border-2 border-gray-800">
      <thead>
        <tr className="bg-gray-800 text-white *:p-2">
          <th>Name</th>
          <th>Case</th>
          <th>Solution</th>
          <th>Status</th>
        </tr>
      </thead>
      <tbody>
        {cases.map((c) => (
          <tr
            key={c.id}
            className="border-t-2 border-t-gray-800 text-center *:px-4 *:py-2 md:*:px-6 lg:*:px-8"
          >
            <td>
              <Link to={c.id}>{c.name}</Link>
            </td>
            <td className="max-w-24 md:max-w-36 lg:max-w-48">
              <Link to={c.id}>
                <ScrambleView scramble={c.scramble} forceAspectSquare />
              </Link>
            </td>
            <td>
              <Link to={c.id}>
                {c.selectedAlgorithmMoves
                  ? c.selectedAlgorithmMoves
                  : 'Algorithm not selected'}
              </Link>
            </td>
            <td>
              <Link to={c.id}>{getHumanReadableStatus(c.status)}</Link>
            </td>
          </tr>
        ))}
      </tbody>
    </table>
  )
}

export default CasesTable
