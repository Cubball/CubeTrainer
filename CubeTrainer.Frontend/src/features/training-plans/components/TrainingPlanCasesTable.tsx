import { Link } from 'react-router'
import ScrambleView from '../../../components/ScrambleView'

export interface Case {
  id: string
  name: string
  defaultScramble: string
  isInPlan: boolean
  solvesToLearnCount?: number
  lastDifficultyRating?: string
  lastSolved?: string
}

export interface TrainingPlanCasesTableProps {
  cases: Case[]
  onCaseAction: (caseId: string, isInPlan: boolean) => void
}

const TrainingPlanCasesTable = ({
  cases,
  onCaseAction,
}: TrainingPlanCasesTableProps) => {
  return (
    <table className="border-2 border-gray-800">
      <thead>
        <tr className="bg-gray-800 text-white *:p-2">
          <th>Name</th>
          <th>Case</th>
          <th>Solves To Learn</th>
          <th>Last Difficulty</th>
          <th>Last Solved</th>
          <th>Action</th>
        </tr>
      </thead>
      <tbody>
        {cases.map((c) => (
          <tr
            key={c.id}
            className="border-t-2 border-t-gray-800 text-center *:px-4 *:py-2 md:*:px-6 lg:*:px-8"
          >
            <td>
              <Link to={`/cases/${c.id}`}>{c.name}</Link>
            </td>
            <td className="max-w-24 md:max-w-36 lg:max-w-48">
              <Link to={`/cases/${c.id}`}>
                <ScrambleView scramble={c.defaultScramble} forceAspectSquare />
              </Link>
            </td>
            <td>
              <Link to={`/cases/${c.id}`}>{c.solvesToLearnCount ?? 'N/A'}</Link>
            </td>
            <td>
              <Link to={`/cases/${c.id}`}>
                {c.lastDifficultyRating ?? 'N/A'}
              </Link>
            </td>
            <td>
              <Link to={`/cases/${c.id}`}>
                {c.lastSolved ? new Date(c.lastSolved).toLocaleString() : 'N/A'}
              </Link>
            </td>
            <td>
              <button
                onClick={() => onCaseAction(c.id, c.isInPlan)}
                className={`cursor-pointer rounded-sm px-4 py-2 text-white ${
                  c.isInPlan ? 'bg-red-700' : 'bg-gray-800'
                }`}
              >
                {c.isInPlan ? 'Remove' : 'Add'}
              </button>
            </td>
          </tr>
        ))}
      </tbody>
    </table>
  )
}

export default TrainingPlanCasesTable
