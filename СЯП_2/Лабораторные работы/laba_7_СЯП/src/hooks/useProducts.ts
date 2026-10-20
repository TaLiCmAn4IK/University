import { useContext } from 'react';
import { ProductContext } from '../contexts/ProductContext';

export function useProducts() {
  const context = useContext(ProductContext);
  if (!context) {
    throw new Error('useProducts должен использоваться внутри ProductProvider');
  }
  return context;
}
