import { useProducts } from '../hooks/useProducts';
import ProductCard from './ProductCard';

const Catalog = () => {
  const { products, isLoading, error } = useProducts();

  if (isLoading) {
    return <p className="status-message">Загрузка товаров...</p>;
  }

  if (error) {
    return <p className="status-message error">{error}</p>;
  }

  if (products.length === 0) {
    return <p className="status-message">Товары не найдены</p>;
  }

  return (
    <section className="catalog">
      <h2>Список товаров</h2>
      <div className="catalog-grid">
        {products.map((product) => (
          <ProductCard key={product.id} product={product} />
        ))}
      </div>
    </section>
  );
};

export default Catalog;
