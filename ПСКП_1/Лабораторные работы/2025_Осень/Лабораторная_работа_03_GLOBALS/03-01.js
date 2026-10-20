const http = require('http');
const readline = require('readline');

let state = 'norm';

const server = http.createServer((req, res) => {
    res.writeHead(200, { 'Content-Type': 'text/html; charset=utf-8' });
    res.end(`<!DOCTYPE html>
<html>
<head><meta charset="utf-8"><title>Состояние</title></head>
<body>
<h1>${state}</h1>
</body>
</html>`);
});

server.listen(5001, () => {
    console.log('Server running at http://localhost:5001/');
});

const rl = readline.createInterface({
    input: process.stdin,
    output: process.stdout,
    prompt: state + '->'
});

rl.prompt();

rl.on('line', (line) => {
    const input = line.trim();
    if (input === 'exit') {
        console.log('exit');
        process.exit(0);
    }
    const validStates = ['norm', 'stop', 'test', 'idle'];
    if (validStates.includes(input)) {
        console.log(`reg = ${state}--> ${input}`);
        state = input;
    } else {
        console.log(input);
    }
    rl.setPrompt(state + '->');
    rl.prompt();
});