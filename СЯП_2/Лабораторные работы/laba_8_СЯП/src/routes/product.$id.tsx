import { createFileRoute, Link, redirect } from '@tanstack/react-router';
import { useProductQuery } from '../hooks/useProductQueries';
import QueryState from '../components/QueryState';
import { isAuthenticated } from '../lib/auth.storage';

export const Route = createFileRoute('/product/$id')({
  beforeLoad: () => {
    if (!isAuthenticated()) {
      throw redirect({ to: '/login' });
    }
  },
  component: ProductDetailPage,
});

function ProductDetailPage() {
  const { id } = Route.useParams();
  const productId = Number(id);
  const { data, isLoading, isError, error } = useProductQuery(productId);

  return (
    <QueryState
      isLoading={isLoading}
      isError={isError}
      errorMessage={error?.message}
      isEmpty={!data}
      emptyMessage="Товар не найден"
    >
      {data && (
        <section className="product-detail">
          <Link to="/catalog" className="back-link">
            ← Назад в каталог
          </Link>
          {data.thumbnail && (
            <img src={data.thumbnail} alt={data.title} className="product-detail-image" />
          )}
          <h1>{data.title}</h1>
          <p className="product-price">{data.price} $</p>
          {data.category && <p className="product-category">Категория: {data.category}</p>}
          {data.description && <p className="product-desc">{data.description}</p>}
        </section>
      )}
    </QueryState>
  );
}
