import type { VehicleFormValues } from './vehicle'

export type VehicleFormErrors = Partial<
  Record<keyof VehicleFormValues, string>
>

export type VehicleFormMode = 'create' | 'edit'

export function validateVehicleForm(
  values: VehicleFormValues,
  mode: VehicleFormMode,
): VehicleFormErrors {
  const errors: VehicleFormErrors = {}
  const maximumYear = new Date().getUTCFullYear() + 1
  const year = Number(values.year)

  if (mode === 'create' && !values.registrationNumber.trim()) {
    errors.registrationNumber = 'Registration number is required.'
  } else if (values.registrationNumber.trim().length > 20) {
    errors.registrationNumber = 'Registration number cannot exceed 20 characters.'
  }

  if (!values.make.trim()) {
    errors.make = 'Make is required.'
  } else if (values.make.trim().length > 100) {
    errors.make = 'Make cannot exceed 100 characters.'
  }

  if (!values.model.trim()) {
    errors.model = 'Model is required.'
  } else if (values.model.trim().length > 100) {
    errors.model = 'Model cannot exceed 100 characters.'
  }

  if (!Number.isInteger(year) || year < 1900 || year > maximumYear) {
    errors.year = `Year must be between 1900 and ${maximumYear}.`
  }

  return errors
}
