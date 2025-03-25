import { TwistyPlayer } from 'cubing/twisty'
import { useEffect, useRef } from 'react'

export interface ScrambleViewProps {
  scramble: string
}

const ScrambleView = ({ scramble }: ScrambleViewProps) => {
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

    containerRef.current.innerHTML = ''
    containerRef.current.appendChild(twistyPlayer)

    return () => {
      if (containerRef.current) {
        containerRef.current.innerHTML = ''
      }
    }
  }, [scramble])

  return <div ref={containerRef} />
}

export default ScrambleView
