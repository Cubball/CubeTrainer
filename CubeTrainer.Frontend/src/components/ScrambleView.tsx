import { TwistyPlayer } from 'cubing/twisty'
import type { TwistyPlayerConfig } from 'cubing/twisty'
import { useEffect, useRef } from 'react'

type StickeringMask = NonNullable<
  Exclude<TwistyPlayerConfig['experimentalStickeringMaskOrbits'], string>
>

// Custom OLL stickering mask targeting the D-layer pieces (indices 4-7).
// The standard "OLL" stickering targets U-layer pieces (0-3), but we use
// z2 to flip yellow on top, so the D-layer pieces are the visible ones.
const OLL_STICKERING_MASK: StickeringMask = {
  orbits: {
    EDGES: {
      pieces: [
        // U-layer edges (0-3): Dim
        { facelets: ['dim', 'dim'] },
        { facelets: ['dim', 'dim'] },
        { facelets: ['dim', 'dim'] },
        { facelets: ['dim', 'dim'] },
        // D-layer edges (4-7): show primary facelet only
        { facelets: ['regular', 'ignored'] },
        { facelets: ['regular', 'ignored'] },
        { facelets: ['regular', 'ignored'] },
        { facelets: ['regular', 'ignored'] },
        // E-layer edges (8-11): Dim
        { facelets: ['dim', 'dim'] },
        { facelets: ['dim', 'dim'] },
        { facelets: ['dim', 'dim'] },
        { facelets: ['dim', 'dim'] },
      ],
    },
    CORNERS: {
      pieces: [
        // U-layer corners (0-3): Dim
        { facelets: ['dim', 'dim', 'dim'] },
        { facelets: ['dim', 'dim', 'dim'] },
        { facelets: ['dim', 'dim', 'dim'] },
        { facelets: ['dim', 'dim', 'dim'] },
        // D-layer corners (4-7): show primary facelet only
        { facelets: ['regular', 'ignored', 'ignored'] },
        { facelets: ['regular', 'ignored', 'ignored'] },
        { facelets: ['regular', 'ignored', 'ignored'] },
        { facelets: ['regular', 'ignored', 'ignored'] },
      ],
    },
    CENTERS: {
      pieces: [
        // U center (0): Dim
        { facelets: ['dim', 'dim', 'dim', 'dim'] },
        // Side centers (1-4): Dim
        { facelets: ['dim', 'dim', 'dim', 'dim'] },
        { facelets: ['dim', 'dim', 'dim', 'dim'] },
        { facelets: ['dim', 'dim', 'dim', 'dim'] },
        { facelets: ['dim', 'dim', 'dim', 'dim'] },
        // D center (5): Regular
        { facelets: ['regular', 'regular', 'regular', 'regular'] },
      ],
    },
  },
}

export interface ScrambleViewProps {
  scramble: string
  setupMoves?: string | null
  forceAspectSquare?: boolean // I'm too stupid to figure out how to properly force it to behave like it should
  caseType?: string
}

const ScrambleView = ({
  scramble,
  setupMoves,
  forceAspectSquare,
  caseType,
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
      ...(caseType === 'OLL' && {
        experimentalStickeringMaskOrbits: OLL_STICKERING_MASK,
      }),
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
  }, [fullScramble, caseType])

  return (
    <div
      ref={containerRef}
      className={`max-w-full ${forceAspectSquare ? 'aspect-square' : ''}`}
    />
  )
}

export default ScrambleView
