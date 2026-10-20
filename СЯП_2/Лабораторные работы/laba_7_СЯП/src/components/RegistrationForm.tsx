import React from 'react';
import { useRegistrationForm } from '../hooks/useRegistrationForm';
import Step1 from './Step1';
import Step2 from './Step2';
import Step3 from './Step3';

const RegistrationForm: React.FC = () => {
  const { state, handlers } = useRegistrationForm();
  const { currentStep, formData, errors, isSubmitting } = state;
  const { handleChange, handleNext, handlePrev, handleSubmit } = handlers;

  return (
    <form onSubmit={handleSubmit} className="registration-form">
      {currentStep === 1 && (
        <Step1
          formData={{ email: formData.email, password: formData.password }}
          errors={errors}
          isSubmitting={isSubmitting}
          onChange={handleChange}
        />
      )}

      {currentStep === 2 && (
        <Step2
          formData={{ username: formData.username, city: formData.city }}
          errors={errors}
          isSubmitting={isSubmitting}
          onChange={handleChange}
        />
      )}

      {currentStep === 3 && (
        <Step3
          formData={{ occupation: formData.occupation, agreed: formData.agreed }}
          errors={errors}
          isSubmitting={isSubmitting}
          onChange={handleChange}
        />
      )}

      <div className="buttons">
        {currentStep > 1 && (
          <button type="button" onClick={handlePrev} disabled={isSubmitting}>
            Назад
          </button>
        )}

        {currentStep < 3 && (
          <button type="button" onClick={handleNext} disabled={isSubmitting}>
            Далее
          </button>
        )}

        {currentStep === 3 && (
          <button type="submit" disabled={isSubmitting}>
            {isSubmitting ? 'Регистрация...' : 'Зарегистрироваться'}
          </button>
        )}
      </div>
    </form>
  );
};

export default RegistrationForm;
