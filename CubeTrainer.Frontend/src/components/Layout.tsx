import { NavLink, Outlet, useNavigate } from 'react-router'
import { useAxiosWithAuth } from '../lib/axios'
import { useMutation, useQuery } from '@tanstack/react-query'
import Loader from './Loader'
import Error from './Error'
import logo from '../assets/logo-white.png'
import { useState } from 'react'

const PROFILE_QUERY_KEY = 'profile'

const Layout = () => {
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false)
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
      <header className="fixed z-30 flex h-16 w-full items-center justify-between bg-gray-900 px-4 text-white md:px-16">
        <NavLink to="/">
          <div>
            <img src={logo} className="h-12" />
          </div>
        </NavLink>

        <button
          className="flex h-10 w-10 flex-col items-center justify-center md:hidden"
          onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)}
        >
          <span className="mb-1.5 block h-0.5 w-6 bg-white"></span>
          <span className="mb-1.5 block h-0.5 w-6 bg-white"></span>
          <span className="block h-0.5 w-6 bg-white"></span>
        </button>

        <div className="hidden gap-8 md:flex">
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
          <button
            className="cursor-pointer hover:underline"
            onClick={() => mutate()}
          >
            Log out
          </button>
        </div>
      </header>

      {isMobileMenuOpen && (
        <div className="fixed top-16 left-0 z-20 w-full bg-gray-900 md:hidden">
          <div className="flex flex-col">
            <NavLink
              to="cases"
              className="border-b border-gray-700 px-4 py-4 text-white hover:bg-gray-800"
              onClick={() => setIsMobileMenuOpen(false)}
            >
              My Cases
            </NavLink>
            <NavLink
              to="training-plans"
              className="border-b border-gray-700 px-4 py-4 text-white hover:bg-gray-800"
              onClick={() => setIsMobileMenuOpen(false)}
            >
              My Training Plans
            </NavLink>
            <NavLink
              to="algorithms"
              className="border-b border-gray-700 px-4 py-4 text-white hover:bg-gray-800"
              onClick={() => setIsMobileMenuOpen(false)}
            >
              My Algorithms
            </NavLink>
            <NavLink
              to="profile"
              className="border-b border-gray-700 px-4 py-4 text-white hover:bg-gray-800"
              onClick={() => setIsMobileMenuOpen(false)}
            >
              My Profile
            </NavLink>
            <button
              className="px-4 py-4 text-left text-white hover:bg-gray-800"
              onClick={() => {
                setIsMobileMenuOpen(false)
                mutate()
              }}
            >
              Log out
            </button>
          </div>
        </div>
      )}

      <div className="flex flex-1 px-4 pt-24 pb-8 text-gray-800 md:px-16">
        <Outlet />
      </div>
    </div>
  )
}

export default Layout
