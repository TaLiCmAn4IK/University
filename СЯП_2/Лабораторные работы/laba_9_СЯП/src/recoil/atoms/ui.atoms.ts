// еденица состояния + вычисляемое состояние
import { atom, selector } from 'recoil';
import { localStorageEffect } from '../effects/localStorage';

export type ViewMode = 'grid' | 'list';
export type ThemeMode = 'light' | 'dark';

export interface UiSettings {
  viewMode: ViewMode;
  theme: ThemeMode;
}

export const DEFAULT_UI_SETTINGS: UiSettings = {
  viewMode: 'grid',
  theme: 'light',
};

export const uiSettingsState = atom<UiSettings>({
  key: 'uiSettingsState',
  default: DEFAULT_UI_SETTINGS,
  effects: [localStorageEffect<UiSettings>('lab9_ui_settings')],
});

export const catalogGridClassSelector = selector<string>({
  key: 'catalogGridClassSelector',
  get: ({ get }) => {
    const { viewMode } = get(uiSettingsState);
    return viewMode === 'grid' ? 'catalog-grid grid-view' : 'catalog-grid list-view';
  },
});
