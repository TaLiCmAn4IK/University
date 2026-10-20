// Импортируем хуки и типы React(механизм для передачи данных глубоко по дереву компонентов)
import {
  createContext,
  useCallback,
  useEffect,
  useMemo,
  useReducer,
  type ReactNode,
} from 'react';
// Импортируем API-функции для работы с сервером
import {
  createProduct as apiCreate,
  deleteProduct as apiDelete,
  fetchProducts,
  updateProduct as apiUpdate,
} from '../api/products.api';
// Импортируем action creators
import {
  addProduct,
  fetchError,
  fetchStart,
  fetchSuccess,
  removeProduct,
  updateProductAction,
} from '../actions/product.actions';
import { initialProductState, productReducer } from '../reducer/product.reducer';
import { ProductSchema } from '../schemas/product.schema';
import type { Product } from '../schemas/product.schema';
import type { ProductView } from '../types/product.types';

interface ProductContextValue {
  products: ProductView[];
  isLoading: boolean;
  error: string | null;
  loadProducts: () => Promise<void>;
  createProduct: (payload: Omit<Product, 'id'>) => Promise<void>;
  updateProduct: (id: number, payload: Omit<Product, 'id'>) => Promise<void>;
  deleteProduct: (id: number) => Promise<void>;
}

export const ProductContext = createContext<ProductContextValue | null>(null);

function validateProduct(data: Product): Product | null {
  const result = ProductSchema.safeParse(data);
  if (!result.success) {
    const flattened = result.error.flatten(); // { fieldErrors: { title: ["ошибка"] } }
    const messages = Object.entries(flattened.fieldErrors)
      .filter(([, value]) => value && value.length > 0)
      .map(([field, value]) => `${field}: ${value?.join(', ')}`);
    alert(`Ошибка валидации:\n${messages.join('\n')}`);
    return null;
  }
  return result.data;
}

export function ProductProvider({ children }: { children: ReactNode }) {
  const [state, dispatch] = useReducer(productReducer, initialProductState);

  const loadProducts = useCallback(async () => {
    dispatch(fetchStart());
    try {
      const products = await fetchProducts();
      dispatch(fetchSuccess(products));
    } catch (error) {
      const message = error instanceof Error ? error.message : 'Неизвестная ошибка';
      dispatch(fetchError(message));
    }
  }, []);

  useEffect(() => {
    void loadProducts();
  }, [loadProducts]);

  const createProduct = useCallback(async (payload: Omit<Product, 'id'>) => {
    const tempId = Date.now();
    const candidate: Product = { id: tempId, ...payload };
    const validated = validateProduct(candidate);
    if (!validated) return;

    const created = await apiCreate(payload);
    const productToAdd: ProductView = validateProduct(created) ?? { ...created };
    dispatch(addProduct(productToAdd));
  }, []);

  const updateProduct = useCallback(async (id: number, payload: Omit<Product, 'id'>) => {
    const candidate: Product = { id, ...payload };
    const validated = validateProduct(candidate);
    if (!validated) return;

    const updated = await apiUpdate(id, payload);
    const existing = state.products.find((p) => p.id === id);
    const productToUpdate: ProductView = {
      ...(validateProduct(updated) ?? updated),
      thumbnail: existing?.thumbnail,
      description: existing?.description ?? updated.title,
    };
    dispatch(updateProductAction(productToUpdate));
  }, [state.products]);

  const deleteProduct = useCallback(async (id: number) => {
    await apiDelete(id);
    dispatch(removeProduct(id));
  }, []);

  const value = useMemo(
    () => ({
      products: state.products,
      isLoading: state.isLoading,
      error: state.error,
      loadProducts,
      createProduct,
      updateProduct,
      deleteProduct,
    }),
    [state, loadProducts, createProduct, updateProduct, deleteProduct],
  );

  return <ProductContext.Provider value={value}>{children}</ProductContext.Provider>;
}
