import { createRootRoute, Outlet } from '@tanstack/react-router';
import Navbar from '../components/Navbar';

export const Route = createRootRoute({
  component: RootLayout,
});

function RootLayout() {
  return (
    <div className="app">
      <Navbar />
      <main className="main-content">
        <Outlet />
      </main>
    </div>
  );
}
