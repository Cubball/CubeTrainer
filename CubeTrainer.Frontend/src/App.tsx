import { BrowserRouter, Route, Routes } from 'react-router'
import Login from './features/auth/pages/Login'
import Register from './features/auth/pages/Register'
import Layout from './components/Layout'
import NotFound from './components/NotFound'
import Trainer from './features/trainer/pages/Trainer'
import MyCases from './features/cases/pages/MyCases'
import CaseDetails from './features/cases/pages/CaseDetails'

const App = () => {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="login" element={<Login />} />
        <Route path="register" element={<Register />} />
        <Route element=<Layout />>
          <Route path="/" element={<Trainer />} />
          <Route path="cases" element={<MyCases />} />
          <Route path="cases/:id" element={<CaseDetails />} />
        </Route>
        <Route path="*" element={<NotFound />} />
      </Routes>
    </BrowserRouter>
  )
}

export default App
