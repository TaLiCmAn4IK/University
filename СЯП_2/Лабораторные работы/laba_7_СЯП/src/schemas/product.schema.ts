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

export const ProductInputSchema = ProductSchema.omit({ id: true }).extend({
  id: z.number().optional(),
});

export type ProductInput = z.infer<typeof ProductInputSchema>;
