import { Link } from 'react-router'

export function CreateVehiclePage() {
  return (
    <section className="placeholder-page" aria-labelledby="create-vehicle-heading">
      <p className="eyebrow">Vehicles</p>
      <h1 id="create-vehicle-heading">Add vehicle</h1>
      <p>
        The create form will be added in the next milestone. This route already
        gives that task a stable, refresh-safe URL.
      </p>
      <Link className="button button--secondary" to="/">
        Back to vehicle inventory
      </Link>
    </section>
  )
}
