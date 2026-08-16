import { Link } from 'react-router'
import type { Vehicle } from '../models/vehicle'
import { Button } from './Button'

interface VehicleTableProps {
  vehicles: Vehicle[]
  deletingId?: string
  onDelete: (vehicle: Vehicle) => void
}

const createdDateFormatter = new Intl.DateTimeFormat('en-ZA', {
  dateStyle: 'medium',
})

function formatCreatedDate(value: string): string {
  const date = new Date(value)
  return Number.isNaN(date.getTime()) ? 'Unavailable' : createdDateFormatter.format(date)
}

export function VehicleTable({ deletingId, onDelete, vehicles }: VehicleTableProps) {
  return (
    <div className="table-wrapper">
      <table className="vehicle-table">
        <caption className="visually-hidden">Vehicles in the current page</caption>
        <thead>
          <tr>
            <th scope="col">Registration</th>
            <th scope="col">Make</th>
            <th scope="col">Model</th>
            <th scope="col">Year</th>
            <th scope="col">Created</th>
            <th scope="col"><span className="visually-hidden">Actions</span></th>
          </tr>
        </thead>
        <tbody>
          {vehicles.map((vehicle) => {
            const isDeleting = deletingId === vehicle.id

            return (
              <tr key={vehicle.id}>
                <td data-label="Registration">{vehicle.registrationNumber}</td>
                <td data-label="Make">{vehicle.make}</td>
                <td data-label="Model">{vehicle.model}</td>
                <td data-label="Year">{vehicle.year}</td>
                <td data-label="Created">{formatCreatedDate(vehicle.createdDate)}</td>
                <td className="vehicle-table__actions" data-label="Actions">
                  <div className="vehicle-table__action-buttons">
                    <Link
                      className="button button--secondary"
                      to={`/vehicles/edit?registrationNumber=${encodeURIComponent(vehicle.registrationNumber)}`}
                    >
                      Edit
                    </Link>
                    <Button
                      type="button"
                      variant="danger"
                      isLoading={isDeleting}
                      onClick={() => onDelete(vehicle)}
                    >
                      Delete
                    </Button>
                  </div>
                </td>
              </tr>
            )
          })}
        </tbody>
      </table>
    </div>
  )
}
