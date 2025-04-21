import { create } from 'zustand'

interface CountdownState {
  isVisible: boolean
  isShrinking: boolean
  show: () => void
  hide: () => void
  start: () => void
}

export const useCountdownStore = create<CountdownState>((set) => ({
  isVisible: false,
  isShrinking: false,
  show: () => set({ isVisible: true, isShrinking: false }),
  hide: () => set({ isVisible: false, isShrinking: false }),
  start: () => set({ isShrinking: true }),
}))
