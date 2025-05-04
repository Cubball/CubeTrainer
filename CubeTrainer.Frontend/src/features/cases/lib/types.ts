export const STATUS = {
  LEARNED: 'Learned',
  IN_PROGRESS: 'InProgress',
  NOT_LEARNED: 'NotLearned',
} as const

export type CaseStatus = (typeof STATUS)[keyof typeof STATUS]

export const STATUS_OPTIONS = [
  { value: STATUS.LEARNED, label: 'Learned' },
  { value: STATUS.IN_PROGRESS, label: 'In Progress' },
  { value: STATUS.NOT_LEARNED, label: 'Not Learned' },
]
