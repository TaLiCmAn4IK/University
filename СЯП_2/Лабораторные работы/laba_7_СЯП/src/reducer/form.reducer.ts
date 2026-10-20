import type { IFormState, TFormAction } from '../types/form.types';

export const initialState: IFormState = {
  currentStep: 1,
  formData: {
    email: '',
    password: '',
    username: '',
    city: '',
    occupation: '',
    agreed: false,
  },
  errors: {
    email: '',
    password: '',
    username: '',
    city: '',
    occupation: '',
    agreed: '',
  },
  isSubmitting: false,
};

export function registrationReducer(state: IFormState, action: TFormAction): IFormState {
  switch (action.type) {
    case 'UPDATE_FIELD':
      return {
        ...state,
        formData: { ...state.formData, [action.field]: action.value },
        errors: { ...state.errors, [action.field]: '' },
      };

    case 'SET_ERROR':
      return {
        ...state,
        errors: { ...state.errors, [action.field]: action.message },
      };

    case 'NEXT_STEP':
      return {
        ...state,
        currentStep: (state.currentStep + 1) as 1 | 2 | 3,
      };

    case 'PREV_STEP':
      return {
        ...state,
        currentStep: (state.currentStep - 1) as 1 | 2 | 3,
      };

    case 'SUBMIT_START':
      return {
        ...state,
        isSubmitting: true,
      };

    case 'SUBMIT_SUCCESS':
    case 'SUBMIT_SUCCESS_AUTH':
      return {
        ...state,
        isSubmitting: false,
      };

    default:
      return state;
  }
}
