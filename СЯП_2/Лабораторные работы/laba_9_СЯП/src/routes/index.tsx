import { createFileRoute, Link } from '@tanstack/react-router';

export const Route = createFileRoute('/')({
  component: LandingPage,
});

function LandingPage() {
  return (
    <section className="landing">
      <h1>Лабораторная работа №8</h1>
      <p>TanStack Router + TanStack Query на базе проекта из лабораторной №7.</p>
      <ul>
        <li>Файловая маршрутизация и Auth Guard через <code>beforeLoad</code></li>
        <li>Загрузка данных через <code>useQuery</code> вместо <code>useEffect</code></li>
        <li>CRUD через <code>useMutation</code> и <code>invalidateQueries</code></li>
        <li>Фильтр категорий в URL через Search Params и Zod</li>
      </ul>
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
