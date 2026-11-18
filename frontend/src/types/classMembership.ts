export interface JoinClassResponse {
  className: string
  alreadyMember: boolean
}

export interface ClassChangeConfirmation {
  currentClassName: string
  newClassName: string
}
