const promises = [
    Promise.resolve('Успех 1'),
    Promise.reject('Ошибка 1'),
    Promise.resolve('Успех 2'),
    Promise.reject('Ошибка 2'),
    Promise.resolve('Успех 3')
];

Promise.allSettled(promises)
    .then((results) => {
        const fulfilledOnly = results
            .filter(result => result.status === 'fulfilled')
            .map(result => result.value);
        
        console.log('Только успешные результаты:', fulfilledOnly);
    });