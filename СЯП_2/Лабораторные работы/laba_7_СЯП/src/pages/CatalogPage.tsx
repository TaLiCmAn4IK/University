import { useAuth } from '../hooks/useAuth';
import Catalog from '../components/Catalog';
import ProductForm from '../components/ProductForm';

const CatalogPage = () => {
  const { user, logout } = useAuth();

  return (
    <div className="catalog-page">
      <header className="catalog-header">
        <div>
          <h1>Каталог товаров</h1>
          {user && (
            <p className="user-greeting">
              Здравствуйте, {user.username} ({user.email})
            </p>
          )}
        </div>
        <button type="button" className="logout-btn" onClick={logout}>
          Выйти
        </button>
      </header>

      <section className="admin-panel">
        <h2>Управление товарами</h2>
        <ProductForm />
      </section>

      <Catalog />
    </div>
  );
};

export default CatalogPage;
