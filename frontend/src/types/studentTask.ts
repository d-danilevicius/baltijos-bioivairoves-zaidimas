import type { TaskType } from './classTask'

export type StudentTaskStatus = 'NotDone' | 'Done' | 'Locked'

export interface StudentTask {
  id: string
  type: TaskType
  label: string
  status: StudentTaskStatus
  points: number | null
}

export interface StudentTaskGroup {
  group: number
  isUnlocked: boolean
  points: number
  maxPoints: number
  tasks: StudentTask[]
}

export interface StudentTaskList {
  hasClass: boolean
  groups: StudentTaskGroup[]
  continueTaskId: string | null
}
