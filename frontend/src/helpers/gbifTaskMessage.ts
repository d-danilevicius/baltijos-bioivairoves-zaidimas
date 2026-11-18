import type { Season, TaskType } from '../types/classTask'

export const locationRequired = 'Pasirinkite vietovę.'
export const coordinatesNotNumbers = 'Platuma ir ilguma turi būti skaičiai.'
export const locationOutsideLithuania = 'Pasirinkite vietovę Lietuvos teritorijoje.'
export const seasonRequired = 'Pasirinkite metų laiką.'
export const radiusNote = 'Naudojami stebėjimai ne toliau kaip 10 km nuo vietovės.'
export const gbifLoading = 'Kraunami stebėjimai iš GBIF…'
export const gbifFailed = 'Nepavyko gauti stebėjimų iš GBIF. Bandykite dar kartą.'
export const tooFewObservations =
  'Šioje vietovėje šiuo metų laiku rasta per mažai stebėjimų. Pasirinkite kitą vietovę ar metų laiką.'
export const gbifTasksCreated = 'Užduotys sukurtos.'
export const createGbifTasksTitle = 'Užduotys iš GBIF'
export const createGbifTasksSubmit = 'Gauti stebėjimus ir sukurti užduotis'
export const createGbifTasksRetry = 'Bandyti dar kartą'
export const createFromGbif = 'Sukurti iš GBIF'
export const classTasksTitle = 'Klasės užduotys'
export const noTasksYet = 'Klasė dar neturi užduočių.'

export const seasonLabels: Record<Season, string> = {
  Spring: 'Pavasaris',
  Summer: 'Vasara',
  Autumn: 'Ruduo',
  Winter: 'Žiema',
}

export const seasonOrder: Season[] = ['Spring', 'Summer', 'Autumn', 'Winter']

export const taskTypeLabels: Record<TaskType, string> = {
  RecogniseSpeciesGroup: 'Atpažink rūšių grupę',
  CommonSpecies: 'Rask dažnos vietinės rūšies požymių',
  SeasonalComparison: 'Palygink sezoninius stebėjimus',
  ObservationQuestion: 'Atsakyk į klausimą',
}
