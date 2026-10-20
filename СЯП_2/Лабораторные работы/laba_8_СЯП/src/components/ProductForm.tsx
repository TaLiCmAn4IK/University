import { useState } from 'react';
import { ProductSchema, type Product } from '../schemas/product.schema';
import {
  useCreateProductMutation,
  useUpdateProductMutation,
} from '../hooks/useProductMutations';
import type { ProductView } from '../types/product.types';

interface ProductFormProps {
  initialProduct?: ProductView;
  onCancel?: () => void;
  onSuccess?: () => void;
}

const ProductForm = ({ initialProduct, onCancel, onSuccess }: ProductFormProps) => {
  const createMutation = useCreateProductMutation();
  const updateMutation = useUpdateProductMutation();
  const isEdit = Boolean(initialProduct);
  const [title, setTitle] = useState(initialProduct?.title ?? '');
  const [price, setPrice] = useState(initialProduct?.price?.toString() ?? '');

  const isSubmitting = createMutation.isPending || updateMutation.isPending;

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    const candidate: Product = {
      id: initialProduct?.id ?? Date.now(),
      title: title.trim(),
      price: Number(price),
    };

    const result = ProductSchema.safeParse(candidate);
    if (!result.success) {
      const flattened = result.error.flatten();
      const messages = Object.entries(flattened.fieldErrors)
        .filter(([, value]) => value && value.length > 0)
        .map(([field, value]) => `${field}: ${value?.join(', ')}`);
      alert(`Ошибка валидации:\n${messages.join('\n')}`);
      return;
    }

    const payload = { title: result.data.title, price: result.data.price };

    if (isEdit && initialProduct) {
      updateMutation.mutate(
        { id: initialProduct.id, payload },
        {
          onSuccess: () => onSuccess?.(),
          onError: (error) => alert(error.message),
        },
      );
      return;
    }

    createMutation.mutate(payload, {
      onSuccess: () => {
        setTitle('');
        setPrice('');
        onSuccess?.();
      },
      onError: (error) => alert(error.message),
    });
  };

  return (
    <form className="product-form" onSubmit={handleSubmit}>
      <div className="form-group">
        <label htmlFor="product-title">Название:</label>
        <input
          id="product-title"
          type="text"
          value={title}
          onChange={(e) => setTitle(e.target.value)}
          disabled={isSubmitting}
        />
      </div>
      <div className="form-group">
        <label htmlFor="product-price">Цена:</label>
        <input
          id="product-price"
          type="number"
          min="0"
          step="0.01"
          value={price}
          onChange={(e) => setPrice(e.target.value)}
          disabled={isSubmitting}
        />
      </div>
      <div className="buttons">
        <button type="submit" disabled={isSubmitting}>
          {isSubmitting
            ? 'Сохранение...'
            : isEdit
              ? 'Сохранить изменения'
              : 'Добавить товар'}
        </button>
        {isEdit && onCancel && (
          <button type="button" onClick={onCancel} disabled={isSubmitting}>
            Отмена
          </button>
        )}
      </div>
    </form>
  );
};

export default ProductForm;
