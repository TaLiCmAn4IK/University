import { atom } from 'recoil';
import { localStorageEffect } from '../effects/localStorage';

export interface CartItem {
  id: number;
  quantity: number;
}

export const cartState = atom<CartItem[]>({
  key: 'cartState',
  default: [],
  effects: [localStorageEffect<CartItem[]>('lab9_cart')],
});
