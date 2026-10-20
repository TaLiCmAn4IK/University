import type { AuthUser } from '../types/auth.types';

export const AUTH_USER_KEY = 'auth_user';

export function readAuthUser(): AuthUser | null {
  const raw = localStorage.getItem(AUTH_USER_KEY);
  if (!raw) return null;
  try {
    return JSON.parse(raw) as AuthUser;
  } catch {
    return null;
  }
}

export function isAuthenticated(): boolean {
  return readAuthUser() !== null;
}
