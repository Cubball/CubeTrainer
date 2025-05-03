export const getHumanReadableStatus = (status: string) => {
  switch (status) {
    case 'Learned':
      return 'Learned'
    case 'InProgress':
      return 'In Progress'
    default:
      return 'Not Learned'
  }
}
