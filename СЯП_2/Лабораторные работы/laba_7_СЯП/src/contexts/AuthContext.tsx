import {
  createContext,
  useCallback,
  useMemo,
  useState,
  type ReactNode,
} from 'react';
import type { AuthUser, StoredUser } from '../types/auth.types';
import type { IFormData } from '../schemas/registration.schema';

const AUTH_USER_KEY = 'auth_user';
const REGISTERED_USER_KEY = 'registered_user';

interface AuthContextValue {
  user: AuthUser | null;
  isAuthenticated: boolean;
  login: (email: string, password: string) => boolean;
  loginFromRegistration: (data: IFormData) => void;
  logout: () => void;
}

export const AuthContext = createContext<AuthContextValue | null>(null);

function readAuthUser(): AuthUser | null {
  const raw = localStorage.getItem(AUTH_USER_KEY);
  if (!raw) return null;
  try {
    return JSON.parse(raw) as AuthUser;
  } catch {
    return null;
  }
}

function readRegisteredUser(): StoredUser | null {
  const raw = localStorage.getItem(REGISTERED_USER_KEY);
  if (!raw) return null;
  try {
    return JSON.parse(raw) as StoredUser;
  } catch {
    return null;
  }
}

// компонент-провайдер
export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<AuthUser | null>(() => readAuthUser());

  const persistUser = useCallback((nextUser: AuthUser) => {
    localStorage.setItem(AUTH_USER_KEY, JSON.stringify(nextUser));
    setUser(nextUser);
  }, []);

  const login = useCallback((email: string, password: string): boolean => {
    const registered = readRegisteredUser();
    if (!registered || registered.email !== email || registered.password !== password) {
      return false;
    }
    const authUser: AuthUser = {
      email: registered.email,
      username: registered.username,
      city: registered.city,
      occupation: registered.occupation,
    };
    persistUser(authUser);
    return true;
  }, [persistUser]);

  const loginFromRegistration = useCallback((data: IFormData) => {
    const stored: StoredUser = {
      email: data.email,
      password: data.password,
      username: data.username,
      city: data.city,
      occupation: data.occupation,
    };
    localStorage.setItem(REGISTERED_USER_KEY, JSON.stringify(stored));

    const authUser: AuthUser = {
      email: data.email,
      username: data.username,
      city: data.city,
      occupation: data.occupation,
    };
    persistUser(authUser);
  }, [persistUser]);

  const logout = useCallback(() => {
    localStorage.removeItem(AUTH_USER_KEY);
    setUser(null);
  }, []);

  const value = useMemo(
    () => ({
      user,
      isAuthenticated: user !== null,
      login,
      loginFromRegistration,
      logout,
    }),
    [user, login, loginFromRegistration, logout],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}
