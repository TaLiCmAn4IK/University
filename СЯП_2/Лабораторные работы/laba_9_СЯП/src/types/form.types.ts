import type { IFormData } from '../schemas/registration.schema';

export interface IFormState {
  currentStep: 1 | 2 | 3;
  formData: IFormData;
  errors: Record<keyof IFormData, string>;
  isSubmitting: boolean;
}

export type TFormAction =
  | { type: 'UPDATE_FIELD'; field: keyof IFormData; value: string | boolean }
  | { type: 'SET_ERROR'; field: keyof IFormData; message: string }
  | { type: 'NEXT_STEP' }
  | { type: 'PREV_STEP' }
  | { type: 'SUBMIT_START' }
  | { type: 'SUBMIT_SUCCESS' }
  | { type: 'SUBMIT_SUCCESS_AUTH' };
