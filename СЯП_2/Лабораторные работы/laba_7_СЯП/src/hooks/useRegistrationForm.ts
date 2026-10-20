import { useReducer } from 'react';
import { registrationReducer, initialState } from '../reducer/form.reducer';
import type { IFormData } from '../schemas/registration.schema';
import {
  RegistrationSchema,
  Step1Schema,
  Step2Schema,
  Step3Schema,
} from '../schemas/registration.schema';
import {
  updateField,
  setError,
  nextStep,
  prevStep,
  submitStart,
  submitSuccessAuth,
} from '../actions/form.actions';
import { useAuth } from './useAuth';

function validateStep(step: number, data: Partial<IFormData>): Record<string, string> {
  let schema;

  switch (step) {
    case 1:
      schema = Step1Schema;
      break;
    case 2:
      schema = Step2Schema;
      break;
    case 3:
      schema = Step3Schema;
      break;
    default:
      return {};
  }

  const result = schema.safeParse(data);

  if (!result.success) {
    const errors: Record<string, string> = {};
    result.error.issues.forEach((issue) => {
      const path = issue.path[0] as keyof IFormData;
      errors[path] = issue.message;
    });
    return errors;
  }

  return {};
}

export const useRegistrationForm = () => {
  const { loginFromRegistration } = useAuth();
  const [state, dispatch] = useReducer(registrationReducer, initialState);
  const { currentStep, formData, errors, isSubmitting } = state;

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value, type } = e.target;
    const fieldValue = type === 'checkbox' ? (e.target as HTMLInputElement).checked : value;
    dispatch(updateField(name as keyof IFormData, fieldValue));
  };

  const handleNext = () => {
    const stepErrors = validateStep(currentStep, formData);

    if (Object.keys(stepErrors).length > 0) {
      for (const [field, message] of Object.entries(stepErrors)) {
        dispatch(setError(field as keyof IFormData, message));
      }
      return;
    }

    dispatch(nextStep());
  };

  const handlePrev = () => {
    dispatch(prevStep());
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    const finalValidation = RegistrationSchema.safeParse(formData);

    if (!finalValidation.success) {
      finalValidation.error.issues.forEach((issue) => {
        const path = issue.path[0] as keyof IFormData;
        dispatch(setError(path, issue.message));
      });
      return;
    }

    dispatch(submitStart());

    setTimeout(() => {
      loginFromRegistration(formData);
      dispatch(submitSuccessAuth());
      alert('Регистрация успешна! Вы вошли в каталог.');
    }, 1000);
  };

  return {
    state: { currentStep, formData, errors, isSubmitting },
    handlers: { handleChange, handleNext, handlePrev, handleSubmit },
  };
};
