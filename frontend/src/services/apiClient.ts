import type { ProblemDetails } from '../models/api'
import { appConfig } from './config'

export class ApiError extends Error {
  public readonly status: number
  public readonly problem?: ProblemDetails

  constructor(
    message: string,
    status: number,
    problem?: ProblemDetails,
  ) {
    super(message)
    this.name = 'ApiError'
    this.status = status
    this.problem = problem
  }
}

export function getUserFacingError(error: unknown): string {
  return error instanceof ApiError
    ? error.message
    : 'An unexpected error occurred.'
}

function getErrorMessage(problem: ProblemDetails | undefined, status: number) {
  if (problem?.detail) return problem.detail
  if (problem?.title) return problem.title
  return `The request failed with status ${status}.`
}

export async function apiRequest<T>(
  path: string,
  options: RequestInit = {},
): Promise<T> {
  const headers = new Headers(options.headers)
  headers.set('X-API-Key', appConfig.apiKey)

  if (options.body && !headers.has('Content-Type')) {
    headers.set('Content-Type', 'application/json')
  }

  let response: Response

  try {
    response = await fetch(`${appConfig.apiBaseUrl}${path}`, {
      ...options,
      headers,
    })
  } catch (error) {
    if (error instanceof DOMException && error.name === 'AbortError') {
      throw error
    }

    throw new ApiError(
      'The API could not be reached. Confirm that it is running and trusted.',
      0,
    )
  }

  if (!response.ok) {
    const contentType = response.headers.get('content-type') ?? ''
    const problem = contentType.includes('application/problem+json')
      ? (await response.json()) as ProblemDetails
      : undefined

    throw new ApiError(
      getErrorMessage(problem, response.status),
      response.status,
      problem,
    )
  }

  if (response.status === 204) {
    return undefined as T
  }

  return await response.json() as T
}
