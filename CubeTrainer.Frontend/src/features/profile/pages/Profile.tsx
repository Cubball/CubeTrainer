import axios from 'axios'
import { useState } from 'react'
import { useMutation, useQuery } from '@tanstack/react-query'
import { useForm } from 'react-hook-form'
import { z } from 'zod'
import { zodResolver } from '@hookform/resolvers/zod'
import { toast } from 'react-toastify'
import { useAxiosWithAuth } from '../../../lib/axios'
import Loader from '../../../components/Loader'
import Error from '../../../components/Error'
import { PROFILE_QUERY_KEY } from '../lib/keys'

interface ProfileInfoResponse {
  email: string
}

const passwordSchema = z
  .object({
    oldPassword: z.string().min(8, 'Please enter your current password'),
    newPassword: z.string().min(8, 'Password must be at least 8 characters'),
    confirmPassword: z.string(),
  })
  .refine((data) => data.newPassword === data.confirmPassword, {
    message: "Passwords don't match",
    path: ['confirmPassword'],
  })

type PasswordFormData = z.infer<typeof passwordSchema>

const Profile = () => {
  const [isEditingPassword, setIsEditingPassword] = useState(false)
  const axiosInstance = useAxiosWithAuth()
  const {
    register,
    handleSubmit,
    reset,
    setError,
    formState: { errors },
  } = useForm<PasswordFormData>({
    resolver: zodResolver(passwordSchema),
  })

  const { data, isLoading, isError } = useQuery({
    queryKey: [PROFILE_QUERY_KEY],
    queryFn: () => axiosInstance.get<ProfileInfoResponse>('/manage/info'),
  })

  const passwordMutation = useMutation({
    mutationFn: (data: PasswordFormData) =>
      axiosInstance.post('/manage/info', data),
    onSuccess: () => {
      toast('Password changed successfully', {
        type: 'success',
        theme: 'colored',
      })
      setIsEditingPassword(false)
      reset()
    },
    onError: (error) => {
      if (
        axios.isAxiosError(error) &&
        error.response?.data.errors?.PasswordMismatch[0] ===
          'Incorrect password.'
      ) {
        setError('oldPassword', { message: 'Current password is incorrect' })
      } else {
        setError('root', { message: 'An unknown error occurred' })
      }
      toast('Failed to change password', {
        type: 'error',
        theme: 'colored',
      })
    },
  })

  const handlePasswordSubmit = (data: PasswordFormData) => {
    passwordMutation.mutate(data)
  }

  const cancelPasswordEdit = () => {
    setIsEditingPassword(false)
    reset()
  }

  if (isLoading || passwordMutation.isPending) {
    return <Loader />
  }

  if (isError) {
    return <Error />
  }

  return (
    <div className="flex w-full flex-col items-center gap-8">
      <h1 className="text-2xl font-bold">My Profile</h1>
      <div className="text-xl">Email: {data?.data?.email}</div>
      {isEditingPassword ? (
        <form
          onSubmit={handleSubmit(handlePasswordSubmit)}
          className="flex flex-col gap-2"
        >
          <div>
            <input
              id="oldPassword"
              type="password"
              placeholder="Enter current password"
              {...register('oldPassword')}
              className="w-full rounded-sm border border-gray-400 p-2"
            />
            <p className="text-red-500">{errors.oldPassword?.message}</p>
          </div>
          <div>
            <input
              id="newPassword"
              type="password"
              placeholder="Enter new password"
              {...register('newPassword')}
              className="w-full rounded-sm border border-gray-400 p-2"
            />
            <p className="text-red-500">{errors.newPassword?.message}</p>
          </div>
          <div>
            <input
              id="confirmPassword"
              type="password"
              placeholder="Confirm new password"
              {...register('confirmPassword')}
              className="w-full rounded-sm border border-gray-400 p-2"
            />
            <p className="text-red-500">{errors.confirmPassword?.message}</p>
          </div>
          <div className="mt-2 flex gap-4 *:grow">
            <button
              type="submit"
              className="cursor-pointer rounded-sm bg-gray-800 px-4 py-2 text-white"
            >
              Save Changes
            </button>

            <button
              type="button"
              onClick={cancelPasswordEdit}
              className="cursor-pointer rounded-sm border border-gray-800 px-4 py-2 text-gray-800"
            >
              Cancel
            </button>
          </div>
          {errors.root && <p className="text-red-500">{errors.root.message}</p>}
        </form>
      ) : (
        <button
          onClick={() => setIsEditingPassword(true)}
          className="cursor-pointer rounded-sm bg-gray-800 px-4 py-2 text-white"
        >
          Change Password
        </button>
      )}
    </div>
  )
}

export default Profile
