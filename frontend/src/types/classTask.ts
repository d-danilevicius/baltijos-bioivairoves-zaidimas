export type Season = 'Spring' | 'Summer' | 'Autumn' | 'Winter'

export type TaskType =
  'RecogniseSpeciesGroup' | 'CommonSpecies' | 'SeasonalComparison' | 'ObservationQuestion' | 'TeacherTask'

export interface ClassTask {
  id: string
  type: TaskType
  difficultyGroup: number
  label: string
}

export interface GbifTasksResponse {
  createdCount: number
}

export interface GbifTasksRequest {
  latitude: number
  longitude: number
  season: Season
}

export interface TeacherTaskRequest {
  title: string
  question: string
  correctAnswer: string
  difficultyGroup: number
  noOrganismDisturbanceConfirmed: boolean
}
