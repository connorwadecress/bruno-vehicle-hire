import type { ProblemDetails } from '../models/api'
import { appConfig } from './config'

//repository pattern by using this instead of handling it all in vehicleservice
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

  //two totally different kinds of failure below - this catch, and the !response.ok check
  //this one = the request never completed. no connection, wrong port, dodgy cert
  try {
    response = await fetch(`${appConfig.apiBaseUrl}${path}`, { //fetches with X-API-KEY header
      ...options,
      headers,
    })
  } catch (error) {
    if (error instanceof DOMException && error.name === 'AbortError') {
      throw error
    }

    throw new ApiError(
      'The API could not be reached. Confirm that it is running and trusted.',
      0,  //theres no response at all so theres no status either - thats why we pass 0
    )
  }

  //fetch only rejects if it never got an answer - a bad answer is still an answer
  //so 400/401/404/409/500 all land HERE, not in the catch above. fetch is perfectly happy
  //without this check a 409 duplicate would look like success and the create page would navigate away
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
    return undefined as T // success with no body. -> calling .json() would throw an error
    // using the cast will satisfy the compiler and return as undefined -> so we are lying to typescript here
  }

  return await response.json() as T
}
