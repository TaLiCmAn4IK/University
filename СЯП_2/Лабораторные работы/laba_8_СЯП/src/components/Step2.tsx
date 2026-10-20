import React from 'react';
import type { IFormData } from '../schemas/registration.schema';

interface Step2Props {
  formData: Pick<IFormData, 'username' | 'city'>;
  errors: Record<keyof IFormData, string>;
  isSubmitting: boolean;
  onChange: (e: React.ChangeEvent<HTMLInputElement>) => void;
}

const Step2: React.FC<Step2Props> = ({ formData, errors, isSubmitting, onChange }) => {
  return (
    <div className="step">
      <h3>Шаг 2: Профиль</h3>
      <div className="form-group">
        <label htmlFor="username">Имя пользователя:</label>
        <input
          type="text"
          id="username"
          name="username"
          value={formData.username}
          onChange={onChange}
          disabled={isSubmitting}
        />
        {errors.username && <span className="error">{errors.username}</span>}
      </div>
      <div className="form-group">
        <label htmlFor="city">Город:</label>
        <input
          type="text"
          id="city"
          name="city"
          value={formData.city}
          onChange={onChange}
          disabled={isSubmitting}
        />
        {errors.city && <span className="error">{errors.city}</span>}
      </div>
    </div>
  );
};

export default Step2;
