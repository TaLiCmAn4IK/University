import { z } from 'zod';

export const ProductSchema = z.object({
  id: z.number(),
  title: z.string().min(3, {
    message: 'Название должно содержать минимум 3 символа',
  }),
  price: z.number().min(0, {
    message: 'Цена должна быть не меньше 0',
  }),
});

export type Product = z.infer<typeof ProductSchema>;

export const ProductDetailSchema = ProductSchema.extend({
  category: z.string().optional(),
  thumbnail: z.string().optional(),
  description: z.string().optional(),
});

export type ProductView = z.infer<typeof ProductDetailSchema>;

export const ProductsResponseSchema = z.object({
  products: z.array(z.unknown()),
});
