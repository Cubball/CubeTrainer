import { useNavigate } from 'react-router'

export interface TitleWithBackButtonProps {
  title?: string
}

const TitleWithBackButton = ({ title }: TitleWithBackButtonProps) => {
  const navigate = useNavigate()

  return (
    <div className="grid w-full max-w-7xl grid-cols-[1fr_auto_1fr] items-center">
      <button
        onClick={() => navigate(-1)}
        className="cursor-pointer text-left text-gray-800"
      >
        ← Back
      </button>
      <h1 className="text-2xl font-bold">{title}</h1>
    </div>
  )
}

export default TitleWithBackButton
