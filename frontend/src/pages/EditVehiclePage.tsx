import { useEffect, useState } from 'react'
import { Link, useNavigate, useSearchParams } from 'react-router'
import { Alert } from '../components/Alert'
import { LoadingIndicator } from '../components/LoadingIndicator'
import { VehicleForm } from '../components/VehicleForm'
import type { Vehicle, VehicleFormValues } from '../models/vehicle'
import { ApiError, getUserFacingError } from '../services/apiClient'
import {
  getVehicleByRegistrationNumber,
  updateVehicle,
} from '../services/vehicleService'

function getInitialValues(vehicle: Vehicle): VehicleFormValues {
  return {
    registrationNumber: vehicle.registrationNumber,
    make: vehicle.make,
    model: vehicle.model,
    year: vehicle.year.toString(),
  }
}

export function EditVehiclePage() {
  const navigate = useNavigate()
  const [searchParams] = useSearchParams()
  const registrationNumber = searchParams.get('registrationNumber')?.trim()
  const [isLoading, setIsLoading] = useState(true)
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [loadError, setLoadError] = useState<string>()
  const [serverError, setServerError] = useState<string>()
  const [vehicle, setVehicle] = useState<Vehicle>()

  useEffect(() => {
    if (!registrationNumber) {
      setVehicle(undefined)
      setLoadError('A vehicle registration number is required to edit a vehicle.')
      setIsLoading(false)
      return
    }

    const requestedRegistrationNumber = registrationNumber
    const controller = new AbortController()

    async function loadVehicle() {
      setIsLoading(true)
      setLoadError(undefined)
      setVehicle(undefined)

      try {
        const result = await getVehicleByRegistrationNumber(
          requestedRegistrationNumber,
          controller.signal,
        )
        setVehicle(result)
      } catch (error) {
        if (error instanceof DOMException && error.name === 'AbortError') return

        setLoadError(
          error instanceof ApiError && error.status === 404
            ? `No active vehicle was found with registration number ${requestedRegistrationNumber}.`
            : getUserFacingError(error),
        )
      } finally {
        if (!controller.signal.aborted) setIsLoading(false)
      }
    }

    void loadVehicle()

    return () => controller.abort()
  }, [registrationNumber])

  async function handleSubmit(values: VehicleFormValues) {
    if (!vehicle) return

    setIsSubmitting(true)
    setServerError(undefined)

    try {
      await updateVehicle(vehicle.id, {
        make: values.make,
        model: values.model,
        year: Number(values.year),
      })
      navigate('/')
    } catch (error) {
      setServerError(getUserFacingError(error))
    } finally {
      setIsSubmitting(false)
    }
  }

  if (isLoading) return <LoadingIndicator />

  if (loadError) {
    return (
      <section className="vehicle-form-page" aria-labelledby="edit-vehicle-heading">
        <p className="eyebrow">Vehicles</p>
        <h1 id="edit-vehicle-heading">Edit vehicle</h1>
        <Alert variant="error">{loadError}</Alert>
        <Link className="button button--secondary" to="/">
          Back to vehicle inventory
        </Link>
      </section>
    )
  }

  if (!vehicle) return null

  return (
    <section className="vehicle-form-page" aria-labelledby="edit-vehicle-heading">
      <div className="page-heading">
        <div>
          <p className="eyebrow">Vehicles</p>
          <h1 id="edit-vehicle-heading">Edit vehicle</h1>
          <p className="page-heading__description">
            Update the active fleet details for {vehicle.registrationNumber}.
          </p>
        </div>
      </div>

      <div className="form-panel">
        <VehicleForm
          mode="edit"
          initialValues={getInitialValues(vehicle)}
          isSubmitting={isSubmitting}
          serverError={serverError}
          onSubmit={handleSubmit}
          onCancel={() => navigate('/')}
        />
      </div>
    </section>
  )
}
