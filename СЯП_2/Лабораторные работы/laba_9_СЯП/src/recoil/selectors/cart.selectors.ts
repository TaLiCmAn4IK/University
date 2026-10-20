import { selector } from 'recoil';
import { cartState, type CartItem } from '../atoms/cart.atoms';
import { productsCacheState } from '../atoms/productsCache.atoms';
import type { ProductView } from '../../types/product.types';

export interface CartProductRow extends ProductView {
  quantity: number;
  subtotal: number;
}

export const cartCountState = selector<number>({
  key: 'cartCountState',
  get: ({ get }) => {
    const cart = get(cartState);
    return cart.reduce((sum, item) => sum + item.quantity, 0);
  },
});

export const cartProductsSelector = selector<CartProductRow[]>({
  key: 'cartProductsSelector',
  get: ({ get }) => {
    const cart: CartItem[] = get(cartState);
    const products: ProductView[] = get(productsCacheState);

    return cart
      .map<CartProductRow | null>((item) => {
        const product = products.find((p) => p.id === item.id);
        if (!product) return null;
        return {
          ...product,
          quantity: item.quantity,
          subtotal: product.price * item.quantity,
        };
      })
      .filter((row): row is CartProductRow => row !== null);
  },
});

export const cartTotalState = selector<number>({
  key: 'cartTotalState',
  get: ({ get }) => {
    const rows = get(cartProductsSelector);
    return rows.reduce((sum, row) => sum + row.subtotal, 0);
  },
});
