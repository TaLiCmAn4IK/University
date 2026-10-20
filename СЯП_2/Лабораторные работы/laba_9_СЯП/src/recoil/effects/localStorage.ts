import type { AtomEffect } from 'recoil';

// Загрузка
export function localStorageEffect<T>(key: string): AtomEffect<T> {
  return ({ setSelf, onSet }) => {
    const saved = localStorage.getItem(key);
    if (saved !== null) {
      try {
        setSelf(JSON.parse(saved) as T);
      } catch {
      }
    }
// Сохранение
    onSet((newValue, _oldValue, isReset) => {
      if (isReset) {
        localStorage.removeItem(key);
      } else {
        localStorage.setItem(key, JSON.stringify(newValue));
      }
    });
  };
}
