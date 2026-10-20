import type { Product } from '../schemas/product.schema';

export interface ProductView extends Product {
  thumbnail?: string;
  description?: string;
}

export interface IProductState {
  products: ProductView[];
  isLoading: boolean;
  error: string | null;
}

export type TProductAction =
  | { type: 'FETCH_START' }
  | { type: 'FETCH_SUCCESS'; products: ProductView[] }
  | { type: 'FETCH_ERROR'; message: string }
  | { type: 'ADD_PRODUCT'; product: ProductView }
  | { type: 'UPDATE_PRODUCT'; product: ProductView }
  | { type: 'REMOVE_PRODUCT'; id: number };
