import { Route as CatalogRoute } from '../routes/catalog';
import { useProductsQuery } from '../hooks/useProductQueries';
import QueryState from './QueryState';
import ProductCard from './ProductCard';

const Catalog = () => {
    // получаем параметры фильтрации через url
  const { category } = CatalogRoute.useSearch();

  const { data, isLoading, isError, error } = useProductsQuery();

  const filteredProducts = (data ?? []).filter((product) =>
    category ? product.category === category : true,
  );

  return (
    <QueryState
      isLoading={isLoading}
      isError={isError}
      errorMessage={error?.message}
      isEmpty={filteredProducts.length === 0}
      emptyMessage="Товары не найдены для выбранной категории"
    >
      <section className="catalog">
        <h2>Список товаров</h2>
        <div className="catalog-grid">
          {filteredProducts.map((product) => (
            <ProductCard key={product.id} product={product} />
          ))}
        </div>
      </section>
    </QueryState>
  );
};

export default Catalog;
