import { useQuery } from '@tanstack/react-query';
import { fetchProductById, fetchProducts } from '../api/products.api';
import { PRODUCTS_QUERY_KEY, productDetailKey } from '../lib/query-keys';

const STALE_TIME = 60_000;
const GC_TIME = 300_000;

export function useProductsQuery() {
  return useQuery({
    queryKey: PRODUCTS_QUERY_KEY, // уникальный индефикатор для кеширования
    queryFn: fetchProducts, // функция которая делает запрос и возвращает промис
    staleTime: STALE_TIME,
    gcTime: GC_TIME,
  });
}

export function useProductQuery(id: number) {
  return useQuery({
    queryKey: productDetailKey(id),
    queryFn: () => fetchProductById(id),
    staleTime: STALE_TIME,
    gcTime: GC_TIME,
  });
}
