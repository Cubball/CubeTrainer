import type { TwistyPlayerConfig } from 'cubing/twisty'

export type StickeringMask = NonNullable<
  Exclude<TwistyPlayerConfig['experimentalStickeringMaskOrbits'], string>
>

export const OLL_STICKERING_MASK: StickeringMask = {
  orbits: {
    EDGES: {
      pieces: [
        { facelets: ['dim', 'dim'] },
        { facelets: ['dim', 'dim'] },
        { facelets: ['dim', 'dim'] },
        { facelets: ['dim', 'dim'] },
        { facelets: ['regular', 'ignored'] },
        { facelets: ['regular', 'ignored'] },
        { facelets: ['regular', 'ignored'] },
        { facelets: ['regular', 'ignored'] },
        { facelets: ['dim', 'dim'] },
        { facelets: ['dim', 'dim'] },
        { facelets: ['dim', 'dim'] },
        { facelets: ['dim', 'dim'] },
      ],
    },
    CORNERS: {
      pieces: [
        { facelets: ['dim', 'dim', 'dim'] },
        { facelets: ['dim', 'dim', 'dim'] },
        { facelets: ['dim', 'dim', 'dim'] },
        { facelets: ['dim', 'dim', 'dim'] },
        { facelets: ['regular', 'ignored', 'ignored'] },
        { facelets: ['regular', 'ignored', 'ignored'] },
        { facelets: ['regular', 'ignored', 'ignored'] },
        { facelets: ['regular', 'ignored', 'ignored'] },
      ],
    },
    CENTERS: {
      pieces: [
        { facelets: ['dim', 'dim', 'dim', 'dim'] },
        { facelets: ['dim', 'dim', 'dim', 'dim'] },
        { facelets: ['dim', 'dim', 'dim', 'dim'] },
        { facelets: ['dim', 'dim', 'dim', 'dim'] },
        { facelets: ['dim', 'dim', 'dim', 'dim'] },
        { facelets: ['regular', 'regular', 'regular', 'regular'] },
      ],
    },
  },
}
