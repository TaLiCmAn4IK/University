function fetchFast() {
    return new Promise((resolve) => {
        setTimeout(() => resolve('Fast ответ'), 500);
    });
}

function fetchSlow() {
    return new Promise((resolve) => {
        setTimeout(() => resolve('Slow ответ'), 2000);
    });
}

Promise.race([fetchFast(), fetchSlow()])
    .then((result) => console.log('Победитель гонки:', result))
    .catch((error) => console.error(error));