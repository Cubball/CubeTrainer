import loader from '../assets/loader.svg'

interface LoaderProps {
  fullscreen?: boolean
}

const Loader = ({ fullscreen }: LoaderProps) => {
  return (
    <div
      className={
        'flex w-full items-center justify-center ' +
        (fullscreen ? 'h-screen' : '')
      }
    >
      <img src={loader} width={100} />
    </div>
  )
}

export default Loader
