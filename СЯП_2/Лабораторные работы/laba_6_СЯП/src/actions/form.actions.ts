import type { IFormData } from '../schemas/registration.schema';
import type { TFormAction } from '../types/form.types';

export const updateField = (field: keyof IFormData, value: string | boolean): TFormAction => ({
  type: 'UPDATE_FIELD',
  field,
  value
});

export const setError = (field: keyof IFormData, message: string): TFormAction => ({
  type: 'SET_ERROR',
  field,
  message
});

export const nextStep = (): TFormAction => ({
  type: 'NEXT_STEP'
});

export const prevStep = (): TFormAction => ({
  type: 'PREV_STEP'
});

export const submitStart = (): TFormAction => ({
  type: 'SUBMIT_START'
});

export const submitSuccess = (): TFormAction => ({
  type: 'SUBMIT_SUCCESS'
});