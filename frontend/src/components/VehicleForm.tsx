import {
  type FormEvent,
  useRef,
  useState,
} from 'react'
import type { VehicleFormValues } from '../models/vehicle'
import {
  type VehicleFormErrors,
  type VehicleFormMode,
  validateVehicleForm,
} from '../models/vehicleFormValidation'
import { Button } from './Button'
import { FormField } from './FormField'

interface VehicleFormProps {
  mode: VehicleFormMode
  initialValues: VehicleFormValues
  isSubmitting: boolean
  serverError?: string
  onSubmit: (values: VehicleFormValues) => Promise<void>
  onCancel: () => void
}

function createNormalisedValues(values: VehicleFormValues): VehicleFormValues {
  return {
    registrationNumber: values.registrationNumber.trim(),
    make: values.make.trim(),
    model: values.model.trim(),
    year: values.year.trim(),
  }
}

export function VehicleForm({
  initialValues,
  isSubmitting,
  mode,
  onCancel,
  onSubmit,
  serverError,
}: VehicleFormProps) {
  const [errors, setErrors] = useState<VehicleFormErrors>({})
  const [values, setValues] = useState<VehicleFormValues>(initialValues)
  const errorSummaryRef = useRef<HTMLDivElement>(null)
  const maximumYear = new Date().getUTCFullYear() + 1
  const isCreateMode = mode === 'create'

  function updateValue(field: keyof VehicleFormValues, value: string) {
    setValues((currentValues) => ({
      ...currentValues,
      [field]: value,
    }))

    setErrors((currentErrors) => {
      const nextErrors = { ...currentErrors }
      delete nextErrors[field]
      return nextErrors
    })
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()

    const normalisedValues = createNormalisedValues(values)
    const validationErrors = validateVehicleForm(normalisedValues, mode)

    if (Object.keys(validationErrors).length > 0) {
      setErrors(validationErrors)
      requestAnimationFrame(() => errorSummaryRef.current?.focus())
      return
    }

    setErrors({})
    await onSubmit(normalisedValues)
  }

  return (
    <form className="vehicle-form" noValidate onSubmit={handleSubmit}>
      {serverError ? (
        <div className="alert alert--error" role="alert">
          {serverError}
        </div>
      ) : null}

      {Object.keys(errors).length > 0 ? (
        <div
          ref={errorSummaryRef}
          className="form-error-summary"
          role="alert"
          tabIndex={-1}
        >
          <h2>Please correct the highlighted fields.</h2>
          <ul>
            {Object.entries(errors).map(([field, error]) => (
              <li key={field}>{error}</li>
            ))}
          </ul>
        </div>
      ) : null}

      <FormField
        id="registration-number"
        label="Registration number"
        value={values.registrationNumber}
        maxLength={20}
        readOnly={!isCreateMode}
        disabled={isSubmitting}
        hint={isCreateMode ? 'Maximum 20 characters.' : 'Registration number cannot be changed.'}
        error={errors.registrationNumber}
        onChange={(value) => updateValue('registrationNumber', value)}
      />
      <FormField
        id="make"
        label="Make"
        value={values.make}
        maxLength={100}
        disabled={isSubmitting}
        error={errors.make}
        onChange={(value) => updateValue('make', value)}
      />
      <FormField
        id="model"
        label="Model"
        value={values.model}
        maxLength={100}
        disabled={isSubmitting}
        error={errors.model}
        onChange={(value) => updateValue('model', value)}
      />
      <FormField
        id="year"
        label="Year"
        type="number"
        value={values.year}
        min={1900}
        max={maximumYear}
        disabled={isSubmitting}
        error={errors.year}
        onChange={(value) => updateValue('year', value)}
      />

      <div className="form-actions">
        <Button type="submit" isLoading={isSubmitting}>
          {isCreateMode ? 'Add vehicle' : 'Save changes'}
        </Button>
        <Button
          type="button"
          variant="secondary"
          disabled={isSubmitting}
          onClick={onCancel}
        >
          Cancel
        </Button>
      </div>
    </form>
  )
}
