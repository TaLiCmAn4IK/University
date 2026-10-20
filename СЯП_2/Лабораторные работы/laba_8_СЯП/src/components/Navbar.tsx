import { Link, useNavigate } from '@tanstack/react-router';
import { useAuth } from '../hooks/useAuth';

const Navbar = () => {
  const navigate = useNavigate();
  const { isAuthenticated, user, logout } = useAuth();

  const handleLogout = () => {
    logout();
    void navigate({ to: '/login' });
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
            <span className="navbar-user">{user?.username}</span>
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
