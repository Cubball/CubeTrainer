export interface StarRatingProps {
  rating: number
  onRatingChange?: (rating: number | null) => void
  interactive?: boolean
}

const StarRating = ({
  rating,
  onRatingChange,
  interactive = false,
}: StarRatingProps) => {
  return (
    <div className="flex items-center">
      {[1, 2, 3, 4, 5].map((star) => {
        const isFullStar = star <= Math.floor(rating)
        const hasPartialStar = !isFullStar && star === Math.ceil(rating)
        const partialFill = hasPartialStar ? (rating % 1) * 100 : 0

        return (
          <button
            key={star}
            onClick={interactive ? () => onRatingChange?.(star) : undefined}
            className={`relative text-2xl ${interactive ? 'cursor-pointer' : 'cursor-default'}`}
            disabled={!interactive}
          >
            <span className="text-gray-300">★</span>
            {(isFullStar || hasPartialStar) && (
              <span
                className="absolute inset-0 overflow-hidden text-yellow-500"
                style={{
                  width: hasPartialStar ? `${partialFill}%` : '100%',
                }}
              >
                ★
              </span>
            )}
          </button>
        )
      })}
      {interactive && onRatingChange && !!rating && (
        <button
          onClick={() => onRatingChange(null)}
          className="ml-2 cursor-pointer text-sm text-gray-500 hover:text-gray-400"
        >
          Remove
        </button>
      )}
    </div>
  )
}

export default StarRating
