import { PropsWithChildren } from 'react'
import logo from '../../../assets/logo.png'

const AuthContainer = ({ children }: PropsWithChildren) => {
  return (
    <div className="flex h-screen w-screen flex-col items-center justify-center bg-gray-200">
      <img src={logo} width="100" className="mb-4" />
      <div className="flex flex-col items-center gap-4 rounded-md bg-white p-8">
        {children}
      </div>
    </div>
  )
}

export default AuthContainer
