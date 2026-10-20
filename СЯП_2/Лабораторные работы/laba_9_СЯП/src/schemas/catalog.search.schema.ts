import { z } from 'zod';

export const catalogSearchSchema = z.object({
  category: z.string().optional(),
});

export type CatalogSearch = z.infer<typeof catalogSearchSchema>;
