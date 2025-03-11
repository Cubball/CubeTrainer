import { useForm } from 'react-hook-form'
import { z } from 'zod'
import { zodResolver } from '@hookform/resolvers/zod'
import { useMutation } from '@tanstack/react-query'
import axios from 'axios'
import { axiosInstance } from '../../../lib/axios'
import logo from '../../../assets/logo.png'
import loader from '../../../assets/loader.svg'

const schema = z.object({
  email: z.string().email('Please enter a valid email'),
  password: z.string().min(8, 'Please enter a valid password'),
})

type FormData = z.infer<typeof schema>

const Login = () => {
  const {
    register,
    handleSubmit,
    setError,
    formState: { errors },
  } = useForm<FormData>({
    resolver: zodResolver(schema),
  })

  const mutation = useMutation({
    mutationFn: (data: FormData) =>
      axiosInstance.post('/login', data, {
        params: {
          useCookies: true,
        },
      }),
    onSuccess: () => {
      // TODO: navigate somewhere
    },
    onError: (error) => {
      if (axios.isAxiosError(error) && error.response?.status === 401) {
        setError('root', { message: 'Incorrect password' })
      } else {
        setError('root', { message: 'An unknown error occurred' })
      }
    },
  })

  // TODO: configure colors for theme?
  // TODO: extract common components
  // TODO: link for sign up page
  return (
    <div className="flex h-screen w-screen flex-col items-center justify-center bg-gray-200">
      <img src={logo} width="100" className="mb-4" />
      <div className="flex flex-col items-center gap-4 rounded-md bg-white p-8">
        <h1 className="mb-4 text-center text-xl font-semibold">
          Log in to your account
        </h1>
        <form
          onSubmit={handleSubmit((data) => mutation.mutate(data))}
          className="flex flex-col items-center gap-4"
        >
          <div>
            <input
              placeholder="Enter email"
              {...register('email')}
              className="rounded-sm border border-gray-400 p-2"
            />
            <p className="text-red-500">{errors.email?.message}</p>
          </div>
          <div>
            <input
              type="password"
              placeholder="Enter password"
              {...register('password')}
              className="rounded-sm border border-gray-400 p-2"
            />
            <p className="text-red-500">{errors.password?.message}</p>
            <p className="text-red-500">{errors.root?.message}</p>
          </div>
          {mutation.isPending ? (
            <img src={loader} width="40" />
          ) : (
            <button className="w-full cursor-pointer rounded-sm bg-gray-800 p-2 text-white">
              Login
            </button>
          )}
        </form>
      </div>
    </div>
  )
}

export default Login
