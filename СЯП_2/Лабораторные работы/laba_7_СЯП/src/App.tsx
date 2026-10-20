import ProtectedRoute from './components/ProtectedRoute';
import CatalogPage from './pages/CatalogPage';
import './styles.css';

function App() {
  return (
    <div className="app">
      <ProtectedRoute>
        <CatalogPage />
      </ProtectedRoute>
    </div>
  );
}

export default App;
