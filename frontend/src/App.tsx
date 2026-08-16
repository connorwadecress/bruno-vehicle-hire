import { Link } from 'react-router'
import { AppRoutes } from './routes/AppRoutes'

function App() {
  return (
    <>
      <a className="skip-link" href="#main-content">Skip to vehicle inventory</a>
      <header className="app-header">
        <div className="app-header__inner">
          <Link className="brand" to="/" aria-label="Bruno Vehicle Hire home">
            Bruno Vehicle Hire
          </Link>
          <nav className="app-navigation" aria-label="Primary navigation">
            <Link to="/">Vehicle inventory</Link>
            <Link to="/vehicles/new">Add vehicle</Link>
          </nav>
        </div>
      </header>
      <main id="main-content" className="app-main">
        <AppRoutes />
      </main>
    </>
  )
}

export default App
