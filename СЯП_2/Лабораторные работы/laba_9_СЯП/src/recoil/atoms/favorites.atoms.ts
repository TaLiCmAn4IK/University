import { atom } from 'recoil';
import { localStorageEffect } from '../effects/localStorage';

export const favoritesState = atom<number[]>({
  key: 'favoritesState',
  default: [],
  effects: [localStorageEffect<number[]>('lab9_favorites')],
});
