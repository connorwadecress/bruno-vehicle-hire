import { Route, Routes } from 'react-router'
import { CreateVehiclePage } from '../pages/CreateVehiclePage'
import { EditVehiclePage } from '../pages/EditVehiclePage'
import { NotFoundPage } from '../pages/NotFoundPage'
import { VehiclesPage } from '../pages/VehiclesPage'

export function AppRoutes() {
  return (
    <Routes>
      <Route path="/" element={<VehiclesPage />} />
      <Route path="/vehicles/new" element={<CreateVehiclePage />} />
      <Route path="/vehicles/edit" element={<EditVehiclePage />} />
      <Route path="*" element={<NotFoundPage />} />
    </Routes>
  )
}
