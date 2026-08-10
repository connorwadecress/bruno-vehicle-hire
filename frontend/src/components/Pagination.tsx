import { Button } from './Button'

interface PaginationProps {
  pageNumber: number
  totalPages: number
  totalCount: number
  onPrevious: () => void
  onNext: () => void
}

export function Pagination({
  onNext,
  onPrevious,
  pageNumber,
  totalCount,
  totalPages,
}: PaginationProps) {
  const visibleTotalPages = Math.max(1, totalPages)

  return (
    <nav className="pagination" aria-label="Vehicle list pages">
      <p className="pagination__summary">
        {totalCount} {totalCount === 1 ? 'vehicle' : 'vehicles'} · Page {pageNumber} of {visibleTotalPages}
      </p>
      <div className="pagination__actions">
        <Button type="button" variant="secondary" onClick={onPrevious} disabled={pageNumber <= 1}>
          Previous
        </Button>
        <Button
          type="button"
          variant="secondary"
          onClick={onNext}
          disabled={pageNumber >= visibleTotalPages}
        >
          Next
        </Button>
      </div>
    </nav>
  )
}
