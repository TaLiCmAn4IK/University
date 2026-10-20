const http = require('http');
const fs = require('fs');
const path = require('path');

const PORT = 3000;

http.createServer((req, res) => {
    // Приводим URL к нижнему регистру и отсекаем параметры, если они есть
    const cleanUrl = req.url.split('?')[0].toLowerCase();

    if (req.method === 'GET' && cleanUrl === '/html') {
        const filePath = path.join(__dirname, 'index.html');
        
        fs.readFile(filePath, (err, data) => {
            if (err) {
                res.writeHead(500, { 'Content-Type': 'text/plain; charset=utf-8' });
                res.end('Ошибка: файл index.html не найден в папке со скриптом!');
                return;
            }
            res.writeHead(200, { 'Content-Type': 'text/html; charset=utf-8' });
            res.end(data);
        });
    } else {
        res.writeHead(404, { 'Content-Type': 'text/plain; charset=utf-8' });
        res.end('Страница не найдена. Перейдите на /html');
    }
}).listen(PORT, () => {
    console.log(`Сервер успешно запущен! Откройте в браузере: http://localhost:${PORT}/html`);
});
