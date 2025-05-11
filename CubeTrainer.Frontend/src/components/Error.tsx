import { Link } from 'react-router'
import error from '../assets/error.svg'

interface ErrorProps {
  fullscreen?: boolean
}

const Error = ({ fullscreen }: ErrorProps) => {
  return (
    <div
      className={
        'flex w-full items-center justify-center ' +
        (fullscreen ? 'h-screen bg-gray-200' : '')
      }
    >
      <div className="flex flex-col items-center">
        <img src={error} width={100} />
        <p className="text-center text-lg text-gray-800">
          An unexpected error occured. Please try again later
        </p>
        {fullscreen && (
          <Link
            to="/"
            className="mt-4 cursor-pointer rounded-sm bg-gray-800 p-2 text-white"
          >
            Return to the Home Page
          </Link>
        )}
      </div>
    </div>
  )
}

export default Error
