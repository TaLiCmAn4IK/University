const http = require('http');

const PORT = 3000;

http.createServer((req, res) => {
    if (req.method === 'GET' && req.url === '/api/name') {
        res.writeHead(200, { 'Content-Type': 'text/plain; charset=utf-8' });
        res.end('Потребко Вадим Русланович'); // <-- НАПИШИТЕ СЮДА СВОИ ФИО
    } else {
        res.writeHead(404, { 'Content-Type': 'text/plain; charset=utf-8' });
        res.end('Маршрут не найден. Используйте /api/name');
    }
}).listen(PORT, () => {
    console.log(`Сервер Задания 03 запущен! Ссылка для браузера: http://localhost:${PORT}/api/name`);
});
