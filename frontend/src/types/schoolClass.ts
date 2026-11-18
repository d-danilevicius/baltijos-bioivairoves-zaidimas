import type { Season } from './classTask'

export interface SchoolClass {
  id: string
  name: string
  code: string
  createdAtUtc: string
  // Empty until the GBIF tasks of the class are created
  latitude?: number | null
  longitude?: number | null
  season?: Season | null
}
