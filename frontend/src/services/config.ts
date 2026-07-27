function requireEnvironmentValue(name: keyof ImportMetaEnv): string {
  const value = import.meta.env[name]?.trim()

  if (!value) {
    throw new Error(`Missing required environment value: ${name}`)
  }

  return value
}

const apiBaseUrl = requireEnvironmentValue('VITE_API_BASE_URL')
  .replace(/\/$/, '')

export const appConfig = {
  apiBaseUrl,
  apiKey: requireEnvironmentValue('VITE_API_KEY'),
} as const
