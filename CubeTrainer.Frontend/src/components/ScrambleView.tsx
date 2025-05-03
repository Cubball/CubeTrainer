import { TwistyPlayer } from 'cubing/twisty'
import { useEffect, useRef } from 'react'

export interface ScrambleViewProps {
  scramble: string
  forceAspectSquare?: boolean // I'm too stupid to figure out how to properly force it to behave like it should
}

const ScrambleView = ({ scramble, forceAspectSquare }: ScrambleViewProps) => {
  const containerRef = useRef<HTMLDivElement | null>(null)
  useEffect(() => {
    if (!containerRef.current) {
      return
    }

    const twistyPlayer = new TwistyPlayer({
      background: 'none',
      puzzle: '3x3x3',
      visualization: 'experimental-2D-LL',
      alg: `z2 ${scramble}`,
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
  }, [scramble])

  return (
    <div
      ref={containerRef}
      className={`max-w-full ${forceAspectSquare ? 'aspect-square' : ''}`}
    />
  )
}

export default ScrambleView
