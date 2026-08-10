interface LoadingIndicatorProps {
  label?: string
}

export function LoadingIndicator({ label = 'Loading vehicles…' }: LoadingIndicatorProps) {
  return (
    <div className="loading-indicator" role="status" aria-live="polite">
      <span className="loading-indicator__spinner" aria-hidden="true" />
      <span>{label}</span>
    </div>
  )
}
