import { useMutation, useQueryClient } from '@tanstack/react-query';
import { createProduct, deleteProduct, updateProduct } from '../api/products.api';
import { PRODUCTS_QUERY_KEY, productDetailKey } from '../lib/query-keys';
import type { Product } from '../schemas/product.schema';
import type { ProductView } from '../types/product.types';


export function useCreateProductMutation() {
  const queryClient = useQueryClient();

  return useMutation({
       // Функция, которая делает запрос на сервер
    mutationFn: (payload: Omit<Product, 'id'>) => createProduct(payload),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: PRODUCTS_QUERY_KEY });
    },
  });
}

export function useUpdateProductMutation() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, payload }: { id: number; payload: Omit<Product, 'id'> }) =>
      updateProduct(id, payload),
    onSuccess: (_data, variables) => {
      void queryClient.invalidateQueries({ queryKey: PRODUCTS_QUERY_KEY });
      void queryClient.invalidateQueries({ queryKey: productDetailKey(variables.id) });
    },
  });
}

export function useDeleteProductMutation() {
  const queryClient = useQueryClient();

  return useMutation<void, Error, number, { previous?: ProductView[] }>({
    mutationFn: (id: number) => deleteProduct(id),

    // 1) Оптимистично удаляем из кэша до ответа сервера
    onMutate: async (id) => {
      await queryClient.cancelQueries({ queryKey: PRODUCTS_QUERY_KEY });
      const previous = queryClient.getQueryData<ProductView[]>(PRODUCTS_QUERY_KEY);

      queryClient.setQueryData<ProductView[]>(
        PRODUCTS_QUERY_KEY,
        (old = []) => old.filter((p) => p.id !== id),
      );

      return { previous };
    },

    // 2) Откат при ошибке
    onError: (_err, _id, context) => {
      if (context?.previous) {
        queryClient.setQueryData(PRODUCTS_QUERY_KEY, context.previous);
      }
    },

    // 3) Финальная синхронизация
    onSettled: () => {
      void queryClient.invalidateQueries({ queryKey: PRODUCTS_QUERY_KEY });
    },
  });
}