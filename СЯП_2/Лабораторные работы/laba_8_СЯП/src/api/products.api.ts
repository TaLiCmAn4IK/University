import { z } from 'zod';
import {
  ProductDetailSchema,
  ProductsResponseSchema,
  type Product,
  type ProductView,
} from '../schemas/product.schema';

const BASE_URL = 'https://dummyjson.com/products';

function parseProduct(data: unknown): ProductView {
  try {
    return ProductDetailSchema.parse(data);
  } catch (e) {
    if (e instanceof z.ZodError) {
      throw new Error('Ошибка структуры данных');
    }
    throw e;
  }
}

export async function fetchProducts(): Promise<ProductView[]> {
  const response = await fetch(BASE_URL);
  if (!response.ok) {
    throw new Error(`Ошибка загрузки: ${response.status}`);
  }
  const json: unknown = await response.json();

  let parsed;
  try {
    parsed = ProductsResponseSchema.parse(json);
  } catch (e) {
    if (e instanceof z.ZodError) {
      throw new Error('Ошибка структуры данных');
    }
    throw e;
  }

  return parsed.products.map(parseProduct);
}

export async function fetchProductById(id: number): Promise<ProductView> {
  const response = await fetch(`${BASE_URL}/${id}`);
  if (!response.ok) {
    throw new Error(`Ошибка загрузки товара: ${response.status}`);
  }
  const json: unknown = await response.json();
  return parseProduct(json);
}

export async function createProduct(payload: Omit<Product, 'id'>): Promise<ProductView> {
  const response = await fetch(`${BASE_URL}/add`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload),
  });
  if (!response.ok) {
    throw new Error(`Ошибка создания: ${response.status}`);
  }
  const json: unknown = await response.json();
  return parseProduct(json);
}

export async function updateProduct(
  id: number,
  payload: Omit<Product, 'id'>,
): Promise<ProductView> {
  const response = await fetch(`${BASE_URL}/${id}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload),
  });
  if (!response.ok) {
    throw new Error(`Ошибка обновления: ${response.status}`);
  }
  const json: unknown = await response.json();
  return parseProduct(json);
}

export async function deleteProduct(id: number): Promise<void> {
  const response = await fetch(`${BASE_URL}/${id}`, { method: 'DELETE' });
  if (!response.ok) {
    throw new Error(`Ошибка удаления: ${response.status}`);
  }
}