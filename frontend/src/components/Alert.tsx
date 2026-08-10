import type { PropsWithChildren } from 'react'

type AlertProps = PropsWithChildren<{
  variant: 'error' | 'success'
}>

export function Alert({ children, variant }: AlertProps) {
  const isError = variant === 'error'

  return (
    <div
      className={`alert alert--${variant}`}
      role={isError ? 'alert' : 'status'}
      aria-live={isError ? 'assertive' : 'polite'}
    >
      {children}
    </div>
  )
}
