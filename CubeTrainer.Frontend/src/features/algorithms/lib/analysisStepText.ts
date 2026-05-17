import { AnalysisStep } from './analysis'

export const ANALYSIS_HAND_TEXT: Record<string, string> = {
  Left: 'Left',
  Right: 'Right',
  left: 'Left',
  right: 'Right',
}

export const ANALYSIS_MOTION_TYPE_TEXT: Record<string, string> = {
  WristUp: 'wrist up',
  WristDown: 'wrist down',
  DoubleWristUp: 'wrist up twice',
  DoubleWristDown: 'wrist down twice',
  TripleWristUp: 'wrist up three times',
  TripleWristDown: 'wrist down three times',
  IndexPull: 'index finger flick',
  IndexPush: 'index finger push',
  DoubleIndexPull: 'index finger flick followed by middle finger flick',
  ThumbPush: 'thumb push up',
  ThumbPull: 'thumb push down',
  DoubleThumbPull: 'thumb push up twice',
  RingPull: 'ring finger flick',
  RingPush: 'ring finger push',
  DoubleRingPull: 'ring finger flick followed by pinky finger flick',
  MiddlePull: 'middle finger flick',
  MiddlePush: 'middle finger push',
  DoubleMiddlePull: 'middle finger flick twice',
}

export const ANALYSIS_HAND_OFFSET_TEXT: Record<string, string> = {
  Home: 'home grip',
  ThumbOnU: 'thumb on U face',
  ThumbOnD: 'thumb on D face',
  FlippedTop: 'flipped grip from the top',
  FlippedBottom: 'flipped grip from the bottom',
}

const getHandText = (hand: string) => ANALYSIS_HAND_TEXT[hand] ?? hand

const getMotionTypeText = (motionType: string) =>
  ANALYSIS_MOTION_TYPE_TEXT[motionType] ?? motionType

const getHandOffsetText = (handOffset: string) =>
  ANALYSIS_HAND_OFFSET_TEXT[handOffset] ?? handOffset

export const getAnalysisStepTitle = (step: AnalysisStep) => {
  if (step.type === 'motion') {
    return `${step.move} - ${getHandText(step.hand)} ${getMotionTypeText(step.motionType)}`
  }

  if (step.type === 'regrip') {
    return `${getHandText(step.hand)} hand - regrip to ${getHandOffsetText(step.newHandOffset)}`
  }

  return `${step.move} rotation`
}
