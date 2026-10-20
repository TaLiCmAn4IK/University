import { useEffect } from 'react';
import { useSetRecoilState } from 'recoil';
import { useProductsQuery } from '../../hooks/useProductQueries';
import { productsCacheState } from '../atoms/productsCache.atoms';

export function useSyncProductsCache() {
  const setProducts = useSetRecoilState(productsCacheState);
  const { data } = useProductsQuery();

  useEffect(() => {
    if (data) setProducts(data);
  }, [data, setProducts]);
}
