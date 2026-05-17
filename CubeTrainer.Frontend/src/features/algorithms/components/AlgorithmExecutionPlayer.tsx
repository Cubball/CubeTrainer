import { TwistyPlayer } from 'cubing/twisty'
import type { TwistyPlayerConfig } from 'cubing/twisty'
import { useEffect, useMemo, useRef, useState } from 'react'
import { OLL_STICKERING_MASK } from '../../../lib/twisty'

export interface AlgorithmExecutionPlayerProps {
  scramble: string
  setupMoves?: string | null
  caseType?: string
  moves: string
  moveCount: number
  onMoveIndexChange?: (moveIndex: number | null) => void
  onPlayingChange?: (isPlaying: boolean) => void
}

const TEMPO_OPTIONS = [
  { value: 0.25, label: '0.5x' },
  { value: 0.5, label: '1.0x' },
  { value: 0.75, label: '1.5x' },
  { value: 1, label: '2.0x' },
] as const
const DEFAULT_TEMPO_SCALE = 0.5

const resolveMoveIndex = (
  patternIndex: number,
  moveCount: number,
  hasCurrentMoves: boolean,
  hasStartingMoves: boolean,
) => {
  if (moveCount === 0) {
    return null
  }

  if (hasCurrentMoves || hasStartingMoves) {
    return Math.min(Math.max(patternIndex, 0), moveCount - 1)
  }

  if (patternIndex <= 0) {
    return 0
  }

  if (patternIndex >= moveCount) {
    return null
  }

  return patternIndex
}

const AlgorithmExecutionPlayer = ({
  scramble,
  setupMoves,
  caseType,
  moves,
  moveCount,
  onMoveIndexChange,
  onPlayingChange,
}: AlgorithmExecutionPlayerProps) => {
  const containerRef = useRef<HTMLDivElement | null>(null)
  const playerRef = useRef<TwistyPlayer | null>(null)
  const isAtEndRef = useRef(false)
  const [isPlaying, setIsPlaying] = useState(false)
  const [tempoScale, setTempoScale] = useState<number>(DEFAULT_TEMPO_SCALE)
  const [currentMoveIndex, setCurrentMoveIndex] = useState<number | null>(
    moveCount > 0 ? 0 : null,
  )

  const setupAlg = useMemo(
    () => ['z2', scramble, setupMoves].filter(Boolean).join(' ').trim(),
    [scramble, setupMoves],
  )

  useEffect(() => {
    if (!containerRef.current) {
      return
    }

    const config: TwistyPlayerConfig = {
      background: 'none',
      puzzle: '3x3x3',
      visualization: '3D',
      alg: moves,
      controlPanel: 'none',
      tempoScale: DEFAULT_TEMPO_SCALE,
      ...(caseType === 'OLL' && {
        experimentalStickeringMaskOrbits: OLL_STICKERING_MASK,
      }),
    }
    if (setupAlg.length > 0) {
      config.experimentalSetupAlg = setupAlg
    }

    const twistyPlayer = new TwistyPlayer(config)
    twistyPlayer.classList.add('h-72')
    twistyPlayer.classList.add('w-full')
    twistyPlayer.classList.add('max-w-full')

    containerRef.current.innerHTML = ''
    containerRef.current.appendChild(twistyPlayer)
    playerRef.current = twistyPlayer

    let isDisposed = false

    const setActiveMoveIndex = (moveIndex: number | null) => {
      if (isDisposed) {
        return
      }

      setCurrentMoveIndex(moveIndex)
      onMoveIndexChange?.(moveIndex)
    }

    const handleTimelineInfo = (timelineInfo: {
      playing: boolean
      atEnd?: boolean
    }) => {
      if (isDisposed) {
        return
      }

      const isAtEnd = Boolean(timelineInfo.atEnd)
      isAtEndRef.current = isAtEnd
      setIsPlaying(timelineInfo.playing)
      onPlayingChange?.(timelineInfo.playing)
      if (isAtEnd) {
        setActiveMoveIndex(null)
      }
    }

    const handleCurrentMoveInfo = (currentMoveInfo: {
      patternIndex: number
      currentMoves: unknown[]
      movesStarting: unknown[]
    }) => {
      if (isAtEndRef.current) {
        setActiveMoveIndex(null)
        return
      }

      setActiveMoveIndex(
        resolveMoveIndex(
          currentMoveInfo.patternIndex,
          moveCount,
          currentMoveInfo.currentMoves.length > 0,
          currentMoveInfo.movesStarting.length > 0,
        ),
      )
    }

    twistyPlayer.experimentalModel.coarseTimelineInfo.addFreshListener(
      handleTimelineInfo,
    )
    twistyPlayer.experimentalModel.currentMoveInfo.addFreshListener(
      handleCurrentMoveInfo,
    )

    twistyPlayer.jumpToStart({ flash: false })
    isAtEndRef.current = false
    setActiveMoveIndex(moveCount > 0 ? 0 : null)

    return () => {
      isDisposed = true
      twistyPlayer.experimentalModel.coarseTimelineInfo.removeFreshListener(
        handleTimelineInfo,
      )
      twistyPlayer.experimentalModel.currentMoveInfo.removeFreshListener(
        handleCurrentMoveInfo,
      )
      playerRef.current = null
      isAtEndRef.current = false
      setIsPlaying(false)
      onPlayingChange?.(false)
      onMoveIndexChange?.(null)

      if (containerRef.current) {
        containerRef.current.innerHTML = ''
      }
    }
  }, [
    caseType,
    moveCount,
    moves,
    onMoveIndexChange,
    onPlayingChange,
    setupAlg,
  ])

  useEffect(() => {
    if (!playerRef.current) {
      return
    }

    playerRef.current.tempoScale = tempoScale
  }, [tempoScale])

  const onPlayPause = () => {
    const twistyPlayer = playerRef.current
    if (!twistyPlayer || moveCount === 0) {
      return
    }

    if (isPlaying) {
      twistyPlayer.pause()
      setIsPlaying(false)
      return
    }

    if (currentMoveIndex === null || currentMoveIndex === moveCount - 1) {
      twistyPlayer.jumpToStart({ flash: false })
      isAtEndRef.current = false
    }

    twistyPlayer.play()
    setIsPlaying(true)
  }

  const onRestart = () => {
    const twistyPlayer = playerRef.current
    if (!twistyPlayer || moveCount === 0) {
      return
    }

    twistyPlayer.pause()
    setIsPlaying(false)
    twistyPlayer.jumpToStart({ flash: false })
    isAtEndRef.current = false
    setCurrentMoveIndex(0)
    onMoveIndexChange?.(0)
  }

  return (
    <div className="flex flex-col gap-3">
      <div
        ref={containerRef}
        className="h-72 w-full rounded border border-gray-200 bg-gray-50"
      />
      <div className="flex flex-wrap items-center justify-center gap-2">
        <div className="mr-2 text-sm text-gray-600">Speed</div>
        {TEMPO_OPTIONS.map((option) => (
          <button
            key={option.value}
            className={`cursor-pointer rounded-sm border px-2 py-1 text-sm ${
              tempoScale === option.value
                ? 'border-gray-800 bg-gray-800 text-white'
                : 'border-gray-300 text-gray-700'
            }`}
            onClick={() => setTempoScale(option.value)}
          >
            {option.label}
          </button>
        ))}
      </div>
      <div className="flex flex-wrap items-center justify-center gap-2">
        <button
          className="cursor-pointer rounded-sm bg-gray-800 px-3 py-1 text-sm text-white disabled:cursor-not-allowed disabled:bg-gray-500"
          onClick={onPlayPause}
          disabled={moveCount === 0}
        >
          {isPlaying ? 'Pause' : 'Play'}
        </button>
        <button
          className="cursor-pointer rounded-sm border border-gray-300 px-3 py-1 text-sm disabled:cursor-not-allowed disabled:opacity-50"
          onClick={onRestart}
          disabled={moveCount === 0}
        >
          Restart
        </button>
      </div>
      <div className="text-center text-sm text-gray-600">
        {moveCount === 0 && 'No playable moves'}
        {moveCount > 0 && currentMoveIndex !== null &&
          `Step ${Math.min(currentMoveIndex + 1, moveCount)} of ${moveCount}`}
        {moveCount > 0 && currentMoveIndex === null && 'Playback complete'}
      </div>
    </div>
  )
}

export default AlgorithmExecutionPlayer
