import { BrowserRouter, Route, Routes } from 'react-router'
import Login from './features/auth/pages/Login'
import Register from './features/auth/pages/Register'
import Layout from './components/Layout'

const App = () => {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="login" element={<Login />} />
        <Route path="register" element={<Register />} />
        <Route element=<Layout />>
          <Route path="/" element={<div>test</div>} />
        </Route>
      </Routes>
    </BrowserRouter>
  )
}

export default App
