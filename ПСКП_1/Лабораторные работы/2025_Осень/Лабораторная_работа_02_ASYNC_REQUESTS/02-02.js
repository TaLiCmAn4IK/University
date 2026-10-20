const http = require('http');
const fs = require('fs');
const path = require('path');

const PORT = 3000;

http.createServer((req, res) => {
    // Проверяем, что пришел GET-запрос на адрес /png
    if (req.method === 'GET' && req.url === '/png') {
        const filePath = path.join(__dirname, 'pic.png');
        
        // Читаем файл картинки
        fs.readFile(filePath, (err, data) => {
            if (err) {
                res.writeHead(404, { 'Content-Type': 'text/plain; charset=utf-8' });
                res.end('Изображение pic.png не найдено в папке сервера.');
                return;
            }
            // Указываем правильный тип контента для PNG-картинок
            res.writeHead(200, { 'Content-Type': 'image/png' });
            res.end(data);
        });
    } else {
        res.writeHead(404, { 'Content-Type': 'text/plain; charset=utf-8' });
        res.end('Страница не найдена. Перейдите по адресу /png');
    }
}).listen(PORT, () => {
    console.log(`Сервер Задания 02 запущен! Ссылка для браузера: http://localhost:${PORT}/png`);
});
