import { z } from 'zod';

export const RegistrationSchema = z.object({
  email: z.string().regex(/^[^\s@]+@[^\s@]+\.[^\s@]+$/, { 
    message: 'Некорректный формат email' 
  }),
  password: z.string().min(8, { 
    message: 'Пароль должен содержать минимум 8 символов' 
  }),
  username: z.string().min(1, { 
    message: 'Имя пользователя не может быть пустым' 
  }),
  city: z.string().min(1, { 
    message: 'Город не может быть пустым' 
  }),
  occupation: z.string().min(1, { 
    message: 'Выберите профессию' 
  }),
  agreed: z.boolean().refine(val => val === true, { 
    message: 'Необходимо согласиться с правилами' 
  }),
});

export type IFormData = z.infer<typeof RegistrationSchema>;

export const Step1Schema = RegistrationSchema.pick({ email: true, password: true });
export const Step2Schema = RegistrationSchema.pick({ username: true, city: true });
export const Step3Schema = RegistrationSchema.pick({ occupation: true, agreed: true });