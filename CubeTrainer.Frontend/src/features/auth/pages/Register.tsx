import { useForm } from 'react-hook-form'
import { z } from 'zod'
import { zodResolver } from '@hookform/resolvers/zod'
import { useMutation } from '@tanstack/react-query'
import axios from 'axios'
import { axiosInstance } from '../../../lib/axios'
import loader from '../../../assets/loader.svg'
import { useNavigate } from 'react-router'
import AuthContainer from '../components/AuthContainer'

const schema = z
  .object({
    email: z.string().email('Please enter a valid email'),
    password: z.string().min(8, 'Please enter a valid password'),
    repeatPassword: z.string(),
  })
  .refine((data) => data.password === data.repeatPassword, {
    message: 'Passwords do not match',
    path: ['repeatPassword'],
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
  const navigate = useNavigate()
  const loginMutation = useMutation({
    mutationFn: (data: FormData) =>
      axiosInstance.post('/login', data, {
        params: {
          useCookies: true,
        },
      }),
    onSuccess: () => navigate('/'),
    onError: () => setError('root', { message: 'An unknown error occurred' }),
  })
  const registerMutation = useMutation({
    mutationFn: async (data: FormData) => {
      await axiosInstance.post('/register', data)
      return data
    },
    onSuccess: (data) => loginMutation.mutate(data),
    onError: (error) => {
      if (
        axios.isAxiosError(error) &&
        error.response?.status === 400 &&
        error.response.data?.errors?.DuplicateEmail
      ) {
        setError('email', { message: 'Email already taken' })
        return
      }

      setError('root', { message: 'An unknown error occurred' })
    },
  })

  return (
    <AuthContainer>
      <h1 className="mb-4 text-center text-xl font-semibold">
        Create an account
      </h1>
      <form
        onSubmit={handleSubmit((data) => registerMutation.mutate(data))}
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
        </div>
        <div>
          <input
            type="password"
            placeholder="Repeat password"
            {...register('repeatPassword')}
            className="rounded-sm border border-gray-400 p-2"
          />
          <p className="text-red-500">{errors.repeatPassword?.message}</p>
        </div>
        {registerMutation.isPending ? (
          <img src={loader} width="40" />
        ) : (
          <>
            <button className="w-full cursor-pointer rounded-sm bg-gray-800 p-2 text-white">
              Sign up
            </button>
            {errors.root && (
              <p className="text-red-500">{errors.root.message}</p>
            )}
          </>
        )}
      </form>
      <p className="text-center">
        Already have an account?
        <br />
        <button
          type="button"
          className="cursor-pointer text-gray-800 underline"
          onClick={() => navigate('login')}
        >
          Sign in
        </button>
      </p>
    </AuthContainer>
  )
}

export default Login
