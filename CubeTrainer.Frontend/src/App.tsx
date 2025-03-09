import { BrowserRouter, Route, Routes } from 'react-router'
import Login from './features/auth/pages/Login'

const App = () => {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="login" element={<Login />} />
      </Routes>
    </BrowserRouter>
  )
}

export default App
