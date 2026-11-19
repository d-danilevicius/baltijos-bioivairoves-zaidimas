import type { TaskType } from './classTask'

export type StudentTaskStatus = 'NotDone' | 'Locked'

export interface StudentTask {
  id: string
  type: TaskType
  label: string
  status: StudentTaskStatus
}

export interface StudentTaskGroup {
  group: number
  isUnlocked: boolean
  tasks: StudentTask[]
}

export interface StudentTaskList {
  hasClass: boolean
  groups: StudentTaskGroup[]
  continueTaskId: string | null
}
