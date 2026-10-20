import { createRootRoute, Outlet } from '@tanstack/react-router';
import { useRecoilValue } from 'recoil';
import { useEffect } from 'react';
import { uiSettingsState } from '../recoil/atoms/ui.atoms';
import Navbar from '../components/Navbar';

export const Route = createRootRoute({
  component: RootLayout,
});

function RootLayout() {
  // Прокидываем выбранную тему в атрибут data-theme на <body>:
  // это даёт CSS-правилам точку опоры для смены палитры.
  const { theme } = useRecoilValue(uiSettingsState);

  useEffect(() => {
    document.body.dataset.theme = theme;
    return () => {
      delete document.body.dataset.theme;
    };
  }, [theme]);

  return (
    <div className="app">
      <Navbar />
      <main className="main-content">
        <Outlet />
      </main>
    </div>
  );
}
