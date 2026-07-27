export interface Vehicle {
  id: string
  registrationNumber: string
  make: string
  model: string
  year: number
  createdDate: string
}

export interface CreateVehicleRequest {
  registrationNumber: string
  make: string
  model: string
  year: number
}

export interface UpdateVehicleRequest {
  make: string
  model: string
  year: number
}

export interface VehicleFormValues {
  registrationNumber: string
  make: string
  model: string
  year: string
}
