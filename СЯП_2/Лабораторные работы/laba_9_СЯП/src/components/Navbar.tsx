import { Link, useNavigate } from '@tanstack/react-router';
import { useAuth } from '../hooks/useAuth';
import { useResetSettings } from '../recoil/callbacks/useResetSettings';
import CartBadge from './CartBadge';

const Navbar = () => {
  const navigate = useNavigate();
  const { isAuthenticated, user, logout } = useAuth();
  const resetSettings = useResetSettings();

  const handleLogout = () => {
    logout();
    void navigate({ to: '/login' });
  };

  const handleReset = () => {
    if (!confirm('Сбросить все настройки, избранное и корзину?')) return;
    resetSettings();
  };

  return (
    <nav className="navbar">
      <div className="navbar-brand">
        <Link to="/">ShopLab</Link>
      </div>
      <div className="navbar-links">
        <Link to="/">Главная</Link>
        {isAuthenticated ? (
          <>
            <Link to="/catalog">Каталог</Link>
            <Link to="/cart" className="cart-link">
              🛍
              <CartBadge />
            </Link>
            <span className="navbar-user">{user?.username}</span>
            <button type="button" className="nav-reset" onClick={handleReset}>
              Сбросить настройки
            </button>
            <button type="button" className="nav-logout" onClick={handleLogout}>
              Выйти
            </button>
          </>
        ) : (
          <Link to="/login">Вход</Link>
        )}
      </div>
    </nav>
  );
};

export default Navbar;
