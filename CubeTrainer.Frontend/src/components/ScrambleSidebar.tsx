import ScrambleView from './ScrambleView'

export interface ScrambleSidebarProps {
  scramble: string
  hint?: string
  hintVisible: boolean
  setHintVisible: (visible: boolean) => void
  onRegenerateClick: () => void
}

const ScrambleSidebar = ({
  scramble,
  hint,
  hintVisible,
  setHintVisible,
  onRegenerateClick,
}: ScrambleSidebarProps) => {
  return (
    <div className="flex h-full w-full flex-col items-center justify-center gap-4">
      <p className="w-full px-4 text-center text-xl font-bold">
        Scramble:
        <br />
        {scramble}
      </p>
      <div className="h-fit">
        <ScrambleView scramble={scramble} />
      </div>
      <button
        className="w-1/2 max-w-60 cursor-pointer rounded-sm bg-gray-800 px-4 py-2 text-white"
        onClick={(e) => {
          onRegenerateClick()
          e.currentTarget.blur()
        }}
      >
        Regenerate
      </button>
      <button
        className="w-1/2 max-w-60 cursor-pointer rounded-sm bg-gray-800 px-4 py-2 text-white"
        onClick={(e) => {
          setHintVisible(!hintVisible)
          e.currentTarget.blur()
        }}
      >
        {hintVisible ? 'Hide Hint' : 'Show Hint'}
      </button>
      <p
        className={
          'mb-4 w-full px-4 text-center text-xl font-bold' +
          (hintVisible ? '' : ' opacity-0')
        }
      >
        Hint:
        <br />
        {hint ?? "You haven't seleceted an algorithm for this case"}
      </p>
    </div>
  )
}

export default ScrambleSidebar
