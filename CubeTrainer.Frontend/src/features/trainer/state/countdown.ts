import { create } from 'zustand'

interface CountdownState {
  isVisible: boolean
  isShrinking: boolean
  start: () => void
  startShrinking: () => void
  stop: () => void
}

export const useCountdownStore = create<CountdownState>((set) => ({
  isVisible: false,
  isShrinking: false,
  start: () => set({ isVisible: true }),
  stop: () => set({ isVisible: false, isShrinking: false }),
  startShrinking: () => set({ isShrinking: true }),
}))
