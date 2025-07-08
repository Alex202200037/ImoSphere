// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// --- Animated Counters ---
document.addEventListener('DOMContentLoaded', function() {
  document.querySelectorAll('.counter').forEach(function(counter) {
    const animate = () => {
      const target = +counter.getAttribute('data-target');
      const duration = 1200;
      const start = 0;
      let startTimestamp = null;
      const step = (timestamp) => {
        if (!startTimestamp) startTimestamp = timestamp;
        const progress = Math.min((timestamp - startTimestamp) / duration, 1);
        counter.textContent = Math.floor(progress * (target - start) + start).toLocaleString();
        if (progress < 1) {
          window.requestAnimationFrame(step);
        } else {
          counter.textContent = target.toLocaleString();
        }
      };
      window.requestAnimationFrame(step);
    };
    if (counter.getBoundingClientRect().top < window.innerHeight) {
      animate();
    } else {
      const onScroll = () => {
        if (counter.getBoundingClientRect().top < window.innerHeight) {
          animate();
          window.removeEventListener('scroll', onScroll);
        }
      };
      window.addEventListener('scroll', onScroll);
    }
  });
});

// --- Simple Slider for Featured Properties & Testimonials ---
function createSimpleSlider(containerSelector, cardSelector, prevBtn, nextBtn) {
  const container = document.querySelector(containerSelector);
  if (!container) return;
  const cards = container.querySelectorAll(cardSelector);
  let current = 0;
  function show(index) {
    cards.forEach((c, i) => {
      c.style.display = (i === index) ? 'block' : 'none';
    });
  }
  show(current);
  if (prevBtn) {
    document.querySelector(prevBtn).onclick = () => {
      current = (current - 1 + cards.length) % cards.length;
      show(current);
    };
  }
  if (nextBtn) {
    document.querySelector(nextBtn).onclick = () => {
      current = (current + 1) % cards.length;
      show(current);
    };
  }
}
// Exemplo de uso: createSimpleSlider('.featured-slider', '.featured-card', '#featured-prev', '#featured-next');
// Exemplo de uso: createSimpleSlider('.testimonials-slider', '.testimonial-card', '#testimonials-prev', '#testimonials-next');

// --- Scroll suave para âncoras ---
document.querySelectorAll('a[href^="#"]').forEach(anchor => {
  anchor.addEventListener('click', function(e) {
    const target = document.querySelector(this.getAttribute('href'));
    if (target) {
      e.preventDefault();
      target.scrollIntoView({ behavior: 'smooth' });
    }
  });
});

// --- Botão Voltar ao Topo ---
(function() {
  const btn = document.createElement('button');
  btn.innerHTML = '<i class="fas fa-arrow-up"></i>';
  btn.className = 'btn btn-primary back-to-top';
  btn.style.position = 'fixed';
  btn.style.bottom = '32px';
  btn.style.right = '32px';
  btn.style.display = 'none';
  btn.style.zIndex = '9999';
  document.body.appendChild(btn);
  window.addEventListener('scroll', function() {
    btn.style.display = window.scrollY > 300 ? 'block' : 'none';
  });
  btn.onclick = function() {
    window.scrollTo({ top: 0, behavior: 'smooth' });
  };
})();

// --- Microinterações premium nos botões ---
document.querySelectorAll('.btn').forEach(btn => {
  btn.addEventListener('pointerdown', function(e) {
    const ripple = document.createElement('span');
    ripple.className = 'ripple';
    ripple.style.left = `${e.offsetX}px`;
    ripple.style.top = `${e.offsetY}px`;
    this.appendChild(ripple);
    setTimeout(() => ripple.remove(), 600);
  });
});

function toggleTheme() {
    const body = document.body;
    const icon = document.getElementById('themeIcon');
    const isWhite = body.classList.toggle('white-theme');
    if (icon) {
        icon.classList.toggle('fa-moon', !isWhite);
        icon.classList.toggle('fa-sun', isWhite);
    }
    localStorage.setItem('theme', isWhite ? 'white' : 'dark');
}

document.addEventListener('DOMContentLoaded', function() {
    const savedTheme = localStorage.getItem('theme');
    const body = document.body;
    const icon = document.getElementById('themeIcon');
    if (savedTheme === 'white') {
        body.classList.add('white-theme');
        if (icon) {
            icon.classList.remove('fa-moon');
            icon.classList.add('fa-sun');
        }
    } else {
        body.classList.remove('white-theme');
        if (icon) {
            icon.classList.remove('fa-sun');
            icon.classList.add('fa-moon');
        }
    }
});
