import type { TaskType } from './classTask'
import type { Submission } from './submission'

export interface ObservationCard {
  label: string
  datasetName: string | null
  license: string | null
  gbifUrl: string
  locality: string | null
  latitude?: number
  longitude?: number
  coordinatesHidden: boolean
}

export interface StudentTaskDetails {
  id: string
  type: TaskType
  difficultyGroup: number
  label: string
  title: string | null
  question: string
  options: string[]
  observation: ObservationCard | null
  submission: Submission | null
}
