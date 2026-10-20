const http = require('http');
const fs = require('fs');
const path = require('path');

const PORT = 3000;

const sendFile = (res, fileName, contentType) => {
    const filePath = path.join(__dirname, fileName);
    fs.readFile(filePath, (err, data) => {
        if (err) {
            res.writeHead(500, { 'Content-Type': 'text/plain; charset=utf-8' });
            res.end(`Ошибка: файл ${fileName} не найден на сервере!`);
            return;
        }
        res.writeHead(200, { 'Content-Type': contentType });
        res.end(data);
    });
};

http.createServer((req, res) => {
    if (req.method === 'GET') {
        if (req.url === '/api/name') {
            res.writeHead(200, { 'Content-Type': 'text/plain; charset=utf-8' });
            res.end('Потребко Вадим Русланович'); 
        } 
        else if (req.url === '/xmlhttprequest') {
            sendFile(res, 'xmlhttprequest.html', 'text/html; charset=utf-8');
        } else if (req.url === '/fetch') {
            sendFile(res, 'fetch.html', 'text/html; charset=utf-8');
        } else if (req.url === '/jquery') {
            sendFile(res, 'jquery.html', 'text/html; charset=utf-8');
        } 
        // НОВЫЙ МАРШРУТ: отдаем локальный файл библиотеки jQuery браузеру
        else if (req.url === '/jquery.min.js') {
            sendFile(res, 'jquery.min.js', 'application/javascript; charset=utf-8');
        } 
        else {
            res.writeHead(404, { 'Content-Type': 'text/plain; charset=utf-8' });
            res.end('Страница не найдена.');
        }
    }
}).listen(PORT, () => {
    console.log(`Общий сервер запущен успешно!\n` +
                `Проверка Задания 04: http://localhost:${PORT}/xmlhttprequest\n` +
                `Проверка Задания 05: http://localhost:${PORT}/fetch\n` +
                `Проверка Задания 06: http://localhost:${PORT}/jquery`);
});
