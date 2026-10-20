import React from 'react';
import type { IFormData } from '../schemas/registration.schema';

interface Step1Props {
  formData: Pick<IFormData, 'email' | 'password'>;
  errors: Record<keyof IFormData, string>;
  isSubmitting: boolean;
  onChange: (e: React.ChangeEvent<HTMLInputElement>) => void;
}

const Step1: React.FC<Step1Props> = ({ formData, errors, isSubmitting, onChange }) => {
  return (
    <div className="step">
      <h3>Шаг 1: Аккаунт</h3>
      <div className="form-group">
        <label htmlFor="email">Email:</label>
        <input
          type="email"
          id="email"
          name="email"
          value={formData.email}
          onChange={onChange}
          disabled={isSubmitting}
        />
        {errors.email && <span className="error">{errors.email}</span>}
      </div>
      <div className="form-group">
        <label htmlFor="password">Пароль:</label>
        <input
          type="password"
          id="password"
          name="password"
          value={formData.password}
          onChange={onChange}
          disabled={isSubmitting}
        />
        {errors.password && <span className="error">{errors.password}</span>}
      </div>
    </div>
  );
};

export default Step1;