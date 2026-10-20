import { useState } from 'react';
import RegistrationForm from '../components/RegistrationForm';
import LoginForm from '../components/LoginForm';

const AuthPage = () => {
  const [mode, setMode] = useState<'login' | 'register'>('login');

  return (
    <div className="auth-page">
      <h1>{mode === 'login' ? 'Вход' : 'Регистрация'}</h1>
      <div className="auth-tabs">
        <button
          type="button"
          className={mode === 'login' ? 'active' : ''}
          onClick={() => setMode('login')}
        >
          Войти
        </button>
        <button
          type="button"
          className={mode === 'register' ? 'active' : ''}
          onClick={() => setMode('register')}
        >
          Зарегистрироваться
        </button>
      </div>
      {mode === 'login' ? <LoginForm /> : <RegistrationForm />}
    </div>
  );
};

export default AuthPage;
