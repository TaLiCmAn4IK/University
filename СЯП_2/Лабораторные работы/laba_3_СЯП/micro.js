console.log('Начало');        

setTimeout(() => console.log('Таймаут'), 0);

Promise.resolve()
    .then(() => console.log('Промис 1'))   
    .then(() => console.log('Промис 2'));  

console.log('Конец');          