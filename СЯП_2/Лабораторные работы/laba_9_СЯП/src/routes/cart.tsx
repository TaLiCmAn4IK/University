import { createFileRoute, redirect } from '@tanstack/react-router';
import CartList from '../components/CartList';
import { isAuthenticated } from '../lib/auth.storage';

export const Route = createFileRoute('/cart')({
  beforeLoad: () => {
    if (!isAuthenticated()) {
      throw redirect({ to: '/login' });
    }
  },
  component: CartList,
});
