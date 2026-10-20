const http = require('http');
const url = require('url');

function factorial(n) {
    if (n <= 1) return 1;
    return n * factorial(n - 1);
}

const server = http.createServer((req, res) => {
    const parsedUrl = url.parse(req.url, true);

    if (parsedUrl.pathname === '/fact') {
        const k = parseInt(parsedUrl.query.k, 10);
        if (!isNaN(k)) {
            const fact = factorial(k);
            res.writeHead(200, { 'Content-Type': 'application/json' });
            res.end(JSON.stringify({ k: k, fact: fact }));
        } else {
            res.writeHead(400, { 'Content-Type': 'text/plain; charset=utf-8' });
            res.end('Invalid parameter k');
        }
    } else {
        res.writeHead(404, { 'Content-Type': 'text/plain; charset=utf-8' });
        res.end('Not found');
    }
});

server.listen(5002, () => {
    console.log('Server running at http://localhost:5002/');
});