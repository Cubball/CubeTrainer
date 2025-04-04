import { NavLink, Outlet, useNavigate } from 'react-router'
import { useAxiosWithAuth } from '../lib/axios'
import { useMutation, useQuery } from '@tanstack/react-query'
import Loader from './Loader'
import Error from './Error'
import logo from '../assets/logo-white.png'

const PROFILE_QUERY_KEY = 'profile'

// TODO: responsive design
const Layout = () => {
  const axios = useAxiosWithAuth()
  const navigate = useNavigate()
  const { isLoading, isError } = useQuery({
    queryKey: [PROFILE_QUERY_KEY],
    queryFn: () => axios.get('/manage/info'),
  })
  const {
    mutate,
    isPending,
    isError: isLogoutError,
  } = useMutation({
    mutationFn: () => axios.post('/logout'),
    onSuccess: () => navigate('/login'),
  })

  if (isLoading || isPending) {
    return (
      <div className="bg-gray-200">
        <Loader fullscreen />
      </div>
    )
  }

  if (isError || isLogoutError) {
    return (
      <div className="bg-gray-200">
        <Error fullscreen />
      </div>
    )
  }

  return (
    <div className="flex min-h-screen flex-col bg-gray-200">
      <header className="fixed flex h-16 w-full items-center justify-between bg-gray-900 px-16 text-white">
        <NavLink to="/">
          <div>
            <img src={logo} className="h-12" />
          </div>
        </NavLink>
        <div className="flex gap-8">
          <NavLink to="cases" className="hover:underline">
            My Cases
          </NavLink>
          <NavLink to="training-plans" className="hover:underline">
            My Training Plans
          </NavLink>
          <NavLink to="algorithms" className="hover:underline">
            My Algorithms
          </NavLink>
          <NavLink to="profile" className="hover:underline">
            My Profile
          </NavLink>
        </div>
        <button
          className="cursor-pointer hover:underline"
          onClick={() => mutate()}
        >
          Log out
        </button>
      </header>
      <div className="flex flex-1 px-16 pt-24 pb-8 text-gray-800">
        <Outlet />
      </div>
    </div>
  )
}

export default Layout
