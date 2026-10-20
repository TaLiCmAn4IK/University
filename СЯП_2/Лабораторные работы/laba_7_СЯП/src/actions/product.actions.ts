import type { ProductView } from '../types/product.types';
import type { TProductAction } from '../types/product.types';

export const fetchStart = (): TProductAction => ({ type: 'FETCH_START' });

export const fetchSuccess = (products: ProductView[]): TProductAction => ({
  type: 'FETCH_SUCCESS',
  products,
});

export const fetchError = (message: string): TProductAction => ({
  type: 'FETCH_ERROR',
  message,
});

export const addProduct = (product: ProductView): TProductAction => ({
  type: 'ADD_PRODUCT',
  product,
});

export const updateProductAction = (product: ProductView): TProductAction => ({
  type: 'UPDATE_PRODUCT',
  product,
});

export const removeProduct = (id: number): TProductAction => ({
  type: 'REMOVE_PRODUCT',
  id,
});
