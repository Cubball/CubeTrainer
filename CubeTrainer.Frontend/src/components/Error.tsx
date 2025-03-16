import error from '../assets/error.svg'

interface ErrorProps {
  fullscreen?: boolean
}

const Error = ({ fullscreen }: ErrorProps) => {
  return (
    <div
      className={
        'flex w-full items-center justify-center ' +
        (fullscreen ? 'h-screen' : '')
      }
    >
      <div className="flex flex-col items-center">
        <img src={error} width={100} />
        <p className="text-center text-lg text-gray-800">
          An unexpected error occured. Please try again later
        </p>
      </div>
    </div>
  )
}

export default Error
