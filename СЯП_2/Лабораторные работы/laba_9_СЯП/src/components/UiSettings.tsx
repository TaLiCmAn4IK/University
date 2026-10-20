import { useRecoilState } from 'recoil';
import { uiSettingsState, type ThemeMode, type ViewMode } from '../recoil/atoms/ui.atoms';

/**
 * Переключатели viewMode (grid/list) и theme (light/dark).
 * Пользуемся useRecoilState, т.к. нужно и читать текущие значения
 * (подсветка активной кнопки), и писать в них.
 */
const UiSettings = () => {
  const [settings, setSettings] = useRecoilState(uiSettingsState);

  const setViewMode = (viewMode: ViewMode) => setSettings({ ...settings, viewMode });
  const setTheme = (theme: ThemeMode) => setSettings({ ...settings, theme });

  return (
    <div className="ui-settings">
      <div className="settings-group">
        <span className="settings-label">Вид:</span>
        <button
          type="button"
          className={settings.viewMode === 'grid' ? 'active' : ''}
          onClick={() => setViewMode('grid')}
        >
          Сетка
        </button>
        <button
          type="button"
          className={settings.viewMode === 'list' ? 'active' : ''}
          onClick={() => setViewMode('list')}
        >
          Список
        </button>
      </div>
      <div className="settings-group">
        <span className="settings-label">Тема:</span>
        <button
          type="button"
          className={settings.theme === 'light' ? 'active' : ''}
          onClick={() => setTheme('light')}
        >
          Светлая
        </button>
        <button
          type="button"
          className={settings.theme === 'dark' ? 'active' : ''}
          onClick={() => setTheme('dark')}
        >
          Тёмная
        </button>
      </div>
    </div>
  );
};

export default UiSettings;
