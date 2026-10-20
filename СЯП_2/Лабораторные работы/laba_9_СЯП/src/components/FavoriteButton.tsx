import { useRecoilCallback, useRecoilValue } from 'recoil';
import { favoritesState } from '../recoil/atoms/favorites.atoms';

interface FavoriteButtonProps {
  productId: number;
}

const FavoriteButton = ({ productId }: FavoriteButtonProps) => {
  const favorites = useRecoilValue(favoritesState);
  const isFavorite = favorites.includes(productId);

  const toggle = useRecoilCallback(
    ({ set, snapshot }) => async () => {
      const current = await snapshot.getPromise(favoritesState);
      if (current.includes(productId)) {
        set(
          favoritesState,
          current.filter((id) => id !== productId),
        );
      } else {
        set(favoritesState, [...current, productId]);
      }
    },
    [productId],
  );

  return (
    <button
      type="button"
      className={`favorite-button ${isFavorite ? 'active' : ''}`}
      aria-label={isFavorite ? 'Убрать из избранного' : 'Добавить в избранное'}
      onClick={toggle}
    >
      {isFavorite ? '♥' : '♡'}
    </button>
  );
};

export default FavoriteButton;
