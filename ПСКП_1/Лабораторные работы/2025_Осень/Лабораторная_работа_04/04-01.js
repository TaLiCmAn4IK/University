const http = require('http');
const url = require('url');
const DB = require('./DB');

const db = new DB();

db.on('GET', (data) => console.log('Событие GET:', data));
db.on('POST', (data) => console.log('Событие POST:', data));
db.on('PUT', (data) => console.log('Событие PUT:', data));
db.on('DELETE', (data) => console.log('Событие DELETE:', data));

const server = http.createServer((req, res) => {
    const parsedUrl = url.parse(req.url, true);
    
    res.setHeader('Access-Control-Allow-Origin', '*');
    res.setHeader('Access-Control-Allow-Methods', 'GET, POST, PUT, DELETE, OPTIONS');
    res.setHeader('Access-Control-Allow-Headers', 'Content-Type');

    if (req.method === 'OPTIONS') {
        res.writeHead(204);
        return res.end();
    }

    if (parsedUrl.pathname === '/api/db') {
        if (req.method === 'GET') {
            const data = db.select();
            res.writeHead(200, { 'Content-Type': 'application/json' });
            res.end(JSON.stringify(data));
        } 
        else if (req.method === 'POST' || req.method === 'PUT') {
            let body = '';
            req.on('data', chunk => body += chunk.toString());
            req.on('end', () => {
                try {
                    const json = JSON.parse(body);
                    const result = req.method === 'POST' ? db.insert(json) : db.update(json);
                    if (result) {
                        res.writeHead(200, { 'Content-Type': 'application/json' });
                        res.end(JSON.stringify(result));
                    } else {
                        res.writeHead(404, { 'Content-Type': 'application/json' });
                        res.end(JSON.stringify({ error: 'Row not found' }));
                    }
                } catch (e) {
                    res.writeHead(400, { 'Content-Type': 'application/json' });
                    res.end(JSON.stringify({ error: 'Invalid JSON' }));
                }
            });
        } 
        else if (req.method === 'DELETE') {
            const id = parseInt(parsedUrl.query.id, 10);
            if (isNaN(id)) {
                res.writeHead(400, { 'Content-Type': 'application/json' });
                return res.end(JSON.stringify({ error: 'Invalid id' }));
            }
            const result = db.delete(id);
            if (result) {
                res.writeHead(200, { 'Content-Type': 'application/json' });
                res.end(JSON.stringify(result));
            } else {
                res.writeHead(404, { 'Content-Type': 'application/json' });
                res.end(JSON.stringify({ error: 'Row not found' }));
            }
        } 
        else {
            res.writeHead(405); res.end('Method Not Allowed');
        }
    } else {
        res.writeHead(404); res.end('Not Found');
    }
});

server.listen(5001, () => {
    console.log('04-01 Server running at http://localhost:5001/api/db');
});