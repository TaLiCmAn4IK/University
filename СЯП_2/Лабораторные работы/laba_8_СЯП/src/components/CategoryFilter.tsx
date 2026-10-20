import { useNavigate } from '@tanstack/react-router';
import { Route as CatalogRoute } from '../routes/catalog';

const CATEGORIES = [
  { value: '', label: 'Все категории' },
  { value: 'smartphones', label: 'Смартфоны' },
  { value: 'laptops', label: 'Ноутбуки' },
  { value: 'fragrances', label: 'Ароматы' },
  { value: 'skincare', label: 'Уход за кожей' },
  { value: 'groceries', label: 'Продукты' },
  { value: 'home-decoration', label: 'Декор' },
  { value: 'furniture', label: 'Мебель' },
];

const CategoryFilter = () => {
  const { category } = CatalogRoute.useSearch();
  const navigate = useNavigate({ from: '/catalog' });

  return (
    <div className="category-filter form-group">
      <label htmlFor="category">Категория (Search Params):</label>
      <select
        id="category"
        value={category ?? ''}
        onChange={(e) => {
          const value = e.target.value;
          void navigate({
            search: (prev) => ({
              ...prev,
              category: value || undefined,
            }),
          });
        }}
      >
        {CATEGORIES.map((item) => (
          <option key={item.value || 'all'} value={item.value}>
            {item.label}
          </option>
        ))}
      </select>
    </div>
  );
};

export default CategoryFilter;
