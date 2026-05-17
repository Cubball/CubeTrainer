export interface CostModifier {
  value: number
  description: string
}

export interface AnalysisStepCost {
  totalCost: number
  baseCost: number
  multipliers: CostModifier[]
  penalties: CostModifier[]
}

interface BaseAnalysisStep {
  cost: AnalysisStepCost
}

export interface MotionAnalysisStep extends BaseAnalysisStep {
  type: 'motion'
  move: string
  hand: string
  motionType: string
}

export interface RegripAnalysisStep extends BaseAnalysisStep {
  type: 'regrip'
  hand: string
  newHandOffset: string
}

export interface RotationAnalysisStep extends BaseAnalysisStep {
  type: 'rotation'
  move: string
}

export type AnalysisStep =
  | MotionAnalysisStep
  | RegripAnalysisStep
  | RotationAnalysisStep

export interface AnalysisResult {
  totalCost: number
  steps: AnalysisStep[]
}

export interface AnalysisStepWithMoveIndex {
  step: AnalysisStep
  stepIndex: number
  moveIndex: number | null
}

export interface AnalysisStepWithPlaybackIndex extends AnalysisStepWithMoveIndex {
  playbackIndex: number | null
}

export interface AnalysisPlaybackPlan {
  playbackAlg: string
  playbackCount: number
  playbackToMoveTokenIndex: Array<number | null>
  steps: AnalysisStepWithPlaybackIndex[]
}

export const formatCost = (value: number) =>
  value.toFixed(2).replace(/\.00$/, '').replace(/(\.\d)0$/, '$1')

export const getMoveTokens = (moves: string) =>
  moves
    .trim()
    .split(/\s+/)
    .filter((token) => token.length > 0)

export const mapAnalysisStepsToMoveIndices = (steps: AnalysisStep[]) => {
  let nextMoveIndex = 0
  const raw = steps.map((step, stepIndex) => {
    const moveIndex = nextMoveIndex
    if (step.type !== 'regrip') {
      nextMoveIndex += 1
    }

    return {
      step,
      stepIndex,
      moveIndex,
    }
  })

  const moveCount = nextMoveIndex
  const mappedSteps: AnalysisStepWithMoveIndex[] = raw.map((item) => ({
    ...item,
    moveIndex:
      moveCount > 0
        ? Math.min(item.moveIndex, moveCount - 1)
        : null,
  }))

  return {
    moveCount,
    steps: mappedSteps,
  }
}

export const buildAnalysisPlaybackPlan = (
  mappedSteps: AnalysisStepWithMoveIndex[],
  moveTokens: string[],
): AnalysisPlaybackPlan => {
  if (mappedSteps.length === 0) {
    return {
      playbackAlg: moveTokens.join(' '),
      playbackCount: moveTokens.length,
      playbackToMoveTokenIndex: moveTokens.map((_token, tokenIndex) => tokenIndex),
      steps: [],
    }
  }

  const playbackUnits: string[] = []
  const playbackToMoveTokenIndex: Array<number | null> = []
  const stepPlaybackIndex = new Map<number, number | null>()
  let nextMoveTokenIndex = 0

  for (const mappedStep of mappedSteps) {
    if (mappedStep.step.type === 'regrip') {
      stepPlaybackIndex.set(mappedStep.stepIndex, playbackUnits.length)
      playbackUnits.push('.')
      playbackToMoveTokenIndex.push(null)
      continue
    }

    if (nextMoveTokenIndex >= moveTokens.length) {
      stepPlaybackIndex.set(mappedStep.stepIndex, null)
      continue
    }

    stepPlaybackIndex.set(mappedStep.stepIndex, playbackUnits.length)
    playbackUnits.push(moveTokens[nextMoveTokenIndex])
    playbackToMoveTokenIndex.push(nextMoveTokenIndex)
    nextMoveTokenIndex += 1
  }

  while (nextMoveTokenIndex < moveTokens.length) {
    playbackUnits.push(moveTokens[nextMoveTokenIndex])
    playbackToMoveTokenIndex.push(nextMoveTokenIndex)
    nextMoveTokenIndex += 1
  }

  return {
    playbackAlg: playbackUnits.join(' '),
    playbackCount: playbackUnits.length,
    playbackToMoveTokenIndex,
    steps: mappedSteps.map((mappedStep) => ({
      ...mappedStep,
      playbackIndex: stepPlaybackIndex.get(mappedStep.stepIndex) ?? null,
    })),
  }
}
