document.addEventListener('DOMContentLoaded', () => {
  // Инициализация бургер-меню (ваш исходный код)
  burgerMenu();

  // ПЕРЕКЛЮЧАТЕЛЬ ЯЗЫКА
  const langToggle = document.getElementById('language-toggle');
  if (langToggle) {
    langToggle.addEventListener('click', function() {
      this.textContent = this.textContent === 'EN' ? 'RU' : 'EN';
    });
  }

  // ПЛАВНЫЙ СКРОЛЛ
document.querySelectorAll('a[href^="#"]').forEach(anchor => {
  anchor.addEventListener('click', function(e) {
    const href = this.getAttribute('href');
    
    // Пропускаем пустые якоря (href="#")
    if (href === '#') return;
    
    e.preventDefault();
    const target = document.querySelector(href);
    
    if (target) {
      target.scrollIntoView({
        behavior: 'smooth',
        block: 'start'
      });
    }
  });
});

  // АНИМАЦИЯ СЕКЦИЙ
  const observer = new IntersectionObserver((entries) => {
    entries.forEach(entry => {
      if (entry.isIntersecting) {
        entry.target.classList.add('is-visible');
        observer.unobserve(entry.target);
      }
    });
  }, { threshold: 0.2 });

  document.querySelectorAll('section').forEach(sec => observer.observe(sec));

  //МЕНЮ ВХОДА
  const loginContainer = document.querySelector('.header__login-container');
  if (loginContainer) {
    const loginBtn = loginContainer.querySelector('.header__login');
    const dropdown = loginContainer.querySelector('.login-dropdown');

    loginBtn.addEventListener('click', (e) => {
      e.stopPropagation();
      const isOpen = dropdown.style.visibility === 'visible';
      
      dropdown.style.visibility = isOpen ? 'hidden' : 'visible';
      dropdown.style.opacity = isOpen ? '0' : '1';
      dropdown.style.transform = isOpen ? 'translateY(10px)' : 'translateY(0)';
      loginBtn.setAttribute('aria-expanded', String(!isOpen));
    });

    document.addEventListener('click', () => {
      dropdown.style.visibility = 'hidden';
      dropdown.style.opacity = '0';
      dropdown.style.transform = 'translateY(10px)';
      loginBtn.setAttribute('aria-expanded', 'false');
    });

    document.addEventListener('keydown', (e) => {
      if (e.key === 'Escape') {
        dropdown.style.visibility = 'hidden';
        dropdown.style.opacity = '0';
        dropdown.style.transform = 'translateY(10px)';
        loginBtn.setAttribute('aria-expanded', 'false');
      }
    });
  }

  //МОДАЛЬНЫЕ ОКНА ДЛЯ "ПОДРОБНЕЕ"
  // Создаем HTML для модального окна
  const modalHTML = `
    <div class="modal-overlay hidden">
      <div class="modal">
        <div class="modal-header">
          <h3 class="modal-title"></h3>
          <button class="modal-close">&times;</button>
        </div>
        <div class="modal-content">
          <p class="modal-date"></p>
          <div class="modal-text"></div>
        </div>
      </div>
    </div>
  `;
  document.body.insertAdjacentHTML('beforeend', modalHTML);

  const modal = document.querySelector('.modal-overlay');
  const modalClose = document.querySelector('.modal-close');

  // Обработчики для кнопок "Подробнее"
  document.querySelectorAll('.news-card__link, .life-card__link').forEach(btn => {
    btn.addEventListener('click', function(e) {
      e.preventDefault();
      openModal(this);
    });
  });

  // Функция открытия модального окна
  function openModal(button) {
    const card = button.closest('.news-card, .life-card');
    const title = card.querySelector('[class*="__title"]').textContent;
    const date = card.querySelector('[class*="__date"]').textContent;
    const excerpt = card.querySelector('[class*="__excerpt"]').textContent;

    modal.querySelector('.modal-title').textContent = title;
    modal.querySelector('.modal-date').textContent = `Дата: ${date}`;
    modal.querySelector('.modal-text').innerHTML = `
      <p>${excerpt}</p>
      <p><strong>Подробности:</strong></p>
      <p>${getFullDescription(title)}</p>
    `;

    modal.classList.remove('hidden');
    document.body.style.overflow = 'hidden';
  }

  // Функция закрытия модального окна
  function closeModal() {
    modal.classList.add('hidden');
    document.body.style.overflow = 'auto';
  }

  // База данных полных описаний
  function getFullDescription(title) {
    const descriptions = {
      "Открытие новой лаборатории AI": "Лаборатория оснащена 20 рабочими станциями с GPU NVIDIA RTX 4090. Доступ открыт для студентов с 3 курса по предварительной записи.",
      "Победа на чемпионате мира по программированию": "Команда БГУП решила все задачи за рекордное время. Награждение состоится 20 мая в 15:00 в актовом зале.",
      "Новый спортивный зал открыт": "Площадь зала 500 м², работает ежедневно с 8:00 до 22:00. Доступны секции по 8 видам спорта.",
      "Запуск онлайн-курсов на платформе Moodle": "Доступны курсы по Python, JavaScript и машинному обучению. Первые 100 регистраций получат сертификаты бесплатно.",
      "Студенческий хакатон по Big Data": "Продолжительность 48 часов. Призы: стажировки в ведущих IT-компаниях страны.",
      "Концерт творческих коллективов": "В программе участвовало 15 коллективов. Полная запись доступна на официальном YouTube-канале университета."
    };
    return descriptions[title] || "Дополнительная информация будет опубликована в ближайшее время.";
  }

  // Обработчики закрытия модального окна
  modalClose.addEventListener('click', closeModal);
  modal.addEventListener('click', (e) => {
    if (e.target === modal) closeModal();
  });
  document.addEventListener('keydown', (e) => {
    if (e.key === 'Escape') closeModal();
  });

  // ====================== ПОКАЗ/СКРЫТИЕ КАРТОЧЕК ======================
  function setupToggleButtons(showBtnId, hideBtnId, cardsSel, sectionId) {
    const showBtn = document.getElementById(showBtnId);
    const hideBtn = document.getElementById(hideBtnId);
    const cards = document.querySelectorAll(cardsSel);

    if (showBtn && hideBtn) {
      showBtn.addEventListener('click', () => {
        cards.forEach(card => card.classList.remove('hidden'));
        showBtn.classList.add('hidden');
        hideBtn.classList.remove('hidden');
      });

      hideBtn.addEventListener('click', () => {
        cards.forEach((card, i) => i >= 3 && card.classList.add('hidden'));
        hideBtn.classList.add('hidden');
        showBtn.classList.remove('hidden');
        document.getElementById(sectionId)?.scrollIntoView({ behavior: 'smooth', block: 'start' });
      });
    }
  }

  setupToggleButtons('load-more-news', 'hide-news', '.news-card', 'news');
  setupToggleButtons('load-more-life', 'hide-life', '.life-card', 'student-life');
});

// ====================== БУРГЕР-МЕНЮ ======================
function burgerMenu() {
  const burgerBtn = document.querySelector('.burger-menu');
  const mobileMenu = document.createElement('div');
  const overlay = document.createElement('div');

  mobileMenu.className = 'mobile-menu';
  mobileMenu.innerHTML = `
    <nav class="mobile-menu__nav">
      <ul>
        <li><a href="index.html" class="active">Главная</a></li>
        <li><a href="applicants.html">Абитуриентам</a></li>
        <li><a href="faculties.html">Факультеты</a></li>
        <li><a href="#contacts">Контакты</a></li>
      </ul>
    </nav>
    <div class="mobile-menu__actions">
      <div class="header__language">
        <button id="mobile-language-toggle">RU</button>
      </div>
      <div class="header__login-container">
        <button class="header__login" aria-haspopup="true">Вход</button>
        <div class="login-dropdown">
          <a href="login.html?type=student" class="login-dropdown__item">
            <svg class="dropdown-icon" viewBox="0 0 24 24" width="16" height="16">
              <path fill="currentColor" d="M12,3L1,9L12,15L21,10.09V17H23V9M5,13.18V17.18L12,21L19,17.18V13.18L12,17L5,13.18Z"/>
            </svg>
            Личный кабинет студента
          </a>
          <a href="login.html?type=employee" class="login-dropdown__item">
            <svg class="dropdown-icon" viewBox="0 0 24 24" width="16" height="16">
              <path fill="currentColor" d="M12,4A4,4 0 0,1 16,8A4,4 0 0,1 12,12A4,4 0 0,1 8,8A4,4 0 0,1 12,4M12,14C16.42,14 20,15.79 20,18V20H4V18C4,15.79 7.58,14 12,14Z"/>
            </svg>
            Личный кабинет сотрудника
          </a>
        </div>
      </div>
    </div>
  `;

  overlay.className = 'menu-overlay';
  document.body.append(mobileMenu, overlay);

  const mobileLangToggle = document.getElementById('mobile-language-toggle');
  if (mobileLangToggle) {
    mobileLangToggle.addEventListener('click', function() {
      this.textContent = this.textContent === 'EN' ? 'RU' : 'EN';
    });
  }

  burgerBtn.addEventListener('click', () => {
    const isExpanded = burgerBtn.getAttribute('aria-expanded') === 'true';
    burgerBtn.setAttribute('aria-expanded', !isExpanded);
    mobileMenu.classList.toggle('active');
    overlay.classList.toggle('active');
    document.body.style.overflow = isExpanded ? 'auto' : 'hidden';
  });

  overlay.addEventListener('click', closeMenu);
  mobileMenu.querySelectorAll('a').forEach(link => {
    link.addEventListener('click', closeMenu);
  });

  function closeMenu() {
    burgerBtn.setAttribute('aria-expanded', 'false');
    mobileMenu.classList.remove('active');
    overlay.classList.remove('active');
    document.body.style.overflow = 'auto';
  }
}