document.addEventListener('DOMContentLoaded', () => {
  // Изменяем заголовок по ?type=
  const params  = new URLSearchParams(location.search);
  const t       = params.get('type');                // 'student' | 'employee'
  const h1      = document.querySelector('.login-title');

  if (t === 'employee') {
    h1.textContent = 'Личный кабинет сотрудника';
  } else {
    h1.textContent = 'Личный кабинет студента';
  }

  // Ссылки на DOM-элементы 
  const loginForm     = document.getElementById('loginForm');
  const loginInput    = document.getElementById('login');
  const passwordInput = document.getElementById('password');
  const togglePassword = document.querySelector('.toggle-password');

  // Валидация поля «Логин» 
  if (loginInput) {
    loginInput.addEventListener('input', (e) => {
      const value = e.target.value;
      const regex = /^[a-zA-Z0-9]*$/; // допустимы только латинские буквы и цифры

      if (!regex.test(value)) {
        // Недопустимые символы
        e.target.value = value.replace(/[^a-zA-Z0-9]/g, '');
        e.target.setCustomValidity('Только английские буквы и цифры');
      } else {
        e.target.setCustomValidity('');
      }
    });
  }

  // Переключатель видимости пароля
  if (togglePassword && passwordInput) {
    togglePassword.addEventListener('click', () => {
      const isPassword = passwordInput.type === 'password';
      const eyeIcon    = togglePassword.querySelector('.eye-icon');

      passwordInput.type = isPassword ? 'text' : 'password';

      // Меняем иконку «глаз» (открытый / закрытый)
      if (eyeIcon) {
        eyeIcon.innerHTML = isPassword
          ? '<path class="eye-closed" d="M11.83 9L15 12.16V12a3 3 0 0 0-3-3h-.17m-4.3.8l1.55 1.55c-.05.21-.08.42-.08.65a3 3 0 0 0 3 3c.22 0 .44-.03.65-.08l1.55 1.55c-.67.33-1.41.53-2.2.53a5 5 0 0 1-5-5c0-.79.2-1.53.53-2.2M2 4.27l2.28 2.28.45.45C3.08 8.3 1.78 10 1 12c1.73 4.39 6 7.5 11 7.5 1.55 0 3.03-.3 4.38-.84l.43.42L19.73 22 21 20.73 3.27 3M8.53 10.08l1.55 1.55c-.05.21-.08.42-.08.65a3 3 0 0 0 3 3c.22 0 .44-.03.65-.08l1.55 1.55c-.67.33-1.41.53-2.2.53a5 5 0 0 1-5-5c0-.79.2-1.53.53-2.2Z"/>'
          : '<path class="eye-open" d="M12 9a3 3 0 0 1 3 3 3 3 0 0 1-3 3 3 3 0 0 1-3-3 3 3 0 0 1 3-3m0-4.5c5 0 9.27 3.11 11 7.5-1.73 4.39-6 7.5-11 7.5S2.73 16.39 1 12c1.73-4.39 6-7.5 11-7.5M3.18 12a9.821 9.821 0 0 0 17.64 0 9.821 9.821 0 0 0-17.64 0z"/>';
      }
    });
  }

  // Обработка отправки формы 
  if (loginForm) {
    loginForm.addEventListener('submit', (e) => {
      e.preventDefault();

      // Проверяем валидность HTML5
      if (!loginForm.checkValidity()) {
        document.querySelectorAll('.error-message')
          .forEach(msg => { msg.style.display = 'block'; });
        return;
      }

      // Собираем данные формы
      const formData = {
        login:     loginInput.value,
        password:  passwordInput.value,
        timestamp: new Date().toISOString()
      };

      downloadXML(formData);
    });
  }

  // Функция скачивания XML
  /**
   * @param {{login: string, password: string, timestamp: string}} data
   */
  function downloadXML(data) {
    // Экранируем спец-символы для XML
    const escapeXML = (str) => str
      .replace(/&/g, '&amp;')
      .replace(/</g, '&lt;')
      .replace(/>/g, '&gt;')
      .replace(/"/g, '&quot;')
      .replace(/'/g, '&apos;');

    // Шаблон XML
    const xmlString = `<?xml version="1.0" encoding="UTF-8"?>
<loginData>
  <login>${escapeXML(data.login)}</login>
  <password>${escapeXML(data.password)}</password>
  <timestamp>${escapeXML(data.timestamp)}</timestamp>
</loginData>`;

    const blob = new Blob([xmlString], { type: 'application/xml' });
    const url  = URL.createObjectURL(blob);
    const a    = document.createElement('a');

    a.href     = url;
    a.download = `login_data_${Date.now()}.xml`;
    document.body.appendChild(a);
    a.click();

    document.body.removeChild(a);
    URL.revokeObjectURL(url);
  }
});
