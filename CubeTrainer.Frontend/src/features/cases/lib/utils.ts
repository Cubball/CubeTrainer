import { STATUS } from './types'

export const getHumanReadableStatus = (status: string) => {
  switch (status) {
    case STATUS.LEARNED:
      return 'Learned'
    case STATUS.IN_PROGRESS:
      return 'In Progress'
    default:
      return 'Not Learned'
  }
}
