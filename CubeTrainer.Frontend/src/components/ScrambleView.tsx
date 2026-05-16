import { TwistyPlayer } from 'cubing/twisty'
import { useEffect, useRef } from 'react'

export interface ScrambleViewProps {
  scramble: string
  setupMoves?: string | null
  forceAspectSquare?: boolean // I'm too stupid to figure out how to properly force it to behave like it should
}

const ScrambleView = ({
  scramble,
  setupMoves,
  forceAspectSquare,
}: ScrambleViewProps) => {
  const containerRef = useRef<HTMLDivElement | null>(null)
  const fullScramble = setupMoves ? `${scramble} ${setupMoves}` : scramble

  useEffect(() => {
    if (!containerRef.current) {
      return
    }

    const twistyPlayer = new TwistyPlayer({
      background: 'none',
      puzzle: '3x3x3',
      visualization: 'experimental-2D-LL',
      alg: `z2 ${fullScramble}`,
      controlPanel: 'none',
    })
    twistyPlayer.classList.add('max-w-full')
    twistyPlayer.classList.add('max-h-full')

    containerRef.current.innerHTML = ''
    containerRef.current.appendChild(twistyPlayer)

    return () => {
      if (containerRef.current) {
        containerRef.current.innerHTML = ''
      }
    }
  }, [fullScramble])

  return (
    <div
      ref={containerRef}
      className={`max-w-full ${forceAspectSquare ? 'aspect-square' : ''}`}
    />
  )
}

export default ScrambleView
