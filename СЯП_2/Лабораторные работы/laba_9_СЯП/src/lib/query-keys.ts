export const PRODUCTS_QUERY_KEY = ['products'] as const;

export const productDetailKey = (id: number) => ['products', id] as const;
