import { useNavigate } from 'react-router'
import error from '../assets/error.svg'

const NotFound = () => {
  const navigate = useNavigate()
  return (
    <div className="flex h-screen w-full items-center justify-center bg-gray-200">
      <div className="flex flex-col items-center gap-4">
        <img src={error} width={100} />
        <p className="text-center text-lg text-gray-800">
          The page you are looking for does not exist
        </p>
        <button
          className="cursor-pointer rounded-sm bg-gray-800 px-4 py-2 text-white"
          onClick={() => navigate('/')}
        >
          Return to the Home Page
        </button>
      </div>
    </div>
  )
}

export default NotFound
