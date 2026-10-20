import React, { useState } from 'react';
import Button from './components/ui/Button';
import Input from './components/ui/Input';
import Badge from './components/ui/Badge';
import LayoutCard from './components/ui/LayoutCard';
import './App.css';

const App: React.FC = () => {
  const [userName, setUserName] = useState('');
  const [userEmail, setUserEmail] = useState('');
  const [isSaving, setIsSaving] = useState(false);

  const handleSave = () => {
    if (!userName || !userEmail) {
      alert('Заполните все поля');
      return;
    }
    setIsSaving(true);
    setTimeout(() => {
      alert(`Сохранено!\nИмя: ${userName}\nEmail: ${userEmail}`);
      setIsSaving(false);
    }, 1500);
  };

  return (
    <div className="app">
      <h1>UI Kit</h1>
      <p className="subtitle">Базовые компоненты для системы мониторинга</p>

      <LayoutCard title="Кнопки (Button)">
        <div className="flex-row">
          <Button variant="primary">Primary</Button>
          <Button variant="secondary">Secondary</Button>
          <Button variant="danger">Danger</Button>
        </div>
        <div className="flex-row">
          <Button size="small">Small</Button>
          <Button size="medium">Medium</Button>
          <Button size="large">Large</Button>
        </div>
        <div className="flex-row">
          <Button isLoading>Loading</Button>
          <Button disabled>Disabled</Button>
        </div>
      </LayoutCard>

      <LayoutCard title="Поля ввода (Input)">
        <Input
          label="Имя пользователя"
          placeholder="Введите имя"
          value={userName}
          onChange={(e) => setUserName(e.target.value)}
          isFullWidth
        />
        <Input
          label="Email"
          type="email"
          placeholder="user@example.com"
          value={userEmail}
          onChange={(e) => setUserEmail(e.target.value)}
          isFullWidth
        />
        <Input
          label="Пароль"
          type="password"
          placeholder="Введите пароль"
          error="Пароль слишком короткий"
          isFullWidth
        />
      </LayoutCard>

      <LayoutCard title="Индикаторы (Badge)">
        <div className="flex-row">
          <Badge color="green" text="Online" />
          <Badge color="red" text="Offline" />
          <Badge color="orange" text="Pending" />
          <Badge color="blue" text="Admin" />
        </div>
        <p style={{ marginTop: 16 }}>
          Статус пользователя: <Badge color="green" text="Активен" />
        </p>
      </LayoutCard>

      <LayoutCard
        title="Регистрация"
        footer={
          <Button
            variant="primary"
            onClick={handleSave}
            isLoading={isSaving}
          >
            Сохранить
          </Button>
        }
      >
        <Input
          label="Имя"
          placeholder="Введите имя"
          value={userName}
          onChange={(e) => setUserName(e.target.value)}
          isFullWidth
        />
        <Input
          label="Email"
          type="email"
          placeholder="user@example.com"
          value={userEmail}
          onChange={(e) => setUserEmail(e.target.value)}
          isFullWidth
        />
      </LayoutCard>
    </div>
  );
};

export default App;