import { createFileRoute, Link } from '@tanstack/react-router';

export const Route = createFileRoute('/')({
  component: LandingPage,
});

function LandingPage() {
  return (
    <section className="landing">
      <h1>Лабораторная работа №8</h1>
      <div className="landing-actions">
        <Link to="/catalog" className="button-link primary">
          Перейти в каталог
        </Link>
        <Link to="/login" className="button-link">
          Войти / зарегистрироваться
        </Link>
      </div>
    </section>
  );
}
