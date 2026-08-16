import { Link } from 'react-router'

export function EditVehiclePage() {
  return (
    <section className="placeholder-page" aria-labelledby="edit-vehicle-heading">
      <p className="eyebrow">Vehicles</p>
      <h1 id="edit-vehicle-heading">Edit vehicle</h1>
      <p>
        The edit form will be added after the reusable form is complete. The
        route preserves the selected registration number in the URL.
      </p>
      <Link className="button button--secondary" to="/">
        Back to vehicle inventory
      </Link>
    </section>
  )
}
