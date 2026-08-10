import { useEffect, useState } from 'react'
import type { PagedResult } from '../models/api'
import type { Vehicle } from '../models/vehicle'
import { deleteVehicle, getVehicles } from '../services/vehicleService'
import { getUserFacingError } from '../services/apiClient'
import { Alert } from '../components/Alert'
import { Button } from '../components/Button'
import { LoadingIndicator } from '../components/LoadingIndicator'
import { Pagination } from '../components/Pagination'
import { VehicleTable } from '../components/VehicleTable'

const pageSize = 10

export function VehiclesPage() {
  const [deletingId, setDeletingId] = useState<string>()
  const [error, setError] = useState<string>()
  const [isLoading, setIsLoading] = useState(true)
  const [page, setPage] = useState<PagedResult<Vehicle>>()
  const [pageNumber, setPageNumber] = useState(1)
  const [refreshVersion, setRefreshVersion] = useState(0)
  const [successMessage, setSuccessMessage] = useState<string>()

  useEffect(() => {
    const controller = new AbortController()

    async function loadVehicles() {
      setIsLoading(true)
      setError(undefined)

      try {
        const result = await getVehicles(pageNumber, pageSize, controller.signal)
        setPage(result)
      } catch (requestError) {
        if (requestError instanceof DOMException && requestError.name === 'AbortError') return
        setError(getUserFacingError(requestError))
      } finally {
        if (!controller.signal.aborted) setIsLoading(false)
      }
    }

    void loadVehicles()

    return () => controller.abort()
  }, [pageNumber, refreshVersion])

  function refreshPage() {
    setRefreshVersion((current) => current + 1)
  }

  async function handleDelete(vehicle: Vehicle) {
    const confirmed = window.confirm(
      `Delete ${vehicle.registrationNumber}? This vehicle will no longer appear in the active list.`,
    )

    if (!confirmed) return

    setDeletingId(vehicle.id)
    setError(undefined)
    setSuccessMessage(undefined)

    try {
      await deleteVehicle(vehicle.id)
      setSuccessMessage(`${vehicle.registrationNumber} was deleted.`)

      const isLastItemOnPage = page?.items.length === 1
      if (isLastItemOnPage && pageNumber > 1) {
        setPageNumber((current) => current - 1)
      } else {
        refreshPage()
      }
    } catch (requestError) {
      setError(getUserFacingError(requestError))
    } finally {
      setDeletingId(undefined)
    }
  }

  return (
    <section className="vehicles-page" aria-labelledby="vehicles-heading">
      <div className="page-heading">
        <div>
          <p className="eyebrow">Operations</p>
          <h1 id="vehicles-heading">Vehicle inventory</h1>
          <p className="page-heading__description">
            View active vehicles and keep the hire fleet accurate.
          </p>
        </div>
        <span className="page-heading__status">Live API list</span>
      </div>

      {successMessage ? <Alert variant="success">{successMessage}</Alert> : null}
      {error ? (
        <div className="vehicles-page__error">
          <Alert variant="error">{error}</Alert>
          <Button type="button" variant="secondary" onClick={refreshPage}>
            Try again
          </Button>
        </div>
      ) : null}

      {isLoading ? <LoadingIndicator /> : null}

      {!isLoading && !error && page?.items.length === 0 ? (
        <div className="empty-state">
          <h2>No active vehicles yet</h2>
          <p>Vehicles added to the hire fleet will appear here.</p>
        </div>
      ) : null}

      {!isLoading && !error && page && page.items.length > 0 ? (
        <>
          <VehicleTable
            vehicles={page.items}
            deletingId={deletingId}
            onDelete={handleDelete}
          />
          <Pagination
            pageNumber={page.pageNumber}
            totalPages={page.totalPages}
            totalCount={page.totalCount}
            onPrevious={() => setPageNumber((current) => Math.max(1, current - 1))}
            onNext={() => setPageNumber((current) => current + 1)}
          />
        </>
      ) : null}
    </section>
  )
}
