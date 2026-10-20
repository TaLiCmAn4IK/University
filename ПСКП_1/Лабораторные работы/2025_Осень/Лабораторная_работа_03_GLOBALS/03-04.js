const http = require('http');
const url = require('url');

function factorialAsync(n, callback) {
    if (n <= 1) {
        process.nextTick(() => callback(1));
    } else {
        factorialAsync(n - 1, (prev) => {
            process.nextTick(() => callback(n * prev));
        });
    }
}

const server = http.createServer((req, res) => {
    const parsedUrl = url.parse(req.url, true);

    if (parsedUrl.pathname === '/fact') {
        const k = parseInt(parsedUrl.query.k, 10);
        if (!isNaN(k)) {
            factorialAsync(k, (fact) => {
                res.writeHead(200, { 'Content-Type': 'application/json' });
                res.end(JSON.stringify({ k: k, fact: fact }));
            });
        } else {
            res.writeHead(400, { 'Content-Type': 'text/plain; charset=utf-8' });
            res.end('Invalid parameter k');
        }
    } else if (parsedUrl.pathname === '/') {
        res.writeHead(200, { 'Content-Type': 'text/html; charset=utf-8' });
        res.end(`<!DOCTYPE html>
<html>
<head><meta charset="utf-8"><title>nextTick</title></head>
<body>
<h3>Результаты (process.nextTick):</h3>
<div id="results"></div>
<script>
(async function() {
    const start = Date.now();
    const container = document.getElementById('results');
    for (let x = 1; x <= 20; x++) {
        const t = Date.now() - start;
        const response = await fetch('/fact?k=' + x);
        const data = await response.json();
        const p = document.createElement('p');
        p.textContent = t + ' - ' + data.k + '/' + data.fact;
        container.appendChild(p);
    }
    const total = Date.now() - start;
    const p = document.createElement('p');
    p.innerHTML = '<b>Общая продолжительность: ' + total + ' мс</b>';
    container.appendChild(p);
})();
</script>
</body>
</html>`);
    } else {
        res.writeHead(404, { 'Content-Type': 'text/plain; charset=utf-8' });
        res.end('Not found');
    }
});

server.listen(5004, () => {
    console.log('Server running at http://localhost:5004/');
});