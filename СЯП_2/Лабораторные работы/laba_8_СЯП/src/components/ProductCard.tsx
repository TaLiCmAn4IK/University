import { Link } from '@tanstack/react-router';
import { useState } from 'react';
import { useDeleteProductMutation } from '../hooks/useProductMutations';
import type { ProductView } from '../types/product.types';
import ProductForm from './ProductForm';

interface ProductCardProps {
  product: ProductView;
}

const ProductCard = ({ product }: ProductCardProps) => {
  const deleteMutation = useDeleteProductMutation();
  const [isEditing, setIsEditing] = useState(false);

  const handleDelete = () => {
    if (!confirm(`Удалить товар «${product.title}»?`)) return;
    deleteMutation.mutate(product.id, {
      onError: (error) => {
        alert(error.message);
      },
    });
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
      <h3>
        <Link to="/product/$id" params={{ id: String(product.id) }}>
          {product.title}
        </Link>
      </h3>
      <p className="product-price">{product.price} $</p>
      {product.category && <p className="product-category">{product.category}</p>}
      {product.description && <p className="product-desc">{product.description}</p>}
      <div className="product-actions">
        <Link to="/product/$id" params={{ id: String(product.id) }} className="button-link">
          Подробнее
        </Link>
        <button type="button" onClick={() => setIsEditing(true)}>
          Редактировать
        </button>
        <button
          type="button"
          className="danger"
          onClick={handleDelete}
          disabled={deleteMutation.isPending}
        >
          {deleteMutation.isPending ? 'Удаление...' : 'Удалить'}
        </button>
      </div>
    </article>
  );
};

export default ProductCard;
