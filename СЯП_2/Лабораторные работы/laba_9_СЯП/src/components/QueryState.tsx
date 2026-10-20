import type { ReactNode } from 'react';

interface QueryStateProps {
  isLoading: boolean;
  isError: boolean;
  errorMessage?: string;
  isEmpty?: boolean;
  emptyMessage?: string;
  children: ReactNode;
}

const QueryState = ({
  isLoading,
  isError,
  errorMessage,
  isEmpty,
  emptyMessage = 'Данные не найдены',
  children,
}: QueryStateProps) => {
  if (isLoading) {
    return <p className="status-message">Загрузка...</p>;
  }

  if (isError) {
    return <p className="status-message error">{errorMessage ?? 'Ошибка загрузки данных'}</p>;
  }

  if (isEmpty) {
    return <p className="status-message">{emptyMessage}</p>;
  }

  return <>{children}</>;
};

export default QueryState;
