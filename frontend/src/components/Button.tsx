import type { ButtonHTMLAttributes, PropsWithChildren } from 'react'

type ButtonProps = PropsWithChildren<
  ButtonHTMLAttributes<HTMLButtonElement> & {
    variant?: 'primary' | 'secondary' | 'danger'
    isLoading?: boolean
  }
>

export function Button({
  children,
  className = '',
  disabled,
  isLoading = false,
  variant = 'primary',
  ...buttonProps
}: ButtonProps) {
  return (
    <button
      {...buttonProps}
      className={`button button--${variant} ${className}`.trim()}
      disabled={disabled || isLoading}
      aria-busy={isLoading || undefined}
    >
      {isLoading ? 'Working…' : children}
    </button>
  )
}
