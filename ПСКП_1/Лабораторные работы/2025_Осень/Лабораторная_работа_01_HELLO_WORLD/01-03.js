const http = require('http');

http.createServer((req, res) => {
    let body = '';
    
    req.on('data', chunk => {
        body += chunk;
    });

    req.on('end', () => {
        res.writeHead(200, { 'Content-Type': 'text/html; charset=utf-8' });
        res.end(`
            <h1>Информация о запросе</h1>
            <p><b>Метод:</b> ${req.method}</p>
            <p><b>Версия:</b> ${req.httpVersion}</p>
            <p><b>URI (URL):</b> ${req.url}</p>
            <p><b>Тело запроса:</b> ${body || 'пусто'}</p>
            <hr>
            <h2>Заголовки:</h2>
            <pre>${JSON.stringify(req.headers, null, 2)}</pre>
        `);
    });
}).listen(3001, () => {
    console.log('Сервер 01-03 запущен на http://localhost:3001');
});
