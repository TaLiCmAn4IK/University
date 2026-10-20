import { createFileRoute, redirect } from '@tanstack/react-router';
import { useAuth } from '../hooks/useAuth';
import Catalog from '../components/Catalog';
import CategoryFilter from '../components/CategoryFilter';
import ProductForm from '../components/ProductForm';
import UiSettings from '../components/UiSettings';
import { isAuthenticated } from '../lib/auth.storage';
import { catalogSearchSchema } from '../schemas/catalog.search.schema';

export const Route = createFileRoute('/catalog')({
  validateSearch: catalogSearchSchema,
  beforeLoad: () => {
    if (!isAuthenticated()) {
      throw redirect({ to: '/login' });
    }
  },
  component: CatalogRoute,
});

function CatalogRoute() {
  const { user } = useAuth();

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
        <UiSettings />
      </header>

      <CategoryFilter />

      <section className="admin-panel">
        <h2>Управление товарами</h2>
        <ProductForm />
      </section>

      <Catalog />
    </div>
  );
}
