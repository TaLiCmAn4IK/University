import type { Product } from '../schemas/product.schema';

const BASE_URL = 'https://dummyjson.com/products';

interface DummyProduct {
  id: number;
  title: string;
  price: number;
  thumbnail?: string;
  description?: string;
}

interface ProductsResponse {
  products: DummyProduct[];
}

function toProduct(item: DummyProduct): Product & { thumbnail?: string; description?: string } {
  return {
    id: item.id,
    title: item.title,
    price: item.price,
    thumbnail: item.thumbnail,
    description: item.description,
  };
}

export async function fetchProducts(): Promise<(Product & { thumbnail?: string; description?: string })[]> {
  const response = await fetch(BASE_URL);
  if (!response.ok) {
    throw new Error(`Ошибка загрузки: ${response.status}`);
  }
  const data = (await response.json()) as ProductsResponse;
  return data.products.map(toProduct);
}

export async function createProduct(payload: Omit<Product, 'id'>): Promise<Product> {
  const response = await fetch(`${BASE_URL}/add`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload),
  });
  if (!response.ok) {
    throw new Error(`Ошибка создания: ${response.status}`);
  }
  const data = (await response.json()) as DummyProduct;
  return toProduct(data);
}

export async function updateProduct(id: number, payload: Omit<Product, 'id'>): Promise<Product> {
  const response = await fetch(`${BASE_URL}/${id}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload),
  });
  if (!response.ok) {
    throw new Error(`Ошибка обновления: ${response.status}`);
  }
  const data = (await response.json()) as DummyProduct;
  return toProduct(data);
}

export async function deleteProduct(id: number): Promise<void> {
  const response = await fetch(`${BASE_URL}/${id}`, { method: 'DELETE' });
  if (!response.ok) {
    throw new Error(`Ошибка удаления: ${response.status}`);
  }
}
