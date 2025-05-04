interface PaginationProps {
  currentPage: number
  totalPages: number
  onPageChange: (page: number) => void
}

const Pagination = ({
  currentPage,
  totalPages,
  onPageChange,
}: PaginationProps) => {
  return (
    <div className="flex items-center gap-4">
      <div className="flex gap-2">
        <button
          onClick={() => onPageChange(1)}
          disabled={currentPage === 1}
          className="cursor-pointer px-2 py-1 hover:bg-gray-300 disabled:opacity-50"
        >
          &laquo;
        </button>
        <button
          onClick={() => onPageChange(currentPage - 1)}
          disabled={currentPage === 1}
          className="cursor-pointer px-2 py-1 hover:bg-gray-300 disabled:opacity-50"
        >
          &lsaquo;
        </button>
        <span className="flex items-center px-2">
          Page {currentPage} of {totalPages}
        </span>
        <button
          onClick={() => onPageChange(currentPage + 1)}
          disabled={currentPage >= totalPages}
          className="cursor-pointer px-2 py-1 hover:bg-gray-300 disabled:opacity-50"
        >
          &rsaquo;
        </button>
        <button
          onClick={() => onPageChange(totalPages)}
          disabled={currentPage >= totalPages}
          className="cursor-pointer px-2 py-1 hover:bg-gray-300 disabled:opacity-50"
        >
          &raquo;
        </button>
      </div>
    </div>
  )
}

export default Pagination
