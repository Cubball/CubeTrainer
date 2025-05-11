import { useState } from 'react'
import { Link } from 'react-router'
import Modal from '../../../components/Modal'

interface TrainingPlan {
  id: string
  name: string
}

export interface TrainingPlansTableProps {
  trainingPlans: TrainingPlan[]
  onTrainingPlanDelete?: (planId: string) => void
}

const TrainingPlansTable = ({
  trainingPlans,
  onTrainingPlanDelete,
}: TrainingPlansTableProps) => {
  const [isDeleteModalOpen, setIsDeleteModalOpen] = useState(false)
  const [plan, setPlan] = useState<TrainingPlan | null>(null)

  const onDeleteClick = (plan: TrainingPlan) => {
    setPlan(plan)
    setIsDeleteModalOpen(true)
  }

  const handleDeleteConfirm = () => {
    setIsDeleteModalOpen(false)
    if (plan && onTrainingPlanDelete) {
      // TODO: check for capture
      onTrainingPlanDelete?.(plan.id)
    }
  }

  return (
    <>
      <table className="border-2 border-gray-800 md:min-w-lg">
        <thead>
          <tr className="bg-gray-800 text-white *:p-2">
            <th>Name</th>
            <th colSpan={2}>Actions</th>
          </tr>
        </thead>
        <tbody>
          {trainingPlans.map((plan) => (
            <tr
              key={plan.id}
              className="border-t-2 border-t-gray-800 text-center *:px-4 *:py-2 md:*:px-6 lg:*:px-8"
            >
              <td>
                <Link to={plan.id}>{plan.name}</Link>
              </td>
              <td className="flex justify-center gap-2">
                <Link
                  to={`${plan.id}/train`}
                  className="cursor-pointer rounded-sm bg-gray-800 px-3 py-1 text-white"
                >
                  Train
                </Link>
                <button
                  onClick={() => onDeleteClick(plan)}
                  className="cursor-pointer rounded-sm bg-red-700 px-3 py-1 text-white"
                >
                  Delete
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
      {plan && (
        <Modal
          isOpen={isDeleteModalOpen}
          onClose={() => setIsDeleteModalOpen(false)}
          title="Confirm Deletion"
          actions={
            <>
              <button
                onClick={() => setIsDeleteModalOpen(false)}
                className="cursor-pointer rounded-sm border px-4 py-2 text-center"
              >
                Cancel
              </button>
              <button
                onClick={handleDeleteConfirm}
                className="cursor-pointer rounded-sm bg-red-700 px-4 py-2 text-center text-white"
              >
                Delete
              </button>
            </>
          }
        >
          <p>
            Are you sure you want to delete training plan "{plan.name}"? This
            action cannot be undone.
          </p>
        </Modal>
      )}
    </>
  )
}

export default TrainingPlansTable
