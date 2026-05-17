import { useEffect, useMemo, useRef, useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { useAxiosWithAuth } from '../../../lib/axios'
import Modal from '../../../components/Modal'
import { ALGORITHM_DETAILS_QUERY_KEY } from '../lib/keys'
import AlgorithmExecutionPlayer from './AlgorithmExecutionPlayer'
import {
  AnalysisResult,
  buildAnalysisPlaybackPlan,
  CostModifier,
  formatCost,
  getMoveTokens,
  mapAnalysisStepsToMoveIndices,
} from '../lib/analysis'
import { getAnalysisStepTitle } from '../lib/analysisStepText'

export interface AlgorithmAnalysisData {
  id: string
  moves: string
  setupMoves?: string | null
  case: {
    id: string
    name: string
    type: string
    defaultScramble: string
  }
  analysis?: AnalysisResult | null
}

interface AlgorithmDetailsResponse {
  algorithm: AlgorithmAnalysisData
}

interface AlgorithmAnalysisModalProps {
  isOpen: boolean
  onClose: () => void
  algorithm?: AlgorithmAnalysisData | null
  algorithmId?: string
}

const formatModifiers = (modifiers: CostModifier[], operation: 'x' | '+') => {
  if (modifiers.length === 0) {
    return 'none'
  }

  return modifiers
    .map(
      (modifier) =>
        `${operation}${formatCost(modifier.value)} ${modifier.description}`,
    )
    .join('; ')
}

const AlgorithmAnalysisModal = ({
  isOpen,
  onClose,
  algorithm,
  algorithmId,
}: AlgorithmAnalysisModalProps) => {
  const axios = useAxiosWithAuth()
  const [activeMoveIndex, setActiveMoveIndex] = useState<number | null>(null)
  const [isPlaybackPlaying, setIsPlaybackPlaying] = useState(false)
  const stepRefs = useRef<Record<number, HTMLDivElement | null>>({})

  const shouldFetchAlgorithm = isOpen && !algorithm && Boolean(algorithmId)
  const { data, isLoading, isError } = useQuery({
    queryKey: [ALGORITHM_DETAILS_QUERY_KEY, algorithmId],
    queryFn: () =>
      axios.get<AlgorithmDetailsResponse>(`/algorithms/${algorithmId}`),
    enabled: shouldFetchAlgorithm,
  })

  const resolvedAlgorithm = algorithm ?? data?.data.algorithm ?? null
  const analysis = resolvedAlgorithm?.analysis ?? null

  const mappedSteps = useMemo(
    () => mapAnalysisStepsToMoveIndices(analysis?.steps ?? []),
    [analysis],
  )

  const moveTokens = useMemo(
    () => getMoveTokens(resolvedAlgorithm?.moves ?? ''),
    [resolvedAlgorithm?.moves],
  )
  const playbackPlan = useMemo(
    () => buildAnalysisPlaybackPlan(mappedSteps.steps, moveTokens),
    [mappedSteps.steps, moveTokens],
  )
  const moveCount = playbackPlan.playbackCount

  const activeStepIndices = useMemo(() => {
    if (!isPlaybackPlaying || activeMoveIndex === null) {
      return new Set<number>()
    }

    return new Set(
      playbackPlan.steps
        .filter((step) => step.playbackIndex === activeMoveIndex)
        .map((step) => step.stepIndex),
    )
  }, [activeMoveIndex, isPlaybackPlaying, playbackPlan.steps])

  const activeMoveTokenIndex = useMemo(() => {
    if (activeMoveIndex === null) {
      return null
    }

    return playbackPlan.playbackToMoveTokenIndex[activeMoveIndex] ?? null
  }, [activeMoveIndex, playbackPlan.playbackToMoveTokenIndex])

  const firstActiveStepIndex = useMemo(() => {
    if (activeStepIndices.size === 0) {
      return null
    }

    return Math.min(...activeStepIndices)
  }, [activeStepIndices])

  useEffect(() => {
    if (!isOpen) {
      setActiveMoveIndex(null)
      setIsPlaybackPlaying(false)
      return
    }

    stepRefs.current = {}
  }, [isOpen, resolvedAlgorithm?.id])

  useEffect(() => {
    if (firstActiveStepIndex === null) {
      return
    }

    const targetStep = stepRefs.current[firstActiveStepIndex]
    if (!targetStep) {
      return
    }

    targetStep.scrollIntoView({
      block: 'center',
      behavior: 'smooth',
    })
  }, [firstActiveStepIndex])

  const title = resolvedAlgorithm
    ? `${resolvedAlgorithm.case.name} Analysis`
    : 'Algorithm Analysis'

  return (
    <Modal
      isOpen={isOpen}
      onClose={onClose}
      title={title}
      panelClassName="max-w-6xl max-h-[92vh] overflow-hidden"
      bodyClassName="h-[78vh] min-h-0 py-4"
      actions={
        <button
          onClick={onClose}
          className="cursor-pointer rounded-sm bg-gray-800 px-4 py-2 text-center text-white"
        >
          Close
        </button>
      }
    >
      <div className="h-full min-h-0">
        {isLoading && (
          <div className="py-8 text-center text-gray-600">Loading analysis...</div>
        )}
        {!isLoading && (isError || !resolvedAlgorithm) && (
          <div className="py-8 text-center text-red-700">
            Failed to load the algorithm analysis.
          </div>
        )}
        {!isLoading && !isError && resolvedAlgorithm && (
          <div className="grid h-full min-h-0 gap-4 lg:grid-cols-[minmax(22rem,26rem)_1fr]">
            <div className="flex min-h-0 flex-col gap-3">
              <AlgorithmExecutionPlayer
                scramble={resolvedAlgorithm.case.defaultScramble}
                setupMoves={resolvedAlgorithm.setupMoves}
                caseType={resolvedAlgorithm.case.type}
                moves={playbackPlan.playbackAlg}
                moveCount={moveCount}
                onMoveIndexChange={setActiveMoveIndex}
                onPlayingChange={setIsPlaybackPlaying}
              />
              <div className="rounded-sm border border-gray-200 bg-gray-50 p-3">
                <h4 className="mb-1 text-sm font-semibold">Algorithm</h4>
                <div className="flex flex-wrap gap-1 break-words font-mono text-sm leading-6">
                  {moveTokens.map((token, tokenIndex) => {
                    const isExecutingMove =
                      isPlaybackPlaying && activeMoveTokenIndex === tokenIndex

                    return (
                      <span
                        key={`${token}-${tokenIndex}`}
                        className={
                          isExecutingMove
                            ? 'rounded-sm bg-blue-100 px-1 font-semibold text-blue-800 ring-1 ring-blue-300'
                            : undefined
                        }
                      >
                        {token}
                      </span>
                    )
                  })}
                </div>
                <div className="mt-2 grid grid-cols-[auto_1fr] gap-x-2 gap-y-1 text-sm">
                  <div>Total cost:</div>
                  <div className="font-semibold">
                    {analysis ? formatCost(analysis.totalCost) : 'N/A'}
                  </div>
                  <div>Steps:</div>
                  <div className="font-semibold">
                    {analysis ? analysis.steps.length : 0}
                  </div>
                </div>
              </div>
            </div>

            <div className="flex min-h-0 flex-col">
              <div className="mb-2 text-lg font-semibold">Execution Steps</div>
              {!analysis && (
                <div className="rounded-sm border border-amber-200 bg-amber-50 px-3 py-2 text-sm text-amber-900">
                  Detailed analysis is unavailable for this algorithm.
                </div>
              )}
              {analysis && (
                <div className="flex min-h-0 flex-col gap-1.5 overflow-y-auto pr-1">
                  {playbackPlan.steps.map((mappedStep) => {
                    const isActive = activeStepIndices.has(mappedStep.stepIndex)

                    return (
                      <div
                        key={`${mappedStep.step.type}-${mappedStep.stepIndex}`}
                        ref={(element) => {
                          stepRefs.current[mappedStep.stepIndex] = element
                        }}
                        className={`rounded-sm border px-2 py-1.5 text-xs transition-colors ${
                          isActive
                            ? 'border-blue-500 bg-blue-50'
                            : 'border-gray-200 bg-white'
                        }`}
                      >
                        <div className="flex items-center justify-between gap-2">
                          <div className="font-semibold">
                            Step {mappedStep.stepIndex + 1}
                          </div>
                          <div className="font-mono text-[11px]">
                            {formatCost(mappedStep.step.cost.totalCost)}
                          </div>
                        </div>
                        <div className="mt-0.5 text-sm leading-tight">
                          {getAnalysisStepTitle(mappedStep.step)}
                        </div>
                        <div className="mt-1 grid grid-cols-[auto_1fr] gap-x-2 gap-y-0.5 text-[11px] text-gray-600">
                          <div>Base:</div>
                          <div className="font-mono">
                            {formatCost(mappedStep.step.cost.baseCost)}
                          </div>
                          <div>Multipliers:</div>
                          <div className="font-mono">
                            {formatModifiers(mappedStep.step.cost.multipliers, 'x')}
                          </div>
                          <div>Penalties:</div>
                          <div className="font-mono">
                            {formatModifiers(mappedStep.step.cost.penalties, '+')}
                          </div>
                        </div>
                        {isActive && isPlaybackPlaying && (
                          <div className="mt-1 text-[11px] font-semibold text-blue-700">
                            Executing
                          </div>
                        )}
                      </div>
                    )
                  })}
                </div>
              )}
            </div>
          </div>
        )}
      </div>
    </Modal>
  )
}

export default AlgorithmAnalysisModal
