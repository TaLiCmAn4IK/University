function checkStock(item) {
    return new Promise((resolve, reject) => {
        setTimeout(() => {
            const hasStock = Math.random() > 1; // 80% успеха
            if (hasStock) {
                console.log(`Товар "${item}" в наличии`);
                resolve({ item, price: 600 });
            } else {
                reject(`Товар "${item}" отсутствует на складе`);
            }
        }, 1000);
    });
}

function processPayment(order) {
    return new Promise((resolve, reject) => {
        setTimeout(() => {
            const hasMoney = Math.random() > 0.3; // 70% успеха
            if (hasMoney) {
                console.log(`Оплата заказа ${order.item} прошла успешно`);
                resolve({ ...order, paid: true });
            } else {
                reject('Недостаточно средств на счету');
            }
        }, 2000);
    });
}

function deliverOrder() {
    return new Promise((resolve) => {
        setTimeout(() => {
            console.log('Заказ доставлен');
            resolve('Доставлено');
        }, 1500);
    });
}

checkStock('Пицца')
    .then(processPayment)
    .then(deliverOrder)
    .then((result) => console.log('Статус:', result))
    .catch((error) => console.error('Ошибка:', error))
    .finally(() => console.log('Спасибо за заказ, приходите еще!'));