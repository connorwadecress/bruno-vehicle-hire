import { VehiclesPage } from './pages/VehiclesPage'

function App() {
  return (
    <>
      <a className="skip-link" href="#main-content">Skip to vehicle inventory</a>
      <header className="app-header">
        <div className="app-header__inner">
          <a className="brand" href="/" aria-label="Bruno Vehicle Hire home">
            Bruno Vehicle Hire
          </a>
          <p>Fleet management</p>
        </div>
      </header>
      <main id="main-content" className="app-main">
        <VehiclesPage />
      </main>
    </>
  )
}

export default App
