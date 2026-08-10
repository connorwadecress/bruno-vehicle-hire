import type { PagedResult } from '../models/api'
import type {
  CreateVehicleRequest,
  UpdateVehicleRequest,
  Vehicle,
} from '../models/vehicle'
import { apiRequest } from './apiClient'

export function getVehicles(
  pageNumber: number,
  pageSize: number,
  signal?: AbortSignal,
) {
  const query = new URLSearchParams({
    pageNumber: pageNumber.toString(),
    pageSize: pageSize.toString(),
  })

  return apiRequest<PagedResult<Vehicle>>(
    `/api/vehicles?${query}`,
    { signal },
  )
}

export function getVehicleByRegistrationNumber(
  registrationNumber: string,
  signal?: AbortSignal,
) {
  const encodedRegistrationNumber = encodeURIComponent(registrationNumber)

  return apiRequest<Vehicle>(
    `/api/vehicles/registration/${encodedRegistrationNumber}`,
    { signal },
  )
}

export function createVehicle(request: CreateVehicleRequest) {
  return apiRequest<Vehicle>('/api/vehicles', {
    method: 'POST',
    body: JSON.stringify(request),
  })
}

export function updateVehicle(id: string, request: UpdateVehicleRequest) {
  return apiRequest<Vehicle>(`/api/vehicles/${encodeURIComponent(id)}`, {
    method: 'PUT',
    body: JSON.stringify(request),
  })
}

export function deleteVehicle(id: string) {
  return apiRequest<void>(`/api/vehicles/${encodeURIComponent(id)}`, {
    method: 'DELETE',
  })
}