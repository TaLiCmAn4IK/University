const fireworksContainer = document.querySelector('.fireworks');

function createFirework() {
    const firework = document.createElement('div');
    firework.className = 'firework';

    const size = Math.random() * 30 + 10; // Random size between 10 and 40
    firework.style.width = `${size}px`;
    firework.style.height = `${size}px`;
    firework.style.left = `${Math.random() * window.innerWidth}px`;
    firework.style.animationDelay = `${Math.random() * 2}s`; // Random delay

    fireworksContainer.appendChild(firework);

    setTimeout(() => {
        fireworksContainer.removeChild(firework);
    }, 1000); // Remove firework after the animation
}

// Create fireworks at intervals
setInterval(createFirework, 300);