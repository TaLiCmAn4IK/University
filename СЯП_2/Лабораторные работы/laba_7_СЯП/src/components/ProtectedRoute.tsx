import type { ReactNode } from 'react';
import { useAuth } from '../hooks/useAuth';
import AuthPage from '../pages/AuthPage';

interface ProtectedRouteProps {
  children: ReactNode;
}

const ProtectedRoute = ({ children }: ProtectedRouteProps) => {
  const { isAuthenticated } = useAuth();

  if (!isAuthenticated) {
    return <AuthPage />;
  }

  return <>{children}</>;
};

export default ProtectedRoute;
