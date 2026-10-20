document.addEventListener('DOMContentLoaded', () => {
  const tabButtons  = document.querySelectorAll('.tab-btn');
  const tabContents = document.querySelectorAll('.tab-content');

  tabButtons.forEach(button => {
    button.addEventListener('click', () => {
      tabButtons.forEach(btn => btn.classList.remove('active'));
      tabContents.forEach(content => content.classList.remove('active'));

      button.classList.add('active');
      const tabId = button.getAttribute('data-tab');
      document.getElementById(tabId)?.classList.add('active');
    });
  });

  // FAQ-аккордеон
  const faqQuestions = document.querySelectorAll('.faq-question');

  faqQuestions.forEach(question => {
    question.addEventListener('click', () => {
      const isExpanded = question.getAttribute('aria-expanded') === 'true';
      const answer     = question.nextElementSibling;

      question.setAttribute('aria-expanded', String(!isExpanded));
      answer.setAttribute('aria-hidden',     String(isExpanded));

      if (!isExpanded) {
        faqQuestions.forEach(q => {
          if (q !== question && q.getAttribute('aria-expanded') === 'true') {
            q.setAttribute('aria-expanded', 'false');
            q.nextElementSibling.setAttribute('aria-hidden', 'true');
          }
        });
      }
    });
  });

  // Форма «Вопрос» 
  const admissionForm = document.getElementById('admission-form');

  if (admissionForm) {
    admissionForm.addEventListener('submit', (e) => {
      e.preventDefault();

      const name     = document.getElementById('name').value.trim();
      const email    = document.getElementById('email').value.trim();
      const question = document.getElementById('question').value.trim();

      console.log('Form submitted:', { name, email, question });

      alert('Спасибо за ваш вопрос! Мы свяжемся с вами в ближайшее время.');
      admissionForm.reset();
    });
  }

  // Анимация появления при скролле
  const animateOnScroll = () => {
    const elements = document.querySelectorAll('.specialty-card, .timeline-item');

    elements.forEach(el => {
      const elTop          = el.getBoundingClientRect().top;
      const triggerPoint   = window.innerHeight / 1.3;

      if (elTop < triggerPoint) {
        el.style.opacity   = '1';
        el.style.transform = 'translateY(0)';
      }
    });
  };

  document.querySelectorAll('.specialty-card, .timeline-item').forEach(el => {
    el.style.opacity   = '0';
    el.style.transform = 'translateY(20px)';
    el.style.transition = 'opacity 0.5s ease, transform 0.5s ease';
  });

  animateOnScroll();
  window.addEventListener('scroll', animateOnScroll);
});
//  Форма «Вопрос»
const admissionForm = document.getElementById('admission-form');

if (admissionForm) {
  admissionForm.addEventListener('submit', (e) => {
    e.preventDefault();

    const name = document.getElementById('name').value.trim();
    const email = document.getElementById('email').value.trim();
    const question = document.getElementById('question').value.trim();

    // Создаем XML структуру
    const xmlString = `<?xml version="1.0" encoding="UTF-8"?>
<admissionQuestion>
  <date>${new Date().toISOString()}</date>
  <applicant>
    <name>${escapeXml(name)}</name>
    <email>${escapeXml(email)}</email>
  </applicant>
  <question>${escapeXml(question)}</question>
</admissionQuestion>`;

    const blob = new Blob([xmlString], { type: 'application/xml' });
    
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = 'admission_question.xml';
    
    document.body.appendChild(a);
    a.click();
    
    // Убираем ссылку из DOM
    setTimeout(() => {
      document.body.removeChild(a);
      URL.revokeObjectURL(url);
    }, 100);

    // Оповещение пользователя
    admissionForm.reset();
  });
}

// Функция для экранирования XML-специальных символов
function escapeXml(unsafe) {
  return unsafe.replace(/[<>&'"]/g, function (c) {
    switch (c) {
      case '<': return '&lt;';
      case '>': return '&gt;';
      case '&': return '&amp;';
      case '\'': return '&apos;';
      case '"': return '&quot;';
      default: return c;
    }
  });
}