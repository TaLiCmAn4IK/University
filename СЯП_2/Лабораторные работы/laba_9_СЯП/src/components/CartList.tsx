import { useRecoilCallback, useRecoilValue } from 'recoil';
import { cartState } from '../recoil/atoms/cart.atoms';
import {
  cartProductsSelector,
  cartTotalState,
} from '../recoil/selectors/cart.selectors';
import { useSyncProductsCache } from '../recoil/hooks/useSyncProductsCache';
import { useClearCart } from '../recoil/callbacks/useResetSettings';
import { useProductsQuery } from '../hooks/useProductQueries';
import QueryState from './QueryState';

const CartList = () => {
  useSyncProductsCache();
  const { isLoading, isError, error } = useProductsQuery();

  const rows = useRecoilValue(cartProductsSelector); //!
  const total = useRecoilValue(cartTotalState); //!
  const clearCart = useClearCart();

  const updateQuantity = useRecoilCallback( //!
    ({ set, snapshot }) =>
      async (id: number, delta: number) => {
        const current = await snapshot.getPromise(cartState);
        set(
          cartState,
          current
            .map((item) =>
              item.id === id ? { ...item, quantity: item.quantity + delta } : item,
            )
            // Если количество упало до 0 — убираем позицию.
            .filter((item) => item.quantity > 0),
        );
      },
    [],
  );

  const removeItem = useRecoilCallback(
    ({ set, snapshot }) =>
      async (id: number) => {
        const current = await snapshot.getPromise(cartState);
        set(
          cartState,
          current.filter((item) => item.id !== id),
        );
      },
    [],
  );

  return (
    <section className="cart-page">
      <header className="cart-header">
        <h1>Корзина</h1>
        {rows.length > 0 && (
          <button type="button" className="danger" onClick={clearCart}>
            Очистить всё
          </button>
        )}
      </header>

      <QueryState
        isLoading={isLoading && rows.length === 0}
        isError={isError}
        errorMessage={error?.message}
        isEmpty={rows.length === 0}
        emptyMessage="Ваша корзина пуста"
      >
        <ul className="cart-list">
          {rows.map((row) => (
            <li key={row.id} className="cart-row">
              {row.thumbnail && (
                <img src={row.thumbnail} alt={row.title} className="cart-thumb" />
              )}
              <div className="cart-row-info">
                <h3>{row.title}</h3>
                <p className="product-price">{row.price} $</p>
              </div>
              <div className="cart-row-controls">
                <button type="button" onClick={() => updateQuantity(row.id, -1)}>
                  −
                </button>
                <span className="qty">{row.quantity}</span>
                <button type="button" onClick={() => updateQuantity(row.id, +1)}>
                  +
                </button>
              </div>
              <div className="cart-row-subtotal">
                {row.subtotal.toFixed(2)} $
              </div>
              <button
                type="button"
                className="danger"
                onClick={() => removeItem(row.id)}
              >
                Удалить
              </button>
            </li>
          ))}
        </ul>

        <footer className="cart-footer">
          <strong>Итого:</strong> {total.toFixed(2)} $
        </footer>
      </QueryState>
    </section>
  );
};

export default CartList;
