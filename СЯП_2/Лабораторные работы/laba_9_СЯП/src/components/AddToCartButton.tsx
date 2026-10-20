import { useRecoilCallback } from 'recoil';
import { cartState } from '../recoil/atoms/cart.atoms';

interface AddToCartButtonProps {
  productId: number;
}

const AddToCartButton = ({ productId }: AddToCartButtonProps) => {
  const addToCart = useRecoilCallback(
    ({ set, snapshot }) => async () => {
      const current = await snapshot.getPromise(cartState);
      const existing = current.find((item) => item.id === productId);
      if (existing) {
        set(
          cartState,
          current.map((item) =>
            item.id === productId ? { ...item, quantity: item.quantity + 1 } : item,
          ),
        );
      } else {
        set(cartState, [...current, { id: productId, quantity: 1 }]);
      }
    },
    [productId],
  );

  return (
    <button type="button" className="cart-button" onClick={addToCart}>
      В корзину
    </button>
  );
};

export default AddToCartButton;
