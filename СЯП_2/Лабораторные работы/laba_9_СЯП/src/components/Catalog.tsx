import { useRecoilValue } from 'recoil';
import { Route as CatalogRoute } from '../routes/catalog';
import { useProductsQuery } from '../hooks/useProductQueries';
import { catalogGridClassSelector } from '../recoil/atoms/ui.atoms';
import { useSyncProductsCache } from '../recoil/hooks/useSyncProductsCache';
import QueryState from './QueryState';
import ProductCard from './ProductCard';

const Catalog = () => {
  const { category } = CatalogRoute.useSearch();
  const { data, isLoading, isError, error } = useProductsQuery();

  useSyncProductsCache();

  const gridClass = useRecoilValue(catalogGridClassSelector);

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
        <div className={gridClass}>
          {filteredProducts.map((product) => (
            <ProductCard key={product.id} product={product} />
          ))}
        </div>
      </section>
    </QueryState>
  );
};

export default Catalog;
