import { useRecoilValue } from 'recoil';
import { cartCountState } from '../recoil/selectors/cart.selectors';


const CartBadge = () => {
  const count = useRecoilValue(cartCountState);
  if (count === 0) return null;
  return <span className="cart-badge">{count}</span>;
};

export default CartBadge;
