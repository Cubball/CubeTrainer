import axios, { CreateAxiosDefaults } from 'axios'
import { useNavigate } from 'react-router'

const axiosConfig: CreateAxiosDefaults = {
  baseURL: import.meta.env.VITE_API_BASE_URL,
  withCredentials: true,
}

export const axiosInstance = axios.create(axiosConfig)

const authAxiosInstance = axios.create(axiosConfig)

export const useAxiosWithAuth = () => {
  const navigate = useNavigate()
  authAxiosInstance.interceptors.response.use(
    (res) => res,
    (err) => {
      if (axios.isAxiosError(err) && err.response?.status === 401) {
        navigate('/login')
      }

      return Promise.reject(err)
    },
  )
  return authAxiosInstance
}
