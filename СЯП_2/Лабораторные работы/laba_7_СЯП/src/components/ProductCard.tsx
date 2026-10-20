import { useState } from 'react';
import { useProducts } from '../hooks/useProducts';
import type { ProductView } from '../types/product.types';
import ProductForm from './ProductForm';

interface ProductCardProps {
  product: ProductView;
}

const ProductCard = ({ product }: ProductCardProps) => {
  const { deleteProduct } = useProducts();
  const [isEditing, setIsEditing] = useState(false);

  const handleDelete = async () => {
    if (!confirm(`Удалить товар «${product.title}»?`)) return;
    try {
      await deleteProduct(product.id);
    } catch (error) {
      const message = error instanceof Error ? error.message : 'Ошибка удаления';
      alert(message);
    }
  };

  if (isEditing) {
    return (
      <article className="product-card editing">
        <ProductForm
          initialProduct={product}
          onCancel={() => setIsEditing(false)}
          onSuccess={() => setIsEditing(false)}
        />
      </article>
    );
  }

  return (
    <article className="product-card">
      {product.thumbnail && (
        <img src={product.thumbnail} alt={product.title} className="product-thumb" />
      )}
      <h3>{product.title}</h3>
      <p className="product-price">{product.price} $</p>
      {product.description && <p className="product-desc">{product.description}</p>}
      <div className="product-actions">
        <button type="button" onClick={() => setIsEditing(true)}>
          Редактировать
        </button>
        <button type="button" className="danger" onClick={() => void handleDelete()}>
          Удалить
        </button>
      </div>
    </article>
  );
};

export default ProductCard;
