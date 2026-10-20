import { atom } from 'recoil';
import type { ProductView } from '../../types/product.types';

// Атом-кэш для товаров
export const productsCacheState = atom<ProductView[]>({
  key: 'productsCacheState',
  default: [],
});
