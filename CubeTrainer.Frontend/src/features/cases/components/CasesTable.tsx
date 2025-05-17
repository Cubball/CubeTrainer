import { Link } from 'react-router'
import ScrambleView from '../../../components/ScrambleView'
import { getHumanReadableStatus } from '../lib/utils'
import { useState } from 'react'

export interface Case {
  id: string
  name: string
  status: string
  scramble: string
  selectedAlgorithmMoves: string
}

export interface CasesTableProps {
  cases: Case[]
}

type SortBy = 'name' | 'status'

const CasesTable = ({ cases }: CasesTableProps) => {
  const [sortBy, setSortBy] = useState<SortBy>('status')
  const [ascending, setAscending] = useState<boolean>(true)

  const getSortIcon = (field: SortBy) => {
    if (sortBy !== field) {
      return null
    }

    return ascending ? ' ↑' : ' ↓'
  }

  const toggleSort = (field: SortBy) => {
    if (sortBy === field) {
      setAscending(!ascending)
    } else {
      setSortBy(field)
      setAscending(true)
    }
  }

  const getSortedCases = (cases: Case[]) =>
    cases.sort((a, b) => {
      if (sortBy === 'name') {
        return ascending
          ? a.name.localeCompare(b.name, undefined, { numeric: true })
          : b.name.localeCompare(a.name, undefined, { numeric: true })
      } else if (sortBy === 'status') {
        return ascending
          ? a.status.localeCompare(b.status)
          : b.status.localeCompare(a.status)
      }
      return 0
    })

  return (
    <table className="border-2 border-gray-800">
      <thead>
        <tr className="bg-gray-800 text-white *:p-2">
          <th
            onClick={() => toggleSort('name')}
            className="cursor-pointer hover:underline"
          >
            Name{getSortIcon('name')}
          </th>
          <th>Case</th>
          <th>Solution</th>
          <th
            onClick={() => toggleSort('status')}
            className="cursor-pointer hover:underline"
          >
            Status{getSortIcon('status')}
          </th>
        </tr>
      </thead>
      <tbody>
        {getSortedCases(cases).map((c) => (
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
