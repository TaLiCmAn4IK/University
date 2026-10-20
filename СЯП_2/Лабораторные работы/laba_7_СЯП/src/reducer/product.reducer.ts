import type { IProductState, TProductAction } from '../types/product.types';

export const initialProductState: IProductState = {
  products: [],
  isLoading: false,
  error: null,
};

export function productReducer(state: IProductState, action: TProductAction): IProductState {
  switch (action.type) {
    case 'FETCH_START':
      return { ...state, isLoading: true, error: null };

    case 'FETCH_SUCCESS':
      return { ...state, isLoading: false, products: action.products };

    case 'FETCH_ERROR':
      return { ...state, isLoading: false, error: action.message };

    case 'ADD_PRODUCT':
      return { ...state, products: [...state.products, action.product] };

    case 'UPDATE_PRODUCT':
      return {
        ...state,
        products: state.products.map((product) =>
          product.id === action.product.id ? action.product : product,
        ),
      };

    case 'REMOVE_PRODUCT':
      return {
        ...state,
        products: state.products.filter((product) => product.id !== action.id),
      };

    default:
      return state;
  }
}
