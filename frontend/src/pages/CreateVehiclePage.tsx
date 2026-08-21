import { useState } from 'react'
import { useNavigate } from 'react-router'
import { VehicleForm } from '../components/VehicleForm'
import type { VehicleFormValues } from '../models/vehicle'
import { getUserFacingError } from '../services/apiClient'
import { createVehicle } from '../services/vehicleService'

const initialValues: VehicleFormValues = {
  registrationNumber: '',
  make: '',
  model: '',
  year: new Date().getUTCFullYear().toString(),
}

export function CreateVehiclePage() {
  const navigate = useNavigate()
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [serverError, setServerError] = useState<string>()

  async function handleSubmit(values: VehicleFormValues) {
    setIsSubmitting(true)
    setServerError(undefined)

    try {
      await createVehicle({
        registrationNumber: values.registrationNumber,
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

  return (
    <section className="vehicle-form-page" aria-labelledby="create-vehicle-heading">
      <div className="page-heading">
        <div>
          <h1 id="create-vehicle-heading">Add vehicle</h1>
        </div>
      </div>

      <div className="form-panel">
        <VehicleForm
          mode="create"
          initialValues={initialValues}
          isSubmitting={isSubmitting}
          serverError={serverError}
          onSubmit={handleSubmit}
          onCancel={() => navigate('/')}
        />
      </div>
    </section>
  )
}
