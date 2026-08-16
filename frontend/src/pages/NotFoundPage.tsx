import { Link } from 'react-router'

export function NotFoundPage() {
  return (
    <section className="placeholder-page" aria-labelledby="not-found-heading">
      <p className="eyebrow">Not found</p>
      <h1 id="not-found-heading">This page does not exist</h1>
      <p>Check the address or return to the vehicle inventory.</p>
      <Link className="button button--primary" to="/">
        Go to vehicle inventory
      </Link>
    </section>
  )
}
