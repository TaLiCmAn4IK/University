import { useRecoilCallback } from 'recoil';
import { uiSettingsState, DEFAULT_UI_SETTINGS } from '../atoms/ui.atoms';
import { favoritesState } from '../atoms/favorites.atoms';
import { cartState } from '../atoms/cart.atoms';

export function useResetSettings() {
  return useRecoilCallback(({ set, reset }) => () => {
    set(uiSettingsState, DEFAULT_UI_SETTINGS);
    reset(favoritesState);
    reset(cartState);
  }, []);
}

export function useClearCart() {
  return useRecoilCallback(({ reset }) => () => {
    reset(cartState);
  }, []);
}

export function useClearFavorites() {
  return useRecoilCallback(({ reset }) => () => {
    reset(favoritesState);
  }, []);
}
