interface FormFieldProps {
  id: string
  label: string
  value: string
  error?: string
  hint?: string
  type?: 'text' | 'number'
  maxLength?: number
  min?: number
  max?: number
  disabled?: boolean
  readOnly?: boolean
  onChange: (value: string) => void
}

export function FormField({
  disabled,
  error,
  hint,
  id,
  label,
  max,
  maxLength,
  min,
  onChange,
  readOnly,
  type = 'text',
  value,
}: FormFieldProps) {
  const hintId = hint ? `${id}-hint` : undefined
  const errorId = error ? `${id}-error` : undefined
  const describedBy = [hintId, errorId].filter(Boolean).join(' ') || undefined

  return (
    <div className="form-field">
      <label htmlFor={id}>{label}</label>
      {hint ? <p id={hintId} className="form-field__hint">{hint}</p> : null}
      <input
        id={id}
        type={type}
        value={value}
        min={min}
        max={max}
        maxLength={maxLength}
        disabled={disabled}
        readOnly={readOnly}
        aria-invalid={Boolean(error) || undefined}
        aria-describedby={describedBy}
        onChange={(event) => onChange(event.target.value)}
      />
      {error ? <p id={errorId} className="form-field__error">{error}</p> : null}
    </div>
  )
}
