import React from 'react';
import type { IFormData } from '../schemas/registration.schema';

interface Step3Props {
  formData: Pick<IFormData, 'occupation' | 'agreed'>;
  errors: Record<keyof IFormData, string>;
  isSubmitting: boolean;
  onChange: (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => void;
}

const Step3: React.FC<Step3Props> = ({ formData, errors, isSubmitting, onChange }) => {
  return (
    <div className="step">
      <h3>Шаг 3: О себе</h3>
      <div className="form-group">
        <label htmlFor="occupation">Профессия:</label>
        <select
          id="occupation"
          name="occupation"
          value={formData.occupation}
          onChange={onChange}
          disabled={isSubmitting}
        >
          <option value="">Выберите профессию</option>
          <option value="developer">Разработчик</option>
          <option value="designer">Дизайнер</option>
          <option value="manager">Менеджер</option>
          <option value="analyst">Аналитик</option>
          <option value="tester">Тестировщик</option>
        </select>
        {errors.occupation && <span className="error">{errors.occupation}</span>}
      </div>
      <div className="form-group checkbox">
        <label>
          <input
            type="checkbox"
            name="agreed"
            checked={formData.agreed}
            onChange={onChange}
            disabled={isSubmitting}
          />
          Согласен с правилами
        </label>
        {errors.agreed && <span className="error">{errors.agreed}</span>}
      </div>
    </div>
  );
};

export default Step3;