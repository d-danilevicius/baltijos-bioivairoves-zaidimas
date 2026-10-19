import { Route, Routes } from 'react-router-dom'
import HomePage from './pages/HomePage'
import RegisterPage from './pages/RegisterPage'
import LoginPage from './pages/LoginPage'
import ProfilePage from './pages/ProfilePage'
import ClassListPage from './pages/ClassListPage'
import NewClassPage from './pages/NewClassPage'
import ClassDetailsPage from './pages/ClassDetailsPage'
import NotFoundPage from './pages/NotFoundPage'
import ProtectedRoute from './components/protectedRoute'

export default function App() {
  return (
    <Routes>
      <Route path="/" element={<HomePage />} />
      <Route path="/register" element={<RegisterPage />} />
      <Route path="/login" element={<LoginPage />} />
      <Route element={<ProtectedRoute />}>
        <Route path="/profile" element={<ProfilePage />} />
      </Route>
      <Route element={<ProtectedRoute allowedRoles={['Teacher']} />}>
        <Route path="/classes" element={<ClassListPage />} />
        <Route path="/classes/new" element={<NewClassPage />} />
        <Route path="/classes/:id" element={<ClassDetailsPage />} />
      </Route>
      <Route path="*" element={<NotFoundPage />} />
    </Routes>
  )
}
