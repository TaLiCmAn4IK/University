// faculties.js   ·   интерактив страницы «Факультеты»

document.addEventListener('DOMContentLoaded', () => {
  //Подсветка активного пункта меню
  const currentPage = window.location.pathname.split('/').pop() || 'index.html';

  document.querySelectorAll('.header__nav a').forEach(link => {
    const href = link.getAttribute('href');

    link.classList.remove('active');   // сбрасываем
    if (href === currentPage) {
      link.classList.add('active');    // подсвечиваем текущий
    }
  });

  document.querySelectorAll('.faculty-item').forEach(item => {
    const header = item.querySelector('.faculty-item__header');
    const list   = item.querySelector('.faculty-item__departments');

    // Изначально сворачиваем список
    list.style.maxHeight  = '0';
    list.style.overflow   = 'hidden';
    list.style.transition = 'max-height 0.3s ease';

    // Переключаем по клику
    header.addEventListener('click', () => {
      const isOpen = item.classList.toggle('open');

      list.style.maxHeight = isOpen ? `${list.scrollHeight}px` : '0';
    });
  });
});
