import { createFileRoute, redirect } from '@tanstack/react-router';
import AuthPage from '../pages/AuthPage';
import { isAuthenticated } from '../lib/auth.storage';

export const Route = createFileRoute('/login')({
  beforeLoad: () => {
    if (isAuthenticated()) {
      throw redirect({ to: '/catalog' });
    }
  },
  component: LoginRoute,
});

function LoginRoute() {
  return <AuthPage />;
}
