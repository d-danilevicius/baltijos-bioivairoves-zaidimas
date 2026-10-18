export interface RegisterRequest {
  email: string
  userName: string
  password: string
  role: 'Student' | 'Teacher'
}

export interface RegisterResponse201 {
  id: string
  userName: string
  role: 'Student' | 'Teacher'
}

export interface LoginRequest {
  email: string
  password: string
}

export interface LoggedInUser {
  id: string
  userName: string
  role: 'Student' | 'Teacher' | 'Administrator'
}

export interface CurrentUser {
  id: string
  userName: string
  role: LoggedInUser['role']
}

export interface ProblemDetailsWithErrors {
  type?: string
  title?: string
  status?: number
  detail?: string
  instance?: string
  errors?: Record<string, string[]>
}
